using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.ObjCRuntime;

namespace NBTExplorer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        //[STAThread]
        static void Main (string[] args)
        {
			NSApplication.Init ();
			NSApplication.Main (args);

            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            ///Application.Run(new MainForm());
        }

        /*public static void StaticInitFailure (Exception e)
        {
            Console.WriteLine("Static Initialization Failure:");

            Exception original = e;
            while (e != null) {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                e = e.InnerException;
            }

            MessageBox.Show("Application failed during static initialization: " + original.Message);
            Application.Exit();
        }*/
    }
}
