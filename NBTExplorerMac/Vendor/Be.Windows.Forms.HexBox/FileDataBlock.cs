using System;

namespace Be.Windows.Forms
{
    internal sealed class FileDataBlock : DataBlock
    {
        long _length;
        long _fileOffset;

        public FileDataBlock(long fileOffset, long length)
        {
            _fileOffset = fileOffset;
            _length = length;
        }

        public long FileOffset
        {
            get
            {
                return _fileOffset;
            }
        }

        public override long Length
        {
            get
            {
                return _length;
            }
        }

        public void SetFileOffset(long value)
        {
            _fileOffset = value;
        }

        public void RemoveBytesFromEnd(long count)
        {
            if (count > _length)
            {
                throw new ArgumentOutOfRangeException("count");
            }

            _length -= count;
        }

        public void RemoveBytesFromStart(long count)
        {
            if (count > _length)
            {
                throw new ArgumentOutOfRangeException("count");
            }

            _fileOffset += count;
            _length -= count;
        }

        public override void RemoveBytes(long position, long count)
        {
            if (position > _length)
            {
                throw new ArgumentOutOfRangeException("position");
            }

            if (position + count > _length)
            {
                throw new ArgumentOutOfRangeException("count");
            }

            long prefixLength = position;
            long prefixFileOffset = _fileOffset;

            long suffixLength = _length - count - prefixLength;
            long suffixFileOffset = _fileOffset + position + count;

            if (prefixLength > 0 && suffixLength > 0)
            {
                _fileOffset = prefixFileOffset;
                _length = prefixLength;
                _map.AddAfter(this, new FileDataBlock(suffixFileOffset, suffixLength));
                return;
            }

            if (prefixLength > 0)
            {
                _fileOffset = prefixFileOffset;
                _length = prefixLength;
            }
            else
            {
                _fileOffset = suffixFileOffset;
                _length = suffixLength;
            }
        }
    }
}
