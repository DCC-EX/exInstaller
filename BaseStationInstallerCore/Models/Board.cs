using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using ArduinoUploader.Hardware;

namespace BaseStationInstaller.Models
{

    public class Board
    {
        public Board(string name, /*ArduinoModel platform,*/ string fqbn)
        {
            Name = name;
            //Platform = platform;
            FQBN = fqbn;
        }
        public string Name { get; set; }
        //public ArduinoModel Platform { get; set; }

        public string FQBN;
    }
}
