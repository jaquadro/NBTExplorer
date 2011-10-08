using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Substrate.Core;
using Substrate.Nbt;

namespace Substrate
{
    public static class Dimension 
    {
        public const int NETHER = -1;
        public const int DEFAULT = 0;
    }

    public class DimensionNotFoundException : Exception { }
}
