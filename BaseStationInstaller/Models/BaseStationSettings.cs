using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ArduinoUploader.Hardware;

namespace BaseStationInstaller.Models
{
    public struct Config
    {
        public string Name { get; set; }
        public string Git { get; set; }
        public List<Dependency> Dependencies { get; set; }
        public string BuildCommand { get; set; }
        public string InitCommand { get; set; }
        public List<Board> SupportedBoards { get; set; }
        public List<MotorShield> SupportedMotorShields { get; set; }
        public string DisplayName { get; set; }
        public string OutputFileName { get; set; }

        public string ConfigFile { get; set; }


    }

    public struct Dependency
    {
        public string Name { get; set; }

        public string Link { get; set; }
        public string FileName { get; set; }

        public string checksum { get; set; }
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
                    OutputFileName = "DCCppEX",
                    ConfigFile = @"DCCpp_EX\DCCppEX\Config.h",
                    //Dependencies = new List<Dependency>()
                    //{
                    //    new Dependency{Name = "platformio", FileName = "get-platformio.py", Link = "https://raw.githubusercontent.com/platformio/platformio/develop/scripts/get-platformio.py" },
                    //    new Dependency{Name = "python", FileName = "python.zip", Link = "https://www.python.org/ftp/python/3.8.2/python-3.8.2-embed-amd64.zip", checksum = "1a98565285491c0ea65450e78afe6f8d" }
                    //},
                    Dependencies = new List<Dependency>()
                    {
                        new Dependency{ Name = "arduino-1.8.12", FileName = "arduino-1.8.12.zip", Link = "https://downloads.arduino.cc/arduino-1.8.12-windows.zip", checksum = "92471d21a38c33a8095dc243cd7e8e28" }
                    },
                    //InitCommand = "platformio init",
                    InitCommand = $@"-dump-prefs -logger=machine -hardware { Directory.GetCurrentDirectory()}\arduino-1.8.12\arduino-1.8.12\hardware -tools { Directory.GetCurrentDirectory()}\arduino-1.8.12\arduino-1.8.12\tools-builder -tools { Directory.GetCurrentDirectory()}\arduino-1.8.12\arduino-1.8.12\hardware\tools\avr -built-in-libraries { Directory.GetCurrentDirectory()}\arduino-1.8.12\arduino-1.8.12\libraries -fqbn={{0}} -ide-version=10812 -build-path { Directory.GetCurrentDirectory()}\BaseStationEX\Build -warnings=all -build-cache { Directory.GetCurrentDirectory()}\BaseStationEX\Cache -prefs=build.warn_data_percentage=75 -prefs=runtime.tools.avr-gcc.path={ Directory.GetCurrentDirectory()}\arduino-1.8.12\arduino-1.8.12\hardware\tools\avr -prefs=runtime.tools.avr-gcc-7.3.0-atmel3.6.1-arduino5.path={ Directory.GetCurrentDirectory()}\arduino-1.8.12\arduino-1.8.12\hardware\tools\avr -prefs=runtime.tools.arduinoOTA.path={ Directory.GetCurrentDirectory()}\arduino-1.8.12\arduino-1.8.12\hardware\tools\avr -prefs=runtime.tools.arduinoOTA-1.3.0.path={ Directory.GetCurrentDirectory()}\arduino-1.8.12\arduino-1.8.12\hardware\tools\avr -prefs=runtime.tools.avrdude.path={ Directory.GetCurrentDirectory()}\arduino-1.8.12\arduino-1.8.12\hardware\tools\avr -prefs=runtime.tools.avrdude-6.3.0-arduino17.path={ Directory.GetCurrentDirectory()}\arduino-1.8.12\arduino-1.8.12\hardware\tools\avr -verbose { Directory.GetCurrentDirectory()}\BaseStationEX\DCCpp_EX\DCCppEX\DCCppEX.ino",

