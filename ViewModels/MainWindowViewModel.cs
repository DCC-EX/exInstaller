using BaseStationInstaller.Models;
using System;
using System.Collections.Generic;
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
            }
        }
    }
}
