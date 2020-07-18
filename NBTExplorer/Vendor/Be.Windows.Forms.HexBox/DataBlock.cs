namespace Be.Windows.Forms
{
    internal abstract class DataBlock
    {
        internal DataMap _map;
        internal DataBlock _nextBlock;
        internal DataBlock _previousBlock;

        public abstract long Length { get; }

        public DataMap Map => _map;

        public DataBlock NextBlock => _nextBlock;

        public DataBlock PreviousBlock => _previousBlock;

        public abstract void RemoveBytes(long position, long count);
    }
}