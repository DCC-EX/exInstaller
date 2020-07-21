using BaseStationInstaller.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseStationInstaller.ViewModels
{
    public interface IMainWindowViewModel
    {
        public string Status { get; set; }
        public string SelectedComPort { get; set; }

        public int Progress { get; set; }

        public Board SelectedBoard { get; set; }

        public Config SelectedConfig { get; }
        public bool Busy { get; set; }
    }
}