                    //BuildCommand = "platformio run",
                    BuildCommand = $@"-compile -logger=machine -hardware { Directory.GetCurrentDirectory()}\arduino-1.8.12\arduino-1.8.12\hardware -tools { Directory.GetCurrentDirectory()}\arduino-1.8.12\arduino-1.8.12\tools-builder -tools { Directory.GetCurrentDirectory()}\arduino-1.8.12\arduino-1.8.12\hardware\tools\avr -built-in-libraries { Directory.GetCurrentDirectory()}\arduino-1.8.12\arduino-1.8.12\libraries -fqbn={{0}} -ide-version=10812 -build-path { Directory.GetCurrentDirectory()}\BaseStationEX\Build -warnings=all -build-cache { Directory.GetCurrentDirectory()}\BaseStationEX\Cache -prefs=build.warn_data_percentage=75 -prefs=runtime.tools.avr-gcc.path={ Directory.GetCurrentDirectory()}\arduino-1.8.12\arduino-1.8.12\hardware\tools\avr -prefs=runtime.tools.avr-gcc-7.3.0-atmel3.6.1-arduino5.path={ Directory.GetCurrentDirectory()}\arduino-1.8.12\arduino-1.8.12\hardware\tools\avr -prefs=runtime.tools.arduinoOTA.path={ Directory.GetCurrentDirectory()}\arduino-1.8.12\arduino-1.8.12\hardware\tools\avr -prefs=runtime.tools.arduinoOTA-1.3.0.path={ Directory.GetCurrentDirectory()}\arduino-1.8.12\arduino-1.8.12\hardware\tools\avr -prefs=runtime.tools.avrdude.path={ Directory.GetCurrentDirectory()}\arduino-1.8.12\arduino-1.8.12\hardware\tools\avr -prefs=runtime.tools.avrdude-6.3.0-arduino17.path={ Directory.GetCurrentDirectory()}\arduino-1.8.12\arduino-1.8.12\hardware\tools\avr -verbose { Directory.GetCurrentDirectory()}\BaseStationEX\DCCpp_EX\DCCppEX\DCCppEX.ino",

