using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseStationInstaller.Models
{
    public struct Config
    {
        public string Name { get; set; }
        public string Git { get; set; }
        public List<string> Dependencies { get; set; }
        public string BuildCommand { get; set; }
        public List<Board> SupportedBoards { get; set; }
        public List<MotorShield> SupportedMotorShields { get; set; }
    }

    public static class BaseStationSettings
    {
        public static Dictionary<string, Config> BaseStationDefaults = new Dictionary<string, Config>()
        {
            {
                "BaseStation", new Config
                {
                    Name = "Base Station",
                    Git = "https://github.com/DCC-EX/BaseStation.git",
                    Dependencies = new List<string>()
                    {
                        "https://raw.githubusercontent.com/platformio/platformio/develop/scripts/get-platformio.py",
                        "https://www.python.org/ftp/python/3.8.2/python-3.8.2-embed-amd64.zip"
                    },
                    BuildCommand = "platformio run",
                    SupportedBoards = new List<Board>()
                    {
                        new Board("Uno", "atmelavr"),
                        new Board("Mega", "atmelavr"),
                        new Board("ESP32", "espressif32"),
                    },
                    SupportedMotorShields = new List<MotorShield>()
                    {
                        new MotorShield("Arduino Motor Shield"),
                        new MotorShield("Pololu MC33926 Motor Shield"),
                        new MotorShield("BTS7960B Motor Shield"),
                    }
                }
            },
            {
                "BaseStationClassic", new Config
                {
                    Name = "Base Station Classic",
                    Git = "https://github.com/DCC-EX/BaseStation-Classic.git",
                    Dependencies = new List<string>()
                    {
                        "https://www.arduino.cc/download_handler.php?f=/arduino-1.8.12-windows.zip"
                    },
                    BuildCommand = @".\arduino-1.8.12\arduino-builder -compile -logger=machine -hardware .\arduino-1.8.12\hardware -tools .\arduino-1.8.12\tools-builder -tools .\arduino-1.8.12\hardware\tools\avr -built-in-libraries .\arduino-1.8.12\libraries -fqbn=arduino:avr:uno -ide-version=10812 -build-path .\BaseStationClassic\Build -warnings=all -build-cache .\BaseStationClassic\Cache -prefs=build.warn_data_percentage=75 -prefs=runtime.tools.avr-gcc.path=.\arduino-1.8.12\hardware\tools\avr -prefs=runtime.tools.avr-gcc-7.3.0-atmel3.6.1-arduino5.path=.\arduino-1.8.12\hardware\tools\avr -prefs=runtime.tools.arduinoOTA.path=.\arduino-1.8.12\hardware\tools\avr -prefs=runtime.tools.arduinoOTA-1.3.0.path=.\arduino-1.8.12\hardware\tools\avr -prefs=runtime.tools.avrdude.path=.\arduino-1.8.12\hardware\tools\avr -prefs=runtime.tools.avrdude-6.3.0-arduino17.path=.\arduino-1.8.12\hardware\tools\avr -verbose .\BaseStation-Classic\DCCpp\DCCpp_Uno.ino",
                    SupportedBoards = new List<Board>()
                    {
                        new Board("uno", "atmelavr"),
                        new Board("mega", "atmelavr"),
                    },
                    SupportedMotorShields = new List<MotorShield>()
                    {
                        new MotorShield("Arduino Motor Shield"),
                        new MotorShield("Pololu MC33926 Motor Shield"),
                    }
                }
            }
        };
    }
}
