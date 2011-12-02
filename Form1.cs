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

            //LoadRegion(@"C:\Users\Justin\AppData\Roaming\.minecraft\saves\Creative1.8\region\r.0.0.mcr");

            LoadDirectory(@"C:\Users\Justin\AppData\Roaming\.minecraft\saves\Creative1.8");
        }

        public void LoadFile (string path) {
            NBTFile nbtstr = new NBTFile(path);

            using (Stream fs = nbtstr.GetDataInputStream()) {
                //LoadNbtStream(fs);
            }
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
            RegionFile rf = new RegionFile(path);

            TreeNode root = new TreeNode(Path.GetFileName(path), 11, 11);

            for (int x = 0; x < 32; x++) {
                for (int y = 0; y < 32; y++) {
                    if (rf.HasChunk(x, y)) {
                        root.Nodes.Add(CreateLazyChunk(rf, x, y));
                    }
                }
            }

            parent.Add(root);
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

        public void LoadLazyChunk (TreeNode node)
        {
            RegionChunkData data = node.Tag as RegionChunkData;
            if (data == null)
                return;

            node.Nodes.Clear();
            LoadNbtStream(node, data.Region.GetChunkDataInputStream(data.X, data.Z));
        }

        public void UnloadLazyChunk (TreeNode node)
        {
            RegionChunkData data = node.Tag as RegionChunkData;
            if (data == null)
                return;

            node.Nodes.Clear();
            node.Nodes.Add(new TreeNode());
        }

        public void LoadLazyNbt (TreeNode node)
        {
            NbtFileData data = node.Tag as NbtFileData;
            if (data == null)
                return;

            node.Nodes.Clear();

            NBTFile file = new NBTFile(data.Path);
            LoadNbtStream(node, file.GetDataInputStream(data.CompressionType));
        }

        public void UnloadLazyNbt (TreeNode node)
        {
            NbtFileData data = node.Tag as NbtFileData;
            if (data == null)
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
        }

        private void NodeCollapse (object sender, TreeViewEventArgs e)
        {
            if (e.Node.Tag == null)
                return;

            if (e.Node.Tag is RegionChunkData) {
                UnloadLazyChunk(e.Node);
            }
            else if (e.Node.Tag is NbtFileData) {
                UnloadLazyNbt(e.Node);
            }
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
                LoadDirectory(dirpath, root.Nodes, Path.GetFileName(dirpath));
            }

            foreach (string filepath in Directory.EnumerateFiles(path)) {
                TryLoadFile(root.Nodes, filepath);
            }

            parent.Add(root);
        }

        public void TryLoadFile (TreeNodeCollection parent, string path)
        {
            if (Path.GetExtension(path) == ".mcr") {
                LoadRegion(parent, path);
                return;
            }

            if (Path.GetExtension(path) == ".dat") {
                try {
                    NBTFile file = new NBTFile(path);
                    NbtTree tree = new NbtTree();
                    tree.ReadFrom(file.GetDataInputStream());
                    parent.Add(CreateLazyNbt(path, CompressionType.GZip));
                    return;
                }
                catch { }

                try {
                    NBTFile file = new NBTFile(path);
                    NbtTree tree = new NbtTree();
                    tree.ReadFrom(file.GetDataInputStream(CompressionType.None));
                    parent.Add(CreateLazyNbt(path, CompressionType.None));
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

    public class RegionChunkData
    {
        public RegionChunkData (RegionFile file, int x, int z)
        {
            Region = file;
            X = x;
            Z = z;
        }

        public RegionFile Region { get; private set; }
        public int X { get; private set; }
        public int Z { get; private set; }
    }

    public class NbtFileData
    {
        public NbtFileData (string path, CompressionType cztype)
        {
            Path = path;
            CompressionType = cztype;
        }

        public string Path { get; private set; }
        public CompressionType CompressionType { get; private set; }
    }
}
