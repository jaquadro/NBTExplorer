using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace NBTExplorer.Model
{
    public class NbtPathEnumerator : IEnumerable<DataNode>
    {
        private class PathPartDesc
        {
            public string Name;
            public DataNode Node;
        }

        private string _pathRoot;
        private List<string> _pathParts = new List<string>();

        public NbtPathEnumerator (string path)
        {
            _pathRoot = Path.GetPathRoot(path);
            _pathParts = new List<string>(path.Substring(_pathRoot.Length).Split('/', '\\'));

            if (string.IsNullOrEmpty(_pathRoot))
                _pathRoot = Directory.GetCurrentDirectory();
        }

        public IEnumerator<DataNode> GetEnumerator ()
        {
            DataNode dataNode = new DirectoryDataNode(_pathRoot);
            dataNode.Expand();

            foreach (DataNode childNode in EnumerateNodes(dataNode, _pathParts))
                yield return childNode;
        }

        IEnumerator IEnumerable.GetEnumerator ()
        {
            return GetEnumerator();
        }

        private IEnumerable<DataNode> EnumerateNodes (DataNode containerNode, List<string> nextLevels)
        {
            containerNode.Expand();
            if (nextLevels.Count == 0) {
                yield return containerNode;
                yield break;
            }

            if (containerNode.Nodes.Count == 0)
                yield break;

            string part = nextLevels[0];
            List<string> remainingLevels = nextLevels.GetRange(1, nextLevels.Count - 1);
            
            if (part == "*") {
                foreach (DataNode childNode in containerNode.Nodes) {
                    foreach (DataNode grandChildNode in EnumerateNodes(childNode, remainingLevels))
                        yield return grandChildNode;
                }
            }
            else if (part == "**") {
                foreach (DataNode childNode in containerNode.Nodes) {
                    foreach (DataNode grandChildNode in EnumerateNodes(childNode, remainingLevels))
                        yield return grandChildNode;

                    foreach (DataNode grandChildNode in EnumerateNodes(childNode, nextLevels))
                        yield return grandChildNode;
                }
            }
            else {
                foreach (var childNode in containerNode.Nodes) {
                    if (childNode.NodePathName == part) {
                        foreach (DataNode grandChildNode in EnumerateNodes(childNode, remainingLevels))
                            yield return grandChildNode;
                    }
                }
            }
        }
    }

    public class NbtPath
    {
        private class PathPart
        {
            public string Name;
            public DataNode Node;
        }

        
        //private List<PathPart> _pathParts = new List<PathPart>();

        private List<DataNode> _nodes;

        internal NbtPath (List<DataNode> nodes)
        {
            _nodes = nodes;
        }

        /*public NbtPath (string path)
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
        }*/
    }
}
