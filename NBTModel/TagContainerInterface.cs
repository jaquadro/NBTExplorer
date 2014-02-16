using System.Collections.Generic;
using Substrate.Nbt;

namespace NBTExplorer.Model
{
    public interface ITagContainer
    {
        int TagCount { get; }

        bool DeleteTag (TagNode tag);
    }

    public interface IMetaTagContainer : ITagContainer
    {
        bool IsNamedContainer { get; }
        bool IsOrderedContainer { get; }

        INamedTagContainer NamedTagContainer { get; }
        IOrderedTagContainer OrderedTagContainer { get; }
    }

    public interface INamedTagContainer : ITagContainer
    {
        IEnumerable<string> TagNamesInUse { get; }

        string GetTagName (TagNode tag);
        TagNode GetTagNode (string name);

        bool AddTag (TagNode tag, string name);
        bool RenameTag (TagNode tag, string name);
        bool ContainsTag (string name);
        bool DeleteTag (string name);
    }

    public interface IOrderedTagContainer : ITagContainer
    {
        int GetTagIndex (TagNode tag);
        bool InsertTag (TagNode tag, int index);
        bool AppendTag (TagNode tag);
    }
}
