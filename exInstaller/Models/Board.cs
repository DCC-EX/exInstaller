using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using ArduinoUploader.Hardware;

namespace exInstaller.Models
{

    public class Board
    {
        public Board(string name, /*ArduinoModel platform,*/ string fqbn, List<Platform> platforms, List<MotorShield> shields, List<Library> libraries)
        {
            Name = name;
            //Platform = platform;
            FQBN = fqbn;
            Platforms = platforms;
            SupportedMotoShields = shields;
            ExtraLibraries = libraries;
        }

        public Board() { }
        public string Name { get; set; }
        //public ArduinoModel Platform { get; set; }

        public string FQBN { get; set; }

        public List<Platform> Platforms { get; set; }

        public List<MotorShield> SupportedMotoShields { get; set; }

        public List<Library> ExtraLibraries { get; set; }
    }
}
