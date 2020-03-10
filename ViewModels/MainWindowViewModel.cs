using BaseStationInstaller.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BaseStationInstaller.ViewModels

{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
            SelectedConfig = BaseStationSettings.BaseStationDefaults[0];
            RefreshComports();
        }

        #region Bindings
        public IList<Config> BaseStations
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
                SelectedBoard = SelectedSupportedBoards[0];
                SelectedSupportedMotorShields = new ObservableCollection<MotorShield>(SelectedConfig.SupportedMotorShields);
                SelectedMotorShield = SelectedSupportedMotorShields[0];
                WiringDiagram = SelectedConfig.WiringDiagram;
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
            Status = "SOON™";
        }


        private void DownloadPreReqs()
        {
            foreach(string link in SelectedConfig.Dependencies)
            {
               
            }
        }
       
        #endregion
    }
}
