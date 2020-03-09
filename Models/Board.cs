using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseStationInstaller.Models
{

    public class Board
    {
        public Board(string name, string platform)
        {
            Name = name;
            Platform = platform;
        }
        public string Name;
        public string Platform;
    }
}
