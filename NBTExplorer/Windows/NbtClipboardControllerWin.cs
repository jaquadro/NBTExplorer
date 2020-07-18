using NBTModel.Interop;
using Substrate.Nbt;
using System;
using System.Windows.Forms;

namespace NBTExplorer.Windows
{
    public class NbtClipboardControllerWin : INbtClipboardController
    {
        public bool ContainsData => Clipboard.ContainsData(typeof(NbtClipboardDataWin).FullName);

        public void CopyToClipboard(NbtClipboardData data)
        {
            var dataWin = new NbtClipboardDataWin(data);
            Clipboard.SetData(typeof(NbtClipboardDataWin).FullName, dataWin);
        }

        public NbtClipboardData CopyFromClipboard()
        {
            var clip = Clipboard.GetData(typeof(NbtClipboardDataWin).FullName) as NbtClipboardDataWin;
            if (clip == null)
                return null;

            var node = clip.Node;
            if (node == null)
                return null;

            return new NbtClipboardData(clip.Name, node);
        }
    }

    [Serializable]
    public class NbtClipboardDataWin
    {
        private byte[] _data;
        private string _name;

        public NbtClipboardDataWin(NbtClipboardData data)
        {
            Name = data.Name;
            Node = data.Node;
        }

        public string Name
        {
            get => _name;
            set => _name = value;
        }

        public TagNode Node
        {
            get => NbtClipboardData.DeserializeNode(_data);
            set => _data = NbtClipboardData.SerializeNode(value);
        }
    }
}