using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Core
{
    public struct RegionKey : IEquatable<RegionKey>
    {
        readonly int rx;
        readonly int rz;

        public RegionKey (int _rx, int _rz)
        {
            rx = _rx;
            rz = _rz;
        }

        public bool Equals (RegionKey ck)
        {
            return this.rx == ck.rx && this.rz == ck.rz;
        }

        public override bool Equals (Object o)
        {
            try {
                return this == (RegionKey)o;
            }
            catch {
                return false;
            }
        }

        public override int GetHashCode ()
        {
            int hash = 23;
            hash = hash * 37 + rx;
            hash = hash * 37 + rz;
            return hash;
        }

        public static bool operator == (RegionKey k1, RegionKey k2)
        {
            return k1.rx == k2.rx && k1.rz == k2.rz;
        }

        public static bool operator != (RegionKey k1, RegionKey k2)
        {
            return k1.rx != k2.rx || k1.rz != k2.rz;
        }

        public override string ToString ()
        {
            return "(" + rx + ", " + rz + ")";
        }
    }
}
