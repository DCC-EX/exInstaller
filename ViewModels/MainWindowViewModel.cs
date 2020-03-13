using ArduinoUploader;
using BaseStationInstaller.Models;
using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public class MainWindowViewModel : ViewModelBase, IArduinoUploaderLogger
    {
        public MainWindowViewModel()
        {
            WiringDiagram = "pack://application:,,,/Resources/dcc-ex-logo.png";
            SelectedConfig = BaseStationSettings.BaseStationDefaults[0];
            Task task = new Task(RefreshComports);
            task.Start();
        }

        #region Bindings
        public List<Config> BaseStations
        {
            get
            {
                return BaseStationSettings.BaseStationDefaults;
            }
        }

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
                if (SelectedMotorShield != null)
                {
                    WiringDiagram = BaseStationSettings.GetWiringDiagram(SelectedBoard.Platform, SelectedMotorShield.ShieldType);
                }
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
                if (SelectedBoard != null)
                {
                    WiringDiagram = BaseStationSettings.GetWiringDiagram(SelectedBoard.Platform, SelectedMotorShield.ShieldType);
                }
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
                _status = value;
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
        private async void RefreshComports()
        {
            Busy = true;
            RefreshingPorts = true;
            Progress = 0;
            Status = "Refreshing Ports...";
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
            Status = "Idle";
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
            Status = "Starting Dependency Downloads";
            Thread.Sleep(500);
            foreach (Dependency dep in SelectedConfig.Dependencies)
            {
                Status = $"Downloading {dep.Name}";
                if (!File.Exists($@"./{dep.FileName}"))
                {
                    using (WebClient client = new WebClient())
                    {
                        client.DownloadFileCompleted += Client_DownloadFileCompleted; ;
                        client.DownloadProgressChanged += Client_DownloadProgressChanged;
                        await client.DownloadFileTaskAsync(new Uri(dep.Link), $@"./{dep.FileName}");
                    }
                } 
                else
                {
                    if (!Directory.Exists($@".\{dep.Name}") && dep.FileName.Contains(".zip"))
                    {
                        if (dep.checksum.Equals(BaseStationSettings.CalculateMD5($@"./{dep.Name}"))) {
                            ZipFile.ExtractToDirectory($@"./{dep.FileName}", $@"./{dep.Name}");
                        } 
                        else
                        {
                            File.Delete($@"./{dep.Name}");
                            Busy = false;
                            RefreshingPorts = false;
                            Thread.Sleep(5000);
                            Status = "Prequisite download failed please try again";
                            CommandManager.InvalidateRequerySuggested();
                            return;
                        }
                    }
                }
            }
            Status = $"Gitting {SelectedConfig.DisplayName}";
            if (SelectedConfig.Name == "BaseStation")
            {
                GetPlatformIO();
            }
            await GitCode();
        }

        private void GetPlatformIO()
        {
            Status = "Getting Platform IO";
            
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
        }


        private void CompileSketch()
        {
            Status = "Changing MotorShield options";
            Progress = 5;
            string[] config = File.ReadAllLines(@".\BaseStationClassic\DCCpp\Config.h");
            Progress = 10;
            config[16] = $"#define MOTOR_SHIELD_TYPE   {(int)SelectedMotorShield.ShieldType}";
            Progress = 15;
            File.WriteAllLines(@".\BaseStationClassic\DCCpp\Config.h", config);
            Progress = 20;
            Thread.Sleep(1000);
            Status = "Compiling Base Station Classic Sketch";
            
            if (!Directory.Exists(@".\BaseStationClassic\Build"))
            {
                Directory.CreateDirectory(@".\BaseStationClassic\Build");
            }
            if (!Directory.Exists(@".\BaseStationClassic\Cache"))
            {
                Directory.CreateDirectory(@".\BaseStationClassic\Cache");
            }
            Progress = 25;
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = $@".\arduino-1.8.12\arduino-1.8.12\arduino-builder.exe";
            start.Arguments = String.Format($@"{SelectedConfig.InitCommand}", SelectedBoard.FQBN);
            start.UseShellExecute = true;
            start.WindowStyle = ProcessWindowStyle.Hidden;
            Process process = new Process();
            process.StartInfo = start;
            process.Start();
            process.WaitForExit();
            Progress = 50;
            start.Arguments = String.Format($@"{SelectedConfig.BuildCommand}", SelectedBoard.FQBN);
            Console.WriteLine(start.Arguments);
            process.Start();
            process.WaitForExit();
            Status = $"Compilation Complete";
            RefreshingPorts = true;
            Thread.Sleep(1000);
            Status = $"Uploading to {SelectedComPort}";
            Progress = 75;
            if (File.Exists(@"./upload.log"))
            {
                File.Delete(@"./upload.log");
            }
            ArduinoSketchUploader uploader = new ArduinoSketchUploader(
             new ArduinoSketchUploaderOptions()
             {
                 FileName = $@"{Directory.GetCurrentDirectory()}\BaseStationClassic\Build\DCCpp.ino.hex",
                 PortName = SelectedComPort,
                 ArduinoModel = SelectedBoard.Platform
             },this);
            try
            {
                uploader.UploadSketch();
                Status = "Upload Completed Successfully. Please check upload.log for more details";
            }
            catch (Exception e)
            {
                Status = "Upload Failed!!! Please check upload.log for more details";               
                File.AppendAllText(@"./upload.log", $"Message: {e.Message} {Environment.NewLine} StackTrace: {e.StackTrace}");
            }
            Progress = 100;
            Thread.Sleep(1000);
            Progress = 0;
            Busy = false;
            RefreshingPorts = false;
        }
        private void Client_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            currDep++;
            Status = "Download Completed";
            Progress = 100;
            if (currDep == SelectedConfig.Dependencies.Count)
            {
                foreach (Dependency dep in SelectedConfig.Dependencies)
                {
                    if (!Directory.Exists($@"./{dep.Name}"))
                    {
                        Status = $"Extracting {dep.Name}";
                        if (dep.FileName.Contains(".zip"))
                        {
                            ZipFile.ExtractToDirectory($@"./{dep.FileName}", $@"./{dep.Name}");
                        }
                    }
                }
            }
        }

        private async Task GitCode()
        {
            CloneOptions options = new CloneOptions();
            options.RepositoryOperationCompleted = new LibGit2Sharp.Handlers.RepositoryOperationCompleted(GotCode);
            Progress = 0;
            if (!Directory.Exists($"./{SelectedConfig.Name}"))
            {
                Repository.Clone(SelectedConfig.Git, $"./{SelectedConfig.Name}", options);
            } else
            {
                if (!Repository.IsValid($"./{SelectedConfig.Name}") )
                {
                    Directory.Delete($"./{SelectedConfig.Name}",true);
                    Repository.Clone(SelectedConfig.Git, $"./{SelectedConfig.Name}", options);
                } 
                else
                {
                    CompileSketch();
                }
            }

        }

        private void GotCode(RepositoryOperationContext context)
        { 
            if (SelectedConfig.Name.Equals("BaseStationClassic")) {
                CompileSketch();
            }
        }

        private void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            Progress = e.ProgressPercentage;
        }

        public void Error(string message, Exception e)
        {
            File.AppendAllText(@"./upload.log", $"Message: {e.Message} {Environment.NewLine} StackTrace: {e.StackTrace}");
        }

        public void Warn(string message)
        {
            File.AppendAllText(@"./upload.log", $"{message}{Environment.NewLine}");
        }

        public void Info(string message)
        {
            Status = message;
            File.AppendAllText(@"./upload.log", $"{message}{Environment.NewLine}");
        }

        public void Debug(string message)
        {
#if DEBUG
            File.AppendAllText(@"./upload.log", $"{message}");
#endif
        }

        public void Trace(string message)
        {
#if DEBUG
            File.AppendAllText(@"./upload.log", $"{message}");
#endif
        }

#endregion
    }
}
