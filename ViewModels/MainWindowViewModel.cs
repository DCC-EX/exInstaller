using BaseStationInstaller.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseStationInstaller.ViewModels

{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
            SelectedConfig = BaseStationSettings.BaseStationDefaults.ElementAt(0);
        }

        public IDictionary<string, Config> BaseStations
        {
            get
            {
                return BaseStationSettings.BaseStationDefaults;
            }
        }

        private KeyValuePair<string,Config> _selectedConfig;
        public KeyValuePair<string, Config> SelectedConfig
        {
            get
            {
                return _selectedConfig;
            }
            set
            {
                _selectedConfig = value;
                RaisePropertyChanged("SelectedConfig");
                SelectedSupportedBoards = new ObservableCollection<Board>(SelectedConfig.Value.SupportedBoards);
                SelectedBoard = SelectedSupportedBoards[0];
                SelectedSupportedMotorShields = new ObservableCollection<MotorShield>(SelectedConfig.Value.SupportedMotorShields);
                SelectedMotorShield = SelectedSupportedMotorShields[0];
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
    }
}
