using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Substrate;
using Substrate.Nbt;
using System.IO;
using Substrate.Core;

namespace NBTPlus
{
    public partial class Form1 : Form
    {
        private static Dictionary<TagType, int> _tagIconIndex;

        public Form1 ()
        {
            InitializeComponent();

            _nodeTree.BeforeExpand += NodeExpand;
            _nodeTree.AfterCollapse += NodeCollapse;
            _nodeTree.AfterSelect += NodeSelected;

            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            path = Path.Combine(path, ".minecraft");
            path = Path.Combine(path, "saves");

            if (!Directory.Exists(path)) {
                path = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);
            }

            LoadDirectory(path);
            _nodeTree.Nodes[0].Expand();
        }

        public void LoadNbtStream (TreeNodeCollection parent, Stream stream)
        {
            LoadNbtStream(parent, stream, null);
        }

        public void LoadNbtStream (TreeNodeCollection parent, Stream stream, string name)
        {
            string text = !String.IsNullOrEmpty(name) ? name : null;
            text = text ?? "[root]";

            TreeNode root = new TreeNode(text, 9, 9);

            LoadNbtStream(root, stream);

            parent.Add(root);
        }

        public void LoadNbtStream (TreeNode node, Stream stream)
        {
            NbtTree tree = new NbtTree();
            tree.ReadFrom(stream);

            if (node.Tag != null && node.Tag is NbtDataNode) {
                (node.Tag as NbtDataNode).Tree = tree;
            }

            PopulateNodeFromTag(node, tree.Root);
        }

        private TreeNode NodeFromTag (TagNode tag)
        {
            return NodeFromTag(tag, null);
        }

        private TreeNode NodeFromTag (TagNode tag, string name)
        {
            string text = String.IsNullOrEmpty(name) ? "" : name + ": ";

            TreeNode node = new TreeNode();
            node.ImageIndex = _tagIconIndex[tag.GetTagType()];
            node.SelectedImageIndex = _tagIconIndex[tag.GetTagType()];
            node.Tag = tag;

            switch (tag.GetTagType()) {
                case TagType.TAG_BYTE:
                case TagType.TAG_SHORT:
                case TagType.TAG_INT:
                case TagType.TAG_LONG:
                case TagType.TAG_FLOAT:
                case TagType.TAG_DOUBLE:
                case TagType.TAG_STRING:
                    node.Text = text + tag.ToString();
                    break;

                case TagType.TAG_BYTE_ARRAY:
                    node.Text = text + tag.ToTagByteArray().Length + " bytes";
                    break;

                case TagType.TAG_LIST:
                    node.Text = text + tag.ToTagList().Count + " entries";
                    PopulateNodeFromTag(node, tag.ToTagList());
                    break;

                case TagType.TAG_COMPOUND:
                    node.Text = text + tag.ToTagCompound().Count + " entries";
                    PopulateNodeFromTag(node, tag.ToTagCompound());
                    break;
            }

            return node;
        }

        private void PopulateNodeFromTag (TreeNode node, TagNodeList list)
        {
            foreach (TagNode tag in list) {
                node.Nodes.Add(NodeFromTag(tag));
            }
        }

        private void PopulateNodeFromTag (TreeNode node, TagNodeCompound dict)
        {
            var list = new SortedList<TagKey, TagNode>();
            foreach (KeyValuePair<string, TagNode> kv in dict) {
                list.Add(new TagKey(kv.Key, kv.Value.GetTagType()), kv.Value);
            }

            foreach (KeyValuePair<TagKey, TagNode> kv in list) {
                node.Nodes.Add(NodeFromTag(kv.Value, kv.Key.Name));
            }
        }

        public void LoadWorld (string path)
        {
            NbtWorld world = NbtWorld.Open(path);

            BetaWorld beta = world as BetaWorld;
            if (beta != null) {
                LoadBetaWorld(beta);
            }
        }

        private void LoadBetaWorld (BetaWorld world)
        {
            RegionManager rm = world.GetRegionManager();
        }

        public void LoadRegion (TreeNodeCollection parent, string path)
        {
            TreeNode root = new TreeNode(Path.GetFileName(path), 11, 11);
            LoadRegion(root, path);
            parent.Add(root);
        }

        public void LoadRegion (TreeNode node, string path)
        {
            RegionFile rf = new RegionFile(path);

            for (int x = 0; x < 32; x++) {
                for (int y = 0; y < 32; y++) {
                    if (rf.HasChunk(x, y)) {
                        node.Nodes.Add(CreateLazyChunk(rf, x, y));
                    }
                }
            }
        }

