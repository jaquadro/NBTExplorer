using System;
using Substrate;

namespace NBToolkit
{
    

    public abstract class TKFilter
    {
        //public abstract void ApplyChunk (NBT_Tree root);

        public abstract void Run ();

        public INBTWorld GetWorld (TKOptions opt)
        {
            INBTWorld world = null;
            try {
                if (opt.OPT_ALPHA) {
                    world = AlphaWorld.Open(opt.OPT_WORLD);
                }
                else {
                    world = BetaWorld.Open(opt.OPT_WORLD);
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
