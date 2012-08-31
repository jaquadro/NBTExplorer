using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Substrate;
using Substrate.Core;
using Substrate.Nbt;
using NBTExplorer.Model;

namespace NBTExplorer
{
    public partial class MainForm : Form
    {
        private static Dictionary<TagType, int> _tagIconIndex;

        private IconRegistry _iconRegistry;

        static MainForm ()
        {
            _tagIconIndex = new Dictionary<TagType, int>();
            _tagIconIndex[TagType.TAG_BYTE] = 0;
            _tagIconIndex[TagType.TAG_SHORT] = 1;
            _tagIconIndex[TagType.TAG_INT] = 2;
            _tagIconIndex[TagType.TAG_LONG] = 3;
            _tagIconIndex[TagType.TAG_FLOAT] = 4;
            _tagIconIndex[TagType.TAG_DOUBLE] = 5;
            _tagIconIndex[TagType.TAG_BYTE_ARRAY] = 6;
            _tagIconIndex[TagType.TAG_STRING] = 7;
            _tagIconIndex[TagType.TAG_LIST] = 8;
            _tagIconIndex[TagType.TAG_COMPOUND] = 9;
            _tagIconIndex[TagType.TAG_INT_ARRAY] = 14;
        }

        public MainForm ()
        {
            InitializeComponent();
            InitializeIconRegistry();

            _nodeTree.BeforeExpand += _nodeTree_BeforeExpand;
            _nodeTree.AfterSelect += _nodeTree_AfterSelect;
            _nodeTree.NodeMouseDoubleClick += _nodeTree_NodeMouseDoubleClick;

            _buttonEdit.Click += _buttonEdit_Click;
            _buttonRename.Click += _buttonRename_Click;
            _buttonDelete.Click += _buttonDelete_Click;
            _buttonCopy.Click += _buttonCopy_Click;
            _buttonCut.Click += _buttonCut_Click;
            _buttonPaste.Click += _buttonPaste_Click;
            _buttonAddTagByte.Click += _buttonAddTagByte_Click;
            _buttonAddTagByteArray.Click += _buttonAddTagByteArray_Click;
            _buttonAddTagCompound.Click += _buttonAddTagCompound_Click;
            _buttonAddTagDouble.Click += _buttonAddTagDouble_Click;
            _buttonAddTagFloat.Click += _buttonAddTagFloat_Click;
            _buttonAddTagInt.Click += _buttonAddTagInt_Click;
            _buttonAddTagIntArray.Click += _buttonAddTagIntArray_Click;
            _buttonAddTagList.Click += _buttonAddTagList_Click;
            _buttonAddTagLong.Click += _buttonAddTagLong_Click;
            _buttonAddTagShort.Click += _buttonAddTagShort_Click;
            _buttonAddTagString.Click += _buttonAddTagString_Click;

            _menuItemExit.Click += _menuItemExit_Click;
            _menuItemEditValue.Click += _menuItemEditValue_Click;
            _menuItemRename.Click += _menuItemRename_Click;
            _menuItemDelete.Click += _menuItemDelete_Click;
            _menuItemCopy.Click += _menuItemCopy_Click;
            _menuItemCut.Click += _menuItemCut_Click;
            _menuItemPaste.Click += _menuItemPaste_Click;
            _menuItemAbout.Click += _menuItemAbout_Click;

            /*_nodeTree.BeforeExpand += NodeExpand;
            _nodeTree.AfterCollapse += NodeCollapse;
            _nodeTree.AfterSelect += NodeSelected;
            _nodeTree.NodeMouseClick += NodeClicked;
            _nodeTree.NodeMouseDoubleClick += NodeDoubleClicked;

            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1) {
                OpenFile(args[1]);
            }
            else {
                OpenMinecraftDir();
            }*/

            //OpenDirectory(@"F:\Minecraft\tps");

            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1) {
                OpenFile(args[1]);
            }
            else {
                OpenMinecraftDirectory();
            }
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
            _iconRegistry.Register(typeof(NbtFileDataNode), 12);
            _iconRegistry.Register(typeof(TagIntArrayDataNode), 14);
        }

