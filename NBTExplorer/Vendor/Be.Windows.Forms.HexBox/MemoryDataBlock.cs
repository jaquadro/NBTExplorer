using System;

namespace Be.Windows.Forms
{
    internal sealed class MemoryDataBlock : DataBlock
    {
        public MemoryDataBlock(byte data)
        {
            Data = new[] { data };
        }

        public MemoryDataBlock(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            Data = (byte[])data.Clone();
        }

        public override long Length => Data.LongLength;

        public byte[] Data { get; private set; }

        public void AddByteToEnd(byte value)
        {
            var newData = new byte[Data.LongLength + 1];
            Data.CopyTo(newData, 0);
            newData[newData.LongLength - 1] = value;
            Data = newData;
        }

        public void AddByteToStart(byte value)
        {
            var newData = new byte[Data.LongLength + 1];
            newData[0] = value;
            Data.CopyTo(newData, 1);
            Data = newData;
        }

        public void InsertBytes(long position, byte[] data)
        {
            var newData = new byte[Data.LongLength + data.LongLength];
            if (position > 0)
                Array.Copy(Data, 0, newData, 0, position);
            Array.Copy(data, 0, newData, position, data.LongLength);
            if (position < Data.LongLength)
                Array.Copy(Data, position, newData, position + data.LongLength, Data.LongLength - position);
            Data = newData;
        }

        public override void RemoveBytes(long position, long count)
        {
            var newData = new byte[Data.LongLength - count];

            if (position > 0)
                Array.Copy(Data, 0, newData, 0, position);
            if (position + count < Data.LongLength)
                Array.Copy(Data, position + count, newData, position, newData.LongLength - position);

            Data = newData;
        }
    }
}