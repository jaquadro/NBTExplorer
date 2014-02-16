using System;
using System.Collections.Generic;
using System.Text;
using NBTExplorer.Model;

namespace NBTUtil.Ops
{
    class PrintOperation : ConsoleOperation
    {
        public override bool CanProcess (DataNode dataNode)
        {
            return true;
        }

        public override bool Process (DataNode dataNode, ConsoleOptions options)
        {
            Console.WriteLine(TypePrinter.Print(dataNode, options.ShowTypes));

            if (dataNode.IsContainerType) {
                foreach (var child in dataNode.Nodes)
                    Console.WriteLine(" | " + TypePrinter.Print(child, options.ShowTypes));
            }

            return true;
        }
    }
}
