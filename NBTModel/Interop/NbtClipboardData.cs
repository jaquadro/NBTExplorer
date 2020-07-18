using Substrate.Nbt;
using System;
using System.IO;

namespace NBTModel.Interop
{
    public class NbtClipboardData
    {
        public NbtClipboardData(string name, TagNode node)
        {
            Name = name;
            Node = node;
        }

        public string Name { get; set; }

        public TagNode Node { get; set; }

        public static byte[] SerializeNode(TagNode node)
        {
            var root = new TagNodeCompound();
            root.Add("root", node);
            var tree = new NbtTree(root);

            using (var ms = new MemoryStream())
            {
                tree.WriteTo(ms);
                var data = new byte[ms.Length];
                Array.Copy(ms.GetBuffer(), data, ms.Length);

                return data;
            }
        }

        public static TagNode DeserializeNode(byte[] data)
        {
            var tree = new NbtTree();
            using (var ms = new MemoryStream(data))
            {
                tree.ReadFrom(ms);
            }

            var root = tree.Root;
            if (root == null || !root.ContainsKey("root"))
                return null;

            return root["root"];
        }
    }
}