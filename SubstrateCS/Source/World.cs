using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Substrate.Core;
using Substrate.Nbt;

namespace Substrate
{
    /// <summary>
    /// A collection of constants to specify different Minecraft world dimensions.
    /// </summary>
    public static class Dimension 
    {
        /// <summary>
        /// Specifies the Nether dimension.
        /// </summary>
        public const int NETHER = -1;

        /// <summary>
        /// Specifies the default overworld.
        /// </summary>
        public const int DEFAULT = 0;

        /// <summary>
        /// Specifies the Enderman dimension, The End.
        /// </summary>
        public const int THE_END = 1;
    }

    public class DimensionNotFoundException : Exception { }
}
