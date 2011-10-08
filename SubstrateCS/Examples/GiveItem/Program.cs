using System;
using Substrate;

// This example will insert x amount of an item into a player's
// inventory in an SMP server (where there is a player directory)

namespace GiveItem
{
    class Program
    {
        static void Main (string[] args)
        {
            if (args.Length != 4) {
                Console.WriteLine("Usage: GiveItem <world> <player> <item-id> <cnt>");
                return;
            }

            string dest = args[0];
            string player = args[1];
            int itemid = Convert.ToInt32(args[2]);
            int count = Convert.ToInt32(args[3]);

            // Open the world and grab its player manager
            BetaWorld world = BetaWorld.Open(dest);
            PlayerManager pm = world.GetPlayerManager();

            // Check that the named player exists
            if (!pm.PlayerExists(player)) {
                Console.WriteLine("No such player {0}!", player);
                return;
            }

            // Get player (returned object is independent of the playermanager)
            Player p = pm.GetPlayer(player);
            
            // Find first slot to place item
            for (int i = 0; i < p.Items.Capacity; i++) {
                if (!p.Items.ItemExists(i)) {
                    // Create the item and set its stack count
                    Item item = new Item(itemid);
                    item.Count = count;
                    p.Items[i] = item;

                    // Don't keep adding items
                    break;
                }
            }

            // Save the player
            pm.SetPlayer(player, p);
        }
    }
}
