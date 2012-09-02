using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using NBTExplorer.Forms;
using NBTExplorer.Model;
using Substrate.Nbt;

namespace NBTExplorer
{
    public partial class MainForm : Form
    {
        private static Dictionary<TagType, int> _tagIconIndex;

        private IconRegistry _iconRegistry;

        private string _openFolderPath = null;

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

            FormClosing += MainForm_Closing;

            _nodeTree.BeforeExpand += _nodeTree_BeforeExpand;
            _nodeTree.AfterCollapse += _nodeTree_AfterCollapse;
            _nodeTree.AfterSelect += _nodeTree_AfterSelect;
            _nodeTree.NodeMouseDoubleClick += _nodeTree_NodeMouseDoubleClick;
            _nodeTree.NodeMouseClick += _nodeTree_NodeMouseClick;
            _nodeTree.DragEnter += _nodeTree_DragEnter;
            _nodeTree.DragDrop += _nodeTree_DragDrop;

            _buttonOpen.Click += _buttonOpen_Click;
            _buttonOpenFolder.Click += _buttonOpenFolder_Click;
            _buttonSave.Click += _buttonSave_Click;
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
            _buttonFindNext.Click += _buttonFindNext_Click;

            _menuItemOpen.Click += _menuItemOpen_Click;
            _menuItemOpenFolder.Click += _menuItemOpenFolder_Click;
            _menuItemOpenMinecraftSaveFolder.Click += _menuItemOpenMinecraftSaveFolder_Click;
            _menuItemSave.Click += _menuItemSave_Click;
            _menuItemExit.Click += _menuItemExit_Click;
            _menuItemEditValue.Click += _menuItemEditValue_Click;
            _menuItemRename.Click += _menuItemRename_Click;
            _menuItemDelete.Click += _menuItemDelete_Click;
            _menuItemCopy.Click += _menuItemCopy_Click;
            _menuItemCut.Click += _menuItemCut_Click;
            _menuItemPaste.Click += _menuItemPaste_Click;
            _menuItemFind.Click += _menuItemFind_Click;
            _menuItemFindNext.Click += _menuItemFindNext_Click;
            _menuItemAbout.Click += _menuItemAbout_Click;

            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1) {
                string[] paths = new string[args.Length - 1];
                Array.Copy(args, 1, paths, 0, paths.Length);
                OpenPaths(paths);
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

        public void OpenFile ()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.RestoreDirectory = true;
            ofd.Multiselect = true;

            if (ofd.ShowDialog() == DialogResult.OK) {
                OpenPaths(ofd.FileNames);
            }
        }

        private void OpenFolder ()
        {
            FolderBrowserDialog ofd = new FolderBrowserDialog();
            if (_openFolderPath != null)
                ofd.SelectedPath = _openFolderPath;

            if (ofd.ShowDialog() == DialogResult.OK) {
                _openFolderPath = ofd.SelectedPath;
                OpenPaths(new string[] { ofd.SelectedPath });
            }
        }

