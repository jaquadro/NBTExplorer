using System;
using System.Collections.Generic;
using System.Text;

namespace NBToolkit.Map
{
    using NBT;

    public class ChunkVerifier : NBTVerifier
    {
        protected static NBTCompoundNode levelSchema = new NBTCompoundNode()
        {
            new NBTCompoundNode("Level")
            {
                new NBTArrayNode("Blocks", 32768),
                new NBTArrayNode("Data", 16384),
                new NBTArrayNode("SkyLight", 16384),
                new NBTArrayNode("BlockLight", 16384),
                new NBTArrayNode("HeightMap", 256),
                new NBTListNode("Entities", NBT_Type.TAG_COMPOUND),
                new NBTListNode("TileEntities", NBT_Type.TAG_COMPOUND),
                new NBTScalerNode("LastUpdate", NBT_Type.TAG_LONG),
                new NBTScalerNode("xPos", NBT_Type.TAG_INT),
                new NBTScalerNode("zPos", NBT_Type.TAG_INT),
                new NBTScalerNode("TerrainPopulated", NBT_Type.TAG_BYTE),
            },
        };

        public ChunkVerifier (NBT_Tree tree)
            : base(tree.Root, levelSchema)
        {
        }
    }
}
