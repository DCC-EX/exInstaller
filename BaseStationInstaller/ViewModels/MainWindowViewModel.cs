using BaseStationInstaller.Models;
using BaseStationInstaller.Utils;
using LibGit2Sharp;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

using System.Linq;
using System.Net;
using System.Reactive;
using System.Reactive.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;


namespace BaseStationInstaller.ViewModels
{
    public class MainWindowViewModel : ViewModelBase, IMainWindowViewModel
    {
        ArudinoCliHelper helper;
        StreamWriter logWriter;
        Signature sig = new Signature(new Identity("random", "random@random.com"), DateTimeOffset.Now);
        public MainWindowViewModel()
        {
            if (File.Exists("status.log"))
            {
                if (File.Exists("status.old.log"))
                {
                    File.Delete("status.old.log");
                }
                File.Move("status.log", "status.old.log");
               
            }
            logWriter = new StreamWriter("status.log");
            Task task = new Task(InitArduinoCLI);
            task.Start();
            RefreshComPortButton = ReactiveCommand.Create(RefreshComPortsCommand, this.WhenAnyValue(x => x.RefreshingPorts, (refrshing) => {
                return !refrshing;
            }).ObserveOn(RxApp.MainThreadScheduler));
            CompileUpload = ReactiveCommand.Create(CompileandUploadCommand, this.WhenAnyValue(x => x.Busy, (busy) =>
            {
                return !busy;
            }).ObserveOn(RxApp.MainThreadScheduler));
            this.WhenAnyValue(x => x.SelectedConfig).Subscribe(ProcessConfigChange);
            this.WhenAnyValue(x => x.Status).Subscribe(ProcessStatusChange);
        }

        private void ProcessStatusChange(string status)
        {
            if (!String.IsNullOrEmpty(status))
            {
                StatusCaret = status.Length;
                logWriter.Flush();
                logWriter.Write(status);
            }
        }


        private void ProcessConfigChange(Config cfg)
        {
            if (!String.IsNullOrEmpty(SelectedConfig.Name))
            {
                SelectedSupportedBoards = new ObservableCollection<Board>(SelectedConfig.SupportedBoards);
                SelectedSupportedMotorShields = new ObservableCollection<MotorShield>(SelectedConfig.SupportedMotorShields);
                GitCode(SelectedConfig.Git, $@"{SelectedConfig.Name}");
                Task detect = new Task(helper.DetectBoard);
                detect.Start();
            }
        }

        async void InitArduinoCLI()
        {
            Busy = true;
            string cliRuntime = "";
            string destRuntime = "";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                if (RuntimeInformation.OSArchitecture == Architecture.X64)
                {
                    destRuntime = "arduino-cli.exe";
                    cliRuntime = "arduino-cli-runtimes\\Windows_64bit\\arduino-cli.exe";
                }
                else if (RuntimeInformation.OSArchitecture == Architecture.X86)
                {
                    destRuntime = "arduino-cli.exe";
                    cliRuntime = "arduino-cli-runtimes\\Windows_32bit\\arduino-cli.exe";
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                destRuntime = "arduino-cli";
                cliRuntime = "arduino-cli-runtimes/macOS_64bit/arduino-cli";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                switch (RuntimeInformation.OSArchitecture)
                {
                    case Architecture.X86:
                        destRuntime = "arduino-cli";
                        cliRuntime = "arduino-cli-runtimes/Linux_32bit/arduino-cli";
                        break;
                    case Architecture.X64:
                        destRuntime = "arduino-cli";
                        cliRuntime = "arduino-cli-runtimes/Linux_64bit/arduino-cli";
                        break;
                    case Architecture.Arm:
                        destRuntime = "arduino-cli";
                        cliRuntime = "arduino-cli-runtimes/Linux_ARM/arduino-cli";
                        break;
                    case Architecture.Arm64:
                        destRuntime = "arduino-cli";
                        cliRuntime = "arduino-cli-runtimes/Linux_ARM64/arduino-cli";
                        break;
                }
            }
            if (!String.IsNullOrEmpty(destRuntime))
            {
                if (!File.Exists(destRuntime))
                {
                    File.Copy(cliRuntime, destRuntime);
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    {
                        ProcessStartInfo start = new ProcessStartInfo();
                        start.FileName = $@"chmod";
                        start.Arguments = $"a+x {destRuntime}";
                        start.UseShellExecute = false;
                        start.WindowStyle = ProcessWindowStyle.Hidden;
                        start.CreateNoWindow = true;
                        start.RedirectStandardOutput = true;
                        Process process = new Process
                        {
                            StartInfo = start
                        };
                        process.Start();
                    }
                }
                helper = new ArudinoCliHelper(this);
                bool cli = await helper.Init();
                if (cli)
                {
                    SelectedConfig = BaseStationSettings.BaseStationDefaults[0];
                }
            }
            else
            {
                Status += $"This platform is not supported by this installer at this time{Environment.NewLine}";
            }
        }

