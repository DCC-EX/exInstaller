using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;


namespace BaseStationInstaller.Models
{
    public struct Config
    {
        public string Name { get; set; }
        public string Git { get; set; }
        public List<Library> Libraries { get; set; }        
        public List<Board> SupportedBoards { get; set; }
        public List<MotorShield> SupportedMotorShields { get; set; }
        public string DisplayName { get; set; }
        public string InputFileLocation { get; set; }
        public bool AllowAdvanced { get; set; }
        public string ConfigFile { get; set; }


    }

    public struct Library
    {
        public string Name { get; set; }

        public string Repo { get; set; }
        public string Location { get; set; }

        public bool LibraryDownloadAvailable { get; set; }
    }

    public static class BaseStationSettings
    {
        public static List<Config> BaseStationDefaults = new List<Config>()
        {
            {
                new Config
                {
                    DisplayName = "Base Station Extended",
                    Name = "BaseStationEX",
                    Git = "https://github.com/DCC-EX/BaseStation-EX.git",
                    ConfigFile = $@"DCCpp_EX/DCCppEX/Config.h",
                    InputFileLocation = "DCCpp_EX/DCCppEX",
                   Libraries = new List<Library>(),
                   
                   AllowAdvanced = true,
                    SupportedBoards = new List<Board>()
                    {
                        new Board("Uno", "arduino:avr:uno", new List<Platform>()
                   {
                       new Platform("avr", "arduino")
                   }),
                        new Board("Mega", "arduino:avr:mega",  new List<Platform>()
                   {
                       new Platform("avr", "arduino")
                   }),
                    },
                    SupportedMotorShields = new List<MotorShield>()
                    {
                        new MotorShield("Arduino Motor Shield", MotorShieldType.Arduino),
                        new MotorShield("Pololu MC33926 Motor Shield", MotorShieldType.Pololu),
                        new MotorShield("BTS7960B Motor Shield", MotorShieldType.BTS7960B),
                    }
                }
            },
            {
                new Config
                {
                    Name = "BaseStationClassic",
                    DisplayName = "Base Station Classic",
                    ConfigFile = @"DCCpp/Config.h",
                    InputFileLocation = "DCCpp",
                    Git = "https://github.com/DCC-EX/BaseStation-Classic.git",
                    Libraries = new List<Library>(),
                    AllowAdvanced = false,
                    SupportedBoards = new List<Board>()
                    {
                        new Board("Uno",  "arduino:avr:uno",  new List<Platform>()
                   {
                       new Platform("avr", "arduino")
                   }),
                        new Board("Mega",  "arduino:avr:mega",  new List<Platform>()
                   {
                       new Platform("avr", "arduino")
                   }),
                    },
                    SupportedMotorShields = new List<MotorShield>()
                    {
                        new MotorShield("Arduino Motor Shield", MotorShieldType.Arduino),
                        new MotorShield("Pololu MC33926 Motor Shield", MotorShieldType.Pololu),
                    },
                }
            },{
            new Config
                {
                    DisplayName = "CommandStation EX",
                    Name = "CommandStation-EX",
                    Git = "https://github.com/DCC-EX/CommandStation-EX.git",
                    ConfigFile = @"Config.h",
                    InputFileLocation =  @"",
                    AllowAdvanced = true,
                    Libraries = new List<Library>()
                    {
                        new Library{ Name = "DIO2", Location = $@"libraries/DIO2", Repo = "https://github.com/Locoduino/DIO2.git", LibraryDownloadAvailable = true },
                        new Library{ Name = "CommandStation", Location = $@"libraries/CommandStation", Repo = "https://github.com/DCC-EX/CommandStation.git", LibraryDownloadAvailable = false },
                        new Library{ Name = "ArduinoTimers", Location = $@"libraries/ArduinoTimers", Repo = "https://github.com/davidcutting42/ArduinoTimers.git", LibraryDownloadAvailable = false },
                       new Library{ Name = "SparkFun External EEPROM Arduino Library", Location = $@"libraries/SparkFun_External_EEPROM_Arduino_Library", Repo = "https://github.com/sparkfun/SparkFun_External_EEPROM_Arduino_Library.git", LibraryDownloadAvailable = true}
                    },
                   
                    SupportedBoards = new List<Board>()
                    {
                        new Board("Uno", /*ArduinoModel.UnoR3,*/ "arduino:avr:uno",new List<Platform>()
                   {
                       new Platform("avr", "arduino"),
                   }),
                        new Board("Mega", /*ArduinoModel.Mega2560,*/ "arduino:avr:mega",new List<Platform>()
                   {
                       new Platform("avr", "arduino"),
                       
                   }),
                        new Board("SAM21", /*ArduinoModel.Mega2560,*/ "SparkFun:samd", new List<Platform>()
                   {
                       new Platform("avr", "arduino"),
                       new Platform("samd", "arduino"),
                       new Platform("samd", "SparkFun")
                   }),
                    },
                    SupportedMotorShields = new List<MotorShield>()
                    {
                        new MotorShield("Arduino Motor Shield", MotorShieldType.Arduino),
                        new MotorShield("Pololu MC33926 Motor Shield", MotorShieldType.Pololu),
                        new MotorShield("FireBox MK1", MotorShieldType.FireBox_MK1),
                        new MotorShield("FireBox MK1S", MotorShieldType.FireBox_MK1S),
                        //new MotorShield("BTS7960B Motor Shield", MotorShieldType.BTS7960B),
                    }
                }
            },
        };
        public static string CalculateMD5(string filename)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }
    }
}
