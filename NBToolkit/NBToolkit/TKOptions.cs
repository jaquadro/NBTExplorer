using System;
using System.Collections.Generic;
using System.Text;
using NDesk.Options;
using System.IO;

namespace NBToolkit
{
    public class TKOptions
    {
        private OptionSet commonOpt = null;

        public string OPT_WORLD = "";

        // Verbosity
        public bool OPT_V = false;
        public bool OPT_VV = false;
        public bool OPT_HELP = false;

        // Block ID  Include / Exclude conditions by chunk
        public List<int> OPT_CH_INCLUDE = new List<int>();
        public List<int> OPT_CH_EXCLUDE = new List<int>();

        // Chunk coordinate conditions
        public int? CH_X_GE = null;
        public int? CH_X_LE = null;
        public int? CH_Z_GE = null;
        public int? CH_Z_LE = null;

        // Block coordinate conditions
        public int? BL_X_GE = null;
        public int? BL_X_LE = null;
        public int? BL_Z_GE = null;
        public int? BL_Z_LE = null;

        public TKOptions (string[] args)
        {
            commonOpt = new OptionSet()
            {
                { "w|world=", "World directory",
                    v => OPT_WORLD = v },
                { "h|help", "Print this help message",
                    v => OPT_HELP = true },
                { "v", "Verbose output",
                    v => OPT_V = true },
                { "vv", "Very verbose output",
                    v => { OPT_V = true; OPT_VV = true; } },
                { "cxa=", "Update chunks with X-coord equal to or above {VAL}",
                    v => CH_X_GE = Convert.ToInt32(v) },
                { "cxb=", "Update chunks with X-coord equal to or below {VAL}",
                    v => CH_X_LE = Convert.ToInt32(v) },
                { "cza=", "Update chunks with Z-coord equal to or above {VAL}",
                    v => CH_Z_GE = Convert.ToInt32(v) },
                { "czb=", "Update chunks with Z-coord equal to or below {VAL}",
                    v => CH_Z_LE = Convert.ToInt32(v) },
                { "ci=", "Update chunks that contain an {ID} block",
                    v => OPT_CH_INCLUDE.Add(Convert.ToInt32(v) % 256) },
                { "cx=", "Update chunks that don't contain an {ID} block",
                    v => OPT_CH_EXCLUDE.Add(Convert.ToInt32(v) % 256) },
            };

            commonOpt.Parse(args);
        }

        public virtual void PrintUsage ()
        {
            Console.WriteLine("Common Options:");
            commonOpt.WriteOptionDescriptions(Console.Out);
        }

        public virtual void SetDefaults ()
        {
            if (OPT_HELP) {
                this.PrintUsage();
                throw new TKOptionException();
            }

            if (OPT_WORLD.Length == 0) {
                Console.WriteLine("Error: You must specify a World path");
                Console.WriteLine();
                this.PrintUsage();

                throw new TKOptionException();
            }

            if (!File.Exists(Path.Combine(OPT_WORLD, "Level.dat"))) {
                Console.WriteLine("Error: The supplied world path is invalid");
                Console.WriteLine();
                this.PrintUsage();

                throw new TKOptionException();
            }
        }
    }

    class TKOptionException : Exception
    {
        public TKOptionException () { }

        public TKOptionException (String msg) : base(msg) { }

        public TKOptionException (String msg, Exception innerException) : base(msg, innerException) { }
    }
}
