using NBTExplorer.Model;

namespace NBTUtil.Ops
{
    internal class SetListOperation : ConsoleOperation
    {
        public override bool CanProcess(DataNode dataNode)
        {
            if (!(dataNode is TagListDataNode))
                return false;

            return true;
        }

        public override bool Process(DataNode dataNode, ConsoleOptions options)
        {
            var listNode = dataNode as TagListDataNode;

            listNode.Clear();
            foreach (var value in options.Values)
            {
                var tag = TagDataNode.DefaultTag(listNode.Tag.ValueType);
                var tagData = TagDataNode.CreateFromTag(tag);
                if (!tagData.Parse(value))
                    return false;

                if (!listNode.AppendTag(tagData.Tag))
                    return false;
            }

            return true;
        }
    }
}