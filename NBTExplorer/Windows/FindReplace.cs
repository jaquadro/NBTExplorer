using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using NBTExplorer.Controllers;
using Substrate.Nbt;
using NBTExplorer.Model;
using System.Threading;
using NBTExplorer.Model.Search;

namespace NBTExplorer.Windows
{
    public partial class FindReplace : Form
    {
        private MainForm _main;
        private NodeTreeController _mainController;
        private DataNode _mainSearchRoot;

        private RuleTreeController _findController;
        private NodeTreeController _replaceController;

        private ExplorerBarController _explorerManager;

        public FindReplace (MainForm main, NodeTreeController controller, DataNode searchRoot)
        {
            InitializeComponent();

            _main = main;
            _mainController = controller;
            _mainSearchRoot = searchRoot;

            _findController = new RuleTreeController(treeView1);
            treeView1.NodeMouseDoubleClick += (s, e) => {
                _findController.EditSelection();
            };

            //_findController.VirtualRootDisplay = "Find Rules";

            _replaceController = new NodeTreeController(treeView2);
            treeView2.NodeMouseDoubleClick += (s, e) => {
                _replaceController.EditSelection();
            };

            _replaceController.VirtualRootDisplay = "Replacement Tags";

            _explorerStrip.Renderer = new ToolStripExplorerRenderer();
            _explorerStrip.ImageList = _mainController.IconList;

            _explorerManager = new ExplorerBarController(_explorerStrip, _mainController.IconRegistry, _mainController.IconList, searchRoot);
            _explorerManager.SearchRootChanged += (s, e) => {
                _mainSearchRoot = _explorerManager.SearchRoot;
                Reset();
            };
        }

        #region Find Toolbar Buttons

        private void _tbFindDelete_Click (object sender, EventArgs e)
        {
            _findController.DeleteSelection();
        }

        private void _tbFindGroupAnd_Click (object sender, EventArgs e)
        {
            _findController.CreateIntersectNode();
        }

        private void _tbFindGroupOr_Click (object sender, EventArgs e)
        {
            _findController.CreateUnionNode();
        }

        private void _tbFindAny_Click (object sender, EventArgs e)
        {
            _findController.CreateWildcardNode();
        }

        private void _tbFindByte_Click (object sender, EventArgs e)
        {
            _findController.CreateNode(TagType.TAG_BYTE);
        }

        private void _tbFindShort_Click (object sender, EventArgs e)
        {
            _findController.CreateNode(TagType.TAG_SHORT);
        }

        private void _tbFindInt_Click (object sender, EventArgs e)
        {
            _findController.CreateNode(TagType.TAG_INT);
        }

        private void _tbFindLong_Click (object sender, EventArgs e)
        {
            _findController.CreateNode(TagType.TAG_LONG);
        }

        private void _tbFindFloat_Click (object sender, EventArgs e)
        {
            _findController.CreateNode(TagType.TAG_FLOAT);
        }

        private void _tbFindDouble_Click (object sender, EventArgs e)
        {
            _findController.CreateNode(TagType.TAG_DOUBLE);
        }

        private void _tbFindString_Click (object sender, EventArgs e)
        {
            _findController.CreateNode(TagType.TAG_STRING);
        }

        #endregion

        #region Replace Toolbar Icons

        private void _tbReplaceDelete_Click (object sender, EventArgs e)
        {
            _replaceController.DeleteSelection();
        }

        private void _tbReplaceByte_Click (object sender, EventArgs e)
        {
            _replaceController.CreateNode(TagType.TAG_BYTE);
        }

        private void _tbReplaceShort_Click (object sender, EventArgs e)
        {
            _replaceController.CreateNode(TagType.TAG_SHORT);
        }

        private void _tbReplaceInt_Click (object sender, EventArgs e)
        {
            _replaceController.CreateNode(TagType.TAG_INT);
        }

        private void _tbReplaceLong_Click (object sender, EventArgs e)
        {
            _replaceController.CreateNode(TagType.TAG_LONG);
        }

