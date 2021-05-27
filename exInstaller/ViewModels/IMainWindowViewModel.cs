using exInstaller.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace exInstaller.ViewModels
{
    public interface IMainWindowViewModel
    {
        public string Status { get; set; }
        public Tuple<string, string> SelectedComPort { get; set; }

        public int Progress { get; set; }

        public Board SelectedBoard { get; set; }

        public ObservableCollection<Tuple<string, string>> AvailableComPorts { get; set; }

        public Config SelectedConfig { get; }
        public bool Busy { get; set; }
        public bool RefreshingPorts { get; set; }
    }
}
