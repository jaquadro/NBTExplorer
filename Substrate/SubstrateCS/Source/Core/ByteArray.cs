using System;
using System.Collections;
using System.Collections.Generic;

namespace Substrate.Core
{
    public class ByteArray : ICopyable<ByteArray>
    {
        protected readonly byte[] dataArray;

        public ByteArray (byte[] data)
        {
            dataArray = data;
        }

        public byte this[int i]
        {
            get { return dataArray[i]; }
            set { dataArray[i] = value; }
        }

        public int Length
        {
            get { return dataArray.Length; }
        }

        public void Clear ()
        {
            for (int i = 0; i < dataArray.Length; i++)
            {
                dataArray[i] = 0;
            }
        }

        #region ICopyable<yteArray> Members

        public virtual ByteArray Copy ()
        {
            byte[] data = new byte[dataArray.Length];
            dataArray.CopyTo(data, 0);

            return new ByteArray(data);
        }

        #endregion
    }

    public sealed class XZYByteArray : ByteArray
    {
        private readonly int _xdim;
        private readonly int _ydim;
        private readonly int _zdim;

        public XZYByteArray (int xdim, int ydim, int zdim, byte[] data)
            : base(data)
        {
            _xdim = xdim;
            _ydim = ydim;
            _zdim = zdim;

            if (xdim * ydim * zdim != data.Length)
            {
                throw new ArgumentException("Product of dimensions must equal length of data");
            }
        }

        public byte this[int x, int y, int z]
        {
            get
            {
                int index = _ydim * (x * _zdim + z) + y;
                return dataArray[index];
            }

            set
            {
                int index = _ydim * (x * _zdim + z) + y;
                dataArray[index] = value;
            }
        }

        public int XDim
        {
            get { return _xdim; }
        }

        public int YDim
        {
            get { return _ydim; }
        }

        public int ZDim
        {
            get { return _zdim; }
        }

        public int GetIndex (int x, int y, int z)
        {
            return _ydim * (x * _zdim + z) + y;
        }

        public void GetMultiIndex (int index, out int x, out int y, out int z)
        {
            int yzdim = _ydim * _zdim;
            x = index / yzdim;

            int zy = index - (x * yzdim);
            z = zy / _ydim;
            y = zy - (z * _ydim);
        }

        #region ICopyable<XZYByteArray> Members

        public override ByteArray Copy ()
        {
            byte[] data = new byte[dataArray.Length];
            dataArray.CopyTo(data, 0);

            return new XZYByteArray(_xdim, _ydim, _zdim, data);
        }

        #endregion
    }

    public sealed class ZXByteArray : ByteArray
    {
        private readonly int _xdim;
        private readonly int _zdim;

        public ZXByteArray (int xdim, int zdim, byte[] data)
            : base(data)
        {
            _xdim = xdim;
            _zdim = zdim;

            if (xdim * zdim != data.Length)
            {
                throw new ArgumentException("Product of dimensions must equal length of data");
            }
        }

        public byte this[int x, int z]
        {
            get
            {
                int index = z * _xdim + x;
                return dataArray[index];
            }

            set
            {
                int index = z * _xdim + x;
                dataArray[index] = value;
            }
        }

        public int XDim
        {
            get { return _xdim; }
        }

        public int ZDim
        {
            get { return _zdim; }
        }

        #region ICopyable<ZXByteArray> Members

        public override ByteArray Copy ()
        {
            byte[] data = new byte[dataArray.Length];
            dataArray.CopyTo(data, 0);

            return new ZXByteArray(_xdim, _zdim, data);
        }

        #endregion
    }
}
