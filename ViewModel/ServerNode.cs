using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Substrate.Core;
using Substrate.Nbt;
using System.Diagnostics;

namespace NBTExplorer.ViewModel
{
    internal class ServerNode
    {
        private static IDictionary<TagType, int> _tagIconIndex;
        static ServerNode()
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
        #region Tag Deletion

        internal static void DeleteNode(TreeNode node)
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

        internal static IEnumerable<TreeNode> FindNode(TreeNode node,int descriptionImageIndex,string searchName,string searchValue)
        {
            if (node == null)
                yield break;

            bool expand = false;

            if (node.Tag is DataNode)
            {
                DataNode data = node.Tag as DataNode;
                if (!data.Expanded)
                {
                    ServerNode.ExpandNode(node,descriptionImageIndex);
                    expand = true;
                }
            }

            TagNode tag = ServerNode.GetTagNode(node);
            if (tag != null)
            {
                bool mName = searchName == null;
                bool mValue = searchValue == null;

                if (searchName != null)
                {
                    string tagName = GetTagNodeName(node);
                    if (tagName != null)
                        mName = tagName.Contains(searchName);
                }
                if (searchValue != null)
                {
                    string tagValue = GetTagNodeText(node);
                    if (tagValue != null)
                        mValue = tagValue.Contains(searchValue);
                }

                if (mName && mValue)
                {
                    node.TreeView.SelectedNode = node;
                    yield return node;
                }
            }

            foreach (TreeNode sub in node.Nodes)
            {
                foreach (TreeNode s in FindNode(sub,descriptionImageIndex,searchName,searchValue))
                    yield return s;
            }

            if (expand)
            {
                DataNode data = node.Tag as DataNode;
                if (!data.Modified)
                    ServerNode.CollapseNode(node);
            }
        }
        internal static void EditNodeValue(TreeNode node)
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


        internal static void EditNodeName(TreeNode node)
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
        static void LoadNbtStream(TreeNodeCollection parent, Stream stream)
        {
            LoadNbtStream(parent, stream);
        }

       static void LoadNbtStream(TreeNodeCollection parent,int descriptionImageIndex, Stream stream, string name = null)
        {
            var text = !String.IsNullOrEmpty(name) ? name : null;
            text = text ?? "[root]";

            TreeNode root = new TreeNode(text, 9, 9);

            LoadNbtStream(root, descriptionImageIndex, stream);

            parent.Add(root);
        }
       internal static bool NeedsExpand(TreeNode node)
       {
           return node.Tag is DataNode && (node.Tag as DataNode).Expanded == false;

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

       static TreeNode CreateDescriptionNode(string text,int imageIndex)
       {
           var newNode = new TreeNode();
           newNode.Text = text;
           newNode.ImageIndex =imageIndex;
           newNode.SelectedImageIndex = imageIndex;
           return newNode;
       }

       static void PopulateNodeFromTag(TreeNode parentNode,int descriptionIndex, IEnumerable<TagNode> list)
       {
           foreach (TagNode tag in list)
           {
               var node = NodeFromTag(tag,descriptionIndex);
               if (parentNode.Text.StartsWith("Inventory") || parentNode.Text.StartsWith("Item"))
               {

                   Debug.Assert(tag.IsCastableTo(TagType.TAG_COMPOUND));
                   var item = UIHelper.TryGetItemName(tag);

                   if (item != null)
                   {

                       var newNode = CreateDescriptionNode(item.Name,descriptionIndex);
                       newNode.ToolTipText = "StackSize:" + item.StackSize.ToString();

                       node.Nodes.Insert(0, newNode);
                   }




               }


               parentNode.Nodes.Add(node);
           }
       }
       internal static void AddTagToNode( TreeNode node, int descriptionIndex, TagType type)
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

               TreeNode tnode = NodeFromTag(newNode, descriptionIndex, form.NodeName);
               node.Nodes.Add(tnode);

               tnode.TreeView.SelectedNode = tnode;
               tnode.Expand();
           }
           else if (tag is TagNodeList)
           {
               var ltag = tag as TagNodeList;
               if (ltag.ValueType != type)
                   ltag.ChangeValueType(type);

               ltag.Add(newNode);

               TreeNode tnode = NodeFromTag(newNode, descriptionIndex);
               node.Nodes.Add(tnode);
               tnode.TreeView.SelectedNode = tnode;

               tnode.Expand();
           }

           node.Text = GetNodeText(node);

           TreeNode baseNode = BaseNode(node);
           if (baseNode != null)
           {
               (baseNode.Tag as DataNode).Modified = true;
           }
       }
       internal static void CollapseNode(TreeNode node)
       {
           if (node.Tag == null)
               return;

           if (node.Tag is DataNode)
           {
               UnloadLazyDataNode(node);
           }
       }


       static TreeNode NodeFromTag(TagNode tag, int descriptionImageIndex)
       {
           return NodeFromTag(tag, descriptionImageIndex, null);
       }

