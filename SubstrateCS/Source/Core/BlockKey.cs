using System;

namespace Substrate.Core
{
    public struct BlockKey : IEquatable<BlockKey>
    {
        public readonly int x;
        public readonly int y;
        public readonly int z;

        public BlockKey (int _x, int _y, int _z)
        {
            x = _x;
            y = _y;
            z = _z;
        }

        public bool Equals (BlockKey bk)
        {
            return this.x == bk.x && this.y == bk.y && this.z == bk.z;
        }

        public override bool Equals (Object o)
        {
            try {
                return this == (BlockKey)o;
            }
            catch {
                return false;
            }
        }

        public override int GetHashCode ()
        {
            int hash = 23;
            hash = hash * 37 + x;
            hash = hash * 37 + y;
            hash = hash * 37 + z;
            return hash;
        }

        public static bool operator == (BlockKey k1, BlockKey k2)
        {
            return k1.x == k2.x && k1.y == k2.y && k1.z == k2.z;
        }

        public static bool operator != (BlockKey k1, BlockKey k2)
        {
            return k1.x != k2.x || k1.y != k2.y || k1.z != k2.z;
        }

        public override string ToString ()
        {
            return "(" + x + ", " + y + ", " + z + ")";
        }
    }
}
