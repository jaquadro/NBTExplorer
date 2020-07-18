using System.Diagnostics;

namespace Be.Windows.Forms
{
    internal static class Util
    {
        /// <summary>
        ///     Contains true, if we are in design mode of Visual Studio
        /// </summary>
        private static readonly bool _designMode;

        /// <summary>
        ///     Initializes an instance of Util class
        /// </summary>
        static Util()
        {
            _designMode = Process.GetCurrentProcess().ProcessName.ToLower() == "devenv";
        }

        /// <summary>
        ///     Gets true, if we are in design mode of Visual Studio
        /// </summary>
        /// <remarks>
        ///     In Visual Studio 2008 SP1 the designer is crashing sometimes on windows forms.
        ///     The DesignMode property of Control class is buggy and cannot be used, so use our own implementation instead.
        /// </remarks>
        public static bool DesignMode => _designMode;
    }
}