        public void OpenPaths (string[] paths)
        {
            _nodeTree.Nodes.Clear();

            foreach (string path in paths) {
                if (Directory.Exists(path)) {
                    DirectoryDataNode node = new DirectoryDataNode(path);
                    _nodeTree.Nodes.Add(CreateUnexpandedNode(node));
                }
                else if (File.Exists(path)) {
                    NbtFileDataNode node = NbtFileDataNode.TryCreateFrom(path);
                    _nodeTree.Nodes.Add(CreateUnexpandedNode(node));
                }
            }

            if (_nodeTree.Nodes.Count > 0) {
                _nodeTree.Nodes[0].Expand();
            }
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

                OpenPaths(new string[] { path });
            }
            catch (Exception) {
                MessageBox.Show("Could not open default Minecraft save directory");
                try {
                    OpenPaths(new string[] { Directory.GetCurrentDirectory() });
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
            frontNode.ContextMenuStrip = BuildNodeContextMenu(node);

            if (node.HasUnexpandedChildren)
                frontNode.Nodes.Add(new TreeNode());

            return frontNode;
        }

        private ContextMenuStrip BuildNodeContextMenu (DataNode node)
        {
            if (node == null)
                return null;

            ContextMenuStrip menu = new ContextMenuStrip();

            if (node.CanReoderNode) {
                ToolStripMenuItem itemUp = new ToolStripMenuItem("Move &Up", Properties.Resources.ArrowUp, _contextMoveUp_Click);
                ToolStripMenuItem itemDn = new ToolStripMenuItem("Move &Down", Properties.Resources.ArrowDown, _contextMoveDown_Click);

                itemUp.Enabled = node.CanMoveNodeUp;
                itemDn.Enabled = node.CanMoveNodeDown;

                menu.Items.Add(itemUp);
                menu.Items.Add(itemDn);
            }

            return (menu.Items.Count > 0) ? menu : null;
        }

        private void _contextMoveUp_Click (object sender, EventArgs e)
        {
            TreeNode frontNode = _nodeTree.SelectedNode;
            if (frontNode == null)
                return;

            DataNode node = frontNode.Tag as DataNode;
            if (node == null || !node.CanMoveNodeUp)
                return;

            node.ChangeRelativePosition(-1);
            RefreshChildNodes(frontNode.Parent, node.Parent);
        }

        private void _contextMoveDown_Click (object sender, EventArgs e)
        {

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

        private void CollapseNode (TreeNode node)
        {
            if (node == null || !(node.Tag is DataNode))
                return;

            DataNode backNode = node.Tag as DataNode;
            if (backNode.IsModified)
                return;

            backNode.Collapse();

            node.Nodes.Clear();
            if (backNode.HasUnexpandedChildren)
                node.Nodes.Add(new TreeNode());
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

            node.Nodes.Clear();
            foreach (DataNode child in dataNode.Nodes) {
                if (!currentNodes.ContainsKey(child))
                    node.Nodes.Add(CreateUnexpandedNode(child));
                else
                    node.Nodes.Add(currentNodes[child]);
            }

            foreach (TreeNode child in node.Nodes)
                child.ContextMenuStrip = BuildNodeContextMenu(child.Tag as DataNode);

            /*foreach (DataNode child in dataNode.Nodes) {
                if (!currentNodes.ContainsKey(child))
                    node.Nodes.Add(CreateUnexpandedNode(child));   
                else
                    currentNodes.Remove(child);
            }

            foreach (TreeNode child in currentNodes.Values) {
                node.Nodes.Remove(child);
            }*/

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
                UpdateUI(dataNode);
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

        private void Save ()
        {
            foreach (TreeNode node in _nodeTree.Nodes) {
                DataNode dataNode = node.Tag as DataNode;
                if (dataNode != null)
                    dataNode.Save();
            }

            UpdateUI();
        }

        private bool ConfirmExit ()
        {
            if (CheckModifications()) {
                if (MessageBox.Show("You currently have unsaved changes.  Close anyway?", "Unsaved Changes", MessageBoxButtons.OKCancel) != DialogResult.OK)
                    return false;
            }

            return true;
        }

        private CancelSearchForm _searchForm;
        private SearchState _searchState;

        private void SearchNode (TreeNode node)
        {
            if (node == null || !(node.Tag is DataNode))
                return;

            DataNode dataNode = node.Tag as DataNode;
            if (!dataNode.CanSearchNode)
                return;

            Find form = new Find();
            if (form.ShowDialog() != DialogResult.OK)
                return;

            _searchState = new SearchState() {
                RootNode = dataNode,
                SearchName = form.NameToken,
                SearchValue = form.ValueToken,
                DiscoverCallback = SearchDiscoveryCallback,
                CollapseCallback = SearchCollapseCallback,
                EndCallback = SearchEndCallback,
            };

            SearchNextNode();
        }

        private void SearchNextNode ()
        {
            if (_searchState == null)
                return;

            SearchWorker worker = new SearchWorker(_searchState, this);

            Thread t = new Thread(new ThreadStart(worker.Run));
            t.IsBackground = true;
            t.Start();

            _searchForm = new CancelSearchForm();
            if (_searchForm.ShowDialog(this) == DialogResult.Cancel) {
                worker.Cancel();
                _searchState = null;
            }

            t.Join();
        }

        private void SearchDiscoveryCallback (DataNode node)
        {
            _nodeTree.SelectedNode = FindFrontNode(node);

            if (_searchForm != null) {
                _searchForm.DialogResult = DialogResult.OK;
                _searchForm = null;
            }
        }

        private void SearchCollapseCallback (DataNode node)
        {
            CollapseBelow(node);
        }

        private void SearchEndCallback ()
        {
            _searchForm.DialogResult = DialogResult.OK;
            _searchForm = null;

            MessageBox.Show("End of results");
        }

        private TreeNode GetRootFromDataNodePath (DataNode node, out Stack<DataNode> hierarchy)
        {
            hierarchy = new Stack<DataNode>();
            while (node != null) {
                hierarchy.Push(node);
                node = node.Parent;
            }

            DataNode rootDataNode = hierarchy.Pop();
            TreeNode frontNode = null;
            foreach (TreeNode child in _nodeTree.Nodes) {
                if (child.Tag == rootDataNode)
                    frontNode = child;
            }

            return frontNode;
        }

        private TreeNode FindFrontNode (DataNode node)
        {
            Stack<DataNode> hierarchy;
            TreeNode frontNode = GetRootFromDataNodePath(node, out hierarchy);

            if (frontNode == null)
                return null;

            while (hierarchy.Count > 0) {
                if (!frontNode.IsExpanded) {
                    frontNode.Nodes.Add(new TreeNode());
                    frontNode.Expand();
                }

                DataNode childData = hierarchy.Pop();
                foreach (TreeNode childFront in frontNode.Nodes) {
                    if (childFront.Tag == childData) {
                        frontNode = childFront;
                        break;
                    }
                }
            }

            return frontNode;
        }

        private void CollapseBelow (DataNode node)
        {
            Stack<DataNode> hierarchy;
            TreeNode frontNode = GetRootFromDataNodePath(node, out hierarchy);

            if (frontNode == null)
                return;

            while (hierarchy.Count > 0) {
                if (!frontNode.IsExpanded)
                    return;

                DataNode childData = hierarchy.Pop();
                foreach (TreeNode childFront in frontNode.Nodes) {
                    if (childFront.Tag == childData) {
                        frontNode = childFront;
                        break;
                    }
                }
            }

            if (frontNode.IsExpanded)
                frontNode.Collapse();
        }

        private void UpdateNodeText (TreeNode node)
        {
            if (node == null || !(node.Tag is DataNode))
                return;

            DataNode dataNode = node.Tag as DataNode;
            node.Text = dataNode.NodeDisplay;
        }

        private bool CheckModifications ()
        {
            foreach (TreeNode node in _nodeTree.Nodes) {
                DataNode dataNode = node.Tag as DataNode;
                if (dataNode != null && dataNode.IsModified)
                    return true;
            }

            return false;
        }

        private void UpdateUI ()
        {
            TreeNode selected = _nodeTree.SelectedNode;
            if (selected != null && selected.Tag is DataNode) {
                UpdateUI(selected.Tag as DataNode);
            }
            else {
                _buttonSave.Enabled = CheckModifications();
                _buttonFindNext.Enabled = false;

                _menuItemSave.Enabled = _buttonSave.Enabled;
                _menuItemFind.Enabled = false;
                _menuItemFindNext.Enabled = _searchState != null;
            }
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

            _buttonSave.Enabled = CheckModifications();
            _buttonCopy.Enabled = node.CanCopyNode;
            _buttonCut.Enabled = node.CanCutNode;
            _buttonDelete.Enabled = node.CanDeleteNode;
            _buttonEdit.Enabled = node.CanEditNode;
            _buttonFindNext.Enabled = node.CanSearchNode || _searchState != null;
            _buttonPaste.Enabled = node.CanPasteIntoNode;
            _buttonRename.Enabled = node.CanRenameNode;

            _menuItemSave.Enabled = _buttonSave.Enabled;
            _menuItemCopy.Enabled = node.CanCopyNode;
            _menuItemCut.Enabled = node.CanCutNode;
            _menuItemDelete.Enabled = node.CanDeleteNode;
            _menuItemEditValue.Enabled = node.CanEditNode;
            _menuItemFind.Enabled = node.CanSearchNode;
            _menuItemPaste.Enabled = node.CanPasteIntoNode;
            _menuItemRename.Enabled = node.CanRenameNode;
            _menuItemFind.Enabled = node.CanSearchNode;
            _menuItemFindNext.Enabled = _searchState != null;
        }

        #region Event Handlers

        private void MainForm_Closing (object sender, CancelEventArgs e)
        {
            if (!ConfirmExit())
                e.Cancel = true;
        }

        #region TreeView Event Handlers

        private void _nodeTree_BeforeExpand (object sender, TreeViewCancelEventArgs e)
        {
            ExpandNode(e.Node);
        }

        private void _nodeTree_AfterCollapse (object sender, TreeViewEventArgs e)
        {
            CollapseNode(e.Node);
        }

        private void _nodeTree_AfterSelect (object sender, TreeViewEventArgs e)
        {
            if (e.Node != null)
                UpdateUI(e.Node.Tag as DataNode);
        }

        private void _nodeTree_NodeMouseDoubleClick (object sender, TreeNodeMouseClickEventArgs e)
        {
            EditNode(e.Node);
        }

        private void _nodeTree_NodeMouseClick (object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                _nodeTree.SelectedNode = e.Node;
        }

        private void _nodeTree_DragDrop (object sender, DragEventArgs e)
        {
            OpenPaths((string[])e.Data.GetData(DataFormats.FileDrop));
        }

        private void _nodeTree_DragEnter (object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        #endregion

        #region Toolstrip Event Handlers

        private void _buttonOpen_Click (object sender, EventArgs e)
        {
            OpenFile();
        }

        private void _buttonOpenFolder_Click (object sender, EventArgs e)
        {
            OpenFolder();
        }

        private void _buttonSave_Click (object sender, EventArgs e)
        {
            Save();
        }

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

        private void _buttonFindNext_Click (object sender, EventArgs e)
        {
            if (_searchState != null)
                SearchNextNode();
            else
                SearchNode(_nodeTree.SelectedNode);
        }

        #endregion

        #region Menu Event Handlers

        private void _menuItemOpen_Click (object sender, EventArgs e)
        {
            OpenFile();
        }

        private void _menuItemOpenFolder_Click (object sender, EventArgs e)
        {
            OpenFolder();
        }

        private void _menuItemOpenMinecraftSaveFolder_Click (object sender, EventArgs e)
        {
            OpenMinecraftDirectory();
        }

        private void _menuItemSave_Click (object sender, EventArgs e)
        {
            Save();
        }

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

        private void _menuItemFind_Click (object sender, EventArgs e)
        {
            SearchNode(_nodeTree.SelectedNode);
        }

        private void _menuItemFindNext_Click (object sender, EventArgs e)
        {
            SearchNextNode();
        }

        private void _menuItemAbout_Click (object sender, EventArgs e)
        {
            new About().ShowDialog();
        }

        #endregion

        #endregion
    }
}
