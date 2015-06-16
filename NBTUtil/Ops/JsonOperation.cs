using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NBTExplorer.Model;
using Substrate.Nbt;

namespace NBTUtil.Ops
{
    class JsonOperation : ConsoleOperation
    {
        public override bool CanProcess (DataNode dataNode)
        {
            return dataNode is NbtFileDataNode || dataNode is TagDataNode;
        }

        public override bool Process (DataNode dataNode, ConsoleOptions options)
        {
            if (options.Values.Count == 0)
                return false;

            string jsonPath = options.Values[0];
            using (FileStream stream = File.OpenWrite(jsonPath)) {
                using (StreamWriter writer = new StreamWriter(stream)) {
                    if (dataNode is TagDataNode) {
                        TagDataNode tagNode = dataNode as TagDataNode;
                        WriteNbtTag(writer, tagNode.Tag);
                    }
                    else if (dataNode is NbtFileDataNode) {
                        dataNode.Expand();
                        TagNodeCompound root = new TagNodeCompound();

                        foreach (DataNode child in dataNode.Nodes) {
                            TagDataNode childTagNode = child as TagDataNode;
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

        private void WriteNbtTag (StreamWriter writer, TagNode tag)
        {
            if (tag == null)
                return;

            writer.Write(JSONSerializer.Serialize(tag));
        }
    }
}
