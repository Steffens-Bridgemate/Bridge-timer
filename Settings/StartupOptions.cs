using System;
using System.Collections.Generic;
using System.Text;
using CommandLine;

namespace BridgeTimer.Settings
{
    public class StartupOptions
    {
        [Option('r',"rounds", Required =false,HelpText ="Any number between 0 (continuous play) and 99.")]
        public int? NumberOfRounds { get; set; }

        [Option('p', "play", Required = false,HelpText ="The total number of minutes of playtime, must be at least 2.")]
        public int? PlayTime { get; set; }

        [Option('w', "warning", Required = false,HelpText ="The number of minutes before the end of play to give a warning. Must be at least 1 minute before end of play and at most 1 minute less than the total play time.")]
        public int? WarningTime { get; set; }

        [Option('c', "change", Required = false,HelpText ="The time allotted to find the new table.")]
        public int? ChangeTime { get; set; }
    }
}
