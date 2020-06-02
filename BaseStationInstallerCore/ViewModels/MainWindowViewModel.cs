//using ArduinoUploader;
using BaseStationInstaller.Models;
using BaseStationInstaller.Utils;
using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BaseStationInstaller.ViewModels

{
    public class MainWindowViewModel : ViewModelBase, IMainWindowViewModel
    {
        ArudinoCliHelper helper;
        StreamWriter logWriter;
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
        }


        void InitArduinoCLI()
        {
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
            get
            {
                return _selectedConfig;
            }
            set
            {
                _selectedConfig = value;
                RaisePropertyChanged("SelectedConfig");
                SelectedSupportedBoards = new ObservableCollection<Board>(SelectedConfig.SupportedBoards);
                SelectedSupportedMotorShields = new ObservableCollection<MotorShield>(SelectedConfig.SupportedMotorShields);


            }
        }
        /// <summary>
        /// Updatable colletion of boards pulled from Selected Config
        /// </summary>
        private ObservableCollection<Board> _selectedSupportedBoards;
        public ObservableCollection<Board> SelectedSupportedBoards
        {
            get
            {
                return _selectedSupportedBoards;
            }
            set
            {
                _selectedSupportedBoards = value;
                RaisePropertyChanged("SelectedSupportedBoards");
            }
        }
        /// <summary>
        /// Updatable collection of motorshields supported via Selected Config
        /// </summary>
        private ObservableCollection<MotorShield> _selectedSupportedMotorShields;
        public ObservableCollection<MotorShield> SelectedSupportedMotorShields
        {
            get
            {
                return _selectedSupportedMotorShields;
            }
            set
            {

                _selectedSupportedMotorShields = value;
                RaisePropertyChanged("SelectedSupportedMotorShields");
            }
        }
        /// <summary>
        /// List of comports detected by windows
        /// </summary>
        private ObservableCollection<string> _availableComPorts;
        public ObservableCollection<string> AvailableComPorts
        {
            get
            {
                return _availableComPorts;
            }
            set
            {

                _availableComPorts = value;
                RaisePropertyChanged("AvailableComPorts");
            }
        }

        private Board _selectedBoard;
        public Board SelectedBoard
        {
            get
            {
                return _selectedBoard;
            }
            set
            {
                _selectedBoard = value;
                RaisePropertyChanged("SelectedBoard");
                //if (SelectedMotorShield != null)
                //{
                //    WiringDiagram = BaseStationSettings.GetWiringDiagram(SelectedBoard.Name, SelectedMotorShield.ShieldType);
                //}
            }
        }

        private MotorShield _selectedMotorShield;

        public MotorShield SelectedMotorShield
        {
            get
            {
                return _selectedMotorShield;
            }
            set
            {
                _selectedMotorShield = value;
                RaisePropertyChanged("SelectedMotorShield");
                //if (SelectedBoard != null)
                //{
                //    WiringDiagram = BaseStationSettings.GetWiringDiagram(SelectedBoard.Name, SelectedMotorShield.ShieldType);
                //}
            }
        }

        private string _selectedComPort;

        public string SelectedComPort
        {
            get
            {
                return _selectedComPort;
            }
            set
            {
                _selectedComPort = value;
                RaisePropertyChanged("SelectedComPort");
            }
        }

        private string _status;
        public string Status
        {
            get
            {
                return _status;
            }
            set
            {
                if (value.Contains(Environment.NewLine))
                {
                    _status = value;
                } else
                {
                    _status = value + Environment.NewLine;
                }
                
                if (logWriter != null)
                {
                    /*using (logWriter)
                    {
                        logWriter.Write($"{_status}{Environment.NewLine}");
                        logWriter.Flush();
                    }*/
                }
                RaisePropertyChanged("Status");
            }
        }

        private string _wiringDiagram;
        public string WiringDiagram
        {
            get
            {
                return _wiringDiagram;
            }
            set
            {
                _wiringDiagram = value;
                RaisePropertyChanged("WiringDiagram");
            }
        }

        private bool _refreshingPorts;

        public bool RefreshingPorts
        {
            get
            {
                return _refreshingPorts;
            }
            set
            {
                _refreshingPorts = value;
                RaisePropertyChanged("RefreshingPorts");
            }
        }

        private bool _busy;

        public bool Busy
        {
            get
            {
                return _busy;
            }
            set
            {
                _busy = value;
                RaisePropertyChanged("Busy");
            }
        }

        private int _progress;

        public int Progress
        {
            get
            {
                return _progress;
            }
            set
            {
                _progress = value;
                RaisePropertyChanged("Progress");
            }
        }
        #endregion

        #region Commands
        private DelegateCommand _refreshComPortButton;

        public DelegateCommand RefreshComPortButton
        {
            get
            {
                if (_refreshComPortButton == null)
                {
                    _refreshComPortButton = new DelegateCommand(
                        () =>
                        {
                            Task task = new Task(RefreshComports);
                            task.Start();
                        },
                        () => !RefreshingPorts);
                }

                return _refreshComPortButton;
            }
        }

        private DelegateCommand _compileUpload;

        public DelegateCommand CompileUpload
        {
            get
            {
                if (_compileUpload == null)
                {
                    _compileUpload = new DelegateCommand(() =>
                    {
                        Task task = new Task(ProcessCompileUpload);
                        task.Start();
                    }, () => !Busy);
                }

                return _compileUpload;
            }
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
                CommandManager.InvalidateRequerySuggested();
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
            CommandManager.InvalidateRequerySuggested();
        }

        private async void ProcessCompileUpload()
        {
            Progress = 0;
            Busy = true;
            await DownloadPreReqs();
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
            await GitCode(SelectedConfig.Git,$@".\{SelectedConfig.Name}");
            CompileSketch();
        }

        /*private void GetPlatformIO()
        {
            Status += "Getting Platform IO";
            
            ProcessStartInfo start = new ProcessStartInfo();
            start.Verb = "runas";
            start.FileName = $@"{Directory.GetCurrentDirectory()}\python\python.exe";
            start.Arguments = string.Format("\"{0}\" \"{1}\"", "-I", $@"{ Directory.GetCurrentDirectory()}\get-platformio.py");
            start.UseShellExecute = false;// Do not use OS shell
                                          start.CreateNoWindow = true; // We don't need new window
                                          start.RedirectStandardOutput = true;// Any output, generated by application will be redirected back
                                          start.RedirectStandardError = true; // Any error in standard output will be redirected back (for example exceptions)
            using (Process process = Process.Start(start))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    string stderr = process.StandardError.ReadToEnd(); // Here are the exceptions from our Python script
                    string result = reader.ReadToEnd(); // Here is the result of StdOut(for example: print "test")
                    Console.WriteLine(result);
                }
            }
        }*/


        private void CompileSketch()
        {
            Status += "Changing MotorShield options" + Environment.NewLine;
            Progress = 5;
            string[] config = File.ReadAllLines($@".\{SelectedConfig.Name}\{SelectedConfig.ConfigFile}");
            Progress = 10;
            switch (SelectedConfig.Name) {
                case "BaseStationClassic":
                    config[16] = $"#define MOTOR_SHIELD_TYPE   {(int)SelectedMotorShield.ShieldType}";
                    
                    break;
                case "BaseStationEx":
                    config[17] = $"#define MOTOR_SHIELD_TYPE   {(int)SelectedMotorShield.ShieldType}";
                    break;
                case "CSTest":
                    switch(SelectedMotorShield.ShieldType)
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

        /*private void Client_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            currDep++;
            Status += "Download Completed";
            Progress = 100;
            if (currDep == SelectedConfig.Dependencies.Count)
            {
                foreach (Dependency dep in SelectedConfig.Dependencies)
                {
                    if (!Directory.Exists($@"./{dep.Name}"))
                    {
                        Status += $"Extracting {dep.Name}";
                        if (dep.FileName.Contains(".zip"))
                        {
                            ZipFile.ExtractToDirectory($@"./{dep.FileName}", $@"./{dep.Name}");
                        }
                    }
                }
            }
        }*/

        private async Task GitCode(string url, string location)
        {
            CloneOptions options = new CloneOptions();
            options.RepositoryOperationCompleted = new LibGit2Sharp.Handlers.RepositoryOperationCompleted(GotCode);
            Progress = 0;
            if (!Directory.Exists($"./{location}"))
            {
                Repository.Clone(url, $"./{location}", options);
            } else
            {
                if (!Repository.IsValid($"./{location}") )
                {
                    Directory.Delete($"./{location}",true);
                    Repository.Clone(url, $"./{location}", options);
                } 
                else
                {
                    using (Repository repo = new Repository($"./{location}"))
                    {
                        RepositoryStatus status = repo.RetrieveStatus();
                        if (status.IsDirty)
                        {
                            repo.Reset(ResetMode.Hard);
                        }
                        Commands.Pull(repo, new Signature(new Identity("random", "random@random.com"), DateTimeOffset.Now), new PullOptions());
                    }
                }
            }

        }

        private void GotCode(RepositoryOperationContext context)
        { 
           CompileSketch();
        }

        private void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            Progress = e.ProgressPercentage;
        }

#endregion
    }
}
