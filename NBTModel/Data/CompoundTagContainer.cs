using Substrate.Nbt;
using System.Collections.Generic;

namespace NBTExplorer.Model
{
    public class CompoundTagContainer : INamedTagContainer
    {
        private readonly TagNodeCompound _tag;

        public CompoundTagContainer(TagNodeCompound tag)
        {
            _tag = tag;
        }

        public int TagCount => _tag.Count;

        public IEnumerable<string> TagNamesInUse => _tag.Keys;

        public string GetTagName(TagNode tag)
        {
            foreach (var name in _tag.Keys)
                if (_tag[name] == tag)
                    return name;

            return null;
        }

        public TagNode GetTagNode(string name)
        {
            TagNode tag;
            if (_tag.TryGetValue(name, out tag))
                return tag;
            return null;
        }

        public bool AddTag(TagNode tag, string name)
        {
            if (_tag.ContainsKey(name))
                return false;

            _tag.Add(name, tag);
            return true;
        }

        public bool RenameTag(TagNode tag, string name)
        {
            if (_tag.ContainsKey(name))
                return false;

            var oldName = GetTagName(tag);
            _tag.Remove(oldName);
            _tag.Add(name, tag);

            return true;
        }

        public bool DeleteTag(TagNode tag)
        {
            foreach (var name in _tag.Keys)
                if (_tag[name] == tag)
                    return _tag.Remove(name);

            return false;
        }

        public bool DeleteTag(string name)
        {
            if (!_tag.ContainsKey(name))
                return false;

            return DeleteTag(_tag[name]);
        }

        public bool ContainsTag(string name)
        {
            return _tag.ContainsKey(name);
        }
    }
}