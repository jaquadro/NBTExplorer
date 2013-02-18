using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using NBTExplorer.Model;
using NBTExplorer.Properties;
using Substrate.Nbt;

namespace NBTExplorer.Windows
{
    public partial class MainForm : Form
    {
        private static Dictionary<TagType, int> _tagIconIndex;

        private IconRegistry _iconRegistry;

        private string _openFolderPath = null;

        static MainForm ()
        {
            try {
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
            catch (Exception e) {
                Program.StaticInitFailure(e);
            }
        }

        public MainForm ()
        {
            InitializeComponent();
            InitializeIconRegistry();
            FormHandlers.Register();
            NbtClipboardController.Initialize(new NbtClipboardControllerWin());

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

            UpdateOpenMenu();
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
        }

        private void OpenFile ()
        {
            if (!ConfirmAction("Open new file anyway?"))
                return;

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.RestoreDirectory = true;
            ofd.Multiselect = true;

            if (ofd.ShowDialog() == DialogResult.OK) {
                OpenPaths(ofd.FileNames);
            }

            UpdateUI();
        }

        private void OpenFolder ()
        {
            if (!ConfirmAction("Open new folder anyway?"))
                return;

            FolderBrowserDialog ofd = new FolderBrowserDialog();
            if (_openFolderPath != null)
                ofd.SelectedPath = _openFolderPath;

            if (ofd.ShowDialog() == DialogResult.OK) {
                _openFolderPath = ofd.SelectedPath;
                OpenPaths(new string[] { ofd.SelectedPath });
            }

            UpdateUI();
        }

        private void OpenPaths (string[] paths)
        {
            _nodeTree.Nodes.Clear();

            foreach (string path in paths) {
                if (Directory.Exists(path)) {
                    DirectoryDataNode node = new DirectoryDataNode(path);
                    _nodeTree.Nodes.Add(CreateUnexpandedNode(node));

                    AddPathToHistory(GetRecentDirectories(), path);
                }
                else if (File.Exists(path)) {
                    DataNode node = null;
                    foreach (var item in FileTypeRegistry.RegisteredTypes) {
                        if (item.Value.NamePatternTest(path))
                            node = item.Value.NodeCreate(path);
                    }

                    if (node != null) {
                        _nodeTree.Nodes.Add(CreateUnexpandedNode(node));
                        AddPathToHistory(GetRecentFiles(), path);
                    }
                }
            }

            if (_nodeTree.Nodes.Count > 0) {
                _nodeTree.Nodes[0].Expand();
            }

            UpdateUI();
            UpdateOpenMenu();
        }

        private void OpenMinecraftDirectory ()
        {
            if (!ConfirmAction("Open Minecraft save folder anyway?"))
                return;

            try {
                string path = Environment.ExpandEnvironmentVariables("%APPDATA%");
                if (!Directory.Exists(path)) {
                    path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                }

                path = Path.Combine(path, ".minecraft");
                path = Path.Combine(path, "saves");

                if (!Directory.Exists(path)) {
                    path = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);
                }

                OpenPaths(new string[] { path });
            }
            catch (Exception e) {
                MessageBox.Show("Could not open default Minecraft save directory");
                Console.WriteLine(e.Message);

                try {
                    OpenPaths(new string[] { Directory.GetCurrentDirectory() });
                }
                catch (Exception) {
                    MessageBox.Show("Could not open current directory, this tool is probably not compatible with your platform.");
                    Console.WriteLine(e.Message);
                    Application.Exit();
                }
            }

            UpdateUI();
        }

        private StringCollection GetRecentFiles ()
        {
            try {
                return Settings.Default.RecentFiles;
            }
            catch {
                return null;
            }
        }

        private StringCollection GetRecentDirectories ()
        {
            try {
                return Settings.Default.RecentDirectories;
            }
            catch {
                return null;
            }
        }

