using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseStationInstaller.Models
{
    public class MotorShield
    {
        public MotorShield(string name)
        {
            Name = name;
        }
        public string Name { get; set; }
    }
}
