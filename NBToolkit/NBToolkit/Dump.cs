using System;
using System.Collections.Generic;
using System.Text;

namespace NBToolkit
{
    using Map;
    using Map.NBT;
    using System.IO;

    class Dump : TKFilter
    {
        private ReplaceOptions opt;

        public Dump (ReplaceOptions o)
        {
            opt = o;
        }

        public override void Run ()
        {
            World world = new World(opt.OPT_WORLD);

            StreamWriter fstr = new StreamWriter("json.txt", false);

            foreach (ChunkRef chunk in new FilteredChunkList(world.GetChunkManager(), opt.GetChunkFilter())) {
                string s = JSONSerializer.Serialize(chunk.GetChunkRef().Tree.Root["Level"].ToNBTCompound()["TileEntities"]);
                fstr.Write(s);
            }

            fstr.Close();
        }
    }
}
