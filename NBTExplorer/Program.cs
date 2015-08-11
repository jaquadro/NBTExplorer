using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using NBTExplorer.Windows;

namespace NBTExplorer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main (string[] args)
        {
            Application.ThreadException += AppThreadFailureHandler;
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

            AppDomain.CurrentDomain.UnhandledException += AppDomainFailureHandler;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }

        public static void StaticInitFailure (Exception e)
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
        }

        private static void AppThreadFailureHandler (object sender, ThreadExceptionEventArgs e)
        {
            ProcessException(e.Exception);
        }

        private static void AppDomainFailureHandler (object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception)
                ProcessException(e.ExceptionObject as Exception);
            else if (e.IsTerminating) {
                MessageBox.Show("NBTExplorer encountered an unknown exception object: " + e.ExceptionObject.GetType().FullName,
                    "NBTExplorer failed to run", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }

        private static void ProcessException (Exception ex)
        {
            if (IsMissingSubstrate(ex)) {
                MessageBox.Show("NBTExplorer could not find required assembly \"Substrate.dll\".\n\nIf you obtained NBTExplorer from a ZIP distribution, make sure you've extracted NBTExplorer and all of its supporting files into another directory before running it.", 
                    "NBTExplorer failed to run", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
                return;
            }

            if (IsMissingNBTModel(ex)) {
                MessageBox.Show("NBTExplorer could not find required assembly \"NBTModel.dll\".\n\nIf you obtained NBTExplorer from a ZIP distribution, make sure you've extracted NBTExplorer and all of its supporting files into another directory before running it.",
                    "NBTExplorer failed to run", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
                return;
            }

            StringBuilder errorText = new StringBuilder();
            errorText.AppendLine("NBTExplorer encountered the following exception while trying to run: " + ex.GetType().Name);
            errorText.AppendLine("Message: " + ex.Message);

            Exception ix = ex;
            while (ix.InnerException != null) {
                ix = ix.InnerException;
                errorText.AppendLine();
                errorText.AppendLine("Caused by Inner Exception: " + ix.GetType().Name);
                errorText.AppendLine("Message: " + ix.Message);
            }

            try {
                string logDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "NBTExplorer");
                if (!Directory.Exists(logDir))
                    Directory.CreateDirectory(logDir);

                string logPath = Path.Combine(logDir, "error.log");
                using (var writer = new StreamWriter(logPath, true)) {
                    writer.WriteLine("NBTExplorer Error Report");
                    writer.WriteLine(DateTime.Now);
                    writer.WriteLine("-------");
                    writer.WriteLine(errorText);
                    writer.WriteLine("-------");

                    ix = ex;
                    while (ix != null) {
                        writer.WriteLine(ex.StackTrace);
                        writer.WriteLine("-------");
                        ix = ix.InnerException;
                    }

                    writer.WriteLine();
                }

                errorText.AppendLine();
                errorText.AppendLine("Additional error detail has been written to:\n" + logPath);
            }
            catch { }

            MessageBox.Show(errorText.ToString(), "NBTExplorer failed to run", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Application.Exit();
        }

        private static bool IsMissingSubstrate (Exception ex)
        {
            if (ex is TypeInitializationException && ex.InnerException != null)
                ex = ex.InnerException;
            if (ex is FileNotFoundException) {
                FileNotFoundException fileEx = ex as FileNotFoundException;
                if (fileEx.FileName.Contains("Substrate"))
                    return true;
            }

            return false;
        }

        private static bool IsMissingNBTModel (Exception ex)
        {
            if (ex is TypeInitializationException && ex.InnerException != null)
                ex = ex.InnerException;
            if (ex is FileNotFoundException) {
                FileNotFoundException fileEx = ex as FileNotFoundException;
                if (fileEx.FileName.Contains("NBTModel"))
                    return true;
            }

            return false;
        }
    }
}
