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
    }
}
