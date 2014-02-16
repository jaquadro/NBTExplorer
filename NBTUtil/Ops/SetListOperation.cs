using System;
using System.Collections.Generic;
using System.Text;
using NBTExplorer.Model;
using Substrate.Nbt;

namespace NBTUtil.Ops
{
    class SetListOperation : ConsoleOperation
    {
        public override bool CanProcess (DataNode dataNode)
        {
            if (!(dataNode is TagListDataNode))
                return false;

            return true;
        }

        public override bool Process (DataNode dataNode, ConsoleOptions options)
        {
            TagListDataNode listNode = dataNode as TagListDataNode;

            listNode.Clear();
            foreach (string value in options.Values) {
                TagNode tag = TagDataNode.DefaultTag(listNode.Tag.ValueType);
                TagDataNode tagData = TagDataNode.CreateFromTag(tag);
                if (!tagData.Parse(value))
                    return false;

                if (!listNode.AppendTag(tagData.Tag))
                    return false;
            }

            return true;
        }
    }
}
