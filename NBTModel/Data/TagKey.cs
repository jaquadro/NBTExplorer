using System;
using Substrate.Nbt;

namespace NBTExplorer.Model
{
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

        public int Compare (TagKey x, TagKey y)
        {
            int typeDiff = (int)x.TagType - (int)y.TagType;
            if (typeDiff != 0)
                return typeDiff;

            return String.Compare(x.Name, y.Name, false);
        }

        #endregion

        #region IComparable<TagKey> Members

        public int CompareTo (TagKey other)
        {
            return Compare(this, other);
        }

        #endregion
    }
}
