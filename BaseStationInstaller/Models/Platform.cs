using System;
using System.Collections.Generic;
using System.Text;

namespace BaseStationInstaller.Models
{
    public class Platform
    {

        public Platform(string arch, string plat)
        {
            Architecture = arch;
            Package = plat;
        }

        public Platform() { }

        public string Architecture { get; set; }

        public string Package { get; set; }
    }
}
