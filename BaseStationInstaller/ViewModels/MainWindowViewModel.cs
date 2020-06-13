using BaseStationInstaller.Models;
using BaseStationInstaller.Utils;
using LibGit2Sharp;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Reactive;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BaseStationInstaller.ViewModels
{
    public class MainWindowViewModel : ViewModelBase, IMainWindowViewModel
    {
       ArudinoCliHelper helper;
        StreamWriter logWriter;
        Signature sig = new Signature(new Identity("random", "random@random.com"), DateTimeOffset.Now);
        public MainWindowViewModel()
        {
            //WiringDiagram = "pack://application:,,,/Resources/dcc-ex-logo.png";
            
            SelectedConfig = BaseStationSettings.BaseStationDefaults[0];
            Task task = new Task(RefreshComports);
            task.Start();

            if (File.Exists("status.log"))
            {
                if (File.Exists("status.old.log"))
                {
                    File.Delete("status.old.log");
                }
                File.Move("status.log", "status.old.log");
                //File.Create("status.log");
            }/* else
            {
                File.Create("status.log");
            }*/
            logWriter = new StreamWriter("status.log");
            task = new Task(InitArduinoCLI);
            task.Start();
            RefreshComPortButton = ReactiveCommand.Create(RefreshComPortsCommand);
            CompileUpload = ReactiveCommand.Create(CompileandUploadCommand);
            this.WhenAnyValue(x => x.SelectedConfig).Subscribe(ProcessConfigChange);
            
        }

        private void ProcessConfigChange(Config cfg)
        {
            SelectedSupportedBoards = new ObservableCollection<Board>(SelectedConfig.SupportedBoards);
            SelectedSupportedMotorShields = new ObservableCollection<MotorShield>(SelectedConfig.SupportedMotorShields);
            GitCode(SelectedConfig.Git, $@".\{SelectedConfig.Name}");
        }

        void InitArduinoCLI()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                if (!File.Exists("arduino-cli.exe"))
                {
                    //File.WriteAllBytes("arduino-cli.exe", Resources.ResourceManager);
                }
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                if (!File.Exists("arduino-cli"))
                {
                    //File.WriteAllBytes("arduino-cli", Properties.Resources.file);
                }
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                if (!File.Exists("arduino-cli"))
                {
                    //File.WriteAllBytes("arduino-cli", Properties.Resources.);
                }
            }

            helper = new ArudinoCliHelper(this);
        }

        public void Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            logWriter.Close();
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
        private ObservableCollection<string> _availableComPorts;
        public ObservableCollection<string> AvailableComPorts
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

        private string _selectedComPort;

        public string SelectedComPort
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

            set => this.RaiseAndSetIfChanged(ref _status, value + Environment.NewLine);
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
            Task task = new Task(RefreshComports);
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
        private async void RefreshComports()
        {
            Busy = true;
            RefreshingPorts = true;
            Progress = 0;
            Status += "Refreshing Ports..." + Environment.NewLine;
            for (int i = 0; i < 5; i++)
            {
                Progress += 20;
                Thread.Sleep(500);
                //CommandManager.InvalidateRequerySuggested();
            }
            AvailableComPorts = new ObservableCollection<string>(SerialPort.GetPortNames());
            if (AvailableComPorts.Count >= 1)
            {
                SelectedComPort = AvailableComPorts[0];
            }
            Progress = 100;
            Busy = false;
            RefreshingPorts = false;
            Thread.Sleep(500);
            Progress = 0;
            Status += "Idle" + Environment.NewLine;
            Thread.Sleep(10000);
            //CommandManager.InvalidateRequerySuggested();
        }

        private async void ProcessCompileUpload()
        {
            Progress = 0;
            Busy = true;
            await DownloadPreReqs();
            Task task = new Task(CompileSketch);
            task.Start();
        }

        int currDep = 0;
        private async Task DownloadPreReqs()
        {
            Status += "Starting Dependency Downloads" + Environment.NewLine;
            Thread.Sleep(500);
            foreach (Library dep in SelectedConfig.Libraries)
            {
                if (dep.LibraryDownloadAvailable)
                {
                    helper.GetLibrary(dep.Name);
                }
                else
                {
                    await GitCode(dep.Repo, dep.Location);
                }
            }
            Status += $"Gitting {SelectedConfig.DisplayName}" + Environment.NewLine;
            await GitCode(SelectedConfig.Git, $@".\{SelectedConfig.Name}");
        }

      


        private void CompileSketch()
        {
            Status += "Changing MotorShield options" + Environment.NewLine;
            Progress = 5;
            string[] config = File.ReadAllLines($@".\{SelectedConfig.Name}\{SelectedConfig.ConfigFile}");
            Progress = 10;
            switch (SelectedConfig.Name)
            {
                case "BaseStationClassic":
                    config[16] = $"#define MOTOR_SHIELD_TYPE   {(int)SelectedMotorShield.ShieldType}";

                    break;
                case "BaseStationEx":
                    config[17] = $"#define MOTOR_SHIELD_TYPE   {(int)SelectedMotorShield.ShieldType}";
                    break;
                case "CSTest":
                    switch (SelectedMotorShield.ShieldType)
                    {
                        case MotorShieldType.Arduino:

                            break;
                        case MotorShieldType.Pololu:
                            config[14] = $"// DCC* mainTrack = DCC::Create_Arduino_L298Shield_Main(50);";
                            config[15] = $"// DCC* progTrack = DCC::Create_Arduino_L298Shield_Prog(2);";
                            config[17] = $"DCC* mainTrack = DCC::Create_Pololu_MC33926Shield_Main(50);";
                            config[18] = $"DCC* progTrack = DCC::Create_Pololu_MC33926Shield_Prog(2);";
                            break;
                    }
                    break;

            }
            Progress = 15;
            File.WriteAllLines($@".\{SelectedConfig.Name}\{SelectedConfig.ConfigFile}", config);
            Progress = 20;
            Thread.Sleep(1000);
            Status += $"Compiling {SelectedConfig.DisplayName} Sketch" + Environment.NewLine;
            helper.ArduinoComplieSketch(SelectedBoard.FQBN, $@"{SelectedConfig.Name}/{SelectedConfig.InputFileLocation}");

            RefreshingPorts = true;
            Thread.Sleep(1000);
            Status += $"Uploading to {SelectedComPort}" + Environment.NewLine;
            Progress = 75;
            helper.UploadSketch(SelectedBoard.FQBN, SelectedComPort);
            Progress = 100;
            Thread.Sleep(1000);
            Progress = 0;
            Busy = false;
            RefreshingPorts = false;
        }

        private async Task GitCode(string url, string location)
        {
            
            CloneOptions options = new CloneOptions();
            options.RepositoryOperationCompleted = new LibGit2Sharp.Handlers.RepositoryOperationCompleted(GotCode);
            Progress = 0;
            if (!Directory.Exists($"./{location}"))
            {
                Repository.Clone(url, $"./{location}", options);
            }
            else
            {
                if (!Repository.IsValid($"./{location}"))
                {
                    Directory.Delete($"./{location}", true);
                    Repository.Clone(url, $"./{location}", options);
                }
                else
                {
                    using (Repository repo = new Repository($"./{location}"))
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

        }

        private void GotCode(RepositoryOperationContext context)
        {
            //CompileSketch();
            Status += $"{SelectedConfig.DisplayName} Code obtained from Repository";
        }

        private void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            Progress = e.ProgressPercentage;
        }

        #endregion
    }
}
