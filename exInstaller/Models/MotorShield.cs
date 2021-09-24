using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace exInstaller.Models
{
    public enum MotorShieldType
    {
        ARDUINO,
        POLOLU,
        BTS7960B,
        FIREBOX_MK1,
        FIREBOX_MK1S,
        FUNDUMOTO,
        IBT_2_WITH_ARDUINO
    }


    public class MotorShield
    {

        public static Dictionary<MotorShieldType, string> ExMotoShieldDictonary = new Dictionary<MotorShieldType, string>()
        {
            { MotorShieldType.ARDUINO, "STANDARD_MOTOR_SHIELD"},
            {MotorShieldType.POLOLU,  "POLOLU_MOTOR_SHIELD"},
            {MotorShieldType.FUNDUMOTO,  "FUNDUMOTO_SHIELD"},
            {MotorShieldType.FIREBOX_MK1,  "FIREBOX_MK1"},
            {MotorShieldType.FIREBOX_MK1S,  "FIREBOX_MK1S"},
            {MotorShieldType.IBT_2_WITH_ARDUINO, "IBT_2_WITH_ARDUINO" }

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
