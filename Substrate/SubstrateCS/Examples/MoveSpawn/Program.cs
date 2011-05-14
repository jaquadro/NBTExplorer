using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Substrate;

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

            BetaWorld world = BetaWorld.Open(dest);

            world.Level.SpawnX = x;
            world.Level.SpawnY = y;
            world.Level.SpawnZ = z;

            world.Save();
        }
    }
}
