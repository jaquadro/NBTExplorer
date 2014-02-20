using System;
using System.Collections.Generic;
using System.Text;
using Substrate.Nbt;

namespace NBTExplorer.Model.Search
{
    public enum NumericOperator
    {
        Equals,
        NotEquals,
        GreaterThan,
        LessThan,
        Any,
    }

    public enum StringOperator
    {
        Equals,
        NotEquals,
        Contains,
        NotContains,
        StartsWith,
        EndsWith,
        Any,
    }

    public enum WildcardOperator
    {
        Equals,
        NotEquals,
        Any,
    }

    public abstract class SearchRule
    {
        public static readonly Dictionary<NumericOperator, string> NumericOpStrings = new Dictionary<NumericOperator, string>() {
            { NumericOperator.Equals, "=" },
            { NumericOperator.NotEquals, "!=" },
            { NumericOperator.GreaterThan, ">" },
            { NumericOperator.LessThan, "<" },
            { NumericOperator.Any, "ANY" },
        };

        public static readonly Dictionary<StringOperator, string> StringOpStrings = new Dictionary<StringOperator, string>() {
            { StringOperator.Equals, "=" },
            { StringOperator.NotEquals, "!=" },
            { StringOperator.Contains, "Contains" },
            { StringOperator.NotContains, "Does not Contain" },
            { StringOperator.StartsWith, "Begins With" },
            { StringOperator.EndsWith, "Ends With" },
            { StringOperator.Any, "ANY" },
        };

        public static readonly Dictionary<WildcardOperator, string> WildcardOpStrings = new Dictionary<WildcardOperator, string>() {
            { WildcardOperator.Equals, "=" },
            { WildcardOperator.NotEquals, "!=" },
            { WildcardOperator.Any, "ANY" },
        };

        public abstract string NodeDisplay { get; }

        public virtual bool Matches (TagCompoundDataNode container, List<TagDataNode> matchedNodes)
        {
            return false;
        }

        public virtual bool CanAddRules
        {
            get { return false; }
        }

        protected static TagDataNode GetChild (TagCompoundDataNode container, string name)
        {
            foreach (var child in container.Nodes) {
                TagDataNode tagChild = child as TagDataNode;
                if (tagChild != null && tagChild.NodeName == name)
                    return tagChild;
            }

            return null;
        }
    }

    public abstract class GroupRule : SearchRule
    {
        protected GroupRule ()
        {
            Rules = new List<SearchRule>();
        }

        public List<SearchRule> Rules { get; set; }

        public override bool CanAddRules
        {
            get { return true; }
        }
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

        public NumericOperator Operator { get; set; }

        public override string NodeDisplay
        {
            get { return string.Format("{0} {1} {2}", Name, NumericOpStrings[Operator], Operator != NumericOperator.Any ? Value.ToString() : ""); }
        }

        public override bool Matches (TagCompoundDataNode container, List<TagDataNode> matchedNodes)
        {
            TagDataNode childNode = GetChild(container, Name);
            T data = LookupTag<T>(container, Name);
            if (data == null)
                return false;

            switch (Operator) {
                case NumericOperator.Equals:
                    if (data.ToTagLong() != Value)
                        return false;
                    break;
                case NumericOperator.NotEquals:
                    if (data.ToTagLong() == Value)
                        return false;
                    break;
                case NumericOperator.GreaterThan:
                    if (data.ToTagLong() <= Value)
                        return false;
                    break;
                case NumericOperator.LessThan:
                    if (data.ToTagLong() >= Value)
                        return false;
                    break;
                case NumericOperator.Any:
                    break;
                default:
                    return false;
            }

            if (!matchedNodes.Contains(childNode))
                matchedNodes.Add(childNode);

            return true;
        }
    }

    public class ByteTagRule : IntegralTagRule<TagNodeByte>
    { }

    public class ShortTagRule : IntegralTagRule<TagNodeShort>
    { }

    public class IntTagRule : IntegralTagRule<TagNodeInt>
    { }

    public class LongTagRule : IntegralTagRule<TagNodeLong>
    { }

