using System;
using System.Collections.Generic;
using System.IO;
using NBTExplorer.Model;

namespace NBTUtil.Ops
{
    class DeleteOperation : ConsoleOperation
    {
        public override bool OptionsValid (ConsoleOptions options)
        {
            return true;
        }

        public override bool CanProcess (DataNode dataNode)
        {
            return (dataNode != null) && dataNode.CanDeleteNode && (dataNode.Root != dataNode);
        }

        public override bool Process (DataNode dataNode, ConsoleOptions options)
        {
            return dataNode.DeleteNode();
        }
    }
}
