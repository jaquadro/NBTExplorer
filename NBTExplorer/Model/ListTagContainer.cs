using Substrate.Nbt;

namespace NBTExplorer.Model
{
    public class ListTagContainer : IOrderedTagContainer
    {
        private TagNodeList _tag;

        public ListTagContainer (TagNodeList tag)
        {
            _tag = tag;
        }

        public int TagCount
        {
            get { return _tag.Count; }
        }

        public bool DeleteTag (TagNode tag)
        {
            return _tag.Remove(tag);
        }

        public int GetTagIndex (TagNode tag)
        {
            return _tag.IndexOf(tag);
        }

        public bool InsertTag (TagNode tag, int index)
        {
            if (index < 0 || index > _tag.Count)
                return false;

            _tag.Insert(index, tag);
            return true;
        }
    }
}
