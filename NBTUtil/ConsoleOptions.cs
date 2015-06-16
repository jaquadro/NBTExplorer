using System;
using System.Collections.Generic;
using System.Text;
using NDesk.Options;

namespace NBTUtil
{
    public enum ConsoleCommand
    {
        None,
        Print,
        PrintTree,
        SetValue,
        SetList,
        Json,
        Help,
    }

    class ConsoleOptions
    {
        private OptionSet _options;
        private string _currentKey;

        //public string FilePath { get; private set; }
        //public string TagPath { get; private set; }
        public string Path { get; private set; }

        public ConsoleCommand Command { get; private set; }
        public List<string> Values { get; private set; }
        public bool ShowTypes { get; private set; }

        public ConsoleOptions ()
        {
            Command = ConsoleCommand.None;
            Values = new List<string>();

            _options = new OptionSet() {
                { "path=", "Path to NBT tag from current directory", v => Path = v },
                { "print", "Print the value(s) of a tag", v => Command = ConsoleCommand.Print },
                { "printtree", "Print the NBT tree rooted at a tag", v => Command = ConsoleCommand.PrintTree },
                { "types", "Show data types when printing tags", v => ShowTypes = true },
                { "json=", "Export the NBT tree rooted at a tag as JSON", v => {
                    Command = ConsoleCommand.Json;
                    Values.Add(v);
                }},
                { "setvalue=", "Set a single tag value", v => { 
                    Command = ConsoleCommand.SetValue;
                    _currentKey = "setvalue";
                    if (!string.IsNullOrEmpty(v))
                        Values.Add(v);
                }},
                { "setlist=", "Replace a list tag's contents with one or more values.", v => {
                    Command = ConsoleCommand.SetList;
                    _currentKey = "setlist";
                    if (!string.IsNullOrEmpty(v))
                        Values.Add(v);
                }},
                { "help", "Print this help message", v => Command = ConsoleCommand.Help },
                { "<>", v => {
                    switch (_currentKey) {
                        case "setvalue":
                        case "setlist":
                            Values.Add(v);
                            break;
                    }
                }},
            };
        }

        public void Parse (string[] args)
        {
            _options.Parse(args);
        }

        public void PrintUsage ()
        {
            Console.WriteLine("Program Options:");
            _options.WriteOptionDescriptions(Console.Out);
        }
    }
}
