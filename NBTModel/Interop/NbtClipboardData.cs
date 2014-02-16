using System;
using System.IO;
using Substrate.Nbt;

namespace NBTModel.Interop
{
    public class NbtClipboardData
    {
        private string _name;
        private TagNode _node;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public TagNode Node
        {
            get { return _node; }
            set { _node = value; }
        }

        public NbtClipboardData (string name, TagNode node)
        {
            _name = name;
            _node = node;
        }

        public static byte[] SerializeNode (TagNode node)
        {
            TagNodeCompound root = new TagNodeCompound();
            root.Add("root", node);
            NbtTree tree = new NbtTree(root);

            using (MemoryStream ms = new MemoryStream()) {
                tree.WriteTo(ms);
                byte[] data = new byte[ms.Length];
                Array.Copy(ms.GetBuffer(), data, ms.Length);

                return data;
            }
        }

        public static TagNode DeserializeNode (byte[] data)
        {
            NbtTree tree = new NbtTree();
            using (MemoryStream ms = new MemoryStream(data)) {
                tree.ReadFrom(ms);
            }

            TagNodeCompound root = tree.Root;
            if (root == null || !root.ContainsKey("root"))
                return null;

            return root["root"];
        }
    }
}
