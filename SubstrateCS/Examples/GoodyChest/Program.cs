using System;
using Substrate;
using Substrate.TileEntities;

// GoodyChest is an example that creates chests filled with random
// items throughout the world, according to a probability of
// appearing per chunk.

// Note: This example picks a random item from Substrate's ItemTable,
// which includes all items up to the current version of MC (if Substrate
// itself is up to date).  If a chest gets filled with some of these
// latest items and gets opened in an older MC client, MC will crash.

namespace GoodyChest
{
    class Program
    {
        static Random rand;

        static void Main (string[] args)
        {
            if (args.Length != 2) {
                Console.WriteLine("Usage: GoodyChest <world> <prob>");
                return;
            }

            string dest = args[0];
            double p = Convert.ToDouble(args[1]);

            rand = new Random();

            // Open our world
            BetaWorld world = BetaWorld.Open(dest);
            BetaChunkManager cm = world.GetChunkManager();

            int added = 0;

            // Iterate through every chunk in the world
            // With proability p, pick a random location
            // inside the chunk to place a chest, above the
            // first solid block
            foreach (ChunkRef chunk in cm) {
                if (rand.NextDouble() < p) {
                    int x = rand.Next(chunk.Blocks.XDim);
                    int z = rand.Next(chunk.Blocks.ZDim);
                    int y = chunk.Blocks.GetHeight(x, z);

                    // Can't build this high (-2 to account for new MC 1.6 height limitation)
                    if (y >= chunk.Blocks.YDim - 2) {
                        continue;
                    }

                    // Get a block object, then assign it to the chunk
                    AlphaBlock block = BuildChest();
                    chunk.Blocks.SetBlock(x, y + 1, z, block);

                    // Save the chunk
                    cm.Save();

                    added++;
                }
            }

            // And we're done
            Console.WriteLine("Added {0} goody chests to world", added);
        }

        // This function will create a new Block object of type 'Chest', fills it
        // with random items, and returns it
        static AlphaBlock BuildChest ()
        {
            // A default, appropriate TileEntity entry is created
            AlphaBlock block = new AlphaBlock(BlockType.CHEST);
            TileEntityChest ent = block.GetTileEntity() as TileEntityChest;

            // Unless Substrate has a bug, the TileEntity was definitely a TileEntityChest
            if (ent == null) {
                Console.WriteLine("Catastrophic");
                return null;
            }

            // Loop through each slot in the chest, assign an item
            // with a probability
            for (int i = 0; i < ent.Items.Capacity; i++) {
                if (rand.NextDouble() < 0.3) {
                    // Ask the ItemTable for a random Item type registered with Substrate
                    ItemInfo itype = ItemInfo.GetRandomItem();

                    // Create the item object, give it an appropriate, random count (items in stack)
                    Item item = new Item(itype.ID);
                    item.Count = 1 + rand.Next(itype.StackSize);

                    // Assign the item to the chest at slot i
                    ent.Items[i] = item;
                }
            }

            // That's all, we've got a loaded chest block ready to be
            // inserted into a chunk
            return block;
        }
    }
}
