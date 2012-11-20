using System;
using System.Collections.Generic;
using Substrate.Nbt;

namespace NBTExplorer.Model
{
    public class CompoundTagContainer : INamedTagContainer
    {
        private TagNodeCompound _tag;

        public CompoundTagContainer (TagNodeCompound tag)
        {
            _tag = tag;
        }

        public int TagCount
        {
            get { return _tag.Count; }
        }

        public IEnumerable<string> TagNamesInUse
        {
            get { return _tag.Keys; }
        }

        public string GetTagName (TagNode tag)
        {
            foreach (String name in _tag.Keys)
                if (_tag[name] == tag)
                    return name;

            return null;
        }

        public TagNode GetTagNode (string name)
        {
            TagNode tag;
            if (_tag.TryGetValue(name, out tag))
                return tag;
            return null;
        }

        public bool AddTag (TagNode tag, string name)
        {
            if (_tag.ContainsKey(name))
                return false;

            _tag.Add(name, tag);
            return true;
        }

        public bool RenameTag (TagNode tag, string name)
        {
            if (_tag.ContainsKey(name))
                return false;

            string oldName = GetTagName(tag);
            _tag.Remove(oldName);
            _tag.Add(name, tag);

            return true;
        }

        public bool DeleteTag (TagNode tag)
        {
            foreach (String name in _tag.Keys)
                if (_tag[name] == tag)
                    return _tag.Remove(name);

            return false;
        }

        public bool DeleteTag (string name)
        {
            if (!_tag.ContainsKey(name))
                return false;

            return DeleteTag(_tag[name]);
        }

        public bool ContainsTag (string name)
        {
            return _tag.ContainsKey(name);
        }
    }
}
