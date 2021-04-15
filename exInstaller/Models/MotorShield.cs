using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace exInstaller.Models
{
    public enum MotorShieldType
    {
        Arduino,
        Pololu,
        BTS7960B,
        FireBox_MK1,
        FireBox_MK1S,
        Fundumoto
    }


    public class MotorShield
    {

        public static Dictionary<MotorShieldType, string> ExMotoShieldDictonary = new Dictionary<MotorShieldType, string>()
        {
            { MotorShieldType.Arduino, "STANDARD_MOTOR_SHIELD"},
            {MotorShieldType.Pololu,  "POLOLU_MOTOR_SHIELD"},
            {MotorShieldType.Fundumoto,  "FUNDUMOTO_SHIELD"},
            {MotorShieldType.FireBox_MK1,  "FIREBOX_MK1"},
            {MotorShieldType.FireBox_MK1S,  "FIREBOX_MK1S"},

        };
        public MotorShield(string name, MotorShieldType type)
        {
            Name = name;
            ShieldType = type;
        }

        public MotorShield() { }
        public string Name { get; set; }
        public MotorShieldType ShieldType { get; set; }
    }
}