        private void OpenFile (string path)
        {
            _nodeTree.Nodes.Clear();

            NbtFileDataNode node = NbtFileDataNode.TryCreateFrom(path);

            _nodeTree.Nodes.Add(CreateUnexpandedNode(node));
        }

        private void OpenDirectory (string path)
        {
            _nodeTree.Nodes.Clear();

            DirectoryDataNode node = new DirectoryDataNode(path);

            TreeNode frontNode = CreateUnexpandedNode(node);
            _nodeTree.Nodes.Add(frontNode);

            ExpandNode(frontNode);
            frontNode.Expand();
        }

        private void OpenMinecraftDirectory ()
        {
            try {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                path = Path.Combine(path, ".minecraft");
                path = Path.Combine(path, "saves");

                if (!Directory.Exists(path)) {
                    path = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);
                }

                OpenDirectory(path);
            }
            catch (Exception) {
                MessageBox.Show("Could not open default Minecraft save directory");
                try {
                    OpenDirectory(Directory.GetCurrentDirectory());
                }
                catch (Exception) {
                    MessageBox.Show("Could not open current directory, this tool is probably not compatible with your platform.");
                    Application.Exit();
                }
            }
        }

        private TreeNode CreateUnexpandedNode (DataNode node)
        {
            TreeNode frontNode = new TreeNode(node.NodeDisplay);
            frontNode.ImageIndex = _iconRegistry.Lookup(node.GetType());
            frontNode.SelectedImageIndex = frontNode.ImageIndex;
            frontNode.Tag = node;

            if (node.HasUnexpandedChildren)
                frontNode.Nodes.Add(new TreeNode());

            return frontNode;
        }

        private void ExpandNode (TreeNode node)
        {
            if (node == null || !(node.Tag is DataNode))
                return;

            DataNode backNode = node.Tag as DataNode;
            if (!backNode.HasUnexpandedChildren)
                return;

            node.Nodes.Clear();

            if (!backNode.IsExpanded)
                backNode.Expand();

            foreach (DataNode child in backNode.Nodes)
                node.Nodes.Add(CreateUnexpandedNode(child));
        }

        private void CreateNode (TreeNode node, TagType type)
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

        private void RefreshChildNodes (TreeNode node, DataNode dataNode)
        {
            Dictionary<DataNode, TreeNode> currentNodes = new Dictionary<DataNode, TreeNode>();
            foreach (TreeNode child in node.Nodes) {
                if (child.Tag is DataNode)
                    currentNodes.Add(child.Tag as DataNode, child);
            }

            foreach (DataNode child in dataNode.Nodes) {
                if (!currentNodes.ContainsKey(child))
                    node.Nodes.Add(CreateUnexpandedNode(child));   
                else
                    currentNodes.Remove(child);
            }

            foreach (TreeNode child in currentNodes.Values) {
                node.Nodes.Remove(child);
            }

            if (node.Nodes.Count == 0 && dataNode.HasUnexpandedChildren) {
                ExpandNode(node);
                node.Expand();
            }
        }

        private void EditNode (TreeNode node)
        {
            if (node == null || !(node.Tag is DataNode))
                return;

            DataNode dataNode = node.Tag as DataNode;
            if (!dataNode.CanEditNode)
                return;

            if (dataNode.EditNode()) {
                node.Text = dataNode.NodeDisplay;
            }
        }

        private void RenameNode (TreeNode node)
        {
            if (node == null || !(node.Tag is DataNode))
                return;

            DataNode dataNode = node.Tag as DataNode;
            if (!dataNode.CanRenameNode)
                return;

            if (dataNode.RenameNode()) {
                node.Text = dataNode.NodeDisplay;
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

        private void CopyNode (TreeNode node)
        {
            if (node == null || !(node.Tag is DataNode))
                return;

            DataNode dataNode = node.Tag as DataNode;
            if (!dataNode.CanCopyNode)
                return;

            dataNode.CopyNode();
        }

        private void CutNode (TreeNode node)
        {
            if (node == null || !(node.Tag is DataNode))
                return;

            DataNode dataNode = node.Tag as DataNode;
            if (!dataNode.CanCutNode)
                return;

            if (dataNode.CutNode()) {
                UpdateUI(node.Parent.Tag as DataNode);
                UpdateNodeText(node.Parent);
                node.Remove();
            }
        }

        private void PasteNode (TreeNode node)
        {
            if (node == null || !(node.Tag is DataNode))
                return;

            DataNode dataNode = node.Tag as DataNode;
            if (!dataNode.CanPasteIntoNode)
                return;

            if (dataNode.PasteNode()) {
                node.Text = dataNode.NodeDisplay;
                RefreshChildNodes(node, dataNode);
                UpdateUI(dataNode);
            }
        }

        private void UpdateNodeText (TreeNode node)
        {
            if (node == null || !(node.Tag is DataNode))
                return;

            DataNode dataNode = node.Tag as DataNode;
            node.Text = dataNode.NodeDisplay;
        }

        private void UpdateUI (DataNode node)
        {
            if (node == null)
                return;

            _buttonAddTagByte.Enabled = node.CanCreateTag(TagType.TAG_BYTE);
            _buttonAddTagByteArray.Enabled = node.CanCreateTag(TagType.TAG_BYTE_ARRAY);
            _buttonAddTagCompound.Enabled = node.CanCreateTag(TagType.TAG_COMPOUND);
            _buttonAddTagDouble.Enabled = node.CanCreateTag(TagType.TAG_DOUBLE);
            _buttonAddTagFloat.Enabled = node.CanCreateTag(TagType.TAG_FLOAT);
            _buttonAddTagInt.Enabled = node.CanCreateTag(TagType.TAG_INT);
            _buttonAddTagIntArray.Enabled = node.CanCreateTag(TagType.TAG_INT_ARRAY);
            _buttonAddTagList.Enabled = node.CanCreateTag(TagType.TAG_LIST);
            _buttonAddTagLong.Enabled = node.CanCreateTag(TagType.TAG_LONG);
            _buttonAddTagShort.Enabled = node.CanCreateTag(TagType.TAG_SHORT);
            _buttonAddTagString.Enabled = node.CanCreateTag(TagType.TAG_STRING);

            _buttonCopy.Enabled = node.CanCopyNode;
            _buttonCut.Enabled = node.CanCutNode;
            _buttonDelete.Enabled = node.CanDeleteNode;
            _buttonEdit.Enabled = node.CanEditNode;
            _buttonFindNext.Enabled = node.CanSearchNode; // Not entirely
            _buttonPaste.Enabled = node.CanPasteIntoNode; // Not entirely
            _buttonRename.Enabled = node.CanRenameNode;

            _menuItemCopy.Enabled = node.CanCopyNode;
            _menuItemCut.Enabled = node.CanCutNode;
            _menuItemDelete.Enabled = node.CanDeleteNode;
            _menuItemEditValue.Enabled = node.CanEditNode;
            _menuItemFind.Enabled = node.CanSearchNode;
            _menuItemPaste.Enabled = node.CanPasteIntoNode; // Not entirely
            _menuItemRename.Enabled = node.CanRenameNode;
        }

        #region Event Handlers

        #region TreeView Event Handlers

        private void _nodeTree_BeforeExpand (object sender, TreeViewCancelEventArgs e)
        {
            ExpandNode(e.Node);
        }

        private void _nodeTree_AfterSelect (object sender, TreeViewEventArgs e)
        {
            UpdateUI(e.Node.Tag as DataNode);
        }

        private void _nodeTree_NodeMouseDoubleClick (object sender, TreeNodeMouseClickEventArgs e)
        {
            EditNode(e.Node);
        }

        #endregion

        #region Toolstrip Event Handlers

        private void _buttonEdit_Click (object sender, EventArgs e)
        {
            EditNode(_nodeTree.SelectedNode);
        }

        private void _buttonRename_Click (object sender, EventArgs e)
        {
            RenameNode(_nodeTree.SelectedNode);
        }

        private void _buttonDelete_Click (object sender, EventArgs e)
        {
            DeleteNode(_nodeTree.SelectedNode);
        }

        private void _buttonCopy_Click (object sernder, EventArgs e)
        {
            CopyNode(_nodeTree.SelectedNode);
        }

        private void _buttonCut_Click (object sernder, EventArgs e)
        {
            CutNode(_nodeTree.SelectedNode);
        }

        private void _buttonPaste_Click (object sernder, EventArgs e)
        {
            PasteNode(_nodeTree.SelectedNode);
        }

        private void _buttonAddTagByteArray_Click (object sender, EventArgs e)
        {
            CreateNode(_nodeTree.SelectedNode, TagType.TAG_BYTE_ARRAY);
        }

        private void _buttonAddTagByte_Click (object sender, EventArgs e)
        {
            CreateNode(_nodeTree.SelectedNode, TagType.TAG_BYTE);
        }

        private void _buttonAddTagCompound_Click (object sender, EventArgs e)
        {
            CreateNode(_nodeTree.SelectedNode, TagType.TAG_COMPOUND);
        }

        private void _buttonAddTagDouble_Click (object sender, EventArgs e)
        {
            CreateNode(_nodeTree.SelectedNode, TagType.TAG_DOUBLE);
        }

        private void _buttonAddTagFloat_Click (object sender, EventArgs e)
        {
            CreateNode(_nodeTree.SelectedNode, TagType.TAG_FLOAT);
        }

        private void _buttonAddTagInt_Click (object sender, EventArgs e)
        {
            CreateNode(_nodeTree.SelectedNode, TagType.TAG_INT);
        }

        private void _buttonAddTagIntArray_Click (object sender, EventArgs e)
        {
            CreateNode(_nodeTree.SelectedNode, TagType.TAG_INT_ARRAY);
        }

        private void _buttonAddTagList_Click (object sender, EventArgs e)
        {
            CreateNode(_nodeTree.SelectedNode, TagType.TAG_LIST);
        }

        private void _buttonAddTagLong_Click (object sender, EventArgs e)
        {
            CreateNode(_nodeTree.SelectedNode, TagType.TAG_LONG);
        }

        private void _buttonAddTagShort_Click (object sender, EventArgs e)
        {
            CreateNode(_nodeTree.SelectedNode, TagType.TAG_SHORT);
        }

        private void _buttonAddTagString_Click (object sender, EventArgs e)
        {
            CreateNode(_nodeTree.SelectedNode, TagType.TAG_STRING);
        }

        #endregion

        #region Menu Event Handlers

        private void _menuItemExit_Click (object sender, EventArgs e)
        {
            Close();
        }

        private void _menuItemEditValue_Click (object sender, EventArgs e)
        {
            EditNode(_nodeTree.SelectedNode);
        }

        private void _menuItemRename_Click (object sender, EventArgs e)
        {
            RenameNode(_nodeTree.SelectedNode);
        }

        private void _menuItemDelete_Click (object sender, EventArgs e)
        {
            DeleteNode(_nodeTree.SelectedNode);
        }

        private void _menuItemCopy_Click (object sender, EventArgs e)
        {
            CopyNode(_nodeTree.SelectedNode);
        }

        private void _menuItemCut_Click (object sender, EventArgs e)
        {
            CutNode(_nodeTree.SelectedNode);
        }

        private void _menuItemPaste_Click (object sender, EventArgs e)
        {
            PasteNode(_nodeTree.SelectedNode);
        }

        private void _menuItemAbout_Click (object sender, EventArgs e)
        {
            new About().ShowDialog();
        }

        #endregion

        #endregion
    }
}
