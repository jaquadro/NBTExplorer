using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using NBTExplorer.Model;
using NBTUtil.Ops;
using Substrate.Nbt;

namespace NBTUtil
{
    class ConsoleRunner
    {
        private static readonly Dictionary<ConsoleCommand, ConsoleOperation> _commandTable = new Dictionary<ConsoleCommand, ConsoleOperation>() {
            { ConsoleCommand.SetValue, new EditOperation() },
            { ConsoleCommand.SetList, new SetListOperation() },
            { ConsoleCommand.Print, new PrintOperation() },
            { ConsoleCommand.PrintTree, new PrintTreeOperation() },
            { ConsoleCommand.Json, new JsonOperation() },
        };

        private ConsoleOptions _options;

        public ConsoleRunner ()
        {
            _options = new ConsoleOptions();
        }

        public bool Run (string[] args)
        {
            _options.Parse(args);

            if (_options.Command == ConsoleCommand.Help)
                return PrintHelp();

            if (_options.Path == null)
                return PrintUsage("Error: You must specify a path");
            if (!_commandTable.ContainsKey(_options.Command))
                return PrintUsage("Error: No command specified");

            ConsoleOperation op = _commandTable[_options.Command];
            if (!op.OptionsValid(_options))
                return PrintError("Error: Invalid options specified for the given command");

            int successCount = 0;
            int failCount = 0;

            foreach (var targetNode in new NbtPathEnumerator(_options.Path)) {
                if (!op.CanProcess(targetNode)) {
                    Console.WriteLine(targetNode.NodePath + ": ERROR (invalid command)");
                    failCount++;
                }
                if (!op.Process(targetNode, _options)) {
                    Console.WriteLine(targetNode.NodePath + ": ERROR (apply)");
                    failCount++;
                }

                targetNode.Root.Save();

                Console.WriteLine(targetNode.NodePath + ": OK");
                successCount++;
            }

            Console.WriteLine("Operation complete.  Nodes succeeded: {0}  Nodes failed: {1}", successCount, failCount);

            return true;
        }

        private DataNode OpenFile (string path)
        {
            DataNode node = null;
            foreach (var item in FileTypeRegistry.RegisteredTypes) {
                if (item.Value.NamePatternTest(path))
                    node = item.Value.NodeCreate(path);
            }

            return node;
        }

        private DataNode ExpandDataNode (DataNode dataNode, string tagPath)
        {
            string[] pathParts = tagPath.Split('/');

            DataNode curTag = dataNode;
            curTag.Expand();

            foreach (var part in pathParts) {
                TagDataNode.Container container = curTag as TagDataNode.Container;
                if (curTag == null)
                    throw new Exception("Invalid tag path");

                DataNode childTag = null;
                foreach (var child in curTag.Nodes) {
                    if (child.NodePathName == part)
                        childTag = child;
                }

                if (childTag == null)
                    throw new Exception("Invalid tag path");

                curTag.Expand();
            }

            return curTag;
        }

        private bool PrintHelp ()
        {
            Console.WriteLine("NBTUtil - Copyright 2014 Justin Aquadro");
            _options.PrintUsage();

            return true;
        }

        private bool PrintUsage (string error)
        {
            Console.WriteLine(error);
            _options.PrintUsage();

            return false;
        }

        private bool PrintError (string error)
        {
            Console.WriteLine(error);

            return false;
        }
    }
}
