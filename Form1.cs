using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Substrate;
using Substrate.Nbt;
using System.IO;
using Substrate.Core;

namespace NBTExplorer
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
            _nodeTree.NodeMouseClick += NodeClicked;
            _nodeTree.NodeMouseDoubleClick += NodeDoubleClicked;

            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1) {
                OpenFile(args[1]);
            }
            else {
                OpenMinecraftDir();
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
            TreeNode node = new TreeNode();
            node.ImageIndex = _tagIconIndex[tag.GetTagType()];
            node.SelectedImageIndex = _tagIconIndex[tag.GetTagType()];
            node.Tag = tag;
            node.Text = GetNodeText(name, tag);

            if (tag.GetTagType() == TagType.TAG_LIST) {
                PopulateNodeFromTag(node, tag.ToTagList());
            }
            else if (tag.GetTagType() == TagType.TAG_COMPOUND) {
                PopulateNodeFromTag(node, tag.ToTagCompound());
            }

            return node;
        }

        private string GetTagNodeText (TagNode tag)
        {
            if (tag == null)
                return null;

            switch (tag.GetTagType()) {
                case TagType.TAG_BYTE:
                case TagType.TAG_SHORT:
                case TagType.TAG_INT:
                case TagType.TAG_LONG:
                case TagType.TAG_FLOAT:
                case TagType.TAG_DOUBLE:
                case TagType.TAG_STRING:
                    return tag.ToString();

                case TagType.TAG_BYTE_ARRAY:
                    return tag.ToTagByteArray().Length + " bytes";

                case TagType.TAG_LIST:
                    return tag.ToTagList().Count + " entries";

                case TagType.TAG_COMPOUND:
                    return tag.ToTagCompound().Count + " entries";
            }

            return null;
        }

        private string GetNodeText (string name, string value)
        {
            name = String.IsNullOrEmpty(name) ? "" : name + ": ";
            value = value ?? "";

            return name + value;
        }

        private string GetNodeText (string name, TagNode tag)
        {
            return GetNodeText(name, GetTagNodeText(tag));
        }

        private string GetNodeText (TreeNode node)
        {
            return GetNodeText(GetTagNodeName(node), GetTagNodeText(node));
        }

        private string GetTagNodeText (TreeNode node)
        {
            TagNode tag = GetTagNode(node);
            if (tag == null)
                return null;

            return GetTagNodeText(tag);
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
                        TreeNode child = CreateLazyChunk(rf, x, y);
                        node.Nodes.Add(child);
                        LinkDataNodeParent(child, node);
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

            data.Expanded = true;
        }

        public void LoadLazyNbt (TreeNode node)
        {
            NbtFileData data = node.Tag as NbtFileData;
            if (data == null || data.Modified)
                return;

            node.Nodes.Clear();

            NBTFile file = new NBTFile(data.Path);
            LoadNbtStream(node, file.GetDataInputStream(data.CompressionType));

            data.Expanded = true;
        }

        public void LoadLazyDirectory (TreeNode node)
        {
            DirectoryData data = node.Tag as DirectoryData;
            if (data == null || data.Modified)
                return;

            node.Nodes.Clear();

            foreach (string dirpath in Directory.GetDirectories(data.Path)) {
                node.Nodes.Add(CreateLazyDirectory(dirpath, node));
            }

            foreach (string filepath in Directory.GetFiles(data.Path)) {
                TryLoadFile(node.Nodes, filepath);
            }

            data.Expanded = true;
        }

        public void LoadLazyRegion (TreeNode node)
        {
            RegionData data = node.Tag as RegionData;
            if (data == null || data.Modified)
                return;

            node.Nodes.Clear();
            LoadRegion(node, data.Path);

            data.Expanded = true;
        }

        public void UnloadLazyDataNode (TreeNode node)
        {
            DataNode data = node.Tag as DataNode;
            if (data == null || data.Modified)
                return;

            node.Nodes.Clear();
            node.Nodes.Add(new TreeNode());

            data.Expanded = false;
        }

        private void ExpandNode (TreeNode node)
        {
            if (node.Tag == null)
                return;

            if (node.Tag is DataNode) {
                if ((node.Tag as DataNode).Expanded)
                    return;
            }

            if (node.Tag is RegionChunkData) {
                LoadLazyChunk(node);
            }
            else if (node.Tag is NbtFileData) {
                LoadLazyNbt(node);
            }
            else if (node.Tag is DirectoryData) {
                LoadLazyDirectory(node);
            }
            else if (node.Tag is RegionData) {
                LoadLazyRegion(node);
            }
        }

        private void CollapseNode (TreeNode node)
        {
            if (node.Tag == null)
                return;

            if (node.Tag is DataNode) {
                UnloadLazyDataNode(node);
            }
        }

        private void NodeExpand (object sender, TreeViewCancelEventArgs e)
        {
            ExpandNode(e.Node);
        }

        private void NodeCollapse (object sender, TreeViewEventArgs e)
        {
            CollapseNode(e.Node);
        }

        private void NodeSelected (object sender, TreeViewEventArgs e)
        {
            UpdateToolbar();
        }

        private void NodeClicked (object sender, TreeNodeMouseClickEventArgs e)
        {
            
        }

        private void NodeDoubleClicked (object sender, TreeNodeMouseClickEventArgs e)
        {
            EditNodeValue(_nodeTree.SelectedNode);
        }

        private void UpdateToolbar ()
        {
            TreeNode node = _nodeTree.SelectedNode;
            TagNode tag = node.Tag as TagNode;

            if (tag == null && node.Tag is NbtDataNode) {
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

        public void OpenDirectory (string path)
        {
            _nodeTree.Nodes.Clear();

            LoadDirectory(path);

            if (_nodeTree.Nodes.Count > 0) {
                _nodeTree.Nodes[0].Expand();
            }
        }

        public void OpenPaths (string[] paths)
        {
            _nodeTree.Nodes.Clear();

            foreach (string path in paths) {
                if (Directory.Exists(path)) {
                    LoadDirectory(path);
                }
                else if (File.Exists(path)) {
                    TryLoadFile(_nodeTree.Nodes, path);
                }
            }

            if (_nodeTree.Nodes.Count > 0) {
                _nodeTree.Nodes[0].Expand();
            }
        }

        public void OpenMinecraftDir ()
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

            foreach (string dirpath in Directory.GetDirectories(path)) {
                root.Nodes.Add(CreateLazyDirectory(dirpath, root));
            }

            foreach (string filepath in Directory.GetFiles(path)) {
                TryLoadFile(root.Nodes, filepath);
            }

            parent.Add(root);
        }

        public void TryLoadFile (TreeNodeCollection parent, string path)
        {
            if (Path.GetExtension(path) == ".mcr") {
                TreeNode node = CreateLazyRegion(path);
                parent.Add(node);
                LinkDataNodeParent(node, node.Parent);
                return;
            }

            if (Path.GetExtension(path) == ".dat" || Path.GetExtension(path) == ".schematic") {
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
            ofd.Multiselect = true;

            if (ofd.ShowDialog() == DialogResult.OK) {
                OpenPaths(ofd.FileNames);
            }
        }

        private void OpenFile (string file)
        {
            _nodeTree.Nodes.Clear();
            TryLoadFile(_nodeTree.Nodes, file);
        }

        private string _openFolderPath = null;
        private void OpenFolder ()
        {
            FolderBrowserDialog ofd = new FolderBrowserDialog();
            if (_openFolderPath != null)
                ofd.SelectedPath = _openFolderPath;

            if (ofd.ShowDialog() == DialogResult.OK) {
                _openFolderPath = ofd.SelectedPath;
                OpenDirectory(ofd.SelectedPath);
            }
        }

        private void SaveAll ()
        {
            foreach (TreeNode node in _nodeTree.Nodes) {
                SaveNode(node);
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
            else if (node.Tag is RegionChunkData) {
                SaveRegionChunkNode(node);
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

        private void SaveRegionChunkNode (TreeNode node)
        {
            RegionChunkData data = node.Tag as RegionChunkData;
            if (data == null || !data.Modified)
                return;

            using (Stream str = data.Region.GetChunkDataOutputStream(data.X, data.Z)) {
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
            OpenFolder();
        }

        private void toolStripButton1_Click (object sender, EventArgs e)
        {
            OpenFile();
        }

        private TreeNode BaseNode (TreeNode node)
        {
            if (node == null)
                return null;

            TreeNode baseNode = node;
            while (baseNode.Tag == null || !(baseNode.Tag is DataNode)) {
                baseNode = baseNode.Parent;
            }

            return baseNode;
        }

        #region Tag Deletion

        private void DeleteNode (TreeNode node)
        {
            TreeNode baseNode = BaseNode(node);

            // Can only delete internal NBT nodes
            if (baseNode == null || baseNode == node)
                return;

            (baseNode.Tag as DataNode).Modified = true;

            DeleteNodeNbtTag(node);

            if (node.Parent != null) {
                node.Parent.Text = GetNodeText(node.Parent);
            }

            node.Remove();
        }

        private void _buttonDelete_Click (object sender, EventArgs e)
        {
            DeleteNode(_nodeTree.SelectedNode);
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

        #endregion

        private void _buttonSave_Click (object sender, EventArgs e)
        {
            SaveAll();
        }

        private TagNode GetTagNode (TreeNode node)
        {
            if (node == null)
                return null;

            if (node.Tag is TagNode) {
                return node.Tag as TagNode;
            }
            else if (node.Tag is NbtDataNode) {
                NbtDataNode data = node.Tag as NbtDataNode;
                if (data == null)
                    return null;

                return data.Tree.Root;
            }

            return null;
        }

        private TagNode GetParentTagNode (TreeNode node)
        {
            if (GetTagNode(node) == null)
                return null;

            return GetTagNode(node.Parent);
        }

        private string GetTagNodeName (TreeNode node)
        {
            TagNode tag = GetTagNode(node);
            if (tag == null)
                return null;

            TagNode parentTag = GetTagNode(node.Parent);
            if (parentTag == null)
                return null;

            if (parentTag.GetTagType() != TagType.TAG_COMPOUND)
                return null;

            foreach (KeyValuePair<string, TagNode> sub in parentTag.ToTagCompound()) {
                if (sub.Value == tag)
                    return sub.Key;
            }

            return null;
        }

        private bool SetTagNodeName (TreeNode node, string name)
        {
            TagNode tag = GetTagNode(node);
            if (tag == null)
                return false;

            TagNode parentTag = GetTagNode(node.Parent);
            if (parentTag == null)
                return false;

            if (parentTag.GetTagType() != TagType.TAG_COMPOUND)
                return false;

            if (parentTag.ToTagCompound().ContainsKey(name))
                return false;

            string oldName = null;
            foreach (KeyValuePair<string, TagNode> sub in parentTag.ToTagCompound()) {
                if (sub.Value == tag) {
                    oldName = sub.Key;
                    break;
                }
            }

            parentTag.ToTagCompound().Remove(oldName);
            parentTag.ToTagCompound().Add(name, tag);

            return true;
        }

        private void EditNodeValue (TreeNode node)
        {
            if (node == null)
                return;

            TagNode tag = GetTagNode(node);
            if (tag == null)
                return;

            if (tag.GetTagType() == TagType.TAG_BYTE_ARRAY ||
                tag.GetTagType() == TagType.TAG_LIST ||
                tag.GetTagType() == TagType.TAG_COMPOUND)
                return;

            EditValue form = new EditValue(tag);
            if (form.ShowDialog() == DialogResult.OK) {
                TreeNode baseNode = BaseNode(node);
                if (baseNode != null) {
                    (baseNode.Tag as DataNode).Modified = true;
                }

                node.Text = GetNodeText(node);
            }
        }

        private void _buttonEdit_Click (object sender, EventArgs e)
        {
            EditNodeValue(_nodeTree.SelectedNode);
        }

        private void EditNodeName (TreeNode node)
        {
            if (node == null)
                return;

            TagNode tag = GetTagNode(node);
            if (tag == null)
                return;

            string name = GetTagNodeName(node);
            if (name == null)
                return;

            EditValue form = new EditValue(name);

            TagNode parentTag = GetParentTagNode(node);
            foreach (string key in parentTag.ToTagCompound().Keys) {
                form.InvalidNames.Add(key);
            }

            if (form.ShowDialog() == DialogResult.OK) {
                TreeNode baseNode = BaseNode(node);
                if (baseNode != null) {
                    (baseNode.Tag as DataNode).Modified = true;
                }

                SetTagNodeName(node, form.NodeName);
                node.Text = GetNodeText(node);
            }
        }

        private void _buttonRename_Click (object sender, EventArgs e)
        {
            EditNodeName(_nodeTree.SelectedNode);
        }

        private void AddTagToNode (TreeNode node, TagType type)
        {
            TagNode tag = GetTagNode(node);
            if (tag == null)
                return;

            if (tag.GetTagType() != TagType.TAG_COMPOUND &&
                tag.GetTagType() != TagType.TAG_LIST)
                return;

            if (tag.GetTagType() == TagType.TAG_LIST &&
                tag.ToTagList().ValueType != type &&
                tag.ToTagList().Count > 0)
                return;

            TagNode newNode = null;
            switch (type) {
                case TagType.TAG_BYTE:
                    newNode = new TagNodeByte();
                    break;
                case TagType.TAG_SHORT:
                    newNode = new TagNodeShort();
                    break;
                case TagType.TAG_INT:
                    newNode = new TagNodeInt();
                    break;
                case TagType.TAG_LONG:
                    newNode = new TagNodeLong();
                    break;
                case TagType.TAG_FLOAT:
                    newNode = new TagNodeFloat();
                    break;
                case TagType.TAG_DOUBLE:
                    newNode = new TagNodeDouble();
                    break;
                case TagType.TAG_BYTE_ARRAY:
                    newNode = new TagNodeByteArray();
                    break;
                case TagType.TAG_STRING:
                    newNode = new TagNodeString();
                    break;
                case TagType.TAG_LIST:
                    newNode = new TagNodeList(TagType.TAG_BYTE);
                    break;
                case TagType.TAG_COMPOUND:
                    newNode = new TagNodeCompound();
                    break;
            }

            if (tag is TagNodeCompound) {
                TagNodeCompound ctag = tag as TagNodeCompound;

                EditValue form = new EditValue("");
                foreach (string key in ctag.Keys) {
                    form.InvalidNames.Add(key);
                }

                if (form.ShowDialog() != DialogResult.OK)
                    return;

                ctag.Add(form.NodeName, newNode);

                TreeNode tnode = NodeFromTag(newNode, form.NodeName);
                node.Nodes.Add(tnode);

                _nodeTree.SelectedNode = tnode;
                tnode.Expand();
            }
            else if (tag is TagNodeList) {
                TagNodeList ltag = tag as TagNodeList;
                if (ltag.ValueType != type)
                    ltag.ChangeValueType(type);

                ltag.Add(newNode);

                TreeNode tnode = NodeFromTag(newNode);
                node.Nodes.Add(tnode);

                _nodeTree.SelectedNode = tnode;
                tnode.Expand();
            }

            node.Text = GetNodeText(node);

            TreeNode baseNode = BaseNode(node);
            if (baseNode != null) {
                (baseNode.Tag as DataNode).Modified = true;
            }
        }

        private void _buttonAddTagByte_Click (object sender, EventArgs e)
        {
            AddTagToNode(_nodeTree.SelectedNode, TagType.TAG_BYTE);
        }

        private void _buttonAddTagShort_Click (object sender, EventArgs e)
        {
            AddTagToNode(_nodeTree.SelectedNode, TagType.TAG_SHORT);
        }

        private void _buttonAddTagInt_Click (object sender, EventArgs e)
        {
            AddTagToNode(_nodeTree.SelectedNode, TagType.TAG_INT);
        }

        private void _buttonAddTagLong_Click (object sender, EventArgs e)
        {
            AddTagToNode(_nodeTree.SelectedNode, TagType.TAG_LONG);
        }

        private void _buttonAddTagFloat_Click (object sender, EventArgs e)
        {
            AddTagToNode(_nodeTree.SelectedNode, TagType.TAG_FLOAT);
        }

        private void _buttonAddTagDouble_Click (object sender, EventArgs e)
        {
            AddTagToNode(_nodeTree.SelectedNode, TagType.TAG_DOUBLE);
        }

        private void _buttonAddTagByteArray_Click (object sender, EventArgs e)
        {
            AddTagToNode(_nodeTree.SelectedNode, TagType.TAG_BYTE_ARRAY);
        }

        private void _buttonAddTagString_Click (object sender, EventArgs e)
        {
            AddTagToNode(_nodeTree.SelectedNode, TagType.TAG_STRING);
        }

        private void _buttonAddTagList_Click (object sender, EventArgs e)
        {
            AddTagToNode(_nodeTree.SelectedNode, TagType.TAG_LIST);
        }

        private void _buttonAddTagCompound_Click (object sender, EventArgs e)
        {
            AddTagToNode(_nodeTree.SelectedNode, TagType.TAG_COMPOUND);
        }

        private void saveToolStripMenuItem_Click (object sender, EventArgs e)
        {
            SaveAll();
        }

        private void exitToolStripMenuItem_Click (object sender, EventArgs e)
        {
            Close();
        }

        private TreeNode _rootSearchNode;
        private string _searchName;
        private string _searchValue;
        private IEnumerator<TreeNode> _search;

        private void Find (TreeNode node)
        {
            if (node == null)
                return;

            Find form = new Find();
            if (form.ShowDialog() == DialogResult.OK) {
                _rootSearchNode = node;

                _searchName = form.MatchName ? form.NameToken : null;
                _searchValue = form.MatchValue ? form.ValueToken : null;

                _search = FindNode(node).GetEnumerator();

                FindNext();
            }
        }

        private void FindNext ()
        {
            if (_search == null)
                return;

            toolStripStatusLabel1.Text = "Please wait...";
            Application.DoEvents();

            _nodeTree.SelectedNode = null;

            if (!_search.MoveNext()) {
                _nodeTree.SelectedNode = _rootSearchNode;
                if (_rootSearchNode != null && _rootSearchNode.Tag is DataNode) {
                    if (_rootSearchNode.IsExpanded && !(_rootSearchNode.Tag as DataNode).Expanded) {
                        _rootSearchNode.Collapse();
                    }
                }

                toolStripStatusLabel1.Text = "";

                MessageBox.Show("End of results");
                _search = null;
            }

            toolStripStatusLabel1.Text = "";
        }

        private IEnumerable<TreeNode> FindNode (TreeNode node)
        {
            if (node == null)
                yield break;

            bool expand = false;

            if (node.Tag is DataNode) {
                DataNode data = node.Tag as DataNode;
                if (!data.Expanded) {
                    ExpandNode(node);
                    expand = true;
                }
            }

            TagNode tag = GetTagNode(node);
            if (tag != null) {
                bool mName = _searchName == null;
                bool mValue = _searchValue == null;

                if (_searchName != null) {
                    string tagName = GetTagNodeName(node);
                    if (tagName != null)
                        mName = tagName.Contains(_searchName);
                }
                if (_searchValue != null) {
                    string tagValue = GetTagNodeText(node);
                    if (tagValue != null)
                        mValue = tagValue.Contains(_searchValue);
                }

                if (mName && mValue) {
                    _nodeTree.SelectedNode = node;
                    yield return node;
                }
            }

            foreach (TreeNode sub in node.Nodes) {
                foreach (TreeNode s in FindNode(sub))
                    yield return s;
            }

            if (expand) {
                DataNode data = node.Tag as DataNode;
                if (!data.Modified)
                    CollapseNode(node);
            }
        }

        private void findToolStripMenuItem_Click (object sender, EventArgs e)
        {
            Find(_nodeTree.SelectedNode);
        }

        private void findNextToolStripMenuItem_Click (object sender, EventArgs e)
        {
            FindNext();
        }

        private void _buttonFindNext_Click (object sender, EventArgs e)
        {
            if (_search == null)
                Find(_nodeTree.SelectedNode);
            else
                FindNext();
        }

        private void _buttonOpenFolder_Click (object sender, EventArgs e)
        {
            OpenFolder();
        }

        private void DropFile (DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            OpenPaths(files);
        }

        private void _nodeTree_DragDrop (object sender, DragEventArgs e)
        {
            DropFile(e);
        }

        private void _nodeTree_DragEnter (object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        private void openMinecraftSaveFolderToolStripMenuItem_Click (object sender, EventArgs e)
        {
            OpenMinecraftDir();
        }

        private void toolStripStatusLabel1_Click (object sender, EventArgs e)
        {

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

        public bool Expanded { get; set; }
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
