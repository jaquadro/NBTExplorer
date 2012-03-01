using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Substrate.Core;
using Substrate.Nbt;
using NBTExplorer.ViewModel;

namespace NBTExplorer
{
    public partial class Form1 : Form
    {
        readonly static Dictionary<TagType, int> _tagIconIndex;

        public Form1()
        {
            InitializeComponent();
            findBloatedChunkToolStripMenuItem.ToolTipText = "Chunks with " + BloatedAt.ToString() + " or more entities";
            _nodeTree.BeforeExpand += NodeExpand;
            _nodeTree.AfterCollapse += NodeCollapse;
            _nodeTree.AfterSelect += NodeSelected;
            _nodeTree.NodeMouseClick += NodeClicked;
            _nodeTree.NodeMouseDoubleClick += NodeDoubleClicked;

            var args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                OpenFile(args[1]);
            }
            else
            {
                OpenMinecraftDir();
            }
        }


        void UpdateToolbar()
        {
            TreeNode node = _nodeTree.SelectedNode;
            TagNode tag = node.Tag as TagNode;

            if (tag == null && node.Tag is NbtDataNode)
            {
                NbtDataNode data = node.Tag as NbtDataNode;
                if (data.Tree != null)
                    tag = data.Tree.Root;
            }

            _buttonRename.Enabled = tag != null && node.Tag is TagNode;
            _buttonDelete.Enabled = tag != null && node.Tag is TagNode;
            _buttonEdit.Enabled = tag != null
                && node.Tag is TagNode
                && tag.GetTagType() != TagType.TAG_BYTE_ARRAY
                && tag.GetTagType() != TagType.TAG_COMPOUND
                && tag.GetTagType() != TagType.TAG_LIST;

            if (tag == null || tag.GetTagType() != TagType.TAG_COMPOUND)
                SetTagButtons(false);
            if (tag != null && tag.GetTagType() == TagType.TAG_COMPOUND)
                SetTagButtons(true);
            if (tag != null && tag.GetTagType() == TagType.TAG_LIST && tag.ToTagList().Count == 0)
                SetTagButtons(true);

            if (tag != null && tag.GetTagType() == TagType.TAG_LIST)
            {
                switch (tag.ToTagList().ValueType)
                {
                    case TagType.TAG_BYTE:
                        _buttonAddTagByte.Enabled = true;
                        break;
                    case TagType.TAG_SHORT:
                        _buttonAddTagShort.Enabled = true;
                        break;
                    case TagType.TAG_INT:
                        _buttonAddTagInt.Enabled = true;
                        break;
                    case TagType.TAG_LONG:
                        _buttonAddTagLong.Enabled = true;
                        break;
                    case TagType.TAG_FLOAT:
                        _buttonAddTagFloat.Enabled = true;
                        break;
                    case TagType.TAG_DOUBLE:
                        _buttonAddTagDouble.Enabled = true;
                        break;
                    case TagType.TAG_BYTE_ARRAY:
                        _buttonAddTagByteArray.Enabled = true;
                        break;
                    case TagType.TAG_STRING:
                        _buttonAddTagString.Enabled = true;
                        break;
                    case TagType.TAG_LIST:
                        _buttonAddTagList.Enabled = true;
                        break;
                    case TagType.TAG_COMPOUND:
                        _buttonAddTagCompound.Enabled = true;
                        break;
                }
            }
        }

        void SetTagButtons(bool state)
        {
            var toEnable = new[]
                               {
                                   _buttonAddTagByte, _buttonAddTagShort, _buttonAddTagInt,
                                   _buttonAddTagLong,_buttonAddTagFloat, _buttonAddTagDouble,
                                   _buttonAddTagByteArray, _buttonAddTagString, _buttonAddTagList,
                                   _buttonAddTagCompound
                               };
            foreach (var button in toEnable)
                button.Enabled = state;

        }

        void OpenDirectory(string path)
        {
            _nodeTree.Nodes.Clear();
            findEntityChunkToolStripMenuItem.DropDownItems.Clear();
            LoadDirectory(path);

            if (_nodeTree.Nodes.Count > 0)
            {
                _nodeTree.Nodes[0].Expand();
            }
        }

