using NBTExplorer.Model;
using Substrate.Nbt;
using System.IO;

namespace NBTUtil.Ops
{
    internal class JsonOperation : ConsoleOperation
    {
        public override bool CanProcess(DataNode dataNode)
        {
            return dataNode is NbtFileDataNode || dataNode is TagDataNode;
        }

        public override bool Process(DataNode dataNode, ConsoleOptions options)
        {
            if (options.Values.Count == 0)
                return false;

            var jsonPath = options.Values[0];
            using (var stream = File.OpenWrite(jsonPath))
            {
                using (var writer = new StreamWriter(stream))
                {
                    if (dataNode is TagDataNode)
                    {
                        var tagNode = dataNode as TagDataNode;
                        WriteNbtTag(writer, tagNode.Tag);
                    }
                    else if (dataNode is NbtFileDataNode)
                    {
                        dataNode.Expand();
                        var root = new TagNodeCompound();

                        foreach (var child in dataNode.Nodes)
                        {
                            var childTagNode = child as TagDataNode;
                            if (childTagNode == null)
                                continue;

                            if (childTagNode.Tag != null)
                                root.Add(childTagNode.NodeName, childTagNode.Tag);
                        }

                        WriteNbtTag(writer, root);
                    }
                }
            }

            return true;
        }

        private void WriteNbtTag(StreamWriter writer, TagNode tag)
        {
            if (tag == null)
                return;

            writer.Write(JSONSerializer.Serialize(tag));
        }
    }
}