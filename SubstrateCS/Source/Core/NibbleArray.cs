using System;
using System.Collections;
using System.Collections.Generic;

namespace Substrate.Core
{

    public class NibbleArray : IDataArray, ICopyable<NibbleArray>
    {
        private readonly byte[] _data = null;

        public NibbleArray (int length)
        {
            _data = new byte[(int)Math.Ceiling(length / 2.0)];
        }

        public NibbleArray (byte[] data)
        {
            _data = data;
        }

        public int this[int index]
        {
            get
            {
                int subs = index >> 1;
                if ((index & 1) == 0)
                {
                    return (byte)(_data[subs] & 0x0F);
                }
                else
                {
                    return (byte)((_data[subs] >> 4) & 0x0F);
                }
            }

            set
            {
                int subs = index >> 1;
                if ((index & 1) == 0)
                {
                    _data[subs] = (byte)((_data[subs] & 0xF0) | (value & 0x0F));
                }
                else
                {
                    _data[subs] = (byte)((_data[subs] & 0x0F) | ((value & 0x0F) << 4));
                }
            }
        }

        public int Length
        {
            get { return _data.Length << 1; }
        }

        public int DataWidth
        {
            get { return 4; }
        }

        protected byte[] Data
        {
            get { return _data; }
        }

        public void Clear ()
        {
            for (int i = 0; i < _data.Length; i++)
            {
                _data[i] = 0;
            }
        }

        #region ICopyable<NibbleArray> Members

        public virtual NibbleArray Copy ()
        {
            byte[] data = new byte[_data.Length];
            _data.CopyTo(data, 0);

            return new NibbleArray(data);
        }

        #endregion
    }

    public sealed class XZYNibbleArray : NibbleArray, IDataArray3
    {
        private readonly int _xdim;
        private readonly int _ydim;
        private readonly int _zdim;

        public XZYNibbleArray (int xdim, int ydim, int zdim)
            : base(xdim * ydim * zdim)
        {
            _xdim = xdim;
            _ydim = ydim;
            _zdim = zdim;
        }

        public XZYNibbleArray (int xdim, int ydim, int zdim, byte[] data)
            : base(data)
        {
            _xdim = xdim;
            _ydim = ydim;
            _zdim = zdim;

            if (xdim * ydim * zdim != data.Length * 2)
            {
                throw new ArgumentException("Product of dimensions must equal half length of raw data");
            }
        }

        public int this[int x, int y, int z]
        {
            get
            {
                int index = _ydim * (x * _zdim + z) + y;
                return this[index];
            }

            set
            {
                int index = _ydim * (x * _zdim + z) + y;
                this[index] = value;
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

        #region ICopyable<NibbleArray> Members

        public override NibbleArray Copy ()
        {
            byte[] data = new byte[Data.Length];
            Data.CopyTo(data, 0);

            return new XZYNibbleArray(_xdim, _ydim, _zdim, data);
        }

        #endregion
    }

    public sealed class YZXNibbleArray : NibbleArray, IDataArray3
    {
        private readonly int _xdim;
        private readonly int _ydim;
        private readonly int _zdim;

        public YZXNibbleArray (int xdim, int ydim, int zdim)
            : base(xdim * ydim * zdim)
        {
            _xdim = xdim;
            _ydim = ydim;
            _zdim = zdim;
        }

        public YZXNibbleArray (int xdim, int ydim, int zdim, byte[] data)
            : base(data)
        {
            _xdim = xdim;
            _ydim = ydim;
            _zdim = zdim;

            if (xdim * ydim * zdim != data.Length * 2) {
                throw new ArgumentException("Product of dimensions must equal half length of raw data");
            }
        }

        public int this[int x, int y, int z]
        {
            get
            {
                int index = _xdim * (y * _zdim + z) + x;
                return this[index];
            }

            set
            {
                int index = _xdim * (y * _zdim + z) + x;
                this[index] = value;
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
            return _xdim * (y * _zdim + z) + x;
        }

        public void GetMultiIndex (int index, out int x, out int y, out int z)
        {
            int xzdim = _xdim * _zdim;
            y = index / xzdim;

            int zx = index - (y * xzdim);
            z = zx / _xdim;
            x = zx - (z * _xdim);
        }

        #region ICopyable<NibbleArray> Members

        public override NibbleArray Copy ()
        {
            byte[] data = new byte[Data.Length];
            Data.CopyTo(data, 0);

            return new YZXNibbleArray(_xdim, _ydim, _zdim, data);
        }

        #endregion
    }
}
