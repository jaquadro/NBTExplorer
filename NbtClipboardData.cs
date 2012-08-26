using System;
using System.IO;
using System.Windows.Forms;
using Substrate.Nbt;

namespace NBTExplorer
{
    [Serializable]
    public class NbtClipboardData
    {
        public string Name;

        private byte[] _data;

        [NonSerialized]
        public TagNode Node;

        public NbtClipboardData (String name, TagNode node)
        {
            Name = name;

            TagNodeCompound root = new TagNodeCompound();
            root.Add("root", node);
            NbtTree tree = new NbtTree(root);

            using (MemoryStream ms = new MemoryStream()) {
                tree.WriteTo(ms);
                _data = new byte[ms.Length];
                Array.Copy(ms.GetBuffer(), _data, ms.Length);
            }
        }

        public static bool ContainsData
        {
            get { return Clipboard.ContainsData(typeof(NbtClipboardData).FullName); }
        }

        public void CopyToClipboard ()
        {
            Clipboard.SetData(typeof(NbtClipboardData).FullName, this);
        }

        public static NbtClipboardData CopyFromClipboard ()
        {
            NbtClipboardData clip = Clipboard.GetData(typeof(NbtClipboardData).FullName) as NbtClipboardData;
            if (clip == null)
                return null;

            NbtTree tree = new NbtTree();
            using (MemoryStream ms = new MemoryStream(clip._data)) {
                tree.ReadFrom(ms);
            }

            TagNodeCompound root = tree.Root;
            if (root == null || !root.ContainsKey("root"))
                return null;

            clip.Node = root["root"];
            return clip;
        }
    }
}
