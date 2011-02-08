using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace TestApp
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //NBT.World world = new NBT.World("C:\\Users\\Justin\\AppData\\Roaming\\.minecraft\\saves\\World4");
            NBT.World world = new NBT.World("F:\\Minecraft\\tps - Copy (5)");

            Form1 form = new Form1();
            form.world = world;
            world.LoadStep += form.updateProgressBar;
            world.Loaded += form.PostLoadMap;

            //form.chunk = chunk;

            /*world.Initialize();

            NBT.NBTChunk chunk = new NBT.NBTChunk("C:\\Users\\Justin\\AppData\\Roaming\\.minecraft\\saves\\World4\\0\\0", "c.0.0.dat");
            chunk.LoadChunk();
            chunk.ActivateChunk();*/

            

            Application.Run(form);
        }
    }
}
