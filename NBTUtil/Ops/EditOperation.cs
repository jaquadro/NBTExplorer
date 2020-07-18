using NBTExplorer.Model;

namespace NBTUtil.Ops
{
    internal class EditOperation : ConsoleOperation
    {
        public override bool OptionsValid(ConsoleOptions options)
        {
            if (options.Values.Count == 0)
                return false;
            return true;
        }

        public override bool CanProcess(DataNode dataNode)
        {
            if (!(dataNode is TagDataNode) || !dataNode.CanEditNode)
                return false;
            if (dataNode is TagByteArrayDataNode || dataNode is TagIntArrayDataNode ||
                dataNode is TagShortArrayDataNode || dataNode is TagLongArrayDataNode)
                return false;

            return true;
        }

        public override bool Process(DataNode dataNode, ConsoleOptions options)
        {
            var value = options.Values[0];

            var tagDataNode = dataNode as TagDataNode;
            return tagDataNode.Parse(value);
        }
    }
}