        public TreeNode CreateLazyChunk (RegionFile rf, int x, int z)
        {
            TreeNode node = new TreeNode();
            node.ImageIndex = 9;
            node.SelectedImageIndex = 9;
            node.Text = "Chunk [" + x + ", " + z + "]";
            node.Tag = new RegionChunkData(rf, x, z);
            node.Nodes.Add(new TreeNode());

            return node;
        }

        public TreeNode CreateLazyRegion (string path)
        {
            TreeNode node = new TreeNode();
            node.ImageIndex = 11;
            node.SelectedImageIndex = 11;
            node.Text = Path.GetFileName(path);
            node.Tag = new RegionData(path);
            node.Nodes.Add(new TreeNode());

            return node;
        }

        public TreeNode CreateLazyNbt (string path, CompressionType cztype)
        {
            TreeNode node = new TreeNode();
            node.ImageIndex = 12;
            node.SelectedImageIndex = 12;
            node.Text = Path.GetFileName(path);
            node.Tag = new NbtFileData(path, cztype);
            node.Nodes.Add(new TreeNode());

            return node;
        }

        public TreeNode CreateLazyDirectory (string path)
        {
            TreeNode node = new TreeNode();
            node.ImageIndex = 10;
            node.SelectedImageIndex = 10;
            node.Text = Path.GetFileName(path);
            node.Tag = new DirectoryData(path);
            node.Nodes.Add(new TreeNode());

            return node;
        }

        public TreeNode CreateLazyDirectory (string path, TreeNode parent)
        {
            TreeNode node = CreateLazyDirectory(path);
            LinkDataNodeParent(node, parent);

            return node;
        }

        public TreeNode CreateLazyNbt (string path, CompressionType cztype, TreeNode parent)
        {
            TreeNode node = CreateLazyNbt(path, cztype);
            LinkDataNodeParent(node, parent);

            return node;
        }

        private void LinkDataNodeParent (TreeNode node, TreeNode parent)
        {
            if (node != null && parent != null && node.Tag != null && parent.Tag != null) {
                DataNode nodeDn = node.Tag as DataNode;
                DataNode parentDn = parent.Tag as DataNode;

                if (nodeDn != null && parentDn != null) {
                    nodeDn.Parent = parentDn;
                }
            }
        }

        public void LoadLazyChunk (TreeNode node)
        {
            RegionChunkData data = node.Tag as RegionChunkData;
            if (data == null || data.Modified)
                return;

            node.Nodes.Clear();
            LoadNbtStream(node, data.Region.GetChunkDataInputStream(data.X, data.Z));
        }

        public void LoadLazyNbt (TreeNode node)
        {
            NbtFileData data = node.Tag as NbtFileData;
            if (data == null || data.Modified)
                return;

            node.Nodes.Clear();

            NBTFile file = new NBTFile(data.Path);
            LoadNbtStream(node, file.GetDataInputStream(data.CompressionType));
        }

        public void LoadLazyDirectory (TreeNode node)
        {
            DirectoryData data = node.Tag as DirectoryData;
            if (data == null || data.Modified)
                return;

            node.Nodes.Clear();

            foreach (string dirpath in Directory.EnumerateDirectories(data.Path)) {
                node.Nodes.Add(CreateLazyDirectory(dirpath, node));
            }

            foreach (string filepath in Directory.EnumerateFiles(data.Path)) {
                TryLoadFile(node.Nodes, filepath);
            }
        }

        public void LoadLazyRegion (TreeNode node)
        {
            RegionData data = node.Tag as RegionData;
            if (data == null || data.Modified)
                return;

            node.Nodes.Clear();
            LoadRegion(node, data.Path);
        }

        public void UnloadLazyDataNode (TreeNode node)
        {
            DataNode data = node.Tag as DataNode;
            if (data == null || data.Modified)
                return;

            node.Nodes.Clear();
            node.Nodes.Add(new TreeNode());
        }

        private void NodeExpand (object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node.Tag == null)
                return;

