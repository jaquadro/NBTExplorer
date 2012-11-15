using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using NBTExplorer.Model;
using Substrate.Nbt;
using NBTExplorer.Windows;

namespace NBTExplorer.Controllers
{
    public class NodeTreeController
    {
        private TreeView _nodeTree;
        private IconRegistry _iconRegistry;

        private TagCompoundDataNode _rootData;

        public NodeTreeController (TreeView nodeTree)
        {
            _nodeTree = nodeTree;
            InitializeIconRegistry();

            _rootData = new TagCompoundDataNode(new TagNodeCompound());
        }

        public void CreateNode (TreeNode node, TagType type)
        {
            if (node == null || !(node.Tag is DataNode))
                return;

            DataNode dataNode = node.Tag as DataNode;
            if (!dataNode.CanCreateTag(type))
                return;

            if (dataNode.CreateNode(type)) {
                node.Text = dataNode.NodeDisplay;
                RefreshChildNodes(node, dataNode);
                UpdateUI(dataNode);
            }
        }

        public void CreateRootNode (TagType type)
        {
            if (_rootData.CreateNode(type)) {
                RefreshRootNodes();
                UpdateUI(_rootData);
            }
        }

        private void DeleteNode (TreeNode node)
        {
            if (node == null || !(node.Tag is DataNode))
                return;

            DataNode dataNode = node.Tag as DataNode;
            if (!dataNode.CanDeleteNode)
                return;

            if (dataNode.DeleteNode()) {
                UpdateUI(node.Parent.Tag as DataNode);
                UpdateNodeText(node.Parent);
                node.Remove();
            }
        }

        private void ExpandNode (TreeNode node)
        {
            if (node == null || !(node.Tag is DataNode))
                return;

            if (node.IsExpanded)
                return;

            node.Nodes.Clear();

            DataNode backNode = node.Tag as DataNode;
            if (!backNode.IsExpanded)
                backNode.Expand();

            foreach (DataNode child in backNode.Nodes)
                node.Nodes.Add(CreateUnexpandedNode(child));
        }

        public void RefreshRootNodes ()
        {
            if (_rootData.HasUnexpandedChildren)
                _rootData.Expand();

            Dictionary<DataNode, TreeNode> currentNodes = new Dictionary<DataNode, TreeNode>();
            foreach (TreeNode child in _nodeTree.Nodes) {
                if (child.Tag is DataNode)
                    currentNodes.Add(child.Tag as DataNode, child);
            }

            _nodeTree.Nodes.Clear();
            foreach (DataNode child in _rootData.Nodes) {
                if (!currentNodes.ContainsKey(child))
                    _nodeTree.Nodes.Add(CreateUnexpandedNode(child));
                else
                    _nodeTree.Nodes.Add(currentNodes[child]);
            }

            //if (_nodeTree.Nodes.Count == 0 && _rootData.HasUnexpandedChildren) {
            //    _rootData.Expand();
            //    RefreshRootNodes();
            //}
        }

        public void RefreshChildNodes (TreeNode node, DataNode dataNode)
        {
            Dictionary<DataNode, TreeNode> currentNodes = new Dictionary<DataNode, TreeNode>();
            foreach (TreeNode child in node.Nodes) {
                if (child.Tag is DataNode)
                    currentNodes.Add(child.Tag as DataNode, child);
            }

            node.Nodes.Clear();
            foreach (DataNode child in dataNode.Nodes) {
                if (!currentNodes.ContainsKey(child))
                    node.Nodes.Add(CreateUnexpandedNode(child));
                else
                    node.Nodes.Add(currentNodes[child]);
            }

            //foreach (TreeNode child in node.Nodes)
            //    child.ContextMenuStrip = BuildNodeContextMenu(child.Tag as DataNode);

            if (node.Nodes.Count == 0 && dataNode.HasUnexpandedChildren) {
                ExpandNode(node);
                node.Expand();
            }
        }

        public virtual void UpdateUI (DataNode node)
        {
        }

        private void InitializeIconRegistry ()
        {
            _iconRegistry = new IconRegistry();
            _iconRegistry.DefaultIcon = 15;

            _iconRegistry.Register(typeof(TagByteDataNode), 0);
            _iconRegistry.Register(typeof(TagShortDataNode), 1);
            _iconRegistry.Register(typeof(TagIntDataNode), 2);
            _iconRegistry.Register(typeof(TagLongDataNode), 3);
            _iconRegistry.Register(typeof(TagFloatDataNode), 4);
            _iconRegistry.Register(typeof(TagDoubleDataNode), 5);
            _iconRegistry.Register(typeof(TagByteArrayDataNode), 6);
            _iconRegistry.Register(typeof(TagStringDataNode), 7);
            _iconRegistry.Register(typeof(TagListDataNode), 8);
            _iconRegistry.Register(typeof(TagCompoundDataNode), 9);
            _iconRegistry.Register(typeof(RegionChunkDataNode), 9);
            _iconRegistry.Register(typeof(DirectoryDataNode), 10);
            _iconRegistry.Register(typeof(RegionFileDataNode), 11);
            _iconRegistry.Register(typeof(CubicRegionDataNode), 11);
            _iconRegistry.Register(typeof(NbtFileDataNode), 12);
            _iconRegistry.Register(typeof(TagIntArrayDataNode), 14);
            _iconRegistry.Register(typeof(RootDataNode), 16);
        }

        private void UpdateNodeText (TreeNode node)
        {
            if (node == null || !(node.Tag is DataNode))
                return;

            DataNode dataNode = node.Tag as DataNode;
            node.Text = dataNode.NodeDisplay;
        }

        private TreeNode CreateUnexpandedNode (DataNode node)
        {
            TreeNode frontNode = new TreeNode(node.NodeDisplay);
            frontNode.ImageIndex = _iconRegistry.Lookup(node.GetType());
            frontNode.SelectedImageIndex = frontNode.ImageIndex;
            frontNode.Tag = node;
            //frontNode.ContextMenuStrip = BuildNodeContextMenu(node);

            if (node.HasUnexpandedChildren || node.Nodes.Count > 0)
                frontNode.Nodes.Add(new TreeNode());

            return frontNode;
        }
    }
}
