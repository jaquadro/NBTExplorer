using System;
using Substrate;

// MoveSpawn changes the location of the world spawn location
// (which is separate from individual player spawn locations)

namespace MoveSpawn
{
    class Program
    {
        static void Main (string[] args)
        {
            if (args.Length != 4) {
                Console.WriteLine("Usage: MoveSpawn <world> <x> <y> <z>");
                return;
            }

            string dest = args[0];
            int x = Convert.ToInt32(args[1]);
            int y = Convert.ToInt32(args[2]);
            int z = Convert.ToInt32(args[3]);

            // Open our world
            BetaWorld world = BetaWorld.Open(dest);

            // Set the level's spawn
            // Note: Players do not have separate spawns by default
            // If you wanted to change a player's spawn, you must set all
            // 3 coordinates for it to stick.  It will not take the level's defaults.
            world.Level.Spawn = new SpawnPoint(x, y, z);

            // Save the changes
            world.Save();
        }
    }
}