                    SupportedBoards = new List<Board>()
                    {
                        new Board("Uno", ArduinoModel.UnoR3, "arduino:avr:uno"),
                        new Board("Mega", ArduinoModel.Mega2560, "arduino:avr:mega:cpu=atmega2560"),
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
                    OutputFileName = "DCCpp",
                    ConfigFile = @"DCCpp\Config.h",
                    Git = "https://github.com/DCC-EX/BaseStation-Classic.git",
                    Dependencies = new List<Dependency>()
                    {
                        new Dependency{ Name = "arduino-1.8.12", FileName = "arduino-1.8.12.zip", Link = "https://downloads.arduino.cc/arduino-1.8.12-windows.zip", checksum = "92471d21a38c33a8095dc243cd7e8e28" }
                    },
                    //BuildCommand = $@"{ Directory.GetCurrentDirectory()}\arduino-1.8.12\arduino-1.8.12\arduino-builder -compile -logger=machine -hardware { Directory.GetCurrentDirectory()}\arduino-1.8.12\arduino-1.8.12\hardware -tools { Directory.GetCurrentDirectory()}\arduino-1.8.12\arduino-1.8.12\tools-builder -tools { Directory.GetCurrentDirectory()}\arduino-1.8.12\arduino-1.8.12\hardware\tools\avr -built-in-libraries { Directory.GetCurrentDirectory()}\arduino-1.8.12\arduino-1.8.12\libraries -fqbn=arduino:avr:uno -ide-version=10812 -build-path { Directory.GetCurrentDirectory()}\BaseStationClassic\Build -warnings=all -build-cache { Directory.GetCurrentDirectory()}\BaseStationClassic\Cache -prefs=build.warn_data_percentage=75 -prefs=runtime.tools.avr-gcc.path={ Directory.GetCurrentDirectory()}\arduino-1.8.12\arduino-1.8.12\hardware\tools\avr -prefs=runtime.tools.avr-gcc-7.3.0-atmel3.6.1-arduino5.path={ Directory.GetCurrentDirectory()}\arduino-1.8.12\arduino-1.8.12\hardware\tools\avr -prefs=runtime.tools.arduinoOTA.path={ Directory.GetCurrentDirectory()}\arduino-1.8.12\arduino-1.8.12\hardware\tools\avr -prefs=runtime.tools.arduinoOTA-1.3.0.path={ Directory.GetCurrentDirectory()}\arduino-1.8.12\arduino-1.8.12\hardware\tools\avr -prefs=runtime.tools.avrdude.path={ Directory.GetCurrentDirectory()}\arduino-1.8.12\arduino-1.8.12\hardware\tools\avr -prefs=runtime.tools.avrdude-6.3.0-arduino17.path={ Directory.GetCurrentDirectory()}\arduino-1.8.12\arduino-1.8.12\hardware\tools\avr -verbose { Directory.GetCurrentDirectory()}\BaseStationClassic\DCCpp\DCCpp.ino",
                    BuildCommand = $@"-compile -logger=machine -hardware { Directory.GetCurrentDirectory()}\arduino-1.8.12\arduino-1.8.12\hardware -tools { Directory.GetCurrentDirectory()}\arduino-1.8.12\arduino-1.8.12\tools-builder -tools { Directory.GetCurrentDirectory()}\arduino-1.8.12\arduino-1.8.12\hardware\tools\avr -built-in-libraries { Directory.GetCurrentDirectory()}\arduino-1.8.12\arduino-1.8.12\libraries -fqbn={{0}} -ide-version=10812 -build-path { Directory.GetCurrentDirectory()}\BaseStationClassic\Build -warnings=all -build-cache { Directory.GetCurrentDirectory()}\BaseStationClassic\Cache -prefs=build.warn_data_percentage=75 -prefs=runtime.tools.avr-gcc.path={ Directory.GetCurrentDirectory()}\arduino-1.8.12\arduino-1.8.12\hardware\tools\avr -prefs=runtime.tools.avr-gcc-7.3.0-atmel3.6.1-arduino5.path={ Directory.GetCurrentDirectory()}\arduino-1.8.12\arduino-1.8.12\hardware\tools\avr -prefs=runtime.tools.arduinoOTA.path={ Directory.GetCurrentDirectory()}\arduino-1.8.12\arduino-1.8.12\hardware\tools\avr -prefs=runtime.tools.arduinoOTA-1.3.0.path={ Directory.GetCurrentDirectory()}\arduino-1.8.12\arduino-1.8.12\hardware\tools\avr -prefs=runtime.tools.avrdude.path={ Directory.GetCurrentDirectory()}\arduino-1.8.12\arduino-1.8.12\hardware\tools\avr -prefs=runtime.tools.avrdude-6.3.0-arduino17.path={ Directory.GetCurrentDirectory()}\arduino-1.8.12\arduino-1.8.12\hardware\tools\avr -verbose { Directory.GetCurrentDirectory()}\BaseStationClassic\DCCpp\DCCpp.ino",

                    SupportedBoards = new List<Board>()
                    {
                        new Board("Uno", ArduinoModel.UnoR3, "arduino:avr:uno"),
                        new Board("Mega", ArduinoModel.Mega2560, "arduino:avr:mega:cpu=atmega2560"),
                    },
                    SupportedMotorShields = new List<MotorShield>()
                    {
                        new MotorShield("Arduino Motor Shield", MotorShieldType.Arduino),
                        new MotorShield("Pololu MC33926 Motor Shield", MotorShieldType.Pololu),
                    },
                    //InitCommand = $@"{ Directory.GetCurrentDirectory()}\arduino-1.8.12\arduino-1.8.12\arduino-builder -dump-prefs -logger=machine -hardware { Directory.GetCurrentDirectory()}\arduino-1.8.12\arduino-1.8.12\hardware -tools { Directory.GetCurrentDirectory()}\arduino-1.8.12\arduino-1.8.12\tools-builder -tools { Directory.GetCurrentDirectory()}\arduino-1.8.12\arduino-1.8.12\hardware\tools\avr -built-in-libraries { Directory.GetCurrentDirectory()}\arduino-1.8.12\arduino-1.8.12\libraries -fqbn=arduino:avr:uno -ide-version=10812 -build-path { Directory.GetCurrentDirectory()}\BaseStationClassic\Build -warnings=all -build-cache { Directory.GetCurrentDirectory()}\BaseStationClassic\Cache -prefs=build.warn_data_percentage=75 -prefs=runtime.tools.avr-gcc.path={ Directory.GetCurrentDirectory()}\arduino-1.8.12\arduino-1.8.12\hardware\tools\avr -prefs=runtime.tools.avr-gcc-7.3.0-atmel3.6.1-arduino5.path={ Directory.GetCurrentDirectory()}\arduino-1.8.12\arduino-1.8.12\hardware\tools\avr -prefs=runtime.tools.arduinoOTA.path={ Directory.GetCurrentDirectory()}\arduino-1.8.12\arduino-1.8.12\hardware\tools\avr -prefs=runtime.tools.arduinoOTA-1.3.0.path={ Directory.GetCurrentDirectory()}\arduino-1.8.12\arduino-1.8.12\hardware\tools\avr -prefs=runtime.tools.avrdude.path={ Directory.GetCurrentDirectory()}\arduino-1.8.12\arduino-1.8.12\hardware\tools\avr -prefs=runtime.tools.avrdude-6.3.0-arduino17.path={ Directory.GetCurrentDirectory()}\arduino-1.8.12\arduino-1.8.12\hardware\tools\avr -verbose { Directory.GetCurrentDirectory()}\BaseStationClassic\DCCpp\DCCpp.ino"
                    InitCommand = $@"-dump-prefs -logger=machine -hardware { Directory.GetCurrentDirectory()}\arduino-1.8.12\arduino-1.8.12\hardware -tools { Directory.GetCurrentDirectory()}\arduino-1.8.12\arduino-1.8.12\tools-builder -tools { Directory.GetCurrentDirectory()}\arduino-1.8.12\arduino-1.8.12\hardware\tools\avr -built-in-libraries { Directory.GetCurrentDirectory()}\arduino-1.8.12\arduino-1.8.12\libraries -fqbn={{0}} -ide-version=10812 -build-path { Directory.GetCurrentDirectory()}\BaseStationClassic\Build -warnings=all -build-cache { Directory.GetCurrentDirectory()}\BaseStationClassic\Cache -prefs=build.warn_data_percentage=75 -prefs=runtime.tools.avr-gcc.path={ Directory.GetCurrentDirectory()}\arduino-1.8.12\arduino-1.8.12\hardware\tools\avr -prefs=runtime.tools.avr-gcc-7.3.0-atmel3.6.1-arduino5.path={ Directory.GetCurrentDirectory()}\arduino-1.8.12\arduino-1.8.12\hardware\tools\avr -prefs=runtime.tools.arduinoOTA.path={ Directory.GetCurrentDirectory()}\arduino-1.8.12\arduino-1.8.12\hardware\tools\avr -prefs=runtime.tools.arduinoOTA-1.3.0.path={ Directory.GetCurrentDirectory()}\arduino-1.8.12\arduino-1.8.12\hardware\tools\avr -prefs=runtime.tools.avrdude.path={ Directory.GetCurrentDirectory()}\arduino-1.8.12\arduino-1.8.12\hardware\tools\avr -prefs=runtime.tools.avrdude-6.3.0-arduino17.path={ Directory.GetCurrentDirectory()}\arduino-1.8.12\arduino-1.8.12\hardware\tools\avr -verbose { Directory.GetCurrentDirectory()}\BaseStationClassic\DCCpp\DCCpp.ino"
                }
            },{
            new Config
                {
                    DisplayName = "Daves CommandStation Test",
                    Name = "CSTest",
                    Git = "https://github.com/davidcutting42/CommandStation-DCC.git",
                    OutputFileName = "DCCppEX",
                    ConfigFile = @"DCCpp_EX\DCCppEX\Config.h",
                    //Dependencies = new List<Dependency>()
                    //{
                    //    new Dependency{Name = "platformio", FileName = "get-platformio.py", Link = "https://raw.githubusercontent.com/platformio/platformio/develop/scripts/get-platformio.py" },
                    //    new Dependency{Name = "python", FileName = "python.zip", Link = "https://www.python.org/ftp/python/3.8.2/python-3.8.2-embed-amd64.zip", checksum = "1a98565285491c0ea65450e78afe6f8d" }
                    //},
                    Dependencies = new List<Dependency>()
                    {
                        new Dependency{ Name = "arduino-1.8.12", FileName = "arduino-1.8.12.zip", Link = "https://downloads.arduino.cc/arduino-1.8.12-windows.zip", checksum = "92471d21a38c33a8095dc243cd7e8e28" }
                    },
                    //InitCommand = "platformio init",
                    InitCommand = $@"-dump-prefs -logger=machine -hardware { Directory.GetCurrentDirectory()}\arduino-1.8.12\arduino-1.8.12\hardware -tools { Directory.GetCurrentDirectory()}\arduino-1.8.12\arduino-1.8.12\tools-builder -tools { Directory.GetCurrentDirectory()}\arduino-1.8.12\arduino-1.8.12\hardware\tools\avr -built-in-libraries { Directory.GetCurrentDirectory()}\arduino-1.8.12\arduino-1.8.12\libraries -fqbn={{0}} -ide-version=10812 -build-path { Directory.GetCurrentDirectory()}\BaseStationEX\Build -warnings=all -build-cache { Directory.GetCurrentDirectory()}\BaseStationEX\Cache -prefs=build.warn_data_percentage=75 -prefs=runtime.tools.avr-gcc.path={ Directory.GetCurrentDirectory()}\arduino-1.8.12\arduino-1.8.12\hardware\tools\avr -prefs=runtime.tools.avr-gcc-7.3.0-atmel3.6.1-arduino5.path={ Directory.GetCurrentDirectory()}\arduino-1.8.12\arduino-1.8.12\hardware\tools\avr -prefs=runtime.tools.arduinoOTA.path={ Directory.GetCurrentDirectory()}\arduino-1.8.12\arduino-1.8.12\hardware\tools\avr -prefs=runtime.tools.arduinoOTA-1.3.0.path={ Directory.GetCurrentDirectory()}\arduino-1.8.12\arduino-1.8.12\hardware\tools\avr -prefs=runtime.tools.avrdude.path={ Directory.GetCurrentDirectory()}\arduino-1.8.12\arduino-1.8.12\hardware\tools\avr -prefs=runtime.tools.avrdude-6.3.0-arduino17.path={ Directory.GetCurrentDirectory()}\arduino-1.8.12\arduino-1.8.12\hardware\tools\avr -verbose { Directory.GetCurrentDirectory()}\BaseStationEX\DCCpp_EX\DCCppEX\DCCppEX.ino",

                    //BuildCommand = "platformio run",
                    BuildCommand = $@"-compile -logger=machine -hardware { Directory.GetCurrentDirectory()}\arduino-1.8.12\arduino-1.8.12\hardware -tools { Directory.GetCurrentDirectory()}\arduino-1.8.12\arduino-1.8.12\tools-builder -tools { Directory.GetCurrentDirectory()}\arduino-1.8.12\arduino-1.8.12\hardware\tools\avr -built-in-libraries { Directory.GetCurrentDirectory()}\arduino-1.8.12\arduino-1.8.12\libraries -fqbn={{0}} -ide-version=10812 -build-path { Directory.GetCurrentDirectory()}\BaseStationEX\Build -warnings=all -build-cache { Directory.GetCurrentDirectory()}\BaseStationEX\Cache -prefs=build.warn_data_percentage=75 -prefs=runtime.tools.avr-gcc.path={ Directory.GetCurrentDirectory()}\arduino-1.8.12\arduino-1.8.12\hardware\tools\avr -prefs=runtime.tools.avr-gcc-7.3.0-atmel3.6.1-arduino5.path={ Directory.GetCurrentDirectory()}\arduino-1.8.12\arduino-1.8.12\hardware\tools\avr -prefs=runtime.tools.arduinoOTA.path={ Directory.GetCurrentDirectory()}\arduino-1.8.12\arduino-1.8.12\hardware\tools\avr -prefs=runtime.tools.arduinoOTA-1.3.0.path={ Directory.GetCurrentDirectory()}\arduino-1.8.12\arduino-1.8.12\hardware\tools\avr -prefs=runtime.tools.avrdude.path={ Directory.GetCurrentDirectory()}\arduino-1.8.12\arduino-1.8.12\hardware\tools\avr -prefs=runtime.tools.avrdude-6.3.0-arduino17.path={ Directory.GetCurrentDirectory()}\arduino-1.8.12\arduino-1.8.12\hardware\tools\avr -verbose { Directory.GetCurrentDirectory()}\BaseStationEX\DCCpp_EX\DCCppEX\DCCppEX.ino",

                    SupportedBoards = new List<Board>()
                    {
                        new Board("Uno", ArduinoModel.UnoR3, "arduino:avr:uno"),
                        new Board("Mega", ArduinoModel.Mega2560, "arduino:avr:mega:cpu=atmega2560"),
                        new Board("SAM21", ArduinoModel.Mega2560, "arduino:avr:mega:cpu=atmega2560"),
                    },
                    SupportedMotorShields = new List<MotorShield>()
                    {
                        new MotorShield("Arduino Motor Shield", MotorShieldType.Arduino),
                        new MotorShield("Pololu MC33926 Motor Shield", MotorShieldType.Pololu),
                        //new MotorShield("BTS7960B Motor Shield", MotorShieldType.BTS7960B),
                    }
                }
            },
        };


        public static string GetWiringDiagram(ArduinoModel model, MotorShieldType type)
        {
            string wiringDiagram = "pack://application:,,,/Resources/dcc-ex-logo.png";
            switch (model)
            {
                case ArduinoModel.Mega2560:
                    switch (type)
                    {
                        case MotorShieldType.Arduino:
                            wiringDiagram = "pack://application:,,,/Resources/mega-arduino.png";
                            break;
                        case MotorShieldType.Pololu:
                            wiringDiagram = "pack://application:,,,/Resources/mega-pololu.png";
                            break;
                        case MotorShieldType.BTS7960B:
                            wiringDiagram = "pack://application:,,,/Resources/dcc-ex-logo.png";
                            break;
                    }
                    break;
                case ArduinoModel.UnoR3:
                    switch (type)
                    {
                        case MotorShieldType.Arduino:
                            wiringDiagram = "pack://application:,,,/Resources/uno-arduino.png";
                            break;
                        case MotorShieldType.Pololu:
                            wiringDiagram = "pack://application:,,,/Resources/uno-pololu.png";
                            break;
                        case MotorShieldType.BTS7960B:
                            wiringDiagram = "pack://application:,,,/Resources/dcc-ex-logo.png";
                            break;
                    }
                    break;
            }
            return wiringDiagram;
        }

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
