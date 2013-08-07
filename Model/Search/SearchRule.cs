using System;
using System.Collections.Generic;
using System.Text;
using Substrate.Nbt;

namespace NBTExplorer.Model.Search
{
    public abstract class SearchRule
    {
        public virtual string NodeDisplay { get; }

        public virtual bool Matches (TagCompoundDataNode container, List<TagDataNode> matchedNodes)
        {
            return false;
        }
    }

    public abstract class GroupRule : SearchRule
    {
        public List<SearchRule> Rules { get; set; }
    }

    public class UnionRule : GroupRule
    {
        public override string NodeDisplay
        {
            get { return "Match Any"; }
        }

        public override bool Matches (TagCompoundDataNode container, List<TagDataNode> matchedNodes)
        {
            foreach (var rule in Rules) {
                if (rule.Matches(container, matchedNodes))
                    return true;
            }

            return false;
        }
    }

    public class IntersectRule : GroupRule
    {
        public override string NodeDisplay
        {
            get { return "Match All"; }
        }

        public override bool Matches (TagCompoundDataNode container, List<TagDataNode> matchedNodes)
        {
            foreach (var rule in Rules) {
                if (!rule.Matches(container, matchedNodes))
                    return false;
            }

            return true;
        }
    }

    public class RootRule : IntersectRule
    {
        public override string NodeDisplay
        {
            get { return "Search Rules"; }
        }
    }

    public abstract class TagRule : SearchRule
    {
        public TagType TagType { get; set; }
        public string Name { get; set; }

        protected T LookupTag<T> (TagCompoundDataNode container, string name)
            where T : TagNode
        {
            return container.NamedTagContainer.GetTagNode(name) as T;
        }
    }

    public abstract class IntegralTagRule<T> : TagRule
        where T : TagNode
    {
        public long Value { get; set; }

        public override string NodeDisplay
        {
            get { return string.Format("{0}: {1}", Name, Value); }
        }

        public override bool Matches (TagCompoundDataNode container, List<TagDataNode> matchedNodes)
        {
            T data = LookupTag<T>(container, Name);
            return (data != null && data.ToTagLong() == Value);
        }
    }

    public class ByteTagRule : IntegralTagRule<TagNodeByte>
    { }

    public class ShortTagRule : IntegralTagRule<TagNodeShort>
    { }

    public class IntTagRule : IntegralTagRule<TagNodeInt>
    { }

    public class LongTagRule : IntegralTagRule<TagNodeInt>
    { }

    public abstract class FloatTagRule<T> : TagRule
        where T : TagNode
    {
        public double Value { get; set; }

        public override string NodeDisplay
        {
            get { return string.Format("{0}: {1}", Name, Value); }
        }

        public override bool Matches (TagCompoundDataNode container, List<TagDataNode> matchedNodes)
        {
            T data = LookupTag<T>(container, Name);
            return (data != null && data.ToTagDouble() == Value);
        }
    }

    public class FloatTagRule : FloatTagRule<TagNodeFloat>
    { }

    public class DoubleTagRule : FloatTagRule<TagNodeDouble>
    { }

    public class StringTagRule : TagRule
    {
        public string Value { get; set; }

        public override string NodeDisplay
        {
            get { return string.Format("{0}: {1}", Name, Value); }
        }

        public override bool Matches (TagCompoundDataNode container, List<TagDataNode> matchedNodes)
        {
            TagNodeString data = LookupTag<TagNodeString>(container, Name);
            return (data != null && data.ToTagString() == Value);
        }
    }

    public class WildcardRule : SearchRule
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public override string NodeDisplay
        {
            get { return string.Format("{0}: {1}", Name, Value); }
        }

        public override bool Matches (TagCompoundDataNode container, List<TagDataNode> matchedNodes)
        {
            TagDataNode childNode = GetChild(container, Name);
            TagNode tag = container.NamedTagContainer.GetTagNode(Name);
            if (tag == null)
                return false;

            try {
                switch (tag.GetTagType()) {
                    case TagType.TAG_BYTE:
                    case TagType.TAG_INT:
                    case TagType.TAG_LONG:
                    case TagType.TAG_SHORT:
                        if (long.Parse(Value) == tag.ToTagLong()) {
                            if (!matchedNodes.Contains(childNode))
                                matchedNodes.Add(childNode);
                            return true;
                        }
                        break;
                    case TagType.TAG_FLOAT:
                    case TagType.TAG_DOUBLE:
                        if (double.Parse(Value) == tag.ToTagDouble()) {
                            if (!matchedNodes.Contains(childNode))
                                matchedNodes.Add(childNode);
                        }
                        break;
                    case TagType.TAG_STRING:
                        if (Value == tag.ToTagString()) {
                            if (!matchedNodes.Contains(childNode))
                                matchedNodes.Add(childNode);
                        }
                        break;
                }
            }
            catch { }

            

            return false;
        }

        private TagDataNode GetChild (TagCompoundDataNode container, string name)
        {
            foreach (var child in container.Nodes) {
                TagDataNode tagChild = child as TagDataNode;
                if (tagChild != null && tagChild.NodeName == name)
                    return tagChild;
            }

            return null;
        }
    }
}