            if (e.Node.Tag is RegionChunkData) {
                LoadLazyChunk(e.Node);
            }
            else if (e.Node.Tag is NbtFileData) {
                LoadLazyNbt(e.Node);
            }
            else if (e.Node.Tag is DirectoryData) {
                LoadLazyDirectory(e.Node);
            }
            else if (e.Node.Tag is RegionData) {
                LoadLazyRegion(e.Node);
            }
        }

        private void NodeCollapse (object sender, TreeViewEventArgs e)
        {
            if (e.Node.Tag == null)
                return;

            if (e.Node.Tag is DataNode) {
                UnloadLazyDataNode(e.Node);
            }
        }

        private void NodeSelected (object sender, TreeViewEventArgs e)
        {
            UpdateToolbar();
        }

        private void UpdateToolbar ()
        {
            TreeNode node = _nodeTree.SelectedNode;
            TagNode tag = node.Tag as TagNode;

            _buttonRename.Enabled = tag != null;
            _buttonDelete.Enabled = tag != null;
            _buttonEdit.Enabled = tag != null
                && tag.GetTagType() != TagType.TAG_COMPOUND
                && tag.GetTagType() != TagType.TAG_LIST;

            if (tag == null || tag.GetTagType() != TagType.TAG_COMPOUND)
                SetTagButtons(false);
            if (tag != null && tag.GetTagType() == TagType.TAG_COMPOUND)
                SetTagButtons(true);
            if (tag != null && tag.GetTagType() == TagType.TAG_LIST && tag.ToTagList().Count == 0)
                SetTagButtons(true);

            if (tag != null && tag.GetTagType() == TagType.TAG_LIST) {
                switch (tag.ToTagList().ValueType) {
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

        private void SetTagButtons (bool state)
        {
            _buttonAddTagByte.Enabled = state;
            _buttonAddTagShort.Enabled = state;
            _buttonAddTagInt.Enabled = state;
            _buttonAddTagLong.Enabled = state;
            _buttonAddTagFloat.Enabled = state;
            _buttonAddTagDouble.Enabled = state;
            _buttonAddTagByteArray.Enabled = state;
            _buttonAddTagString.Enabled = state;
            _buttonAddTagList.Enabled = state;
            _buttonAddTagCompound.Enabled = state;
        }

        public void LoadDirectory (string path)
        {
            LoadDirectory(path, _nodeTree.Nodes);
        }

        public void LoadDirectory (string path, TreeNodeCollection parent)
        {
            LoadDirectory(path, parent, path);
        }

        public void LoadDirectory (string path, TreeNodeCollection parent, string name)
        {
            TreeNode root = new TreeNode(name, 10, 10);

            foreach (string dirpath in Directory.EnumerateDirectories(path)) {
                root.Nodes.Add(CreateLazyDirectory(dirpath, root));
            }

            foreach (string filepath in Directory.EnumerateFiles(path)) {
                TryLoadFile(root.Nodes, filepath);
            }

            parent.Add(root);
        }

        public void TryLoadFile (TreeNodeCollection parent, string path)
        {
            if (Path.GetExtension(path) == ".mcr") {
                parent.Add(CreateLazyRegion(path));
                return;
            }

            if (Path.GetExtension(path) == ".dat") {
                try {
                    NBTFile file = new NBTFile(path);
                    NbtTree tree = new NbtTree();
                    tree.ReadFrom(file.GetDataInputStream());
                    TreeNode node = CreateLazyNbt(path, CompressionType.GZip);
                    parent.Add(node);
                    LinkDataNodeParent(node, node.Parent);
                    return;
                }
                catch { }

                try {
                    NBTFile file = new NBTFile(path);
                    NbtTree tree = new NbtTree();
                    tree.ReadFrom(file.GetDataInputStream(CompressionType.None));
                    TreeNode node = CreateLazyNbt(path, CompressionType.None);
                    parent.Add(node);
                    LinkDataNodeParent(node, node.Parent);
                    return;
                }
                catch { }
            }
        }

        static Form1 ()
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
        }

        private void OpenFile ()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.RestoreDirectory = true;
            ofd.Multiselect = false;

            if (ofd.ShowDialog() == DialogResult.OK) {
                _nodeTree.Nodes.Clear();
                TryLoadFile(_nodeTree.Nodes, ofd.FileName);
            }
        }

        private void SaveAll ()
        {
            if (_nodeTree.Nodes.Count > 0) {
                SaveNode(_nodeTree.Nodes[0]);
            }
        }

        private void SaveNode (TreeNode node)
        {
            foreach (TreeNode sub in node.Nodes) {
                if (sub.Tag != null && sub.Tag is DataNode) {
                    SaveNode(sub);
                }
            }

            if (node.Tag is NbtFileData) {
                SaveNbtFileNode(node);
            }
        }

        private void SaveNbtFileNode (TreeNode node)
        {
            NbtFileData data = node.Tag as NbtFileData;
            if (data == null || !data.Modified)
                return;

            NBTFile file = new NBTFile(data.Path);
            using (Stream str = file.GetDataOutputStream(data.CompressionType)) {
                data.Tree.WriteTo(str);
            }
            data.Modified = false;
        }

        private void aboutToolStripMenuItem_Click (object sender, EventArgs e)
        {
            new About().Show();
        }

        private void openToolStripMenuItem_Click (object sender, EventArgs e)
        {
            OpenFile();
        }

        private void openFolderToolStripMenuItem_Click (object sender, EventArgs e)
        {
            FolderBrowserDialog ofd = new FolderBrowserDialog();
            if (ofd.ShowDialog() == DialogResult.OK) {
                _nodeTree.Nodes.Clear();
                LoadDirectory(ofd.SelectedPath);
            }
        }

        private void toolStripButton1_Click (object sender, EventArgs e)
        {
            OpenFile();
        }

        private void toolStripButton5_Click (object sender, EventArgs e)
        {
            if (_nodeTree.SelectedNode == null)
                return;

            TreeNode baseNode = _nodeTree.SelectedNode;
            while (baseNode.Tag == null || !(baseNode.Tag is DataNode)) {
                baseNode = baseNode.Parent;
            }

            // Can only delete internal NBT nodes
            if (baseNode == null || baseNode == _nodeTree.SelectedNode)
                return;

            (baseNode.Tag as DataNode).Modified = true;

            DeleteNodeNbtTag(_nodeTree.SelectedNode);

            _nodeTree.SelectedNode.Remove();
        }

        private void DeleteNodeNbtTag (TreeNode node)
        {
            TagNode tag = node.Tag as TagNode;
            if (tag == null)
                return;

            TagNode parentTag = node.Parent.Tag as TagNode;
            if (parentTag == null) {
                NbtDataNode parentData = node.Parent.Tag as NbtDataNode;
                if (parentData == null)
                    return;

                parentTag = parentData.Tree.Root;
                if (parentTag == null)
                    return;
            }

            switch (parentTag.GetTagType()) {
                case TagType.TAG_LIST:
                    parentTag.ToTagList().Remove(tag);
                    break;

                case TagType.TAG_COMPOUND:
                    DeleteTagFromCompound(parentTag.ToTagCompound(), tag);
                    break;
            }
        }

        private void DeleteTagFromCompound (TagNodeCompound parent, TagNode target)
        {
            string match = "";
            foreach (KeyValuePair<string, TagNode> kv in parent) {
                if (kv.Value == target) {
                    match = kv.Key;
                    break;
                }
            }

            if (match != null) {
                parent.Remove(match);
            }
        }

        private void _buttonSave_Click (object sender, EventArgs e)
        {
            SaveAll();
        }
    }

    public class TagKey : IComparable<TagKey>
    {
        public TagKey (string name, TagType type)
        {
            Name = name;
            TagType = type;
        }

        public string Name { get; set; }
        public TagType TagType { get; set; }
    
        #region IComparer<TagKey> Members

        public int Compare(TagKey x, TagKey y)
        {
            int typeDiff = (int)x.TagType - (int)y.TagType;
            if (typeDiff != 0)
                return typeDiff;

            return String.Compare(x.Name, y.Name, true);
        }

        #endregion

        #region IComparable<TagKey> Members

        public int CompareTo (TagKey other)
        {
            return Compare(this, other);
        }

        #endregion
    }

    public class DataNode 
    {
        public DataNode ()
        {
        }

        public DataNode (DataNode parent)
        {
            Parent = parent;
        }

        public DataNode Parent { get; set; }

        private bool _modified;
        public bool Modified
        {
            get { return _modified; }
            set
            {
                if (value && Parent != null) {
                    Parent.Modified = value;
                }
                _modified = value;
            }
        }
    }

    public class NbtDataNode : DataNode
    {
        public NbtDataNode ()
        {
        }

        public NbtDataNode (DataNode parent)
            : base(parent)
        {
        }

        public NbtTree Tree { get; set; }
    }

    public class RegionChunkData : NbtDataNode
    {
        public RegionChunkData (RegionFile file, int x, int z)
            : this(null, file, x, z)
        {
        }

        public RegionChunkData (DataNode parent, RegionFile file, int x, int z)
            : base(parent)
        {
            Region = file;
            X = x;
            Z = z;
        }

        public RegionFile Region { get; private set; }
        public int X { get; private set; }
        public int Z { get; private set; }
    }

    public class RegionData : DataNode
    {
        public RegionData (string path)
            : this(null, path)
        {
        }

        public RegionData (DataNode parent, string path)
            : base(parent)
        {
            Path = path;
        }

        public string Path { get; private set; }
    }

    public class NbtFileData : NbtDataNode
    {
        public NbtFileData (string path, CompressionType cztype)
            : this(null, path, cztype)
        {
        }

        public NbtFileData (DataNode parent, string path, CompressionType cztype)
            : base(parent)
        {
            Path = path;
            CompressionType = cztype;
        }

        public string Path { get; private set; }
        public CompressionType CompressionType { get; private set; }
    }

    public class DirectoryData : DataNode
    {
        public DirectoryData (string path)
            : this(null, path)
        {
        }

        public DirectoryData (DataNode parent, string path)
            : base(parent)
        {
            Path = path;
        }

        public string Path { get; private set; }
    }
}
