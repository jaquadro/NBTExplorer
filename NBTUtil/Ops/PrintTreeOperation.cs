using NBTExplorer.Model;
using System;

namespace NBTUtil.Ops
{
    internal class PrintTreeOperation : ConsoleOperation
    {
        public override bool CanProcess(DataNode dataNode)
        {
            return true;
        }

        public override bool Process(DataNode dataNode, ConsoleOptions options)
        {
            PrintSubTree(dataNode, options, "", true);

            return true;
        }

        private void PrintSubTree(DataNode dataNode, ConsoleOptions options, string indent, bool last)
        {
            Console.WriteLine(indent + " + " + TypePrinter.Print(dataNode, options.ShowTypes));

            indent += last ? "  " : " |";
            var cnt = 0;

            dataNode.Expand();
            foreach (var child in dataNode.Nodes)
            {
                cnt++;
                PrintSubTree(child, options, indent, cnt == dataNode.Nodes.Count);
            }
        }
    }
}