        public void Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            logWriter.Flush();
            logWriter.Close();
            logWriter.Dispose();
            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) || RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    foreach (Process proc in Process.GetProcessesByName("arduino-cli"))
                    {
                        proc.Kill();
                    }
                }
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    foreach (Process proc in Process.GetProcessesByName("arduino-cli.exe"))
                    {
                        proc.Kill();
                    }
                }
            }
            catch (Exception ex)
            {
                
            }
        }

        #region Bindings
        /// <summary>
        /// List of Available Base Station Configs stored in Settings
        /// </summary>

        //TODO: Move this to not be hard coded
        public List<Config> BaseStations
        {
            get
            {
                return BaseStationSettings.BaseStationDefaults;
            }
        }

        /// <summary>
        /// Selected Base Station you would like to compile
        /// </summary>
        private Config _selectedConfig;
        public Config SelectedConfig
        {
            get => _selectedConfig;
            set => this.RaiseAndSetIfChanged(ref _selectedConfig, value);
            //{
            //    _selectedConfig = value;
            //    //RaiseAndSetIfChanged("SelectedConfig");
            //    SelectedSupportedBoards = new ObservableCollection<Board>(SelectedConfig.SupportedBoards);
            //    SelectedSupportedMotorShields = new ObservableCollection<MotorShield>(SelectedConfig.SupportedMotorShields);
            //    GitCode(SelectedConfig.Git, $@".\{SelectedConfig.Name}");
            //}
        }
        /// <summary>
        /// Updatable colletion of boards pulled from Selected Config
        /// </summary>
        private ObservableCollection<Board> _selectedSupportedBoards;
        public ObservableCollection<Board> SelectedSupportedBoards
        {
            get => _selectedSupportedBoards;

            set => this.RaiseAndSetIfChanged(ref _selectedSupportedBoards, value);
        }
        /// <summary>
        /// Updatable collection of motorshields supported via Selected Config
        /// </summary>
        private ObservableCollection<MotorShield> _selectedSupportedMotorShields;
        public ObservableCollection<MotorShield> SelectedSupportedMotorShields
        {
            get => _selectedSupportedMotorShields;

            set => this.RaiseAndSetIfChanged(ref _selectedSupportedMotorShields, value);
        }
        /// <summary>
        /// List of comports detected by windows
        /// </summary>
        private ObservableCollection<Tuple<string, string>> _availableComPorts;
        public ObservableCollection<Tuple<string,string>> AvailableComPorts
        {
            get => _availableComPorts;

            set => this.RaiseAndSetIfChanged(ref _availableComPorts, value);
        }

        /// <summary>
        /// List of Branches in Git Repo for Selected Config
        /// </summary>
        //private ObservableCollection<string> _branches;
        //public ObservableCollection<string> Branches
        //{
        //    get
        //    {
        //        return _branches;
        //    }
        //    set
        //    {

        //        _branches = value;
        //        //RaiseAndSetIfChanged("Branches");
        //    }
        //}

        private Board _selectedBoard;
        public Board SelectedBoard
        {
            get => _selectedBoard;

            set => this.RaiseAndSetIfChanged(ref _selectedBoard, value);
        }

        private MotorShield _selectedMotorShield;

        public MotorShield SelectedMotorShield
        {
            get => _selectedMotorShield;

            set => this.RaiseAndSetIfChanged(ref _selectedMotorShield, value);
        }

        private Tuple<string, string> _selectedComPort;

        public Tuple<string,string> SelectedComPort
        {
            get => _selectedComPort;

            set => this.RaiseAndSetIfChanged(ref _selectedComPort, value);
        }

        //private string _selectedBranch;

        //public string SelectedBranch
        //{
        //    get
        //    {
        //        return _selectedBranch;
        //    }
        //    set
        //    {
        //        _selectedBranch = value;
        //        //RaiseAndSetIfChanged("SelectedBranch");
        //        using (Repository repo = new Repository($"./{SelectedConfig.Name}"))
        //        {
        //            Branch branch = repo.Branches[$"origin/{SelectedBranch}"];

        //            Branch localbranch = repo.Branches[SelectedBranch];
        //            if (localbranch == null)
        //            {
        //                // repository return null object when branch not exists
        //                localbranch = repo.CreateBranch(SelectedBranch, branch.Tip);
        //            }
        //            Commands.Checkout(repo, localbranch);


        //        }

        //    }
        //}

        private string _status;
        public string Status
        {
            get => _status;

            set => this.RaiseAndSetIfChanged(ref _status, value);
        }


        private int _statusCaret;
        public int StatusCaret
        {
            get => _statusCaret;

            set => this.RaiseAndSetIfChanged(ref _statusCaret, value);
        }
        //private string _wiringDiagram;
        //public string WiringDiagram
        //{
        //    get
        //    {
        //        return _wiringDiagram;
        //    }
        //    set
        //    {
        //        _wiringDiagram = value;
        //        //RaiseAndSetIfChanged("WiringDiagram");
        //    }
        //}

        private bool _refreshingPorts;

        public bool RefreshingPorts
        {
            get => _refreshingPorts;

            set => this.RaiseAndSetIfChanged(ref _refreshingPorts, value);

        }

        private bool _busy;

        public bool Busy
        {
            get => _busy;

            set => this.RaiseAndSetIfChanged(ref _busy, value);
        }

        private int _progress;

        public int Progress
        {
            get => _progress;

            set => this.RaiseAndSetIfChanged(ref _progress, value);
        }
        #endregion

        #region Commands


        public ReactiveCommand<Unit, Unit> RefreshComPortButton
        {
            get;
        }

        void RefreshComPortsCommand()
        {
            Task task = new Task(helper.DetectBoard);
            task.Start();
        }


        void CompileandUploadCommand()
        {

            Task task = new Task(ProcessCompileUpload);
            task.Start();
        }
        public ReactiveCommand<Unit, Unit> CompileUpload
        {
            get;
        }






        #endregion

        #region Methods
        /// <summary>
        /// Refresh list of avaliable comports
        /// </summary>
        private async void ProcessCompileUpload()
        {
            Progress = 0;
            Busy = true;
            bool success = await DownloadPreReqs();
            if (success)
            {
                Task task = new Task(CompileSketch);
                task.Start();
            } else
            {
                RefreshingPorts = false;
                Busy = false;
                Progress = 0;
                Status += $"Failed to download dependencies aborting compilation {Environment.NewLine}";
            }
        }

        int currDep = 0;
        private async Task<bool> DownloadPreReqs()
        {
            Status += $"Starting Dependency Downloads {Environment.NewLine}";
            Thread.Sleep(500);
            foreach(Platform plat in SelectedBoard.Platforms)
            {
                bool success = await helper.GetBoard(plat.Architecture, plat.Package);
                if (!success)
                {
                    return false;
                }
            }
            foreach (Library dep in SelectedConfig.Libraries)
            {
                if (dep.LibraryDownloadAvailable)
                {
                    bool success = await helper.GetLibrary(dep.Name);
                    if (!success)
                    {
                        await GitCode(dep.Repo, dep.Location);
                    }
                }
                else
                {
                    await GitCode(dep.Repo, dep.Location);
                }
            }
            return true;
        }




        private async void CompileSketch()
        {
            Busy = true;
            RefreshingPorts = true;
            Status += $"Changing MotorShield options{Environment.NewLine}";
            Progress = 1;
            string[] config = File.ReadAllLines($@"{SelectedConfig.Name}/{SelectedConfig.ConfigFile}");
            Progress = 2;
            switch (SelectedConfig.Name)
            {
                case "BaseStationClassic":
                    config[16] = $"#define MOTOR_SHIELD_TYPE   {(int)SelectedMotorShield.ShieldType}";
                    break;
                case "BaseStationEx":
                    config[17] = $"#define MOTOR_SHIELD_TYPE   {(int)SelectedMotorShield.ShieldType}";
                    break;
                case "CommandStation-DCC":
                    config[26] = "//#define CONFIG_ARDUINO_MOTOR_SHIELD";
                    switch (SelectedMotorShield.ShieldType)
                    {
                        case MotorShieldType.Arduino:
                            config[25] = "#define CONFIG_ARDUINO_MOTOR_SHIELD";
                            break;
                        case MotorShieldType.Pololu:
                            config[25] = "#define CONFIG_POLOLU_MOTOR_SHIELD";
                            break;
                        case MotorShieldType.FireBox_MK1:
                            config[25] = "#define CONFIG_WSM_FIREBOX_MK1";
                            break;
                        case MotorShieldType.FireBox_MK1S:
                            config[25] = "#define CONFIG_WSM_FIREBOX_MK1S";
                            break;
                    }
                    break;
            }
            File.WriteAllLines($@"{SelectedConfig.Name}/{SelectedConfig.ConfigFile}", config);
            Thread.Sleep(1000);
            Progress = 3;
            Status += $"Compiling {SelectedConfig.DisplayName} Sketch{Environment.NewLine}";
            var dir = new DirectoryInfo($@"./{SelectedConfig.Name}");
            dir.CreateSubdirectory("./build");
            foreach (var file in dir.EnumerateDirectories("_git2*"))
            {
                file.Delete();
            }
            helper.ArduinoComplieSketch(SelectedBoard.FQBN, $@"./{SelectedConfig.Name}/{SelectedConfig.InputFileLocation}");

            RefreshingPorts = true;
            Thread.Sleep(5000);
            Status += $"Uploading to {SelectedComPort}";

            helper.UploadSketch(SelectedBoard.FQBN, SelectedComPort.Item1, $@"{SelectedConfig.Name}/{SelectedConfig.InputFileLocation}");
            
        }

        private async Task GitCode(string url, string location)
        {
            Busy = true;
            Status += $"Obtaining {url} via git{Environment.NewLine}";
            CloneOptions options = new CloneOptions();
            options.RepositoryOperationCompleted = new LibGit2Sharp.Handlers.RepositoryOperationCompleted(GotCode);
            Progress = 0;
            if (!Directory.Exists($"{location}"))
            {
                Repository.Clone(url, $"{location}", options);
            }
            else
            {
                if (!Repository.IsValid($"{location}"))
                {
                    Directory.Delete($"{location}", true);
                    Repository.Clone(url, $"{location}", options);
                }
                else
                {
                    using (Repository repo = new Repository($"{location}"))
                    {

                        Stash stash;
                        RepositoryStatus status = repo.RetrieveStatus();
                        if (status.IsDirty)
                        {
                            //repo.Reset(ResetMode.Hard);

                            stash = repo.Stashes.Add(sig);
                        }
                        Commands.Pull(repo, sig, new PullOptions());
                        if (repo.Stashes.Count() >= 1)
                        {
                            StashApplyStatus stashApply = repo.Stashes.Apply(0, new StashApplyOptions() { ApplyModifiers = StashApplyModifiers.ReinstateIndex });
                            switch (stashApply)
                            {
                                case StashApplyStatus.Applied:
                                    Status += $"Successfully applied Stashed changes in {location}{Environment.NewLine}";
                                    break;
                                case StashApplyStatus.Conflicts:
                                    Status += $"Reverted to Default Checkout on {location} since changes conflict with upstream{Environment.NewLine}";
                                    break;
                                case StashApplyStatus.NotFound:
                                    Status += $"Stash not found for {location}{Environment.NewLine}";
                                    break;
                                case StashApplyStatus.UncommittedChanges:
                                    Status += $"There are uncommited changes in {location}{Environment.NewLine}";
                                    break;
                            }
                        }
                    }
                }
            }
            var dir = new DirectoryInfo(location);
            foreach (var file in dir.EnumerateDirectories("_git2*"))
            {
                file.Delete();
            }
            //if (location.Equals($@".\{SelectedConfig.Name}"))
            //{
            //    Console.WriteLine("setting branch info");
            //    Branches = new ObservableCollection<string>();
            //    using (Repository repo = new Repository($"./{location}"))
            //    {

            //        foreach (Branch b in repo.Branches)
            //        {
            //            Console.WriteLine($"adding branch {b.FriendlyName}");
            //            if (b.FriendlyName.Contains("origin"))
            //            {
            //                Branches.Add(b.FriendlyName.Substring(7));
            //            }
            //        }
            //    }
            //}
            Busy = false;
        }

        private void GotCode(RepositoryOperationContext context)
        {
            //CompileSketch();
            Status += $"{SelectedConfig.DisplayName} Code obtained from Repository{Environment.NewLine}";
        }

        private void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            Progress = e.ProgressPercentage;
        }

        #endregion
    }
}
