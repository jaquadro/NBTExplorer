using System;
using Substrate;

namespace NBToolkit
{
    

    public abstract class TKFilter
    {
        //public abstract void ApplyChunk (NBT_Tree root);

        public abstract void Run ();

        public NBTWorld GetWorld (TKOptions opt)
        {
            NBTWorld world = null;
            try {
                if (opt.OPT_ALPHA) {
                    world = new AlphaWorld(opt.OPT_WORLD, opt.OPT_DIM);
                }
                else {
                    world = new BetaWorld(opt.OPT_WORLD, opt.OPT_REGION, opt.OPT_DIM);
                }
            }
            catch (Exception ex) {
                Console.WriteLine("Error: " + ex.Message);
                Environment.Exit(1);
            }

            return world;
        }
    }
}
