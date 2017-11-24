using System;
using System.Collections.Generic;
using System.Text;
using NBTExplorer.Model;

namespace NBTUtil
{
    static class TypePrinter
    {
        private static Dictionary<Type, string> _key = new Dictionary<Type, string>() {
            { typeof(TagByteDataNode), "b" },
            { typeof(TagShortDataNode), "s" },
            { typeof(TagIntDataNode), "i" },
            { typeof(TagLongDataNode), "l" },
            { typeof(TagFloatDataNode), "f" },
            { typeof(TagDoubleDataNode), "d" },
            { typeof(TagStringDataNode), "T" },
            { typeof(TagByteArrayDataNode), "B" },
            { typeof(TagIntArrayDataNode), "I" },
            { typeof(TagShortArrayDataNode), "S" },
            { typeof(TagLongArrayDataNode), "L" },
            { typeof(TagListDataNode), "L" },
            { typeof(TagCompoundDataNode), "C" },
            { typeof(NbtFileDataNode), "N" },
            { typeof(RegionFileDataNode), "R" },
            { typeof(RegionChunkDataNode), "r" },
            { typeof(CubicRegionDataNode), "R" },
            { typeof(DirectoryDataNode), "/" },
        };

        public static string Print (DataNode node, bool showType)
        {
            if (!_key.ContainsKey(node.GetType()))
                return "";

            if (showType)
                return "<" + _key[node.GetType()] + "> " + node.NodeDisplay;
            else
                return node.NodeDisplay;
        }
    }
}
