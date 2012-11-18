using System;

namespace NBTExplorer.Model
{
    [Flags]
    public enum NodeCapabilities
    {
        None = 0,
        Cut = 0x1,
        Copy = 0x2,
        PasteInto = 0x4,
        Rename = 0x8,
        Edit = 0x10,
        Delete = 0x20,
        CreateTag = 0x40,
        Search = 0x80,
        Reorder = 0x100,
        Refresh = 0x200,
    }

    [Flags]
    public enum GroupCapabilities
    {
        Single = 0x0,
        SiblingSameType = 0x1,
        SiblingMixedType = 0x2 | SiblingSameType,
        MultiSameType = 0x4 | SiblingSameType,
        MultiMixedType = 0x8 | MultiSameType | SiblingMixedType,
        ElideChildren = 0x10,
        All = MultiMixedType | ElideChildren,
    }
}
