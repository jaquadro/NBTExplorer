using System;
using System.Collections.Generic;
using System.Text;
using NDesk.Options;
using System.IO;

namespace NBToolkit
{
    public interface IOptions
    {
        void Parse (string[] args);
        void PrintUsage ();
    }

    public class TKOptions : IOptions
    {
        private OptionSet commonOpt = null;

        public string OPT_WORLD = "";
        public int OPT_DIM = 0;

        // Verbosity
        public bool OPT_V = false;
        public bool OPT_VV = false;
        public bool OPT_HELP = false;
        public bool OPT_ALPHA = false;

        public TKOptions ()
        {
            commonOpt = new OptionSet()
            {
                { "w|world=", "World directory",
                    v => OPT_WORLD = v },
                { "h|help", "Print this help message",
                    v => OPT_HELP = true },
                { "alpha", "Specify that the world is stored as individual chunk files",
                    v => OPT_ALPHA = true },
                { "nether", "Update the Nether instead of the main region",
                    v => OPT_DIM = -1 },
                { "v", "Verbose output",
                    v => OPT_V = true },
                { "vv", "Very verbose output",
                    v => { OPT_V = true; OPT_VV = true; } },
            };
        }

        public TKOptions (string[] args)
            : this()
        {
            Parse(args);
        }

        public virtual void Parse (string[] args)
        {
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
        }
    }

    public class TKOptionException : Exception
    {
        public TKOptionException () { }

        public TKOptionException (String msg) : base(msg) { }

        public TKOptionException (String msg, Exception innerException) : base(msg, innerException) { }
    }
}
