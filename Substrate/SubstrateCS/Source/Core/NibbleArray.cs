using System;
using System.Collections;
using System.Collections.Generic;

namespace Substrate.Core
{

    public class NibbleArray : ICopyable<NibbleArray>
    {
        protected readonly byte[] _data = null;

        public NibbleArray (byte[] data)
        {
            _data = data;
        }

        public byte this[int index]
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
            get
            {
                return _data.Length << 1;
            }
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

    public sealed class XZYNibbleArray : NibbleArray
    {
        private readonly int _xdim;
        private readonly int _ydim;
        private readonly int _zdim;

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

        public byte this[int x, int y, int z]
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

        #region ICopyable<NibbleArray> Members

        public override NibbleArray Copy ()
        {
            byte[] data = new byte[_data.Length];
            _data.CopyTo(data, 0);

            return new XZYNibbleArray(_xdim, _ydim, _zdim, data);
        }

        #endregion
    }
}
