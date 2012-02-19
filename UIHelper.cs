using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Substrate;
using Substrate.Nbt;

namespace NBTExplorer
{
    public class UIHelper
    {

        internal static ItemInfo TryGetItemName(TagNode tag)
        {

            var tagc = tag as TagNodeCompound;
            if (tagc.ContainsKey("id") == false)
                return null;
                int id;
                if (int.TryParse(tagc["id"].ToString(), out id))
                {
                    return Substrate.ItemInfo.ItemTable[id];
                }
                return null;
            
            
        }
    }
}