        private TreeNode CreateUnexpandedNode (DataNode node)
        {
            TreeNode frontNode = new TreeNode(node.NodeDisplay);
            frontNode.ImageIndex = _iconRegistry.Lookup(node.GetType());
            frontNode.SelectedImageIndex = frontNode.ImageIndex;
            frontNode.Tag = node;
            frontNode.ContextMenuStrip = BuildNodeContextMenu(node);

            if (node.HasUnexpandedChildren || node.Nodes.Count > 0)
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
            TreeNode frontNode = _nodeTree.SelectedNode;
            if (frontNode == null)
                return;

            DataNode node = frontNode.Tag as DataNode;
            if (node == null || !node.CanMoveNodeDown)
                return;

            node.ChangeRelativePosition(1);
            RefreshChildNodes(frontNode.Parent, node.Parent);
        }

        private void ExpandNode (TreeNode node)
        {
            if (node == null || !(node.Tag is DataNode))
                return;

            if (node.IsExpanded)
                return;

            node.Nodes.Clear();

            DataNode backNode = node.Tag as DataNode;
            if (!backNode.IsExpanded) {
                backNode.Expand();
                node.Text = backNode.NodeDisplay;
                UpdateUI(backNode);
            }

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
            node.Text = backNode.NodeDisplay;
            UpdateUI(backNode);

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
                UpdateUI(dataNode);
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

        private void DeleteNode (IList<TreeNode> nodes)
        {
            bool? elideChildren = null;
            if (!CanOperateOnNodesEx(nodes, DeleteNodePred, out elideChildren))
                return;

            if (elideChildren == true)
                nodes = ElideChildren(nodes);

            foreach (TreeNode node in nodes) {
                DataNode dataNode = node.Tag as DataNode;
                if (dataNode.DeleteNode()) {
                    UpdateNodeText(node.Parent);
                    node.Remove();
                }
            }

            UpdateUI();
        }

        /*private bool CanDeleteNodes (IList<TreeNode> nodes)
        {
            bool? elideChildren = null;
            return CanDeleteNodesEx(nodes, out elideChildren);
        }

        private bool CanDeleteNodesEx (IList<TreeNode> nodes, out bool? elideChildren)
        {
            GroupCapabilities caps = GroupCapabilities.All;
            elideChildren = null;

            foreach (TreeNode node in nodes) {
                if (node == null || !(node.Tag is DataNode))
                    return false;

                DataNode dataNode = node.Tag as DataNode;
                if (!dataNode.CanDeleteNode)
                    return false;

                caps &= dataNode.DeleteNodeCapabilities;

                bool elideChildrenFlag = (dataNode.DeleteNodeCapabilities & GroupCapabilities.ElideChildren) == GroupCapabilities.ElideChildren;
                if (elideChildren == null)
                    elideChildren = elideChildrenFlag;
                if (elideChildren != elideChildrenFlag)
                    return false;
            }

            if (nodes.Count > 1 && !SufficientCapabilities(nodes, caps))
                return false;

            return true;
        }*/

        delegate bool DataNodePredicate (DataNode dataNode, out GroupCapabilities caps);

        private bool CreateByteNodePred (DataNode dataNode, out GroupCapabilities caps)
        {
            caps = GroupCapabilities.Single;
            return (dataNode != null) && dataNode.CanCreateTag(TagType.TAG_BYTE);
        }

        private bool CreateShortNodePred (DataNode dataNode, out GroupCapabilities caps)
        {
            caps = GroupCapabilities.Single;
            return (dataNode != null) && dataNode.CanCreateTag(TagType.TAG_SHORT);
        }

        private bool CreateIntNodePred (DataNode dataNode, out GroupCapabilities caps)
        {
            caps = GroupCapabilities.Single;
            return (dataNode != null) && dataNode.CanCreateTag(TagType.TAG_INT);
        }

        private bool CreateLongNodePred (DataNode dataNode, out GroupCapabilities caps)
        {
            caps = GroupCapabilities.Single;
            return (dataNode != null) && dataNode.CanCreateTag(TagType.TAG_LONG);
        }

        private bool CreateFloatNodePred (DataNode dataNode, out GroupCapabilities caps)
        {
            caps = GroupCapabilities.Single;
            return (dataNode != null) && dataNode.CanCreateTag(TagType.TAG_FLOAT);
        }

        private bool CreateDoubleNodePred (DataNode dataNode, out GroupCapabilities caps)
        {
            caps = GroupCapabilities.Single;
            return (dataNode != null) && dataNode.CanCreateTag(TagType.TAG_DOUBLE);
        }

        private bool CreateByteArrayNodePred (DataNode dataNode, out GroupCapabilities caps)
        {
            caps = GroupCapabilities.Single;
            return (dataNode != null) && dataNode.CanCreateTag(TagType.TAG_BYTE_ARRAY);
        }

        private bool CreateIntArrayNodePred (DataNode dataNode, out GroupCapabilities caps)
        {
            caps = GroupCapabilities.Single;
            return (dataNode != null) && dataNode.CanCreateTag(TagType.TAG_INT_ARRAY);
        }

        private bool CreateStringNodePred (DataNode dataNode, out GroupCapabilities caps)
        {
            caps = GroupCapabilities.Single;
            return (dataNode != null) && dataNode.CanCreateTag(TagType.TAG_STRING);
        }

        private bool CreateListNodePred (DataNode dataNode, out GroupCapabilities caps)
        {
            caps = GroupCapabilities.Single;
            return (dataNode != null) && dataNode.CanCreateTag(TagType.TAG_LIST);
        }

        private bool CreateCompoundNodePred (DataNode dataNode, out GroupCapabilities caps)
        {
            caps = GroupCapabilities.Single;
            return (dataNode != null) && dataNode.CanCreateTag(TagType.TAG_COMPOUND);
        }

        private bool RenameNodePred (DataNode dataNode, out GroupCapabilities caps)
        {
            caps = dataNode.RenameNodeCapabilities;
            return (dataNode != null) && dataNode.CanRenameNode;
        }

        private bool EditNodePred (DataNode dataNode, out GroupCapabilities caps)
        {
            caps = dataNode.EditNodeCapabilities;
            return (dataNode != null) && dataNode.CanEditNode;
        }

        private bool DeleteNodePred (DataNode dataNode, out GroupCapabilities caps)
        {
            caps = dataNode.DeleteNodeCapabilities;
            return (dataNode != null) && dataNode.CanDeleteNode;
        }

        private bool CutNodePred (DataNode dataNode, out GroupCapabilities caps)
        {
            caps = dataNode.CutNodeCapabilities;
            return (dataNode != null) && dataNode.CanCutNode;
        }

        private bool CopyNodePred (DataNode dataNode, out GroupCapabilities caps)
        {
            caps = dataNode.CopyNodeCapabilities;
            return (dataNode != null) && dataNode.CanCopyNode;
        }

        private bool PasteIntoNodePred (DataNode dataNode, out GroupCapabilities caps)
        {
            caps = dataNode.PasteIntoNodeCapabilities;
            return (dataNode != null) && dataNode.CanPasteIntoNode;
        }

        private bool SearchNodePred (DataNode dataNode, out GroupCapabilities caps)
        {
            caps = dataNode.SearchNodeCapabilites;
            return (dataNode != null) && dataNode.CanSearchNode;
        }

        private bool ReorderNodePred (DataNode dataNode, out GroupCapabilities caps)
        {
            caps = dataNode.ReorderNodeCapabilities;
            return (dataNode != null) && dataNode.CanReoderNode;
        }

        private bool RefreshNodePred (DataNode dataNode, out GroupCapabilities caps)
        {
            caps = dataNode.RefreshNodeCapabilites;
            return (dataNode != null) && dataNode.CanRefreshNode;
        }

        /*private bool CreateTagNodePred (DataNode dataNode, out GroupCapabilities caps)
        {
            caps = GroupCapabilities.Single;
            return (dataNode != null) && dataNode.CanCreateTag
        }*/

        private bool CanOperateOnNodes (IList<TreeNode> nodes, DataNodePredicate pred)
        {
            bool? elideChildren = null;
            return CanOperateOnNodesEx(nodes, pred, out elideChildren);
        }

        private bool CanOperateOnNodesEx (IList<TreeNode> nodes, DataNodePredicate pred, out bool? elideChildren)
        {
            GroupCapabilities caps = GroupCapabilities.All;
            elideChildren = null;

            foreach (TreeNode node in nodes) {
                if (node == null || !(node.Tag is DataNode))
                    return false;

                DataNode dataNode = node.Tag as DataNode;
                GroupCapabilities dataCaps;
                if (!pred(dataNode, out dataCaps))
                    return false;

                caps &= dataCaps;

                bool elideChildrenFlag = (dataNode.DeleteNodeCapabilities & GroupCapabilities.ElideChildren) == GroupCapabilities.ElideChildren;
                if (elideChildren == null)
                    elideChildren = elideChildrenFlag;
                if (elideChildren != elideChildrenFlag)
                    return false;
            }

            if (nodes.Count > 1 && !SufficientCapabilities(nodes, caps))
                return false;

            return true;
        }

        private IList<TreeNode> ElideChildren (IList<TreeNode> nodes)
        {
            List<TreeNode> filtered = new List<TreeNode>();

            foreach (TreeNode node in nodes) {
                TreeNode parent = node.Parent;
                bool foundParent = false;

                while (parent != null) {
                    if (nodes.Contains(parent)) {
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

        private bool CommonContainer (IEnumerable<TreeNode> nodes)
        {
            object container = null;
            foreach (TreeNode node in nodes) {
                DataNode dataNode = node.Tag as DataNode;
                if (container == null)
                    container = dataNode.Parent;

                if (container != dataNode.Parent)
                    return false;
            }

            return true;
        }

        private bool CommonType (IEnumerable<TreeNode> nodes)
        {
            Type datatype = null;
            foreach (TreeNode node in nodes) {
                DataNode dataNode = node.Tag as DataNode;
                if (datatype == null)
                    datatype = dataNode.GetType();

                if (datatype != dataNode.GetType())
                    return false;
            }

            return true;
        }

        private bool SufficientCapabilities (IEnumerable<TreeNode> nodes, GroupCapabilities commonCaps)
        {
            bool commonContainer = CommonContainer(nodes);
            bool commonType = CommonType(nodes);

            bool pass = true;
            if (commonContainer && commonType)
                pass &= ((commonCaps & GroupCapabilities.SiblingSameType) == GroupCapabilities.SiblingSameType);
            else if (commonContainer && !commonType)
                pass &= ((commonCaps & GroupCapabilities.SiblingMixedType) == GroupCapabilities.SiblingMixedType);
            else if (!commonContainer && commonType)
                pass &= ((commonCaps & GroupCapabilities.MultiSameType) == GroupCapabilities.MultiSameType);
            else if (!commonContainer && !commonType)
                pass &= ((commonCaps & GroupCapabilities.MultiMixedType) == GroupCapabilities.MultiMixedType);

            return pass;
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

        private void RefreshNode (TreeNode node)
        {
            if (node == null || !(node.Tag is DataNode))
                return;

            DataNode dataNode = node.Tag as DataNode;
            if (!dataNode.CanRefreshNode)
                return;

            if (!ConfirmAction("Refresh data anyway?"))
                return;

            if (dataNode.RefreshNode()) {
                RefreshChildNodes(node, dataNode);
                UpdateUI(dataNode);
                ExpandToEdge(node);
            }
        }

        private void ExpandToEdge (TreeNode node)
        {
            if (node == null || !(node.Tag is DataNode))
                return;

            DataNode dataNode = node.Tag as DataNode;
            if (dataNode.IsExpanded) {
                if (!node.IsExpanded)
                    node.Expand();

                foreach (TreeNode child in node.Nodes)
                    ExpandToEdge(child);
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

        private bool ConfirmAction (string actionMessage)
        {
            if (CheckModifications()) {
                if (MessageBox.Show("You currently have unsaved changes.  " + actionMessage, "Unsaved Changes", MessageBoxButtons.OKCancel) != DialogResult.OK)
                    return false;
            }

            return true;
        }

        private CancelSearchForm _searchForm;
        private SearchStateWin _searchState;

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

            _searchState = new SearchStateWin(this) {
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

            SearchWorker worker = new SearchWorker(_searchState);

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

        private void SearchEndCallback (DataNode node)
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
            if (_nodeTree.SelectedNodes.Count > 1) {
                UpdateUI(_nodeTree.SelectedNodes);
            }
            else if (selected != null && selected.Tag is DataNode) {
                UpdateUI(selected.Tag as DataNode);
            }
            else {
                DisableButtons(_buttonAddTagByte, _buttonAddTagByteArray, _buttonAddTagCompound, _buttonAddTagDouble, _buttonAddTagFloat,
                    _buttonAddTagInt, _buttonAddTagIntArray, _buttonAddTagList, _buttonAddTagLong, _buttonAddTagShort,
                    _buttonAddTagString, _buttonCopy, _buttonCut, _buttonDelete, _buttonEdit, _buttonPaste, _buttonRefresh,
                    _buttonRename);

                _buttonSave.Enabled = CheckModifications();
                _buttonFindNext.Enabled = false;

                DisableMenuItems(_menuItemCopy, _menuItemCut, _menuItemDelete, _menuItemEditValue, _menuItemPaste, _menuItemRefresh,
                    _menuItemRename);

                _menuItemSave.Enabled = _buttonSave.Enabled;
                _menuItemFind.Enabled = false;
                _menuItemFindNext.Enabled = _searchState != null;
            }
        }

        private void DisableButtons (params ToolStripButton[] buttons)
        {
            foreach (ToolStripButton button in buttons)
                button.Enabled = false;
        }

        private void DisableMenuItems (params ToolStripMenuItem[] buttons)
        {
            foreach (ToolStripMenuItem button in buttons)
                button.Enabled = false;
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
            _buttonCopy.Enabled = node.CanCopyNode && NbtClipboardController.IsInitialized;
            _buttonCut.Enabled = node.CanCutNode && NbtClipboardController.IsInitialized;
            _buttonDelete.Enabled = node.CanDeleteNode;
            _buttonEdit.Enabled = node.CanEditNode;
            _buttonFindNext.Enabled = node.CanSearchNode || _searchState != null;
            _buttonPaste.Enabled = node.CanPasteIntoNode && NbtClipboardController.IsInitialized;
            _buttonRename.Enabled = node.CanRenameNode;
            _buttonRefresh.Enabled = node.CanRefreshNode;

            _menuItemSave.Enabled = _buttonSave.Enabled;
            _menuItemCopy.Enabled = node.CanCopyNode && NbtClipboardController.IsInitialized;
            _menuItemCut.Enabled = node.CanCutNode && NbtClipboardController.IsInitialized;
            _menuItemDelete.Enabled = node.CanDeleteNode;
            _menuItemEditValue.Enabled = node.CanEditNode;
            _menuItemFind.Enabled = node.CanSearchNode;
            _menuItemPaste.Enabled = node.CanPasteIntoNode && NbtClipboardController.IsInitialized;
            _menuItemRename.Enabled = node.CanRenameNode;
            _menuItemRefresh.Enabled = node.CanRefreshNode;
            _menuItemFind.Enabled = node.CanSearchNode;
            _menuItemFindNext.Enabled = _searchState != null;

            UpdateUI(_nodeTree.SelectedNodes);
        }

        private void UpdateUI (IList<TreeNode> nodes)
        {
            if (nodes == null)
                return;

            _buttonAddTagByte.Enabled = CanOperateOnNodes(nodes, CreateByteNodePred);
            _buttonAddTagShort.Enabled = CanOperateOnNodes(nodes, CreateShortNodePred);
            _buttonAddTagInt.Enabled = CanOperateOnNodes(nodes, CreateIntNodePred);
            _buttonAddTagLong.Enabled = CanOperateOnNodes(nodes, CreateLongNodePred);
            _buttonAddTagFloat.Enabled = CanOperateOnNodes(nodes, CreateFloatNodePred);
            _buttonAddTagDouble.Enabled = CanOperateOnNodes(nodes, CreateDoubleNodePred);
            _buttonAddTagByteArray.Enabled = CanOperateOnNodes(nodes, CreateByteArrayNodePred);
            _buttonAddTagIntArray.Enabled = CanOperateOnNodes(nodes, CreateIntArrayNodePred);
            _buttonAddTagString.Enabled = CanOperateOnNodes(nodes, CreateStringNodePred);
            _buttonAddTagList.Enabled = CanOperateOnNodes(nodes, CreateListNodePred);
            _buttonAddTagCompound.Enabled = CanOperateOnNodes(nodes, CreateCompoundNodePred);

            _buttonSave.Enabled = CheckModifications();
            _buttonRename.Enabled = CanOperateOnNodes(nodes, RenameNodePred);
            _buttonEdit.Enabled = CanOperateOnNodes(nodes, EditNodePred);
            _buttonDelete.Enabled = CanOperateOnNodes(nodes, DeleteNodePred);
            _buttonCut.Enabled = CanOperateOnNodes(nodes, CutNodePred) && NbtClipboardController.IsInitialized; ;
            _buttonCopy.Enabled = CanOperateOnNodes(nodes, CopyNodePred) && NbtClipboardController.IsInitialized; ;
            _buttonPaste.Enabled = CanOperateOnNodes(nodes, PasteIntoNodePred) && NbtClipboardController.IsInitialized; ;
            _buttonFindNext.Enabled = CanOperateOnNodes(nodes, SearchNodePred) || _searchState != null;
            _buttonRefresh.Enabled = CanOperateOnNodes(nodes, RefreshNodePred);

            _menuItemSave.Enabled = _buttonSave.Enabled;
            _menuItemRename.Enabled = _buttonRename.Enabled;
            _menuItemEditValue.Enabled = _buttonEdit.Enabled;
            _menuItemDelete.Enabled = _buttonDelete.Enabled;
            _menuItemCut.Enabled = _buttonCut.Enabled;
            _menuItemCopy.Enabled = _buttonCopy.Enabled;
            _menuItemPaste.Enabled = _buttonPaste.Enabled;
            _menuItemFind.Enabled = CanOperateOnNodes(nodes, SearchNodePred);
            _menuItemRefresh.Enabled = _buttonRefresh.Enabled;
            _menuItemFindNext.Enabled = _searchState != null;
        }

        private void UpdateOpenMenu ()
        {
            try {
                if (Settings.Default.RecentDirectories == null)
                    Settings.Default.RecentDirectories = new StringCollection();
                if (Settings.Default.RecentFiles == null)
                    Settings.Default.RecentFiles = new StringCollection();
            }
            catch {
                return;
            }

            _menuItemRecentFolders.DropDown = BuildRecentEntriesDropDown(Settings.Default.RecentDirectories);
            _menuItemRecentFiles.DropDown = BuildRecentEntriesDropDown(Settings.Default.RecentFiles);
        }

        private ToolStripDropDown BuildRecentEntriesDropDown (StringCollection list)
        {
            if (list == null || list.Count == 0)
                return new ToolStripDropDown();

            ToolStripDropDown menu = new ToolStripDropDown();
            foreach (string entry in list) {
                ToolStripMenuItem item = new ToolStripMenuItem("&" + (menu.Items.Count + 1) + " " + entry);
                item.Tag = entry;
                item.Click += _menuItemRecentPaths_Click;

                menu.Items.Add(item);
            }

            return menu;
        }

        private void AddPathToHistory (StringCollection list, string entry)
        {
            if (list == null)
                return;

            foreach (string item in list) {
                if (item == entry) {
                    list.Remove(item);
                    break;
                }
            }

            while (list.Count >= 5)
                list.RemoveAt(list.Count - 1);

            list.Insert(0, entry);
        }

        private GroupCapabilities CommonCapabilities (IEnumerable<GroupCapabilities> capabilities)
        {
            GroupCapabilities caps = GroupCapabilities.All;
            foreach (GroupCapabilities cap in capabilities)
                caps &= cap;
            return caps;
        }

        public void ActionDeleteNode ()
        {
            DeleteNode(_nodeTree.SelectedNodes);

            _nodeTree.SelectedNodes.Clear();
            _nodeTree.SelectedNode = null;
            UpdateUI();
        }

        #region Event Handlers

        private void MainForm_Closing (object sender, CancelEventArgs e)
        {
            Settings.Default.RecentFiles = Settings.Default.RecentFiles;
            Settings.Default.Save();
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
            ActionDeleteNode();
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

        private void _buttonRefresh_Click (object sender, EventArgs e)
        {
            RefreshNode(_nodeTree.SelectedNode);
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
            Settings.Default.Save();
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
            ActionDeleteNode();
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

        private void _menuItemRecentPaths_Click (object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            if (item == null || !(item.Tag is string))
                return;

            OpenPaths(new string[] { item.Tag as string });
        }

        private void refreshToolStripMenuItem_Click (object sender, EventArgs e)
        {
            RefreshNode(_nodeTree.SelectedNode);
        }

        #endregion

        #endregion
    }
}
