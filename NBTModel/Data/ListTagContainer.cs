using System;
using Substrate.Nbt;

namespace NBTExplorer.Model
{
    public class ListTagContainer : IOrderedTagContainer
    {
        private TagNodeList _tag;
        private Action<bool> _modifyHandler;

        public ListTagContainer (TagNodeList tag, Action<bool> modifyHandler)
        {
            _tag = tag;
        }

        public int TagCount
        {
            get { return _tag.Count; }
        }

        public bool DeleteTag (TagNode tag)
        {
            bool result = _tag.Remove(tag);
            if (result)
                SetModified();

            return result;
        }

        public int GetTagIndex (TagNode tag)
        {
            return _tag.IndexOf(tag);
        }

        public bool InsertTag (TagNode tag, int index)
        {
            if (index < 0 || index > _tag.Count)
                return false;

            if (_tag.ValueType != tag.GetTagType())
                return false;

            _tag.Insert(index, tag);

            SetModified();
            return true;
        }

        public bool AppendTag (TagNode tag)
        {
            if (_tag.ValueType != tag.GetTagType())
                return false;

            _tag.Add(tag);

            SetModified();
            return true;
        }

        private void SetModified ()
        {
            if (_modifyHandler != null)
                _modifyHandler(true);
        }
    }
}
