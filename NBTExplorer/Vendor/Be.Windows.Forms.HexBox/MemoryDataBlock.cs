using System;

namespace Be.Windows.Forms
{
    internal sealed class MemoryDataBlock : DataBlock
    {
        byte[] _data;

        public MemoryDataBlock(byte data)
        {
            _data = new byte[] { data };
        }

        public MemoryDataBlock(byte[] data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            _data = (byte[])data.Clone();
        }

        public override long Length
        {
            get
            {
                return _data.LongLength;
            }
        }

        public byte[] Data
        {
            get
            {
                return _data;
            }
        }

        public void AddByteToEnd(byte value)
        {
            byte[] newData = new byte[_data.LongLength + 1];
            _data.CopyTo(newData, 0);
            newData[newData.LongLength - 1] = value;
            _data = newData;
        }

        public void AddByteToStart(byte value)
        {
            byte[] newData = new byte[_data.LongLength + 1];
            newData[0] = value;
            _data.CopyTo(newData, 1);
            _data = newData;
        }

        public void InsertBytes(long position, byte[] data)
        {
            byte[] newData = new byte[_data.LongLength + data.LongLength];
            if (position > 0)
            {
                Array.Copy(_data, 0, newData, 0, position);
            }
            Array.Copy(data, 0, newData, position, data.LongLength);
            if (position < _data.LongLength)
            {
                Array.Copy(_data, position, newData, position + data.LongLength, _data.LongLength - position);
            }
            _data = newData;
        }

        public override void RemoveBytes(long position, long count)
        {
            byte[] newData = new byte[_data.LongLength - count];

            if (position > 0)
            {
                Array.Copy(_data, 0, newData, 0, position);
            }
            if (position + count < _data.LongLength)
            {
                Array.Copy(_data, position + count, newData, position, newData.LongLength - position);
            }

            _data = newData;
        }
    }
}
