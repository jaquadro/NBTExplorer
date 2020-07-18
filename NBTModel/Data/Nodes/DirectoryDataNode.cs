using System;
using System.IO;

namespace NBTExplorer.Model
{
    public class DirectoryDataNode : DataNode
    {
        public DirectoryDataNode(string path)
        {
            NodeDirPath = path;
        }

        protected override NodeCapabilities Capabilities => NodeCapabilities.Search
                                                            | NodeCapabilities.Refresh;

        public string NodeDirPath { get; }

        public override string NodePathName
        {
            get
            {
                var path = NodeDirPath.EndsWith("/") || NodeDirPath.EndsWith("\\") ? NodeDirPath : NodeDirPath + '/';

                var name = Path.GetDirectoryName(path) ?? path.Substring(0, path.Length - 1);
                var sepIndex = Math.Max(name.LastIndexOf('/'), name.LastIndexOf('\\'));

                return sepIndex > 0 ? name.Substring(sepIndex + 1) : name;
            }
        }

        public override string NodeDisplay => Path.GetFileName(NodeDirPath);

        public override bool HasUnexpandedChildren => !IsExpanded;

        public override bool IsContainerType => true;

        protected override void ExpandCore()
        {
            foreach (var dirpath in Directory.GetDirectories(NodeDirPath)) Nodes.Add(new DirectoryDataNode(dirpath));

            foreach (var filepath in Directory.GetFiles(NodeDirPath))
            {
                DataNode node = null;
                foreach (var item in FileTypeRegistry.RegisteredTypes)
                    if (item.Value.NamePatternTest(filepath))
                        node = item.Value.NodeCreate(filepath);

                if (node != null)
                    Nodes.Add(node);
            }
        }

        protected override void ReleaseCore()
        {
            Nodes.Clear();
        }

        public override bool RefreshNode()
        {
            var expandSet = BuildExpandSet(this);
            Release();
            RestoreExpandSet(this, expandSet);

            return expandSet != null;
        }
    }
}