        private void _tbReplaceFloat_Click (object sender, EventArgs e)
        {
            _replaceController.CreateNode(TagType.TAG_FLOAT);
        }

        private void _tbReplaceDouble_Click (object sender, EventArgs e)
        {
            _replaceController.CreateNode(TagType.TAG_DOUBLE);
        }

        private void _tbReplaceByteArray_Click (object sender, EventArgs e)
        {
            _replaceController.CreateNode(TagType.TAG_BYTE_ARRAY);
        }

        private void _tbReplaceIntArray_Click (object sender, EventArgs e)
        {
            _replaceController.CreateNode(TagType.TAG_INT_ARRAY);
        }

        private void _tbReplaceLongArray_Click(object sender, EventArgs e)
        {
            _replaceController.CreateNode(TagType.TAG_LONG_ARRAY);
        }

        private void _tbReplaceString_Click (object sender, EventArgs e)
        {
            _replaceController.CreateNode(TagType.TAG_STRING);
        }

        private void _tbReplaceList_Click (object sender, EventArgs e)
        {
            _replaceController.CreateNode(TagType.TAG_LIST);
        }

        private void _tbReplaceCompound_Click (object sender, EventArgs e)
        {
            _replaceController.CreateNode(TagType.TAG_COMPOUND);
        }

        #endregion

        private void Reset ()
        {
            _searchForm = null;
            _searchState = null;
        }

        private CancelSearchForm _searchForm;
        private ContainerRuleSearchStateWin _searchState;

        private void _buttonFind_Click (object sender, EventArgs e)
        {
            if (_searchState == null) {
                _searchState = new ContainerRuleSearchStateWin(_main) {
                    RuleTags = _findController.Root,
                    RootNode = _mainSearchRoot,
                    DiscoverCallback = SearchDiscoveryCallback,
                    CollapseCallback = SearchCollapseCallback,
                    ProgressCallback = SearchProgressCallback,
                    EndCallback = SearchEndCallback,
                };
            }

            SearchNextNode();
        }

        private void _buttonReplaceAll_Click (object sender, EventArgs e)
        {
            _searchState = new ContainerRuleSearchStateWin(_main) {
                RuleTags = _findController.Root,
                RootNode = _mainSearchRoot,
                DiscoverCallback = SearchDiscoveryReplaceAllCallback,
                CollapseCallback = SearchCollapseCallback,
                ProgressCallback = SearchProgressCallback,
                EndCallback = SearchEndCallback,
                TerminateOnDiscover = false,
            };

            SearchNextNodeContinuous();
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
        }

        private void SearchNextNodeContinuous ()
        {
            if (_searchState == null)
                return;

            SearchWorker worker = new SearchWorker(_searchState);

            Thread t = new Thread(new ThreadStart(RunContinuousReplace));
            t.IsBackground = true;
            t.Start();

            _searchForm = new CancelSearchForm();
            if (_searchForm.ShowDialog(this) == DialogResult.Cancel) {
                worker.Cancel();
                _searchState = null;
            }
        }

        private void RunContinuousReplace ()
        {
            SearchWorker worker = new SearchWorker(_searchState);
            worker.Run();

            Invoke((Action)(() => {
                Reset();
            }));
        }

        private delegate void Action ();
        
        private void SearchDiscoveryCallback (DataNode node)
        {
            _mainController.SelectNode(node);
            _mainController.ExpandSelectedNode();

            if (_searchForm != null) {
                _searchForm.DialogResult = DialogResult.OK;
                _searchForm = null;
            }

            _currentFindNode = node;
        }

        private void SearchDiscoveryReplaceAllCallback (DataNode node)
        {
            _mainController.SelectNode(node);
            _mainController.ExpandSelectedNode();

            _currentFindNode = node;

            ReplaceCurrent();
        }

        private void SearchProgressCallback (DataNode node)
        {
            try {
                _searchForm.SearchPathLabel = node.NodePath;
            }
            catch { }
        }