       static TreeNode NodeFromTag(TagNode tag, int descriptionIndex ,string name)
       {
           var text = SubstrateHelper.GetNodeText(name, tag);
           var node = ServerNode.InitializeTreeNode(_tagIconIndex[tag.GetTagType()], text, tag);


           if (tag.GetTagType() == TagType.TAG_LIST)
           {
               PopulateNodeFromTag(node,descriptionIndex, tag.ToTagList());
           }
           else if (tag.GetTagType() == TagType.TAG_COMPOUND)
           {
               PopulateNodeFromTag(node,descriptionIndex, tag.ToTagCompound());
           }

           return node;
       }
       static void PopulateNodeFromTag(TreeNode node,int descriptionIndex, IEnumerable<KeyValuePair<string, TagNode>> dict)
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
               node.Nodes.Add(NodeFromTag(kv.Value,descriptionIndex, kv.Key.Name));
           }
           if (node.Text.StartsWith("Item"))
           {
               var item = UIHelper.TryGetItemName((TagNode)node.Tag);
               if (item == null)
                   return;
               var newNode = CreateDescriptionNode(item.Name,descriptionIndex);
               node.Nodes.Insert(0, newNode);
           }
       }

       static void LoadNbtStream(TreeNode node,int descriptionIndex, Stream stream)
        {
            NbtTree tree = new NbtTree();
            tree.ReadFrom(stream);

            if (node.Tag != null && node.Tag is NbtDataNode)
            {
                (node.Tag as NbtDataNode).Tree = tree;
            }

            PopulateNodeFromTag(node,descriptionIndex, tree.Root);
        }
       internal static void LoadLazyNbt(TreeNode node,  int descriptionImageIndex)
        {
            NbtFileData data = node.Tag as NbtFileData;
            if (data == null || data.Modified)
                return;

            node.Nodes.Clear();

            NBTFile file = new NBTFile(data.Path);
            LoadNbtStream(node,descriptionImageIndex, file.GetDataInputStream(data.CompressionType));

            data.Expanded = true;
        }

        internal static void LoadRegion(TreeNodeCollection parent, string path)
        {
            TreeNode root = new TreeNode(Path.GetFileName(path), 11, 11);
            LoadRegion(root, path);
            parent.Add(root);
        }

        internal static void TryLoadFile(TreeNodeCollection parent, string path)
        {
            if (Path.GetExtension(path) == ".mcr")
            {
                TreeNode node = ServerNode.CreateLazyRegion(path);
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
                    TreeNode node = ServerNode.CreateLazyNbt(path, CompressionType.GZip);
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
                    TreeNode node = ServerNode.CreateLazyNbt(path, CompressionType.None);
                    parent.Add(node);
                    LinkDataNodeParent(node, node.Parent);
                    return;
                }
                catch { }
            }
        }

        internal static void LoadRegion(TreeNode node, string path)
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

        internal static TreeNode InitializeParentNode(Int32 imageIndex, string text, object tag)
        {

            var node = InitializeTreeNode(imageIndex, text, tag);
            node.Nodes.Add(new TreeNode()); //add stub so that it shows as expandable
            return node;
        }
        internal static TreeNode InitializeTreeNode(Int32 imageIndex, string text, object tag)
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

        internal static void LoadDirectory(string path, TreeNodeCollection parent, string name)
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
       internal static TreeNode CreateLazyChunk(RegionFile rf, int x, int z)
        {
            return InitializeParentNode(9, "Chunk [" + x + ", " + z + "]", new RegionChunkData(rf, x, z));
        }
       internal static TreeNode CreateLazyRegion(string path)
       {
           var fileName = Path.GetFileName(path);
           var node = InitializeParentNode(11, fileName, new RegionData(path));

           return node;
       }

       internal static TreeNode CreateLazyNbt(string path, CompressionType cztype)
       {
           var fileName = Path.GetFileName(path);
           var node = InitializeParentNode(12, fileName, new NbtFileData(path, cztype));

           return node;
       }

       internal static TreeNode CreateLazyDirectory(string path)
       {
           var filename = Path.GetFileName(path);
           var node = InitializeParentNode(10, filename, new DirectoryData(path));

           return node;
       }

       internal static TreeNode CreateLazyDirectory(string path, TreeNode parent)
       {
           TreeNode node = CreateLazyDirectory(path);
           LinkDataNodeParent(node, parent);

           return node;
       }

       internal TreeNode CreateLazyNbt(string path, CompressionType cztype, TreeNode parent)
       {
           var node = CreateLazyNbt(path, cztype);
           LinkDataNodeParent(node, parent);

           return node;
       }

        internal static void LoadLazyDirectory(TreeNode node)
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

        internal static void LoadLazyRegion(TreeNode node)
        {
            RegionData data = node.Tag as RegionData;
            if (data == null || data.Modified)
                return;

            node.Nodes.Clear();
            LoadRegion(node, data.Path);

            data.Expanded = true;
        }

        internal static void UnloadLazyDataNode(TreeNode node)
        {
            DataNode data = node.Tag as DataNode;
            if (data == null || data.Modified)
                return;

            node.Nodes.Clear();
            node.Nodes.Add(new TreeNode());

            data.Expanded = false;
        }

        internal static void LoadLazyChunk(TreeNode node,int descriptionImageIndex)
        {
            RegionChunkData data = node.Tag as RegionChunkData;
            if (data == null || data.Modified)
                return;

            node.Nodes.Clear();
            LoadNbtStream(node, descriptionImageIndex,data.Region.GetChunkDataInputStream(data.X, data.Z));

            data.Expanded = true;
        }
        internal static void ExpandNode(TreeNode node,int descriptionImageIndex)
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
                LoadLazyChunk(node,descriptionImageIndex);
            }
            else if (node.Tag is NbtFileData)
            {
                LoadLazyNbt(node, descriptionImageIndex);
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
    }
}
