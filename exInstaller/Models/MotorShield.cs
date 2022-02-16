using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace exInstaller.Models
{
    public class MotorShield
    {
        public string Name { get; set; }
        public string Declaration { get; set; }

        public MotorShield()
        {

        }

        public MotorShield(string display,string decString )
        {
            Name = display;
            Declaration = decString;
        }
    }
}
