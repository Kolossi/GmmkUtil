﻿using CommandLine;

namespace GmmkUtil
{
    public class Options
    {
        [Option('p', "setprofile", HelpText = "If keyboard connected, immediately set lighting to profile x (1-3)",Required = false)]
        public int Profile { get; set; }

        [Option('a', "showall", HelpText ="Show all connected usb devices", Required = false
#if !DEBUG
            ,Hidden = true
#endif
            )]
        public bool ShowAll { get; set; }

        // Todo!
        //[Option('i', "initprofile", HelpText = "Stay running and monitor for keyboard being inserted, when it is set lighting to profile x (1-3)", Min = 1, Max = 3)]
        //public int InitProfile { get; set; }
    }
}