        void OpenPaths(IEnumerable<string> paths)
        {
            _nodeTree.Nodes.Clear();
            findEntityChunkToolStripMenuItem.DropDownItems.Clear();
            foreach (string path in paths)
            {
                if (Directory.Exists(path))
                {
                    LoadDirectory(path);
                }
                else if (File.Exists(path))
                {
                    ServerNode.TryLoadFile(_nodeTree.Nodes, path);
                }
            }

            if (_nodeTree.Nodes.Count > 0)
            {
                _nodeTree.Nodes[0].Expand();
            }
        }

        void OpenMinecraftDir()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            path = Path.Combine(path, ".minecraft");
            path = Path.Combine(path, "saves");

            if (!Directory.Exists(path))
            {
                path = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);
            }

            OpenDirectory(path);
        }

        void LoadDirectory(string path)
        {
            LoadDirectory(path, _nodeTree.Nodes);
        }

        void LoadDirectory(string path, TreeNodeCollection parent)
        {
            ServerNode.LoadDirectory(path, parent, path);
        }

       

        

        void OpenFile()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.RestoreDirectory = true;
            ofd.Multiselect = true;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                OpenPaths(ofd.FileNames);
                findEntityChunkToolStripMenuItem.DropDownItems.Clear();
            }
        }

        void OpenFile(string file)
        {
            _nodeTree.Nodes.Clear();
            findEntityChunkToolStripMenuItem.DropDownItems.Clear();
            ServerNode.TryLoadFile(_nodeTree.Nodes, file);
        }

        string _openFolderPath = null;
        void OpenFolder()
        {
            FolderBrowserDialog ofd = new FolderBrowserDialog();
            if (_openFolderPath != null)
                ofd.SelectedPath = _openFolderPath;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                _openFolderPath = ofd.SelectedPath;
                OpenDirectory(ofd.SelectedPath);

            }
        }

        void SaveAll()
        {
            foreach (TreeNode node in _nodeTree.Nodes)
            {
                SaveNode(node);
            }
        }

        static void SaveNode(TreeNode node)
        {
            foreach (TreeNode sub in node.Nodes)
            {
                if (sub.Tag != null && sub.Tag is DataNode)
                {
                    SaveNode(sub);
                }
            }

            if (node.Tag is NbtFileData)
            {
                SaveNbtFileNode(node);
            }
            else if (node.Tag is RegionChunkData)
            {
                SaveRegionChunkNode(node);
            }
        }

        static void SaveNbtFileNode(TreeNode node)
        {
            NbtFileData data = node.Tag as NbtFileData;
            if (data == null || !data.Modified)
                return;

            NBTFile file = new NBTFile(data.Path);
            using (Stream str = file.GetDataOutputStream(data.CompressionType))
            {
                data.Tree.WriteTo(str);
            }
            data.Modified = false;
        }

        static void SaveRegionChunkNode(TreeNode node)
        {
            RegionChunkData data = node.Tag as RegionChunkData;
            if (data == null || !data.Modified)
                return;

            using (Stream str = data.Region.GetChunkDataOutputStream(data.X, data.Z))
            {
                data.Tree.WriteTo(str);
            }
            data.Modified = false;
        }
        
        #region Events

        static void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new About().Show();
        }

        void NodeExpand(object sender, TreeViewCancelEventArgs e)
        {
            ServerNode.ExpandNode(e.Node, this.imageList1.Images.Count);
        }

        static void NodeCollapse(object sender, TreeViewEventArgs e)
        {
            ServerNode.CollapseNode(e.Node);
        }

        void NodeSelected(object sender, TreeViewEventArgs e)
        {
            UpdateToolbar();
        }

        static void NodeClicked(object sender, TreeNodeMouseClickEventArgs e)
        {

        }

        void NodeDoubleClicked(object sender, TreeNodeMouseClickEventArgs e)
        {
            ServerNode.EditNodeValue(_nodeTree.SelectedNode);
        }

        void _buttonEdit_Click(object sender, EventArgs e)
        {
            ServerNode.EditNodeValue(_nodeTree.SelectedNode);
        }

        void _buttonSave_Click(object sender, EventArgs e)
        {
            SaveAll();
        }

        void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFile();
        }

        private void openFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFolder();
        }

        void toolStripButton1_Click(object sender, EventArgs e)
        {
            OpenFile();
        }

        void _buttonDelete_Click(object sender, EventArgs e)
        {
            ServerNode.DeleteNode(_nodeTree.SelectedNode);
        }

        void _buttonRename_Click(object sender, EventArgs e)
        {
            ServerNode.EditNodeName(_nodeTree.SelectedNode);
        }

        void _buttonOpenFolder_Click(object sender, EventArgs e)
        {
            OpenFolder();
        }

        void findToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Find(_nodeTree.SelectedNode);
        }

        void findNextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FindNext();
        }

        void _buttonFindNext_Click(object sender, EventArgs e)
        {
            if (_search == null)
                Find(_nodeTree.SelectedNode);
            else
                FindNext();
        }
        void AddTagToNode(TagType type)
        {
            ServerNode.AddTagToNode(_nodeTree.SelectedNode, this.imageList1.Images.Count, type);
        }
        void _buttonAddTagByte_Click(object sender, EventArgs e)
        {
           AddTagToNode(TagType.TAG_BYTE);
        }

        void _buttonAddTagShort_Click(object sender, EventArgs e)
        {
            AddTagToNode(TagType.TAG_SHORT);
        }

        void _buttonAddTagInt_Click(object sender, EventArgs e)
        {
            AddTagToNode(TagType.TAG_INT);
        }

        private void _buttonAddTagLong_Click(object sender, EventArgs e)
        {
            AddTagToNode(TagType.TAG_LONG);
        }

        void _buttonAddTagFloat_Click(object sender, EventArgs e)
        {
            AddTagToNode(TagType.TAG_FLOAT);
        }

        void _buttonAddTagDouble_Click(object sender, EventArgs e)
        {
            AddTagToNode(TagType.TAG_DOUBLE);
        }

        void _buttonAddTagByteArray_Click(object sender, EventArgs e)
        {
            AddTagToNode(TagType.TAG_BYTE_ARRAY);
        }

        void _buttonAddTagString_Click(object sender, EventArgs e)
        {
            AddTagToNode(TagType.TAG_STRING);
        }

        void _buttonAddTagList_Click(object sender, EventArgs e)
        {
            AddTagToNode(TagType.TAG_LIST);
        }

        void _buttonAddTagCompound_Click(object sender, EventArgs e)
        {
            AddTagToNode(TagType.TAG_COMPOUND);
        }

        void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveAll();
        }

        static void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        void _nodeTree_DragDrop(object sender, DragEventArgs e)
        {
            DropFile(e);
        }

        static void _nodeTree_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        void openMinecraftSaveFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenMinecraftDir();
        }

        #endregion

        TreeNode _rootSearchNode;
        string _searchName;
        string _searchValue;
        IEnumerator<TreeNode> _search;

        void Find(TreeNode node)
        {
            if (node == null)
                return;

            Find form = new Find();
            if (form.ShowDialog() == DialogResult.OK)
            {
                _rootSearchNode = node;

                _searchName = form.MatchName ? form.NameToken : null;
                _searchValue = form.MatchValue ? form.ValueToken : null;

                _search = ServerNode.FindNode(node,this.imageList1.Images.Count,_searchName, _searchValue).GetEnumerator();

                FindNext();
            }
        }

        void FindNext()
        {
            if (_search == null)
                return;

            statusStrip1.Visible = true;
            statusStrip1.Text = "Please wait...";
            Application.DoEvents();

            _nodeTree.SelectedNode = null;

            if (!_search.MoveNext())
            {
                _nodeTree.SelectedNode = _rootSearchNode;
                if (_rootSearchNode != null && _rootSearchNode.Tag is DataNode)
                {
                    if (_rootSearchNode.IsExpanded && !(_rootSearchNode.Tag as DataNode).Expanded)
                    {
                        _rootSearchNode.Collapse();
                    }
                }

                statusStrip1.Visible = false;

                MessageBox.Show("End of results");
                _search = null;
            }

            statusStrip1.Visible = false;
        }

       

        void DropFile(DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            OpenPaths(files);
        }


        static IEnumerable<ViewModel.ChunkNode> FindChunks(IEnumerable<ViewModel.RegionNode> nodes, Func<ViewModel.ChunkNode, bool> predicate)
        {
            foreach (var region in nodes)
                foreach (var chunk in region.FindChunks(predicate))
                {
                        yield return chunk;
                }
        }

        int _nodesSearchedForEntity = 0;

       

        IEnumerable<TreeNode> FindRegions(IEnumerable<TreeNode> nodes, Func<TreeNode, bool> predicate)
        {

            foreach (var node in nodes)
            {
                _nodesSearchedForEntity++;
                if (ServerNode.NeedsExpand(node))
                    ServerNode.ExpandNode(node,this.imageList1.Images.Count);
                if (predicate(node))
                    yield return node;
                else
                    foreach (var region in FindRegions(node.Nodes.Cast<TreeNode>(), predicate))
                    {
                        yield return region;
                    }
            }

        }

        /// <summary>
        /// Find next chunk that has entities
        /// </summary>
        /// <returns></returns>
        IEnumerable<ChunkNode> FindNextPopulatedChunk(IEnumerable<ToolStripItem> previouslyFound)
        {
         
            var lastFound =
               previouslyFound.LastOrDefault(
                    t => t.Text.StartsWith("Chunk"));
            if (lastFound != null)
            {

                lastFound.PerformClick();
                Application.DoEvents();
                return FindNextPopulatedFrom(_nodeTree.SelectedNode);
            }
            else return FindNextPopulatedFrom(_nodeTree.Nodes[0]);

        }

        /// <summary>
        /// Find the next chunk that has entities starting from start
        /// </summary>
        /// <param name="start"></param>
        /// <returns></returns>
        IEnumerable<ChunkNode> FindNextPopulatedFrom(TreeNode start)
        {
            if (this._nodeTree.Nodes.Count == 0)
                return Enumerable.Empty<ChunkNode>();

            var entityNodes =
                FindRegions(
                    _nodeTree.Nodes.Cast<TreeNode>(), tn => tn.Text.EndsWith(".mcr")).Select(tn=>new ViewModel.RegionNode(tn,this.imageList1.Images.Count));

            var entityRegions = entityNodes.ToArray();
            try
            {
                if (entityRegions == null || entityRegions.Any() == false)
                {
                    return Enumerable.Empty<ChunkNode>();
                }
                var chunks = FindChunks(entityRegions, tn => tn.Index > start.Index);
                if (chunks == null || chunks.Any() == false)
                    return Enumerable.Empty<ChunkNode>();
                var populatedChunks = FindChunksWithEntities(chunks);
                if (populatedChunks == null || populatedChunks.Any() == false)
                    return Enumerable.Empty<ChunkNode>();
                return populatedChunks;
            }
            finally
            {
                toolStrip1.Text = "Searched " + _nodesSearchedForEntity + " nodes";
                toolStrip1.Visible = true;
            }
        }
        void findEntityChunkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _nodesSearchedForEntity = 0;
            var populatedChunks = FindNextPopulatedChunk(findEntityChunkToolStripMenuItem.DropDownItems.Cast<ToolStripItem>());
            var first = populatedChunks.FirstOrDefault();
            if (first == null)
                return;
            first.EnsureVisible();
            _nodeTree.SelectedNode = first.Node;
            var ddl = new ToolStripButton();
            ddl.Text = first.Text;
            ddl.Width = Text.Length * 5;
            ddl.ToolTipText = first.Parent.Text;

            ddl.Click += (sender2, e2) =>
            {
                first.EnsureVisible();
                _nodeTree.SelectedNode = first.Node;
            };
            findEntityChunkToolStripMenuItem.DropDownItems.Add(ddl);


        }

        IEnumerable<ViewModel.ChunkNode> FindChunksWithEntities(IEnumerable<ViewModel.ChunkNode> chunks)
        {
            foreach (var chunk in chunks)
            {
               
                if (chunk.GetEntityCount()>0)
                    yield return chunk;
            }
        }

        int _nodesSearchedForBloat = 0;
        public const int BloatedAt = 300;
        private void findBloatedChunkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _nodesSearchedForBloat = 0; // findEntityChunkToolStripMenuItem.DropDownItems.Cast<ToolStripItem>()
            var populatedChunks = FindNextPopulatedChunk(findBloatedChunkToolStripMenuItem.DropDownItems.Cast<ToolStripItem>());
            var first = populatedChunks.FirstOrDefault(c => c.GetEntityCount() >= BloatedAt);
            
            if (first == null)
                return;
            first.EnsureVisible();
            first.EnsureEntitiesVisible();
            first.SelectEntityNode();
            
            var ddl = new ToolStripButton();
            ddl.Text = first.Text;
            ddl.Width = Text.Length * 5;
            ddl.ToolTipText = first.Parent.Text;

            ddl.Click += (sender2, e2) =>
            {
                
                first.EnsureVisible();
                _nodeTree.SelectedNode = first.Node;
            };
            findBloatedChunkToolStripMenuItem.DropDownItems.Add(ddl);
        }
    }
}