    public abstract class FloatTagRule<T> : TagRule
        where T : TagNode
    {
        public double Value { get; set; }

        public NumericOperator Operator { get; set; }

        public override string NodeDisplay
        {
            get { return string.Format("{0} {1} {2}", Name, NumericOpStrings[Operator], Operator != NumericOperator.Any ? Value.ToString() : ""); }
        }

        public override bool Matches (TagCompoundDataNode container, List<TagDataNode> matchedNodes)
        {
            TagDataNode childNode = GetChild(container, Name);
            T data = LookupTag<T>(container, Name);
            if (data == null)
                return false;

            switch (Operator) {
                case NumericOperator.Equals:
                    if (data.ToTagDouble() != Value)
                        return false;
                    break;
                case NumericOperator.NotEquals:
                    if (data.ToTagDouble() == Value)
                        return false;
                    break;
                case NumericOperator.GreaterThan:
                    if (data.ToTagDouble() <= Value)
                        return false;
                    break;
                case NumericOperator.LessThan:
                    if (data.ToTagDouble() >= Value)
                        return false;
                    break;
                case NumericOperator.Any:
                    break;
                default:
                    return false;
            }
            
            if (!matchedNodes.Contains(childNode))
                matchedNodes.Add(childNode);

            return true;
        }
    }

    public class FloatTagRule : FloatTagRule<TagNodeFloat>
    { }

    public class DoubleTagRule : FloatTagRule<TagNodeDouble>
    { }

    public class StringTagRule : TagRule
    {
        public string Value { get; set; }

        public StringOperator Operator { get; set; }

        public override string NodeDisplay
        {
            get { return string.Format("{0} {1} {2}", Name, StringOpStrings[Operator], Operator != StringOperator.Any ? '"' + Value + '"' : ""); }
        }

        public override bool Matches (TagCompoundDataNode container, List<TagDataNode> matchedNodes)
        {
            TagDataNode childNode = GetChild(container, Name);
            TagNodeString data = LookupTag<TagNodeString>(container, Name);
            if (data == null)
                return false;

            switch (Operator) {
                case StringOperator.Equals:
                    if (data.ToTagString().Data != Value)
                        return false;
                    break;
                case StringOperator.NotEquals:
                    if (data.ToTagString().Data == Value)
                        return false;
                    break;
                case StringOperator.Contains:
                    if (!data.ToTagString().Data.Contains(Value))
                        return false;
                    break;
                case StringOperator.NotContains:
                    if (data.ToTagString().Data.Contains(Value))
                        return false;
                    break;
                case StringOperator.StartsWith:
                    if (!data.ToTagString().Data.StartsWith(Value))
                        return false;
                    break;
                case StringOperator.EndsWith:
                    if (!data.ToTagString().Data.EndsWith(Value))
                        return false;
                    break;
                case StringOperator.Any:
                    break;
                default:
                    return false;
            }

            if (!matchedNodes.Contains(childNode))
                matchedNodes.Add(childNode);

            return true;
        }
    }

    public class WildcardRule : SearchRule
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public WildcardOperator Operator { get; set; }

        public override string NodeDisplay
        {
            get { return string.Format("{0} {1} {2}", Name, WildcardOpStrings[Operator], Operator != WildcardOperator.Any ? Value : ""); }
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
                        switch (Operator) {
                            case WildcardOperator.Equals:
                                if (long.Parse(Value) != tag.ToTagLong())
                                    return false;
                                break;
                            case WildcardOperator.NotEquals:
                                if (long.Parse(Value) == tag.ToTagLong())
                                    return false;
                                break;
                        }

                        if (!matchedNodes.Contains(childNode))
                            matchedNodes.Add(childNode);
                        return true;
                    case TagType.TAG_FLOAT:
                    case TagType.TAG_DOUBLE:
                        switch (Operator) {
                            case WildcardOperator.Equals:
                                if (double.Parse(Value) != tag.ToTagDouble())
                                    return false;
                                break;
                            case WildcardOperator.NotEquals:
                                if (double.Parse(Value) == tag.ToTagDouble())
                                    return false;
                                break;
                        }
                        
                        if (!matchedNodes.Contains(childNode))
                            matchedNodes.Add(childNode);
                        return true;
                    case TagType.TAG_STRING:
                        switch (Operator) {
                            case WildcardOperator.Equals:
                                if (Value != tag.ToTagString().Data)
                                    return false;
                                break;
                            case WildcardOperator.NotEquals:
                                if (Value == tag.ToTagString().Data)
                                    return false;
                                break;
                        }

                        if (!matchedNodes.Contains(childNode))
                            matchedNodes.Add(childNode);
                        return true;
                }
            }
            catch { }

            return false;
        }

        
    }
}