        private void SearchCollapseCallback (DataNode node)
        {
            _mainController.CollapseBelow(node);
        }

        private void SearchEndCallback (DataNode node)
        {
            if (_searchForm != null) {
                _searchForm.DialogResult = DialogResult.OK;
                _searchForm = null;
            }

            MessageBox.Show("End of Results");
        }

        private DataNode _currentFindNode;

        private void _buttonReplace_Click (object sender, EventArgs e)
        {
            ReplaceCurrent();
        }

        private void ReplaceCurrent ()
        {
            TagCompoundDataNode node = _currentFindNode as TagCompoundDataNode;
            if (node == null)
                return;

            List<TagDataNode> matches = new List<TagDataNode>();
            _findController.Root.Matches(node, matches);

            List<string> replaceNames = new List<string>();
            foreach (DataNode rnode in _replaceController.Root.Nodes)
                replaceNames.Add(rnode.NodeName);

            foreach (var replNode in matches) {
                if (_deleteTagsCheckbox.Checked || replaceNames.Contains(replNode.NodeName))
                    replNode.DeleteNode();
            }

            foreach (TagDataNode tag in _replaceController.Root.Nodes) {
                if (tag == null)
                    continue;

                node.NamedTagContainer.AddTag(tag.Tag, tag.NodeName);
                node.SyncTag();
            }

            _mainController.RefreshTreeNode(node);
        }

        private void _buttonCancel_Click (object sender, EventArgs e)
        {
            Close();
        }

        private void _tbFindEdit_Click (object sender, EventArgs e)
        {
            _findController.EditSelection();
        }

        private void _tbReplaceEdit_Click (object sender, EventArgs e)
        {
            _replaceController.EditSelection();
        }
    }

    public abstract class ContainerRuleSearchState : ISearchState
    {
        public GroupRule RuleTags { get; set; }

        public DataNode RootNode { get; set; }
        public IEnumerator<DataNode> State { get; set; }
        public bool TerminateOnDiscover { get; set; }
        public bool IsTerminated { get; set; }
        public float ProgressRate { get; set; }

        public abstract void InvokeDiscoverCallback (DataNode node);
        public abstract void InvokeProgressCallback (DataNode node);
        public abstract void InvokeCollapseCallback (DataNode node);
        public abstract void InvokeEndCallback (DataNode node);

        protected ContainerRuleSearchState ()
        {
            TerminateOnDiscover = true;
            ProgressRate = .5f;
        }

        public bool TestNode (DataNode node)
        {
            if (RuleTags == null)
                return false;

            TagCompoundDataNode tagNode = node as TagCompoundDataNode;
            if (tagNode == null)
                return false;

            List<TagDataNode> matches = new List<TagDataNode>();
            if (!RuleTags.Matches(tagNode, matches))
                return false;

            return true;
        }
    }

    public class ContainerRuleSearchStateWin : ContainerRuleSearchState
    {
        private ContainerControl _sender;

        public ContainerRuleSearchStateWin (ContainerControl sender)
        {
            _sender = sender;
        }

        public Action<DataNode> DiscoverCallback { get; set; }
        public Action<DataNode> ProgressCallback { get; set; }
        public Action<DataNode> CollapseCallback { get; set; }
        public Action<DataNode> EndCallback { get; set; }

        public override void InvokeDiscoverCallback (DataNode node)
        {
            if (_sender != null && DiscoverCallback != null)
                _sender.Invoke(DiscoverCallback, new object[] { node });
        }

        public override void InvokeProgressCallback (DataNode node)
        {
            if (_sender != null && ProgressCallback != null)
                _sender.Invoke(ProgressCallback, new object[] { node });
        }

        public override void InvokeCollapseCallback (DataNode node)
        {
            if (_sender != null && CollapseCallback != null)
                _sender.Invoke(CollapseCallback, new object[] { node });
        }

        public override void InvokeEndCallback (DataNode node)
        {
            if (_sender != null && EndCallback != null)
                _sender.Invoke(EndCallback, new object[] { node });
        }
    }
}
