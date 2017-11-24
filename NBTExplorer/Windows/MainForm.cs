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
using NBTExplorer.Controllers;

namespace NBTExplorer.Windows
{
    using Predicates = NodeTreeController.Predicates;
    using NBTModel.Interop;

    public partial class MainForm : Form
    {
        private static Dictionary<TagType, int> _tagIconIndex;

        private NodeTreeController _controller;
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
                _tagIconIndex[TagType.TAG_SHORT_ARRAY] = 16;
                _tagIconIndex[TagType.TAG_LONG_ARRAY] = 17;
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

            _controller = new NodeTreeController(_nodeTree);
            _controller.ConfirmAction += _controller_ConfirmAction;
            _controller.SelectionInvalidated += _controller_SelectionInvalidated;

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
            _buttonAddTagLongArray.Click += _buttonAddTagLongArray_Click;
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
            _menuItemOpenInExplorer.Click += _menuItemOpenInExplorer_Click;

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

        void _menuItemOpenInExplorer_Click(object sender, EventArgs e)
        {
            if (_nodeTree.SelectedNode.Tag is DirectoryDataNode) {
                DirectoryDataNode ddNode = _nodeTree.SelectedNode.Tag as DirectoryDataNode;
                try {
                    string path = (!Interop.IsWindows ? "file://" : "") + ddNode.NodeDirPath;
                    System.Diagnostics.Process.Start(path);
                } catch (Win32Exception ex) {
                    MessageBox.Show(ex.Message, "Can't open directory", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
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
            _iconRegistry.Register(typeof(CubicRegionDataNode), 11);
            _iconRegistry.Register(typeof(NbtFileDataNode), 12);
            _iconRegistry.Register(typeof(TagIntArrayDataNode), 14);
            _iconRegistry.Register(typeof(TagShortArrayDataNode), 16);
        }

        private void OpenFile ()
        {
            if (!ConfirmAction("Open new file anyway?"))
                return;

            using (OpenFileDialog ofd = new OpenFileDialog() {
                RestoreDirectory = true,
                Multiselect = true,
                Filter = "All Files|*|NBT Files (*.dat, *.schematic)|*.dat;*.nbt;*.schematic|Region Files (*.mca, *.mcr)|*.mca;*.mcr",
                FilterIndex = 0,
            }) {
                if (ofd.ShowDialog() == DialogResult.OK) {
                    OpenPaths(ofd.FileNames);
                }
            }

            UpdateUI();
        }

        private void OpenFolder ()
        {
            if (!ConfirmAction("Open new folder anyway?"))
                return;

            if ((ModifierKeys & Keys.Control) > 0 && (ModifierKeys & Keys.Shift) == 0) {
                // If the user is holding Control, use a file open dialog and open whichever directory has the selected file.
                // But not if the user is also holding Shift, as Ctrl+Shift+O is the keyboard shortcut for this menu item.
                using (OpenFileDialog ofd = new OpenFileDialog()) {
                    ofd.Title = "Select any file in the directory to open";
                    ofd.Filter = "All files (*.*)|*.*";

                    if (_openFolderPath != null)
                        ofd.InitialDirectory = _openFolderPath;

                    if (ofd.ShowDialog() == DialogResult.OK) {
                        _openFolderPath = Path.GetDirectoryName(ofd.FileName);
                        OpenPaths(new string[] { _openFolderPath });
                    }
                }
            } else {
                // Otherwise, use the standard folder browser dialog.
                using (FolderBrowserDialog ofd = new FolderBrowserDialog()) {
                    if (_openFolderPath != null)
                        ofd.SelectedPath = _openFolderPath;

                    if (ofd.ShowDialog() == DialogResult.OK) {
                        _openFolderPath = ofd.SelectedPath;
                        OpenPaths(new string[] { ofd.SelectedPath });
                    }
                }
            }

            UpdateUI();
        }

        private void OpenPaths (string[] paths)
        {
            int failCount = _controller.OpenPaths(paths);

            foreach (string path in paths) {
                if (Directory.Exists(path))
                    AddPathToHistory(GetRecentDirectories(), path);
                else if (File.Exists(path))
                    AddPathToHistory(GetRecentFiles(), path);
            }

            UpdateUI();
            UpdateOpenMenu();

            if (failCount > 0) {
                MessageBox.Show("One or more selected files failed to open.");
            }
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

        private void ExpandNode (TreeNode node)
        {
            _controller.ExpandNode(node);
            UpdateUI(node.Tag as DataNode);
        }

        private void CollapseNode (TreeNode node)
        {
            _controller.CollapseNode(node);
            UpdateUI(node.Tag as DataNode);
        }

        private bool ConfirmExit ()
        {
            if (_controller.CheckModifications()) {
                if (MessageBox.Show("You currently have unsaved changes.  Close anyway?", "Unsaved Changes", MessageBoxButtons.OKCancel) != DialogResult.OK)
                    return false;
            }

            return true;
        }

        private bool ConfirmAction (string actionMessage)
        {
            if (_controller.CheckModifications()) {
                if (MessageBox.Show("You currently have unsaved changes.  " + actionMessage, "Unsaved Changes", MessageBoxButtons.OKCancel) != DialogResult.OK)
                    return false;
            }

            return true;
        }

        private void _controller_ConfirmAction (object sender, MessageBoxEventArgs e)
        {
            if (_controller.CheckModifications()) {
                if (MessageBox.Show("You currently have unsaved changes.  " + e.Message, "Unsaved Changes", MessageBoxButtons.OKCancel) != DialogResult.OK)
                    e.Cancel = true;
            }
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

                UpdateUI();
            }

            t.Join();
        }

        public void SearchDiscoveryCallback (DataNode node)
        {
            _controller.SelectNode(node);

            if (_searchForm != null) {
                _searchForm.DialogResult = DialogResult.OK;
                _searchForm = null;
            }
        }

        public void SearchCollapseCallback (DataNode node)
        {
            _controller.CollapseBelow(node);
        }

        public void SearchEndCallback (DataNode node)
        {
            if (_searchForm != null) {
                _searchForm.DialogResult = DialogResult.OK;
                _searchForm = null;
            }

            _searchState = null;
            UpdateUI();

            MessageBox.Show("End of results");
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
                    _buttonAddTagInt, _buttonAddTagIntArray, _buttonAddTagList, _buttonAddTagLong, _buttonAddTagLongArray, _buttonAddTagShort,
                    _buttonAddTagString, _buttonCopy, _buttonCut, _buttonDelete, _buttonEdit, _buttonPaste, _buttonRefresh,
                    _buttonRename);

                _buttonSave.Enabled = _controller.CheckModifications();
                _buttonFindNext.Enabled = false;

                DisableMenuItems(_menuItemCopy, _menuItemCut, _menuItemDelete, _menuItemEditValue, _menuItemPaste, _menuItemRefresh,
                    _menuItemRename, _menuItemMoveUp, _menuItemMoveDown);

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
            _buttonAddTagLongArray.Enabled = node.CanCreateTag(TagType.TAG_LONG_ARRAY);
            _buttonAddTagShort.Enabled = node.CanCreateTag(TagType.TAG_SHORT);
            _buttonAddTagString.Enabled = node.CanCreateTag(TagType.TAG_STRING);

            _buttonSave.Enabled = _controller.CheckModifications();
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
            _menuItemMoveUp.Enabled = node.CanMoveNodeUp;
            _menuItemMoveDown.Enabled = node.CanMoveNodeDown;
            _menuItemOpenInExplorer.Enabled = node is DirectoryDataNode;

            UpdateUI(_nodeTree.SelectedNodes);
        }

        private void UpdateUI (IList<TreeNode> nodes)
        {
            
            if (nodes == null)
                return;

            _buttonAddTagByte.Enabled = _controller.CanOperateOnSelection(Predicates.CreateByteNodePred);
            _buttonAddTagShort.Enabled = _controller.CanOperateOnSelection(Predicates.CreateShortNodePred);
            _buttonAddTagInt.Enabled = _controller.CanOperateOnSelection(Predicates.CreateIntNodePred);
            _buttonAddTagLong.Enabled = _controller.CanOperateOnSelection(Predicates.CreateLongNodePred);
            _buttonAddTagFloat.Enabled = _controller.CanOperateOnSelection(Predicates.CreateFloatNodePred);
            _buttonAddTagDouble.Enabled = _controller.CanOperateOnSelection(Predicates.CreateDoubleNodePred);
            _buttonAddTagByteArray.Enabled = _controller.CanOperateOnSelection(Predicates.CreateByteArrayNodePred);
            _buttonAddTagIntArray.Enabled = _controller.CanOperateOnSelection(Predicates.CreateIntArrayNodePred);
            _buttonAddTagLongArray.Enabled = _controller.CanOperateOnSelection(Predicates.CreateLongArrayNodePred);
            _buttonAddTagString.Enabled = _controller.CanOperateOnSelection(Predicates.CreateStringNodePred);
            _buttonAddTagList.Enabled = _controller.CanOperateOnSelection(Predicates.CreateListNodePred);
            _buttonAddTagCompound.Enabled = _controller.CanOperateOnSelection(Predicates.CreateCompoundNodePred);

            _buttonSave.Enabled = _controller.CheckModifications();
            _buttonRename.Enabled = _controller.CanOperateOnSelection(Predicates.RenameNodePred);
            _buttonEdit.Enabled = _controller.CanOperateOnSelection(Predicates.EditNodePred);
            _buttonDelete.Enabled = _controller.CanOperateOnSelection(Predicates.DeleteNodePred);
            _buttonCut.Enabled = _controller.CanOperateOnSelection(Predicates.CutNodePred) && NbtClipboardController.IsInitialized; ;
            _buttonCopy.Enabled = _controller.CanOperateOnSelection(Predicates.CopyNodePred) && NbtClipboardController.IsInitialized; ;
            _buttonPaste.Enabled = _controller.CanOperateOnSelection(Predicates.PasteIntoNodePred) && NbtClipboardController.IsInitialized; ;
            _buttonFindNext.Enabled = _controller.CanOperateOnSelection(Predicates.SearchNodePred) || _searchState != null;
            _buttonRefresh.Enabled = _controller.CanOperateOnSelection(Predicates.RefreshNodePred);

            _menuItemSave.Enabled = _buttonSave.Enabled;
            _menuItemRename.Enabled = _buttonRename.Enabled;
            _menuItemEditValue.Enabled = _buttonEdit.Enabled;
            _menuItemDelete.Enabled = _buttonDelete.Enabled;
            _menuItemMoveUp.Enabled = _controller.CanOperateOnSelection(Predicates.MoveNodeUpPred);
            _menuItemMoveDown.Enabled = _controller.CanOperateOnSelection(Predicates.MoveNodeDownPred);
            _menuItemCut.Enabled = _buttonCut.Enabled;
            _menuItemCopy.Enabled = _buttonCopy.Enabled;
            _menuItemPaste.Enabled = _buttonPaste.Enabled;
            _menuItemFind.Enabled = _controller.CanOperateOnSelection(Predicates.SearchNodePred);
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

        private void _controller_SelectionInvalidated (object sender, EventArgs e)
        {
            UpdateUI();
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
            _controller.EditNode(e.Node);
        }

        private void _nodeTree_NodeMouseClick (object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right) {
                e.Node.ContextMenuStrip = _controller.BuildNodeContextMenu(e.Node, e.Node.Tag as DataNode);
                _nodeTree.SelectedNode = e.Node;
            }
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
            _controller.Save();
        }

        private void _buttonEdit_Click (object sender, EventArgs e)
        {
            _controller.EditSelection();
        }

        private void _buttonRename_Click (object sender, EventArgs e)
        {
            _controller.RenameSelection();
        }

        private void _buttonDelete_Click (object sender, EventArgs e)
        {
            _controller.DeleteSelection();
        }

        private void _buttonCopy_Click (object sernder, EventArgs e)
        {
            _controller.CopySelection();
        }

        private void _buttonCut_Click (object sernder, EventArgs e)
        {
            _controller.CutSelection();
        }

        private void _buttonPaste_Click (object sernder, EventArgs e)
        {
            _controller.PasteIntoSelection();
        }

        private void _buttonAddTagByteArray_Click (object sender, EventArgs e)
        {
            _controller.CreateNode(TagType.TAG_BYTE_ARRAY);
        }

        private void _buttonAddTagByte_Click (object sender, EventArgs e)
        {
            _controller.CreateNode(TagType.TAG_BYTE);
        }

        private void _buttonAddTagCompound_Click (object sender, EventArgs e)
        {
            _controller.CreateNode(TagType.TAG_COMPOUND);
        }

        private void _buttonAddTagDouble_Click (object sender, EventArgs e)
        {
            _controller.CreateNode(TagType.TAG_DOUBLE);
        }

        private void _buttonAddTagFloat_Click (object sender, EventArgs e)
        {
            _controller.CreateNode(TagType.TAG_FLOAT);
        }

        private void _buttonAddTagInt_Click (object sender, EventArgs e)
        {
            _controller.CreateNode(TagType.TAG_INT);
        }

        private void _buttonAddTagIntArray_Click (object sender, EventArgs e)
        {
            _controller.CreateNode(TagType.TAG_INT_ARRAY);
        }

        private void _buttonAddTagList_Click (object sender, EventArgs e)
        {
            _controller.CreateNode(TagType.TAG_LIST);
        }

        private void _buttonAddTagLong_Click (object sender, EventArgs e)
        {
            _controller.CreateNode(TagType.TAG_LONG);
        }

        private void _buttonAddTagLongArray_Click (object sender, EventArgs e)
        {
            _controller.CreateNode(TagType.TAG_LONG_ARRAY);
        }

        private void _buttonAddTagShort_Click (object sender, EventArgs e)
        {
            _controller.CreateNode(TagType.TAG_SHORT);
        }

        private void _buttonAddTagString_Click (object sender, EventArgs e)
        {
            _controller.CreateNode(TagType.TAG_STRING);
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
            _controller.RefreshSelection();
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
            _controller.Save();
        }

        private void _menuItemExit_Click (object sender, EventArgs e)
        {
            Settings.Default.Save();
            Close();
        }

        private void _menuItemEditValue_Click (object sender, EventArgs e)
        {
            _controller.EditSelection();
        }

        private void _menuItemRename_Click (object sender, EventArgs e)
        {
            _controller.RenameSelection();
        }

        private void _menuItemDelete_Click (object sender, EventArgs e)
        {
            _controller.DeleteSelection();
        }

        private void _menuItemCopy_Click (object sender, EventArgs e)
        {
            _controller.CopySelection();
        }

        private void _menuItemCut_Click (object sender, EventArgs e)
        {
            _controller.CutSelection();
        }

        private void _menuItemPaste_Click (object sender, EventArgs e)
        {
            _controller.PasteIntoSelection();
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
            _controller.RefreshSelection();
        }

        private void replaceToolStripMenuItem_Click (object sender, EventArgs e)
        {
            Form form = new FindReplace(this, _controller, _nodeTree.SelectedNode.Tag as DataNode);
            form.Show();
        }

        private void _menuItemMoveUp_Click (object sender, EventArgs e)
        {
            _controller.MoveSelectionUp();
        }

        private void _menuItemMoveDown_Click (object sender, EventArgs e)
        {
            _controller.MoveSelectionDown();
        }

        private void findBlockToolStripMenuItem_Click (object sender, EventArgs e)
        {
            FindBlock form = new FindBlock(_nodeTree.SelectedNode.Tag as DataNode);
            if (form.ShowDialog() == DialogResult.OK) {
                if (form.Result != null) {
                    _controller.SelectNode(form.Result);
                    _controller.ExpandSelectedNode();
                    _controller.ScrollNode(form.Result);
                }
                else
                    MessageBox.Show("Chunk not found.");
            }
        }

        #endregion

        #endregion
    }
}
