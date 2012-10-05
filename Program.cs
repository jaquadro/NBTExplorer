using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace NBTExplorer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main ()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }

        public static void StaticInitFailure (Exception e)
        {
            Console.WriteLine("Static Initialization Failure:");
            while (e != null) {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                e = e.InnerException;
            }

            Application.Exit();
        }
    }
}
