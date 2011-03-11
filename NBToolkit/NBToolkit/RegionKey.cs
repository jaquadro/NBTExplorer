using System;
using System.Collections.Generic;
using System.Text;

namespace NBToolkit
{
    public class RegionKey : IEquatable<RegionKey>
    {
        protected int rx;
        protected int rz;

        public RegionKey (int _rx, int _rz)
        {
            rx = _rx;
            rz = _rz;
        }

        public bool Equals (RegionKey ck)
        {
            return this.rx == ck.rx && this.rz == ck.rz;
        }

        public override int GetHashCode ()
        {
            return rx ^ rz;
        }
    }
}
