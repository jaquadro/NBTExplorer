using System;
using System.Collections.Generic;
using System.Text;
using NBTExplorer.Model;

namespace NBTUtil.Ops
{
    class EditOperation : ConsoleOperation
    {
        public override bool OptionsValid (ConsoleOptions options)
        {
            if (options.Values.Count == 0)
                return false;
            return true;
        }

        public override bool CanProcess (DataNode dataNode)
        {
            if (!(dataNode is TagDataNode) || !dataNode.CanEditNode)
                return false;
            if (dataNode is TagByteArrayDataNode || dataNode is TagIntArrayDataNode || dataNode is TagShortArrayDataNode || dataNode is TagLongArrayDataNode)
                return false;

            return true;
        }

        public override bool Process (DataNode dataNode, ConsoleOptions options)
        {
            string value = options.Values[0];

            TagDataNode tagDataNode = dataNode as TagDataNode;
            return tagDataNode.Parse(value);
        }
    }
}
