using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Substrate.Core;
using Substrate.Nbt;

namespace NBTExplorer
{
    public partial class Form1 : Form
    {
        readonly static Dictionary<TagType, int> _tagIconIndex;

        public Form1()
        {
            InitializeComponent();

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

        void LoadNbtStream(TreeNodeCollection parent, Stream stream)
        {
            LoadNbtStream(parent, stream);
        }

        void LoadNbtStream(TreeNodeCollection parent, Stream stream, string name = null)
        {
            var text = !String.IsNullOrEmpty(name) ? name : null;
            text = text ?? "[root]";

            TreeNode root = new TreeNode(text, 9, 9);

            LoadNbtStream(root, stream);

            parent.Add(root);
        }

        void LoadNbtStream(TreeNode node, Stream stream)
        {
            NbtTree tree = new NbtTree();
            tree.ReadFrom(stream);

            if (node.Tag != null && node.Tag is NbtDataNode)
            {
                (node.Tag as NbtDataNode).Tree = tree;
            }

            PopulateNodeFromTag(node, tree.Root);
        }

        TreeNode NodeFromTag(TagNode tag)
        {
            return NodeFromTag(tag, null);
        }

        TreeNode NodeFromTag(TagNode tag, string name)
        {
            var text = SubstrateHelper.GetNodeText(name, tag);
            var node=InitializeTreeNode(_tagIconIndex[tag.GetTagType()], text, tag);
           

            if (tag.GetTagType() == TagType.TAG_LIST)
            {
                PopulateNodeFromTag(node, tag.ToTagList());
            }
            else if (tag.GetTagType() == TagType.TAG_COMPOUND)
            {
                PopulateNodeFromTag(node, tag.ToTagCompound());
            }

            return node;
        }

        static string GetNodeText(TreeNode node)
        {
            return SubstrateHelper.GetNodeText(GetTagNodeName(node), GetTagNodeText(node));
        }

        static string GetTagNodeText(TreeNode node)
        {
            TagNode tag = GetTagNode(node);
            if (tag == null)
                return null;

            return SubstrateHelper.GetTagNodeText(tag);
        }

        void PopulateNodeFromTag(TreeNode parentNode, IEnumerable<TagNode> list)
        {
            foreach (TagNode tag in list)
            {
                var node = NodeFromTag(tag);
                if (parentNode.Text.StartsWith("Inventory") || parentNode.Text.StartsWith("Item"))
                {

                    Debug.Assert(tag.IsCastableTo(TagType.TAG_COMPOUND));
                    var item = UIHelper.TryGetItemName(tag);

                    if (item != null)
                    {

                        var newNode = new TreeNode();

                        //node.ImageIndex = _tagIconIndex[0];
                        //node.SelectedImageIndex = _tagIconIndex[0];
                        newNode.Text = item.Name;
                        node.ImageIndex = this.imageList1.Images.Count;
                        node.SelectedImageIndex = this.imageList1.Images.Count;
                        newNode.ToolTipText = "StackSize:" + item.StackSize.ToString();

                        node.Nodes.Insert(0, newNode);
                    }




                }


                parentNode.Nodes.Add(node);
            }
        }

        void PopulateNodeFromTag(TreeNode node, IEnumerable<KeyValuePair<string, TagNode>> dict)
        {
            if (dict == null)
                return;
            var list = new SortedList<TagKey, TagNode>();
            foreach (KeyValuePair<string, TagNode> kv in dict)
            {
                list.Add(new TagKey(kv.Key, kv.Value.GetTagType()), kv.Value);
            }

            foreach (KeyValuePair<TagKey, TagNode> kv in list)
            {
                node.Nodes.Add(NodeFromTag(kv.Value, kv.Key.Name));
            }
        }

        void LoadRegion(TreeNodeCollection parent, string path)
        {
            TreeNode root = new TreeNode(Path.GetFileName(path), 11, 11);
            LoadRegion(root, path);
            parent.Add(root);
        }

        static void LoadRegion(TreeNode node, string path)
        {
            RegionFile rf = new RegionFile(path);

            for (int x = 0; x < 32; x++)
            {
                for (int y = 0; y < 32; y++)
                {
                    if (rf.HasChunk(x, y))
                    {
                        TreeNode child = CreateLazyChunk(rf, x, y);
                        node.Nodes.Add(child);
                        LinkDataNodeParent(child, node);
                    }
                }
            }
        }

        static TreeNode CreateLazyChunk(RegionFile rf, int x, int z)
        {
            return InitializeParentNode(9, "Chunk [" + x + ", " + z + "]", new RegionChunkData(rf, x, z));
        }
        static TreeNode InitializeTreeNode(Int32 imageIndex, string text, object tag)
        {
            var node = new TreeNode
            {
                ImageIndex = imageIndex,
                SelectedImageIndex = imageIndex,
                Text = text,
                Tag = tag

            };
            return node;
        }

         static TreeNode InitializeParentNode(Int32 imageIndex, string text, object tag)
         {

             var node = InitializeTreeNode(imageIndex, text, tag);
            node.Nodes.Add(new TreeNode()); //add stub so that it shows as expandable
            return node;
        }

        static TreeNode CreateLazyRegion(string path)
        {
            var fileName = Path.GetFileName(path);
            var node = InitializeParentNode(11, fileName, new RegionData(path));

            return node;
        }

        static TreeNode CreateLazyNbt(string path, CompressionType cztype)
        {
            var fileName = Path.GetFileName(path);
            var node = InitializeParentNode(12, fileName,new NbtFileData(path, cztype));

            return node;
        }

        static TreeNode CreateLazyDirectory(string path)
        {
            var filename = Path.GetFileName(path);
            var node = InitializeParentNode(10, filename, new DirectoryData(path));

            return node;
        }

        static TreeNode CreateLazyDirectory(string path, TreeNode parent)
        {
            TreeNode node = CreateLazyDirectory(path);
            LinkDataNodeParent(node, parent);

            return node;
        }

        TreeNode CreateLazyNbt(string path, CompressionType cztype, TreeNode parent)
        {
            var node = CreateLazyNbt(path, cztype);
            LinkDataNodeParent(node, parent);

            return node;
        }

        static void LinkDataNodeParent(TreeNode node, TreeNode parent)
        {
            if (node != null && parent != null && node.Tag != null && parent.Tag != null)
            {
                DataNode nodeDn = node.Tag as DataNode;
                DataNode parentDn = parent.Tag as DataNode;

                if (nodeDn != null && parentDn != null)
                {
                    nodeDn.Parent = parentDn;
                }
            }
        }

        void LoadLazyChunk(TreeNode node)
        {
            RegionChunkData data = node.Tag as RegionChunkData;
            if (data == null || data.Modified)
                return;

            node.Nodes.Clear();
            LoadNbtStream(node, data.Region.GetChunkDataInputStream(data.X, data.Z));

            data.Expanded = true;
        }

        void LoadLazyNbt(TreeNode node)
        {
            NbtFileData data = node.Tag as NbtFileData;
            if (data == null || data.Modified)
                return;

            node.Nodes.Clear();

            NBTFile file = new NBTFile(data.Path);
            LoadNbtStream(node, file.GetDataInputStream(data.CompressionType));

            data.Expanded = true;
        }

        static void LoadLazyDirectory(TreeNode node)
        {
            var data = node.Tag as DirectoryData;
            if (data == null || data.Modified)
                return;

            node.Nodes.Clear();

            foreach (string dirpath in Directory.EnumerateDirectories(data.Path))
            {
                node.Nodes.Add(CreateLazyDirectory(dirpath, node));
            }

            foreach (string filepath in Directory.EnumerateFiles(data.Path))
            {
                TryLoadFile(node.Nodes, filepath);
            }

            data.Expanded = true;
        }

        static void LoadLazyRegion(TreeNode node)
        {
            RegionData data = node.Tag as RegionData;
            if (data == null || data.Modified)
                return;

            node.Nodes.Clear();
            LoadRegion(node, data.Path);

            data.Expanded = true;
        }

        static void UnloadLazyDataNode(TreeNode node)
        {
            DataNode data = node.Tag as DataNode;
            if (data == null || data.Modified)
                return;

            node.Nodes.Clear();
            node.Nodes.Add(new TreeNode());

            data.Expanded = false;
        }

        void ExpandNode(TreeNode node)
        {
            if (node.Tag == null)
                return;

            if (node.Tag is DataNode)
            {
                if ((node.Tag as DataNode).Expanded)
                    return;
            }

            if (node.Tag is RegionChunkData)
            {
                LoadLazyChunk(node);
            }
            else if (node.Tag is NbtFileData)
            {
                LoadLazyNbt(node);
            }
            else if (node.Tag is DirectoryData)
            {
                LoadLazyDirectory(node);
            }
            else if (node.Tag is RegionData)
            {
                LoadLazyRegion(node);
            }
        }

        static void CollapseNode(TreeNode node)
        {
            if (node.Tag == null)
                return;

            if (node.Tag is DataNode)
            {
                UnloadLazyDataNode(node);
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
                    TryLoadFile(_nodeTree.Nodes, path);
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
            LoadDirectory(path, parent, path);
        }

        static void LoadDirectory(string path, TreeNodeCollection parent, string name)
        {
            TreeNode root = new TreeNode(name, 10, 10);

            foreach (string dirpath in Directory.EnumerateDirectories(path))
            {
                root.Nodes.Add(CreateLazyDirectory(dirpath, root));
            }

            foreach (string filepath in Directory.EnumerateFiles(path))
            {
                TryLoadFile(root.Nodes, filepath);
            }

            parent.Add(root);
        }

        static void TryLoadFile(TreeNodeCollection parent, string path)
        {
            if (Path.GetExtension(path) == ".mcr")
            {
                TreeNode node = CreateLazyRegion(path);
                parent.Add(node);
                LinkDataNodeParent(node, node.Parent);
                return;
            }

            if (Path.GetExtension(path) == ".dat")
            {
                try
                {
                    NBTFile file = new NBTFile(path);
                    NbtTree tree = new NbtTree();
                    tree.ReadFrom(file.GetDataInputStream());
                    TreeNode node = CreateLazyNbt(path, CompressionType.GZip);
                    parent.Add(node);
                    LinkDataNodeParent(node, node.Parent);
                    return;
                }
                catch { }

                try
                {
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

        static Form1()
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
            TryLoadFile(_nodeTree.Nodes, file);
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


        static TreeNode BaseNode(TreeNode node)
        {
            if (node == null)
                return null;

            TreeNode baseNode = node;
            while (baseNode.Tag == null || !(baseNode.Tag is DataNode))
            {
                baseNode = baseNode.Parent;
            }

            return baseNode;
        }

        #region Tag Deletion

        static void DeleteNode(TreeNode node)
        {
            TreeNode baseNode = BaseNode(node);

            // Can only delete internal NBT nodes
            if (baseNode == null || baseNode == node)
                return;

            (baseNode.Tag as DataNode).Modified = true;

            DeleteNodeNbtTag(node);

            if (node.Parent != null)
            {
                node.Parent.Text = GetNodeText(node.Parent);
            }

            node.Remove();
        }



        static void DeleteNodeNbtTag(TreeNode node)
        {
            TagNode tag = node.Tag as TagNode;
            if (tag == null)
                return;

            TagNode parentTag = node.Parent.Tag as TagNode;
            if (parentTag == null)
            {
                NbtDataNode parentData = node.Parent.Tag as NbtDataNode;
                if (parentData == null)
                    return;

                parentTag = parentData.Tree.Root;
                if (parentTag == null)
                    return;
            }

            switch (parentTag.GetTagType())
            {
                case TagType.TAG_LIST:
                    parentTag.ToTagList().Remove(tag);
                    break;

                case TagType.TAG_COMPOUND:
                    DeleteTagFromCompound(parentTag.ToTagCompound(), tag);
                    break;
            }
        }

        static void DeleteTagFromCompound(IDictionary<string, TagNode> parent, TagNode target)
        {
            string match = "";
            foreach (KeyValuePair<string, TagNode> kv in parent)
            {
                if (kv.Value == target)
                {
                    match = kv.Key;
                    break;
                }
            }

            if (match != null)
            {
                parent.Remove(match);
            }
        }

        #endregion


        static TagNode GetTagNode(TreeNode node)
        {
            if (node == null)
                return null;

            if (node.Tag is TagNode)
            {
                return node.Tag as TagNode;
            }
            else if (node.Tag is NbtDataNode)
            {
                NbtDataNode data = node.Tag as NbtDataNode;
                if (data == null)
                    return null;

                return data.Tree.Root;
            }

            return null;
        }

        static TagNode GetParentTagNode(TreeNode node)
        {
            if (GetTagNode(node) == null)
                return null;

            return GetTagNode(node.Parent);
        }

        static string GetTagNodeName(TreeNode node)
        {
            TagNode tag = GetTagNode(node);
            if (tag == null)
                return null;

            TagNode parentTag = GetTagNode(node.Parent);
            if (parentTag == null)
                return null;

            if (parentTag.GetTagType() != TagType.TAG_COMPOUND)
                return null;

            foreach (KeyValuePair<string, TagNode> sub in parentTag.ToTagCompound())
            {
                if (sub.Value == tag)
                    return sub.Key;
            }

            return null;
        }

        static bool SetTagNodeName(TreeNode node, string name)
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
            foreach (KeyValuePair<string, TagNode> sub in parentTag.ToTagCompound())
            {
                if (sub.Value == tag)
                {
                    oldName = sub.Key;
                    break;
                }
            }

            parentTag.ToTagCompound().Remove(oldName);
            parentTag.ToTagCompound().Add(name, tag);

            return true;
        }

        static void EditNodeValue(TreeNode node)
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
            if (form.ShowDialog() == DialogResult.OK)
            {
                TreeNode baseNode = BaseNode(node);
                if (baseNode != null)
                {
                    (baseNode.Tag as DataNode).Modified = true;
                }

                node.Text = GetNodeText(node);
            }
        }



        static void EditNodeName(TreeNode node)
        {
            if (node == null)
                return;

            TagNode tag = GetTagNode(node);
            if (tag == null)
                return;

            string name = GetTagNodeName(node);
            if (name == null)
                return;

            var form = new EditValue(name);

            var parentTag = GetParentTagNode(node);
            foreach (string key in parentTag.ToTagCompound().Keys)
            {
                form.InvalidNames.Add(key);
            }

            if (form.ShowDialog() == DialogResult.OK)
            {
                TreeNode baseNode = BaseNode(node);
                if (baseNode != null)
                {
                    (baseNode.Tag as DataNode).Modified = true;
                }

                SetTagNodeName(node, form.NodeName);
                node.Text = GetNodeText(node);
            }
        }


        void AddTagToNode(TreeNode node, TagType type)
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
            switch (type)
            {
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

            if (tag is TagNodeCompound)
            {
                TagNodeCompound ctag = tag as TagNodeCompound;

                EditValue form = new EditValue("");
                foreach (string key in ctag.Keys)
                {
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
            else if (tag is TagNodeList)
            {
                var ltag = tag as TagNodeList;
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
            if (baseNode != null)
            {
                (baseNode.Tag as DataNode).Modified = true;
            }
        }
        #region Events

        static void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new About().Show();
        }

        void NodeExpand(object sender, TreeViewCancelEventArgs e)
        {
            ExpandNode(e.Node);
        }

        static void NodeCollapse(object sender, TreeViewEventArgs e)
        {
            CollapseNode(e.Node);
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
            EditNodeValue(_nodeTree.SelectedNode);
        }

        void _buttonEdit_Click(object sender, EventArgs e)
        {
            EditNodeValue(_nodeTree.SelectedNode);
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
            DeleteNode(_nodeTree.SelectedNode);
        }

        void _buttonRename_Click(object sender, EventArgs e)
        {
            EditNodeName(_nodeTree.SelectedNode);
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

        void _buttonAddTagByte_Click(object sender, EventArgs e)
        {
            AddTagToNode(_nodeTree.SelectedNode, TagType.TAG_BYTE);
        }

        void _buttonAddTagShort_Click(object sender, EventArgs e)
        {
            AddTagToNode(_nodeTree.SelectedNode, TagType.TAG_SHORT);
        }

        void _buttonAddTagInt_Click(object sender, EventArgs e)
        {
            AddTagToNode(_nodeTree.SelectedNode, TagType.TAG_INT);
        }

        private void _buttonAddTagLong_Click(object sender, EventArgs e)
        {
            AddTagToNode(_nodeTree.SelectedNode, TagType.TAG_LONG);
        }

        void _buttonAddTagFloat_Click(object sender, EventArgs e)
        {
            AddTagToNode(_nodeTree.SelectedNode, TagType.TAG_FLOAT);
        }

        void _buttonAddTagDouble_Click(object sender, EventArgs e)
        {
            AddTagToNode(_nodeTree.SelectedNode, TagType.TAG_DOUBLE);
        }

        void _buttonAddTagByteArray_Click(object sender, EventArgs e)
        {
            AddTagToNode(_nodeTree.SelectedNode, TagType.TAG_BYTE_ARRAY);
        }

        void _buttonAddTagString_Click(object sender, EventArgs e)
        {
            AddTagToNode(_nodeTree.SelectedNode, TagType.TAG_STRING);
        }

        void _buttonAddTagList_Click(object sender, EventArgs e)
        {
            AddTagToNode(_nodeTree.SelectedNode, TagType.TAG_LIST);
        }

        void _buttonAddTagCompound_Click(object sender, EventArgs e)
        {
            AddTagToNode(_nodeTree.SelectedNode, TagType.TAG_COMPOUND);
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

                _search = FindNode(node).GetEnumerator();

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

        IEnumerable<TreeNode> FindNode(TreeNode node)
        {
            if (node == null)
                yield break;

            bool expand = false;

            if (node.Tag is DataNode)
            {
                DataNode data = node.Tag as DataNode;
                if (!data.Expanded)
                {
                    ExpandNode(node);
                    expand = true;
                }
            }

            TagNode tag = GetTagNode(node);
            if (tag != null)
            {
                bool mName = _searchName == null;
                bool mValue = _searchValue == null;

                if (_searchName != null)
                {
                    string tagName = GetTagNodeName(node);
                    if (tagName != null)
                        mName = tagName.Contains(_searchName);
                }
                if (_searchValue != null)
                {
                    string tagValue = GetTagNodeText(node);
                    if (tagValue != null)
                        mValue = tagValue.Contains(_searchValue);
                }

                if (mName && mValue)
                {
                    _nodeTree.SelectedNode = node;
                    yield return node;
                }
            }

            foreach (TreeNode sub in node.Nodes)
            {
                foreach (TreeNode s in FindNode(sub))
                    yield return s;
            }

            if (expand)
            {
                DataNode data = node.Tag as DataNode;
                if (!data.Modified)
                    CollapseNode(node);
            }
        }

        void DropFile(DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            OpenPaths(files);
        }


        static IEnumerable<TreeNode> FindChunks(IEnumerable<TreeNode> nodes,Func<TreeNode,bool> predicate)
        {
            foreach (var region in nodes)
                foreach (var chunk in region.Nodes.Cast<TreeNode>().Where(c => c.Text.StartsWith("Chunk")))
                {
                    if(predicate==null || predicate(chunk))
                    yield return chunk;
                }
        }

        int _nodesSearched = 0;

        static bool NeedsExpand(TreeNode node)
        {
            return node.Tag is DataNode && (node.Tag as DataNode).Expanded == false;

        }

        IEnumerable<TreeNode> FindRegions(IEnumerable<TreeNode> nodes, Func<TreeNode, bool> predicate)
        {

            foreach (var node in nodes)
            {
                _nodesSearched++;
                if (NeedsExpand(node))
                    ExpandNode(node);
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
        IEnumerable<TreeNode> FindNextChunk()
        {
            var lastFound =
                findEntityChunkToolStripMenuItem.DropDownItems.Cast<ToolStripItem>().LastOrDefault(
                    t => t.Text.StartsWith("Chunk"));
            if (lastFound != null)
            {

                lastFound.PerformClick();
                Application.DoEvents();
                return FindFrom(_nodeTree.SelectedNode);
            }
            else return FindFrom(_nodeTree.Nodes[0]);

        }

        /// <summary>
        /// Find the next chunk that has entities starting from start
        /// </summary>
        /// <param name="start"></param>
        /// <returns></returns>
        IEnumerable<TreeNode> FindFrom(TreeNode start)
        {
            if (this._nodeTree.Nodes.Count==0)
                return Enumerable.Empty<TreeNode>();
            
            var entityNodes =
                FindRegions(
                    _nodeTree.Nodes.Cast<TreeNode>(),tn => tn.Text.EndsWith(".mcr"));

            var entityRegions = entityNodes.ToArray();
            try
            {
                if (entityRegions == null || entityRegions.Any() == false)
                {
                    return Enumerable.Empty<TreeNode>();
                }
                var chunks = FindChunks(entityRegions,tn=> tn.Index > start.Index);
                if (chunks == null || chunks.Any() == false)
                    return Enumerable.Empty<TreeNode>();
                var populatedChunks = FindChunksWithEntities(chunks);
                if (populatedChunks == null || populatedChunks.Any() == false)
                    return Enumerable.Empty<TreeNode>();
                return populatedChunks;
            }
            finally
            {
                toolStrip1.Text = "Searched " + _nodesSearched + " nodes";
                toolStrip1.Visible = true;
            }
        }
        void findEntityChunkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _nodesSearched = 0;
            var populatedChunks=FindNextChunk();
            var first = populatedChunks.FirstOrDefault();
            if (first == null)
                return;
            first.EnsureVisible();
            _nodeTree.SelectedNode = first;
            var ddl = new ToolStripButton();
            ddl.Text = first.Text;
            ddl.Width = Text.Length*5;
            ddl.ToolTipText = first.Parent.Text;

            ddl.Click += (sender2, e2) =>
            {
                first.EnsureVisible();
                _nodeTree.SelectedNode = first;
            };
            findEntityChunkToolStripMenuItem.DropDownItems.Add(ddl);


        }

        IEnumerable<TreeNode> FindChunksWithEntities(IEnumerable<TreeNode> chunks)
        {
            foreach (var chunk in chunks)
            {
                if (NeedsExpand(chunk))
                    ExpandNode(chunk);
                var level = chunk.Nodes.Cast<TreeNode>().FirstOrDefault(n => n.Text.StartsWith("Level"));
                if (level == null)
                    continue;
                var entityNode = level.Nodes.Cast<TreeNode>().FirstOrDefault(n => n.Text.StartsWith("Entities"));
                if (entityNode == null)
                    continue;
                var text = entityNode.Text.Where(c => char.IsNumber(c)).Select(c => c.ToString()).Aggregate((c1, c2) => c1 + c2);

                var entityCount = int.Parse(text);
                if (entityCount > 0)
                    yield return chunk;
            }
        }
    }
}
