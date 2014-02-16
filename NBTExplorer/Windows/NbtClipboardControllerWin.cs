using System;
using System.Windows.Forms;
using NBTModel.Interop;
using Substrate.Nbt;

namespace NBTExplorer.Windows
{
    public class NbtClipboardControllerWin : INbtClipboardController
    {
        public bool ContainsData
        {
            get { return Clipboard.ContainsData(typeof(NbtClipboardDataWin).FullName); }
        }

        public void CopyToClipboard (NbtClipboardData data)
        {
            NbtClipboardDataWin dataWin = new NbtClipboardDataWin(data);
            Clipboard.SetData(typeof(NbtClipboardDataWin).FullName, dataWin);
        }

        public NbtClipboardData CopyFromClipboard ()
        {
            NbtClipboardDataWin clip = Clipboard.GetData(typeof(NbtClipboardDataWin).FullName) as NbtClipboardDataWin;
            if (clip == null)
                return null;

            TagNode node = clip.Node;
            if (node == null)
                return null;

            return new NbtClipboardData(clip.Name, node);
        }
    }

    [Serializable]
    public class NbtClipboardDataWin
    {
        private string _name;
        private byte[] _data;

        public NbtClipboardDataWin (NbtClipboardData data)
        {
            Name = data.Name;
            Node = data.Node;
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public TagNode Node
        {
            get { return NbtClipboardData.DeserializeNode(_data); }
            set { _data = NbtClipboardData.SerializeNode(value); }
        }
    }
}
