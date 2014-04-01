using System.IO;
using System.Collections.Generic;
using System;

namespace NBTExplorer.Model
{
    public class DirectoryDataNode : DataNode
    {
        private string _path;

        public DirectoryDataNode (string path)
        {
            _path = path;
        }

        protected override NodeCapabilities Capabilities
        {
            get
            {
                return NodeCapabilities.Search
                    | NodeCapabilities.Refresh;
            }
        }

        public string NodeDirPath
        {
            get { return _path; }
        }

        public override string NodePathName
        {
            get
            {
                string path = (_path.EndsWith("/") || _path.EndsWith("\\")) ? _path : _path + '/';
                
                string name = Path.GetDirectoryName(path) ?? path.Substring(0, path.Length - 1);
                int sepIndex = Math.Max(name.LastIndexOf('/'), name.LastIndexOf('\\'));

                return (sepIndex > 0) ? name.Substring(sepIndex + 1) : name;
            }
        }

        public override string NodeDisplay
        {
            get { return Path.GetFileName(_path); }
        }

        public override bool HasUnexpandedChildren
        {
            get { return !IsExpanded; }
        }

        public override bool IsContainerType
        {
            get { return true; }
        }

        protected override void ExpandCore ()
        {
            foreach (string dirpath in Directory.GetDirectories(_path)) {
                Nodes.Add(new DirectoryDataNode(dirpath));
            }

            foreach (string filepath in Directory.GetFiles(_path)) {
                DataNode node = null;
                foreach (var item in FileTypeRegistry.RegisteredTypes) {
                    if (item.Value.NamePatternTest(filepath))
                        node = item.Value.NodeCreate(filepath);
                }

                if (node != null)
                    Nodes.Add(node);
            }
        }

        protected override void ReleaseCore ()
        {
            Nodes.Clear();
        }

        public override bool RefreshNode ()
        {
            Dictionary<string, object> expandSet = BuildExpandSet(this);
            Release();
            RestoreExpandSet(this, expandSet);

            return expandSet != null;
        }
    }
}
