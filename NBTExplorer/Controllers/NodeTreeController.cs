using NBTExplorer.Model;
using NBTExplorer.Properties;
using NBTExplorer.Utility;
using NBTExplorer.Vendor.MultiSelectTreeView;
using NBTExplorer.Windows;
using Substrate.Nbt;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace NBTExplorer.Controllers
{
    public class MessageBoxEventArgs : EventArgs
    {
        public MessageBoxEventArgs(string message)
        {
            Message = message;
        }

        public bool Cancel { get; set; }
        public string Message { get; set; }
    }

    public class NodeTreeComparer : IComparer
    {
        private readonly NaturalComparer _comparer = new NaturalComparer();

        public int Compare(object x, object y)
        {
            var tx = x as TreeNode;
            var ty = y as TreeNode;
            var dx = tx.Tag as TagDataNode;
            var dy = ty.Tag as TagDataNode;

            if (dx == null || dy == null)
            {
                var nodeOrder = OrderForNode(tx.Tag).CompareTo(OrderForNode(ty.Tag));
                if (nodeOrder != 0)
                    return nodeOrder;
                return _comparer.Compare(tx.Text, ty.Text);
            }

            var px = dx.Parent as TagDataNode;
            var py = dy.Parent as TagDataNode;
            if (px != null && py != null)
                if (px.Tag.GetTagType() == TagType.TAG_LIST || py.Tag.GetTagType() == TagType.TAG_LIST)
                    return 0;

            var idx = dx.Tag.GetTagType();
            var idy = dy.Tag.GetTagType();
            var tagOrder = OrderForTag(idx).CompareTo(OrderForTag(idy));
            if (tagOrder != 0)
                return tagOrder;
            return _comparer.Compare(dx.NodeDisplay, dy.NodeDisplay);
        }

        public int OrderForTag(TagType tagID)
        {
            switch (tagID)
            {
                case TagType.TAG_COMPOUND:
                    return 0;

                case TagType.TAG_LIST:
                    return 1;

                case TagType.TAG_BYTE:
                case TagType.TAG_SHORT:
                case TagType.TAG_INT:
                case TagType.TAG_LONG:
                case TagType.TAG_FLOAT:
                case TagType.TAG_DOUBLE:
                case TagType.TAG_STRING:
                    return 2;

                default:
                    return 3;
            }
        }

        public int OrderForNode(object node)
        {
            if (node is DirectoryDataNode)
                return 0;
            return 1;
        }
    }

    public class NodeTreeController
    {
        private readonly MultiSelectTreeView _multiTree;

        //private TagCompoundDataNode _rootData;

        public NodeTreeController(TreeView nodeTree)
        {
            Tree = nodeTree;
            nodeTree.TreeViewNodeSorter = new NodeTreeComparer();
            _multiTree = nodeTree as MultiSelectTreeView;

            InitializeIconRegistry();
            ShowVirtualRoot = true;

            //_rootData = new TagCompoundDataNode(new TagNodeCompound());
            Root = new RootDataNode();
            RefreshRootNodes();
        }

        public RootDataNode Root { get; }

        public TreeView Tree { get; }

        public IconRegistry IconRegistry { get; private set; }

        public ImageList IconList => _multiTree.ImageList;

        public bool ShowVirtualRoot { get; set; }

        public string VirtualRootDisplay
        {
            get => Root.NodeDisplay;
            set
            {
                Root.SetDisplayName(value);
                if (ShowVirtualRoot && Tree.Nodes.Count > 0)
                    UpdateNodeText(Tree.Nodes[0]);
            }
        }

        private TreeNode SelectedNode
        {
            get
            {
                if (_multiTree != null)
                    return _multiTree.SelectedNode;
                return Tree.SelectedNode;
            }
        }

        public event EventHandler SelectionInvalidated;

        protected virtual void OnSelectionInvalidated(EventArgs e)
        {
            if (SelectionInvalidated != null)
                SelectionInvalidated(this, e);
        }

        private void OnSelectionInvalidated()
        {
            OnSelectionInvalidated(EventArgs.Empty);
        }

        public event EventHandler<MessageBoxEventArgs> ConfirmAction;

        protected virtual void OnConfirmAction(MessageBoxEventArgs e)
        {
            if (ConfirmAction != null)
                ConfirmAction(this, e);
        }

        private bool OnConfirmAction(string message)
        {
            var e = new MessageBoxEventArgs(message);
            OnConfirmAction(e);
            return !e.Cancel;
        }

        public void CreateNode(TreeNode node, TagType type)
        {
            if (node == null || !(node.Tag is DataNode))
                return;

            var dataNode = node.Tag as DataNode;
            if (!dataNode.CanCreateTag(type))
                return;

            if (dataNode.CreateNode(type))
            {
                node.Text = dataNode.NodeDisplay;
                RefreshChildNodes(node, dataNode);
                OnSelectionInvalidated();
            }
        }

        public void CreateNode(TagType type)
        {
            if (SelectedNode == null || Tree.Nodes.Count == 0)
                return;

            if (SelectedNode == null)
                CreateNode(Tree.Nodes[0], type);
            else
                CreateNode(SelectedNode, type);
        }

        public void DeleteNode(TreeNode node)
        {
            if (node == null || !(node.Tag is DataNode))
                return;

            var dataNode = node.Tag as DataNode;
            if (!dataNode.CanDeleteNode)
                return;

            if (dataNode.DeleteNode())
            {
                UpdateNodeText(node.Parent);
                node.Remove();

                RemoveNodeFromSelection(node);
                OnSelectionInvalidated();
            }
        }

        private bool DeleteNode(IList<TreeNode> nodes)
        {
            bool? elideChildren = null;
            if (!CanOperateOnNodesEx(nodes, Predicates.DeleteNodePred, out elideChildren))
                return false;

            if (elideChildren == true)
                nodes = ElideChildren(nodes);

            var selectionModified = false;
            foreach (var node in nodes)
            {
                var dataNode = node.Tag as DataNode;
                if (dataNode.DeleteNode())
                {
                    UpdateNodeText(node.Parent);
                    node.Remove();

                    RemoveNodeFromSelection(node);
                    selectionModified = true;
                }
            }

            if (selectionModified)
                OnSelectionInvalidated();

            return true;
        }

        private bool RemoveNodeFromSelection(TreeNode node)
        {
            var selectionModified = false;

            if (_multiTree != null)
            {
                if (_multiTree.SelectedNodes.Contains(node))
                {
                    _multiTree.SelectedNodes.Remove(node);
                    selectionModified = true;
                }

                if (_multiTree.SelectedNode == node)
                {
                    _multiTree.SelectedNode = null;
                    selectionModified = true;
                }
            }

            if (Tree.SelectedNode == node)
            {
                Tree.SelectedNode = null;
                selectionModified = true;
            }

            return selectionModified;
        }

        public void DeleteSelection()
        {
            if (_multiTree != null)
                DeleteNode(_multiTree.SelectedNodes);
            else
                DeleteNode(SelectedNode);
        }

        public void EditNode(TreeNode node)
        {
            if (node == null || !(node.Tag is DataNode))
                return;

            var dataNode = node.Tag as DataNode;
            if (!dataNode.CanEditNode)
                return;

            if (dataNode.EditNode())
            {
                node.Text = dataNode.NodeDisplay;
                OnSelectionInvalidated();
            }
        }

        public void EditSelection()
        {
            EditNode(SelectedNode);
        }

        private void RenameNode(TreeNode node)
        {
            if (node == null || !(node.Tag is DataNode))
                return;

            var dataNode = node.Tag as DataNode;
            if (!dataNode.CanRenameNode)
                return;

            if (dataNode.RenameNode())
            {
                node.Text = dataNode.NodeDisplay;
                OnSelectionInvalidated();
            }
        }

        public void RenameSelection()
        {
            RenameNode(SelectedNode);
        }

        private void RefreshNode(TreeNode node)
        {
            if (node == null || !(node.Tag is DataNode))
                return;

            var dataNode = node.Tag as DataNode;
            if (!dataNode.CanRefreshNode)
                return;

            if (!OnConfirmAction("Refresh data anyway?"))
                return;

            if (dataNode.RefreshNode())
            {
                RefreshChildNodes(node, dataNode);
                ExpandToEdge(node);
                OnSelectionInvalidated();
            }
        }

        public void RefreshSelection()
        {
            RefreshNode(SelectedNode);
        }

        private void ExpandToEdge(TreeNode node)
        {
            if (node == null || !(node.Tag is DataNode))
                return;

            var dataNode = node.Tag as DataNode;
            if (dataNode.IsExpanded)
            {
                if (!node.IsExpanded)
                    node.Expand();

                foreach (TreeNode child in node.Nodes)
                    ExpandToEdge(child);
            }
        }

        private void CopyNode(TreeNode node)
        {
            if (node == null || !(node.Tag is DataNode))
                return;

            var dataNode = node.Tag as DataNode;
            if (!dataNode.CanCopyNode)
                return;

            dataNode.CopyNode();
        }

        private void CutNode(TreeNode node)
        {
            if (node == null || !(node.Tag is DataNode))
                return;

            var dataNode = node.Tag as DataNode;
            if (!dataNode.CanCutNode)
                return;

            if (dataNode.CutNode())
            {
                UpdateNodeText(node.Parent);
                node.Remove();

                RemoveNodeFromSelection(node);
                OnSelectionInvalidated();
            }
        }

        private void PasteNode(TreeNode node)
        {
            if (node == null || !(node.Tag is DataNode))
                return;

            var dataNode = node.Tag as DataNode;
            if (!dataNode.CanPasteIntoNode)
                return;

            if (dataNode.PasteNode())
            {
                node.Text = dataNode.NodeDisplay;
                RefreshChildNodes(node, dataNode);

                OnSelectionInvalidated();
            }
        }

        public void CopySelection()
        {
            CopyNode(SelectedNode);
        }

        public void CutSelection()
        {
            CutNode(SelectedNode);
        }

        public void PasteIntoSelection()
        {
            PasteNode(SelectedNode);
        }

        public void MoveNodeUp(TreeNode node)
        {
            if (node == null)
                return;

            var datanode = node.Tag as DataNode;
            if (datanode == null || !datanode.CanMoveNodeUp)
                return;

            if (datanode.ChangeRelativePosition(-1))
            {
                RefreshChildNodes(node.Parent, datanode.Parent);
                OnSelectionInvalidated();
            }
        }

        public void MoveNodeDown(TreeNode node)
        {
            if (node == null)
                return;

            var datanode = node.Tag as DataNode;
            if (datanode == null || !datanode.CanMoveNodeDown)
                return;

            if (datanode.ChangeRelativePosition(1))
            {
                RefreshChildNodes(node.Parent, datanode.Parent);
                OnSelectionInvalidated();
            }
        }

        public void MoveSelectionUp()
        {
            MoveNodeUp(SelectedNode);
        }

        public void MoveSelectionDown()
        {
            MoveNodeDown(SelectedNode);
        }

        /*public void CreateRootNode (TagType type)
        {
            if (_rootData.CreateNode(type)) {
                RefreshRootNodes();
                UpdateUI(_rootData);
            }
        }*/

        public void ExpandNode(TreeNode node)
        {
            if (node == null || !(node.Tag is DataNode))
                return;

            if (node.IsExpanded)
                return;

            node.Nodes.Clear();

            var backNode = node.Tag as DataNode;
            if (!backNode.IsExpanded)
            {
                backNode.Expand();
                node.Text = backNode.NodeDisplay;
            }

            foreach (var child in backNode.Nodes)
                node.Nodes.Add(CreateUnexpandedNode(child));
        }

        public void ExpandSelectedNode()
        {
            ExpandNode(SelectedNode);
            if (SelectedNode != null)
                SelectedNode.Expand();
        }

        public void CollapseNode(TreeNode node)
        {
            if (node == null || !(node.Tag is DataNode))
                return;

            var backNode = node.Tag as DataNode;
            if (backNode.IsModified)
                return;

            backNode.Collapse();
            node.Name = backNode.NodeDisplay;

            node.Nodes.Clear();
            if (backNode.HasUnexpandedChildren)
                node.Nodes.Add(new TreeNode());
        }

        public void CollapseSelectedNode()
        {
            CollapseNode(SelectedNode);
        }

        private TreeNode GetRootFromDataNodePath(DataNode node, out Stack<DataNode> hierarchy)
        {
            hierarchy = new Stack<DataNode>();
            while (node != null)
            {
                hierarchy.Push(node);
                node = node.Parent;
            }

            var rootDataNode = hierarchy.Pop();
            TreeNode frontNode = null;
            foreach (TreeNode child in Tree.Nodes)
                if (child.Tag == rootDataNode)
                    frontNode = child;

            return frontNode;
        }

        private TreeNode FindFrontNode(DataNode node)
        {
            Stack<DataNode> hierarchy;
            var frontNode = GetRootFromDataNodePath(node, out hierarchy);

            if (frontNode == null)
                return null;

            while (hierarchy.Count > 0)
            {
                if (!frontNode.IsExpanded)
                {
                    frontNode.Nodes.Add(new TreeNode());
                    frontNode.Expand();
                }

                var childData = hierarchy.Pop();
                foreach (TreeNode childFront in frontNode.Nodes)
                    if (childFront.Tag == childData)
                    {
                        frontNode = childFront;
                        break;
                    }
            }

            return frontNode;
        }

        public void CollapseBelow(DataNode node)
        {
            Stack<DataNode> hierarchy;
            var frontNode = GetRootFromDataNodePath(node, out hierarchy);

            if (frontNode == null)
                return;

            while (hierarchy.Count > 0)
            {
                if (!frontNode.IsExpanded)
                    return;

                var childData = hierarchy.Pop();
                foreach (TreeNode childFront in frontNode.Nodes)
                    if (childFront.Tag == childData)
                    {
                        frontNode = childFront;
                        break;
                    }
            }

            if (frontNode.IsExpanded)
                frontNode.Collapse();
        }

        public void SelectNode(DataNode node)
        {
            if (_multiTree != null) _multiTree.SelectedNode = FindFrontNode(node);
            else Tree.SelectedNode = FindFrontNode(node);
        }

        public void ScrollNode(DataNode node)
        {
            var treeNode = FindFrontNode(node);
            if (treeNode != null)
                treeNode.EnsureVisible();
        }

        public void RefreshRootNodes()
        {
            if (ShowVirtualRoot)
            {
                Tree.Nodes.Clear();
                Tree.Nodes.Add(CreateUnexpandedNode(Root));
                return;
            }

            if (Root.HasUnexpandedChildren)
                Root.Expand();

            var currentNodes = new Dictionary<DataNode, TreeNode>();
            foreach (TreeNode child in Tree.Nodes)
                if (child.Tag is DataNode)
                    currentNodes.Add(child.Tag as DataNode, child);

            Tree.Nodes.Clear();
            foreach (var child in Root.Nodes)
                if (!currentNodes.ContainsKey(child))
                    Tree.Nodes.Add(CreateUnexpandedNode(child));
                else
                    Tree.Nodes.Add(currentNodes[child]);

            //if (_nodeTree.Nodes.Count == 0 && _rootData.HasUnexpandedChildren) {
            //    _rootData.Expand();
            //    RefreshRootNodes();
            //}
        }

        public void RefreshChildNodes(TreeNode node, DataNode dataNode)
        {
            var currentNodes = new Dictionary<DataNode, TreeNode>();
            foreach (TreeNode child in node.Nodes)
                if (child.Tag is DataNode)
                    currentNodes.Add(child.Tag as DataNode, child);

            node.Nodes.Clear();
            foreach (var child in dataNode.Nodes)
                if (!currentNodes.ContainsKey(child))
                    node.Nodes.Add(CreateUnexpandedNode(child));
                else
                    node.Nodes.Add(currentNodes[child]);

            foreach (TreeNode child in node.Nodes)
                child.ContextMenuStrip = BuildNodeContextMenu(child, child.Tag as DataNode);

            if (node.Nodes.Count == 0 && dataNode.HasUnexpandedChildren)
            {
                ExpandNode(node);
                node.Expand();
            }
        }

        public void RefreshTreeNode(DataNode dataNode)
        {
            var node = FindFrontNode(dataNode);
            RefreshChildNodes(node, dataNode);
        }

        private void InitializeIconRegistry()
        {
            IconRegistry = new IconRegistry();
            IconRegistry.DefaultIcon = 15;

            IconRegistry.Register(typeof(TagByteDataNode), 0);
            IconRegistry.Register(typeof(TagShortDataNode), 1);
            IconRegistry.Register(typeof(TagIntDataNode), 2);
            IconRegistry.Register(typeof(TagLongDataNode), 3);
            IconRegistry.Register(typeof(TagFloatDataNode), 4);
            IconRegistry.Register(typeof(TagDoubleDataNode), 5);
            IconRegistry.Register(typeof(TagByteArrayDataNode), 6);
            IconRegistry.Register(typeof(TagStringDataNode), 7);
            IconRegistry.Register(typeof(TagListDataNode), 8);
            IconRegistry.Register(typeof(TagCompoundDataNode), 9);
            IconRegistry.Register(typeof(RegionChunkDataNode), 9);
            IconRegistry.Register(typeof(DirectoryDataNode), 10);
            IconRegistry.Register(typeof(RegionFileDataNode), 11);
            IconRegistry.Register(typeof(CubicRegionDataNode), 11);
            IconRegistry.Register(typeof(NbtFileDataNode), 12);
            IconRegistry.Register(typeof(TagIntArrayDataNode), 14);
            IconRegistry.Register(typeof(TagShortArrayDataNode), 16);
            IconRegistry.Register(typeof(TagLongArrayDataNode), 17);
            IconRegistry.Register(typeof(RootDataNode), 18);
        }

        private void UpdateNodeText(TreeNode node)
        {
            if (node == null || !(node.Tag is DataNode))
                return;

            var dataNode = node.Tag as DataNode;
            node.Text = dataNode.NodeDisplay;
        }

        public bool CheckModifications()
        {
            foreach (TreeNode node in Tree.Nodes)
            {
                var dataNode = node.Tag as DataNode;
                if (dataNode != null && dataNode.IsModified)
                    return true;
            }

            return false;
        }

        private TreeNode CreateUnexpandedNode(DataNode node)
        {
            var frontNode = new TreeNode(node.NodeDisplay);
            frontNode.ImageIndex = IconRegistry.Lookup(node.GetType());
            frontNode.SelectedImageIndex = frontNode.ImageIndex;
            frontNode.Tag = node;
            //frontNode.ContextMenuStrip = BuildNodeContextMenu(node);

            if (node.HasUnexpandedChildren || node.Nodes.Count > 0)
                frontNode.Nodes.Add(new TreeNode());

            return frontNode;
        }

        public ContextMenuStrip BuildNodeContextMenu(TreeNode frontNode, DataNode node)
        {
            if (node == null)
                return null;

            var menu = new ContextMenuStrip();

            if (node.HasUnexpandedChildren || node.Nodes.Count > 0)
                if (frontNode.IsExpanded)
                {
                    var itemCollapse = new ToolStripMenuItem("&Collapse", null, _contextCollapse_Click);
                    itemCollapse.Font = new Font(itemCollapse.Font, FontStyle.Bold);

                    var itemExpandChildren =
                        new ToolStripMenuItem("Expand C&hildren", null, _contextExpandChildren_Click);
                    var itemExpandTree = new ToolStripMenuItem("Expand &Tree", null, _contextExpandTree_Click);

                    menu.Items.AddRange(new ToolStripItem[]
                    {
                        itemCollapse, new ToolStripSeparator(), itemExpandChildren, itemExpandTree
                    });
                }
                else
                {
                    var itemExpand = new ToolStripMenuItem("&Expand", null, _contextExpand_Click);
                    itemExpand.Font = new Font(itemExpand.Font, FontStyle.Bold);

                    menu.Items.Add(itemExpand);
                }

            if (node.CanReoderNode)
            {
                var itemUp = new ToolStripMenuItem("Move &Up", Resources.ArrowUp, _contextMoveUp_Click);
                var itemDn = new ToolStripMenuItem("Move &Down", Resources.ArrowDown, _contextMoveDown_Click);

                itemUp.Enabled = node.CanMoveNodeUp;
                itemDn.Enabled = node.CanMoveNodeDown;

                menu.Items.Add(itemUp);
                menu.Items.Add(itemDn);
            }

            if (node is DirectoryDataNode)
            {
                if (menu.Items.Count > 0)
                    menu.Items.Add(new ToolStripSeparator());

                var itemOpenExplorer = new ToolStripMenuItem("Open in E&xplorer", null, _contextOpenInExplorer_Click);
                menu.Items.Add(itemOpenExplorer);
            }

            return menu.Items.Count > 0 ? menu : null;
        }

        private void _contextCollapse_Click(object sender, EventArgs e)
        {
            if (_multiTree.SelectedNode != null)
                _multiTree.SelectedNode.Collapse();
        }

        private void _contextExpand_Click(object sender, EventArgs e)
        {
            if (_multiTree.SelectedNode != null)
                _multiTree.SelectedNode.Expand();
        }

        private void _contextExpandChildren_Click(object sender, EventArgs e)
        {
            if (_multiTree.SelectedNode != null)
                foreach (TreeNode node in _multiTree.SelectedNode.Nodes)
                    node.Expand();
        }

        private void _contextExpandTree_Click(object sender, EventArgs e)
        {
            if (_multiTree.SelectedNode != null) _multiTree.SelectedNode.ExpandAll();
        }

        private void _contextMoveUp_Click(object sender, EventArgs e)
        {
            MoveSelectionUp();
        }

        private void _contextMoveDown_Click(object sender, EventArgs e)
        {
            MoveSelectionDown();
        }

        private void _contextOpenInExplorer_Click(object sender, EventArgs e)
        {
            if (_multiTree.SelectedNode != null && _multiTree.SelectedNode.Tag is DirectoryDataNode)
            {
                var ddNode = _multiTree.SelectedNode.Tag as DirectoryDataNode;
                try
                {
                    var path = (!Interop.IsWindows ? "file://" : "") + ddNode.NodeDirPath;
                    Process.Start(path);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Can't open directory", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        #region Load / Save

        public void Save()
        {
            foreach (TreeNode node in Tree.Nodes)
            {
                var dataNode = node.Tag as DataNode;
                if (dataNode != null)
                    dataNode.Save();
            }

            OnSelectionInvalidated();
        }

        public int OpenPaths(string[] paths)
        {
            Tree.Nodes.Clear();

            var failCount = 0;

            foreach (var path in paths)
                if (Directory.Exists(path))
                {
                    var node = new DirectoryDataNode(path);
                    Tree.Nodes.Add(CreateUnexpandedNode(node));
                }
                else if (File.Exists(path))
                {
                    DataNode node = null;
                    foreach (var item in FileTypeRegistry.RegisteredTypes)
                        if (item.Value.NamePatternTest(path))
                            node = item.Value.NodeCreate(path);

                    if (node == null)
                        node = NbtFileDataNode.TryCreateFrom(path);
                    if (node != null)
                        Tree.Nodes.Add(CreateUnexpandedNode(node));
                    else
                        failCount++;
                }

            if (Tree.Nodes.Count > 0) Tree.Nodes[0].Expand();

            OnSelectionInvalidated();

            return failCount;
        }

        #endregion Load / Save

        #region Capability Checking

        #region Capability Predicates

        public delegate bool DataNodePredicate(DataNode dataNode, out GroupCapabilities caps);

        public static class Predicates
        {
            public static bool CreateByteNodePred(DataNode dataNode, out GroupCapabilities caps)
            {
                caps = GroupCapabilities.Single;
                return dataNode != null && dataNode.CanCreateTag(TagType.TAG_BYTE);
            }

            public static bool CreateShortNodePred(DataNode dataNode, out GroupCapabilities caps)
            {
                caps = GroupCapabilities.Single;
                return dataNode != null && dataNode.CanCreateTag(TagType.TAG_SHORT);
            }

            public static bool CreateIntNodePred(DataNode dataNode, out GroupCapabilities caps)
            {
                caps = GroupCapabilities.Single;
                return dataNode != null && dataNode.CanCreateTag(TagType.TAG_INT);
            }

            public static bool CreateLongNodePred(DataNode dataNode, out GroupCapabilities caps)
            {
                caps = GroupCapabilities.Single;
                return dataNode != null && dataNode.CanCreateTag(TagType.TAG_LONG);
            }

            public static bool CreateFloatNodePred(DataNode dataNode, out GroupCapabilities caps)
            {
                caps = GroupCapabilities.Single;
                return dataNode != null && dataNode.CanCreateTag(TagType.TAG_FLOAT);
            }

            public static bool CreateDoubleNodePred(DataNode dataNode, out GroupCapabilities caps)
            {
                caps = GroupCapabilities.Single;
                return dataNode != null && dataNode.CanCreateTag(TagType.TAG_DOUBLE);
            }

            public static bool CreateByteArrayNodePred(DataNode dataNode, out GroupCapabilities caps)
            {
                caps = GroupCapabilities.Single;
                return dataNode != null && dataNode.CanCreateTag(TagType.TAG_BYTE_ARRAY);
            }

            public static bool CreateIntArrayNodePred(DataNode dataNode, out GroupCapabilities caps)
            {
                caps = GroupCapabilities.Single;
                return dataNode != null && dataNode.CanCreateTag(TagType.TAG_INT_ARRAY);
            }

            public static bool CreateLongArrayNodePred(DataNode dataNode, out GroupCapabilities caps)
            {
                caps = GroupCapabilities.Single;
                return dataNode != null && dataNode.CanCreateTag(TagType.TAG_LONG_ARRAY);
            }

            public static bool CreateStringNodePred(DataNode dataNode, out GroupCapabilities caps)
            {
                caps = GroupCapabilities.Single;
                return dataNode != null && dataNode.CanCreateTag(TagType.TAG_STRING);
            }

            public static bool CreateListNodePred(DataNode dataNode, out GroupCapabilities caps)
            {
                caps = GroupCapabilities.Single;
                return dataNode != null && dataNode.CanCreateTag(TagType.TAG_LIST);
            }

            public static bool CreateCompoundNodePred(DataNode dataNode, out GroupCapabilities caps)
            {
                caps = GroupCapabilities.Single;
                return dataNode != null && dataNode.CanCreateTag(TagType.TAG_COMPOUND);
            }

            public static bool RenameNodePred(DataNode dataNode, out GroupCapabilities caps)
            {
                caps = dataNode.RenameNodeCapabilities;
                return dataNode != null && dataNode.CanRenameNode;
            }

            public static bool EditNodePred(DataNode dataNode, out GroupCapabilities caps)
            {
                caps = dataNode.EditNodeCapabilities;
                return dataNode != null && dataNode.CanEditNode;
            }

            public static bool DeleteNodePred(DataNode dataNode, out GroupCapabilities caps)
            {
                caps = dataNode.DeleteNodeCapabilities;
                return dataNode != null && dataNode.CanDeleteNode;
            }

            public static bool MoveNodeUpPred(DataNode dataNode, out GroupCapabilities caps)
            {
                caps = dataNode.ReorderNodeCapabilities;
                return dataNode != null && dataNode.CanMoveNodeUp;
            }

            public static bool MoveNodeDownPred(DataNode dataNode, out GroupCapabilities caps)
            {
                caps = dataNode.ReorderNodeCapabilities;
                return dataNode != null && dataNode.CanMoveNodeDown;
            }

            public static bool CutNodePred(DataNode dataNode, out GroupCapabilities caps)
            {
                caps = dataNode.CutNodeCapabilities;
                return dataNode != null && dataNode.CanCutNode;
            }

            public static bool CopyNodePred(DataNode dataNode, out GroupCapabilities caps)
            {
                caps = dataNode.CopyNodeCapabilities;
                return dataNode != null && dataNode.CanCopyNode;
            }

            public static bool PasteIntoNodePred(DataNode dataNode, out GroupCapabilities caps)
            {
                caps = dataNode.PasteIntoNodeCapabilities;
                return dataNode != null && dataNode.CanPasteIntoNode;
            }

            public static bool SearchNodePred(DataNode dataNode, out GroupCapabilities caps)
            {
                caps = dataNode.SearchNodeCapabilites;
                return dataNode != null && dataNode.CanSearchNode;
            }

            public static bool ReorderNodePred(DataNode dataNode, out GroupCapabilities caps)
            {
                caps = dataNode.ReorderNodeCapabilities;
                return dataNode != null && dataNode.CanReoderNode;
            }

            public static bool RefreshNodePred(DataNode dataNode, out GroupCapabilities caps)
            {
                caps = dataNode.RefreshNodeCapabilites;
                return dataNode != null && dataNode.CanRefreshNode;
            }
        }

        #endregion Capability Predicates

        public bool CanOperateOnSelection(DataNodePredicate pred)
        {
            if (_multiTree != null)
                return CanOperateOnNodes(_multiTree.SelectedNodes, pred);
            return CanOperateOnNodes(new List<TreeNode> { Tree.SelectedNode }, pred);
        }

        private bool CanOperateOnNodes(IList<TreeNode> nodes, DataNodePredicate pred)
        {
            bool? elideChildren = null;
            return CanOperateOnNodesEx(nodes, pred, out elideChildren);
        }

        private bool CanOperateOnNodesEx(IList<TreeNode> nodes, DataNodePredicate pred, out bool? elideChildren)
        {
            var caps = GroupCapabilities.All;
            elideChildren = null;

            foreach (var node in nodes)
            {
                if (node == null || !(node.Tag is DataNode))
                    return false;

                var dataNode = node.Tag as DataNode;
                GroupCapabilities dataCaps;
                if (!pred(dataNode, out dataCaps))
                    return false;

                caps &= dataCaps;

                var elideChildrenFlag = (dataNode.DeleteNodeCapabilities & GroupCapabilities.ElideChildren) ==
                                        GroupCapabilities.ElideChildren;
                if (elideChildren == null)
                    elideChildren = elideChildrenFlag;
                if (elideChildren != elideChildrenFlag)
                    return false;
            }

            if (nodes.Count > 1 && !SufficientCapabilities(nodes, caps))
                return false;

            return true;
        }

        private IList<TreeNode> ElideChildren(IList<TreeNode> nodes)
        {
            var filtered = new List<TreeNode>();

            foreach (var node in nodes)
            {
                var parent = node.Parent;
                var foundParent = false;

                while (parent != null)
                {
                    if (nodes.Contains(parent))
                    {
                        foundParent = true;
                        break;
                    }
                    parent = parent.Parent;
                }

                if (!foundParent)
                    filtered.Add(node);
            }

            return filtered;
        }

        private bool CommonContainer(IEnumerable<TreeNode> nodes)
        {
            object container = null;
            foreach (var node in nodes)
            {
                var dataNode = node.Tag as DataNode;
                if (container == null)
                    container = dataNode.Parent;

                if (container != dataNode.Parent)
                    return false;
            }

            return true;
        }

        private bool CommonType(IEnumerable<TreeNode> nodes)
        {
            Type datatype = null;
            foreach (var node in nodes)
            {
                var dataNode = node.Tag as DataNode;
                if (datatype == null)
                    datatype = dataNode.GetType();

                if (datatype != dataNode.GetType())
                    return false;
            }

            return true;
        }

        private bool SufficientCapabilities(IEnumerable<TreeNode> nodes, GroupCapabilities commonCaps)
        {
            var commonContainer = CommonContainer(nodes);
            var commonType = CommonType(nodes);

            var pass = true;
            if (commonContainer && commonType)
                pass &= (commonCaps & GroupCapabilities.SiblingSameType) == GroupCapabilities.SiblingSameType;
            else if (commonContainer && !commonType)
                pass &= (commonCaps & GroupCapabilities.SiblingMixedType) == GroupCapabilities.SiblingMixedType;
            else if (!commonContainer && commonType)
                pass &= (commonCaps & GroupCapabilities.MultiSameType) == GroupCapabilities.MultiSameType;
            else if (!commonContainer && !commonType)
                pass &= (commonCaps & GroupCapabilities.MultiMixedType) == GroupCapabilities.MultiMixedType;

            return pass;
        }

        #endregion Capability Checking
    }
}