using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArduinoUploader.Hardware;

namespace BaseStationInstaller.Models
{

    public class Board
    {
        public Board(string name, ArduinoModel platform)
        {
            Name = name;
            Platform = platform;
        }
        public string Name { get; set; }
        public ArduinoModel Platform { get; set; }
    }
}
