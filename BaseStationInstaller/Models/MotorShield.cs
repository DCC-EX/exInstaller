using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseStationInstaller.Models
{
    public enum MotorShieldType
    {
        Arduino,
        Pololu,
        BTS7960B,
        FireBox_MK1,
        FireBox_MK1S
    }
    public class MotorShield
    {
        public MotorShield(string name, MotorShieldType type)
        {
            Name = name;
            ShieldType = type;
        }
        public string Name { get; set; }
        public MotorShieldType ShieldType { get; set; }
    }
}
