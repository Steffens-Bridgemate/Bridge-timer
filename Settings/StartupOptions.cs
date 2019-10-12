using System;
using System.Collections.Generic;
using System.Text;
using CommandLine;

namespace BridgeTimer.Settings
{
    public class StartupOptions
    {
        [Option('r',"rounds", Required =false)]
        public int? NumberOfRounds { get; set; }
    }
}
