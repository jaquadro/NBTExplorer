using System;
using System.Collections.Generic;
using System.Text;

namespace NBToolkit
{
    public class ChunkKey : IEquatable<ChunkKey>
    {
        protected int cx;
        protected int cz;

        public ChunkKey (int _cx, int _cz)
        {
            cx = _cx;
            cz = _cz;
        }

        public bool Equals (ChunkKey ck)
        {
            return this.cx == ck.cx && this.cz == ck.cz;
        }

        public override int GetHashCode ()
        {
            return cx ^ cz;
        }
    }
}
