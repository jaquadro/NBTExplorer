using System;
using System.Collections;
using System.Collections.Generic;

namespace Substrate.Core
{
    public class ByteArray : ICopyable<ByteArray>
    {
        protected readonly byte[] _data;

        public ByteArray (byte[] data)
        {
            _data = data;
        }

        public byte this[int i]
        {
            get { return _data[i]; }
            set { _data[i] = value; }
        }

        public int Length
        {
            get { return _data.Length; }
        }

        public void Clear ()
        {
            for (int i = 0; i < _data.Length; i++)
            {
                _data[i] = 0;
            }
        }

        #region ICopyable<yteArray> Members

        public virtual ByteArray Copy ()
        {
            byte[] data = new byte[_data.Length];
            _data.CopyTo(data, 0);

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
                return _data[index];
            }

            set
            {
                int index = _ydim * (x * _zdim + z) + y;
                _data[index] = value;
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

        #region ICopyable<XZYByteArray> Members

        public override ByteArray Copy ()
        {
            byte[] data = new byte[_data.Length];
            _data.CopyTo(data, 0);

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
                return _data[index];
            }

            set
            {
                int index = z * _xdim + x;
                _data[index] = value;
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
            byte[] data = new byte[_data.Length];
            _data.CopyTo(data, 0);

            return new ZXByteArray(_xdim, _zdim, data);
        }

        #endregion
    }
}
