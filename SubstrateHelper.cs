using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Substrate.Nbt;
using Substrate;

namespace NBTExplorer
{
    public class SubstrateHelper
    {

        public static  void LoadWorld(string path)
        {
            NbtWorld world = NbtWorld.Open(path);

            BetaWorld beta = world as BetaWorld;
            if (beta != null)
            {
                LoadBetaWorld(beta);
            }
        }

        private static void LoadBetaWorld(BetaWorld world)
        {
            RegionManager rm = world.GetRegionManager();
        }

        public static  string GetNodeText(string name, string value)
        {
            name = String.IsNullOrEmpty(name) ? "" : name + ": ";
            value = value ?? "";

            return name + value;
        }

        public static string GetNodeText(string name, TagNode tag)
        {
            return GetNodeText(name,SubstrateHelper. GetTagNodeText(tag));
        }
        public static string GetTagNodeText(TagNode tag)
        {
            if (tag == null)
                return null;

            switch (tag.GetTagType())
            {
                case TagType.TAG_BYTE:
                case TagType.TAG_SHORT:
                case TagType.TAG_INT:
                case TagType.TAG_LONG:
                case TagType.TAG_FLOAT:
                case TagType.TAG_DOUBLE:
                case TagType.TAG_STRING:
                    return tag.ToString();

                case TagType.TAG_BYTE_ARRAY:
                    return tag.ToTagByteArray().Length + " bytes";

                case TagType.TAG_LIST:
                    return tag.ToTagList().Count + " entries";

                case TagType.TAG_COMPOUND:
                    return tag.ToTagCompound().Count + " entries";
            }

            return null;
        }
    }
}
