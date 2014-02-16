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

            NbtPath path = new NbtPath(_options.Path);
            DataNode targetNode = path.Open();

            if (targetNode == null)
                return PrintError("Error: Invalid path");

            ConsoleOperation op = _commandTable[_options.Command];
            if (!op.OptionsValid(_options))
                return PrintError("Error: Invalid options specified for the given command");
            if (!op.CanProcess(targetNode))
                return PrintError("Error: The given command can't be applied to the given tag");
            if (!op.Process(targetNode, _options))
                return PrintError("Error: Problem encountered applying the given command");

            path.RootNode.Save();

            Console.WriteLine("The operation completed successfully");

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

    class NbtPath
    {
        private class PathPart
        {
            public string Name;
            public DataNode Node;
        }

        private List<PathPart> _pathParts = new List<PathPart>();

        public NbtPath (string path)
        {
            Path = path;

            string[] parts = path.Split('/', '\\');
            foreach (var p in parts) {
                _pathParts.Add(new PathPart() {
                    Name = p,
                });
            }
        }

        public string Path { get; private set; }

        public DataNode RootNode
        {
            get { return (_pathParts.Count == 0) ? null : _pathParts[0].Node; }
        }

        public DataNode TargetNode
        {
            get { return (_pathParts.Count == 0) ? null : _pathParts[_pathParts.Count - 1].Node; }
        }

        public DataNode Open ()
        {
            DataNode dataNode = new DirectoryDataNode(Directory.GetCurrentDirectory());
            dataNode.Expand();

            foreach (var part in _pathParts) {
                DataNode match = null;
                foreach (var child in dataNode.Nodes) {
                    if (child.NodePathName == part.Name)
                        match = child;
                }

                if (match == null)
                    return null;

                part.Node = match;

                dataNode = match;
                dataNode.Expand();
            }

            return dataNode;
        }
    }
}
