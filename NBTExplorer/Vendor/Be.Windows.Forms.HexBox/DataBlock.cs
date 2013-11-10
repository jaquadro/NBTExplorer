using System;

namespace Be.Windows.Forms
{
    internal abstract class DataBlock
    {
        internal DataMap _map;
        internal DataBlock _nextBlock;
        internal DataBlock _previousBlock;

        public abstract long Length
        {
            get;
        }

        public DataMap Map
        {
            get
            {
                return _map;
            }
        }

        public DataBlock NextBlock
        {
            get
            {
                return _nextBlock;
            }
        }

        public DataBlock PreviousBlock
        {
            get
            {
                return _previousBlock;
            }
        }

        public abstract void RemoveBytes(long position, long count);
    }
}
