using System;
using System.Collections.Generic;
using System.Text;
using Substrate;

namespace NBToolkit
{
    class Program
    {
        static void Main (string[] args)
        {
            //Harness harness = new Harness();

            if (args.Length < 1) {
                args = new string[1] { "" };
            }

            try {
                if (args[0] == "oregen") {
                    OregenOptions options = new OregenOptions(args);
                    Oregen filter = new Oregen(options);
                    options.SetDefaults();
                    filter.Run();
                }
                else if (args[0] == "replace") {
                    ReplaceOptions options = new ReplaceOptions(args);
                    Replace filter = new Replace(options);
                    options.SetDefaults();
                    filter.Run();
                }
                else if (args[0] == "purge") {
                    PurgeOptions options = new PurgeOptions(args);
                    Purge filter = new Purge(options);
                    options.SetDefaults();
                    filter.Run();
                }
                else if (args[0] == "dump") {
                    DumpOptions options = new DumpOptions(args);
                    Dump filter = new Dump(options);
                    options.SetDefaults();
                    filter.Run();
                }
                else if (args[0] == "relight") {
                    RelightOptions options = new RelightOptions(args);
                    Relight filter = new Relight(options);
                    options.SetDefaults();
                    filter.Run();
                }
                else if (args[0] == "help") {
                    if (args.Length < 2) {
                        args = new string[2] { "help", "help" };
                    }

                    Console.WriteLine("Command: " + args[1]);
                    Console.WriteLine();

                    TKOptions options = null;

                    if (args[1] == "oregen") {
                        options = new OregenOptions(args);

                        WriteBlock("Generates one or more structured deposits of a single block type randomly within each chunk selected for update.  The deposits are similar to natural ore deposits that appear in the world.  Default values for depth, size, and number of rounds are provided for the true ores (coal, iron, gold, etc,).  Other block types can be used as long as these values are specified in the command.");
                        Console.WriteLine();
                        options.PrintUsage();
                    }
                    else if (args[1] == "replace") {
                        options = new ReplaceOptions(args);

                        WriteBlock("Replaces one block type with another.  By default all matching blocks in the world will be replaced, but updates can be restricted by the available options.  This command can be used to set a new data value on blocks by replacing a block with itself.");
                        Console.WriteLine();
                        options.PrintUsage();
                    }
                    else if (args[1] == "purge") {
                        options = new PurgeOptions(args);

                        WriteBlock("Deletes all chunks matching the chunk filtering options.  If no options are specified, all world chunks will be deleted.  Region files that have all of their chunks purged will also be deleted from the world directory.");
                        Console.WriteLine();
                        options.PrintUsage();
                    }
                    else if (args[1] == "dump") {
                        options = new DumpOptions(args);

                        WriteBlock("Dumps out chunk data in a readable JSON file.  Block data, which are byte arrays, are printed as Bas64-encoded strings.");
                        Console.WriteLine();
                        options.PrintUsage();
                    }
                    else if (args[1] == "relight") {
                        options = new RelightOptions(args);

                        WriteBlock("Recalculates the blocklight and/or skylight values for selected chunks.  This completely resets the lighting in a chunk, recalculates it from existing light sources, then stiches the lighting seamlessly back into neighboring chunks.");
                        Console.WriteLine();
                        options.PrintUsage();
                    }
                    else {
                        WriteBlock("Prints help and usage information for another command.  Available commands are 'oregen', 'replace', 'purge', 'dump', 'relight'.");
                        Console.WriteLine();
                        Console.WriteLine("Usage: nbtoolkit help <command>");
                    }

                    return;
                }
                else {
                    TKOptions options = null;
                    try {
                        options = new TKOptions(args);
                    }
                    catch (TKOptionException) { }

                    Console.WriteLine("Usage: nbtoolkit <command> [options]");
                    Console.WriteLine();
                    Console.WriteLine("Available commands:");
                    Console.WriteLine("  help        Get help and usage info for another command");
                    Console.WriteLine("  oregen      Generate structured deposits of a single block type");
                    Console.WriteLine("  replace     Replace one block type with another");
                    Console.WriteLine("  purge       Delete chunks");
                    Console.WriteLine("  dump        Dump parsed chunk data to a readable JSON file");
                    Console.WriteLine("  relight     Recalculate lighting on chunks");
                    Console.WriteLine();
                    options.PrintUsage();
                    return;
                }
            }
            catch (TKOptionException) {
                return;
            }

            Console.WriteLine("Done");
        }

        public static void WriteBlock (string str)
        {
            string buf = "";

            while (str.Length > 78) {
                for (int i = 78; i >= 0; i--) {
                    if (str[i] == ' ') {
                        if (buf.Length > 0) {
                            buf += "  ";
                        }
                        buf += str.Substring(0, i) + "\n";
                        str = str.Substring(i + 1);
                        break;
                    }
                }
            }

            buf += "  " + str;
            Console.WriteLine(buf);
        }
    }
}
