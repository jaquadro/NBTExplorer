using System;
using System.Collections;
using System.Collections.Generic;

namespace Substrate.Core
{
    public interface IDataArray
    {
        int this[int i] { get; set; }
        int Length { get; }
        int DataWidth { get; }

        void Clear ();
    }

    public interface IDataArray2 : IDataArray
    {
        int this[int x, int z] { get; set; }

        int XDim { get; }
        int ZDim { get; }
    }

    public interface IDataArray3 : IDataArray
    {
        int this[int x, int y, int z] { get; set; }

        int XDim { get; }
        int YDim { get; }
        int ZDim { get; }

        int GetIndex (int x, int y, int z);
        void GetMultiIndex (int index, out int x, out int y, out int z);
    }

    public class ByteArray : IDataArray, ICopyable<ByteArray>
    {
        protected readonly byte[] dataArray;

        public ByteArray (int length)
        {
            dataArray = new byte[length];
        }

        public ByteArray (byte[] data)
        {
            dataArray = data;
        }

        public int this[int i]
        {
            get { return dataArray[i]; }
            set { dataArray[i] = (byte)value; }
        }

        public int Length
        {
            get { return dataArray.Length; }
        }

        public int DataWidth
        {
            get { return 8; }
        }

        public void Clear ()
        {
            for (int i = 0; i < dataArray.Length; i++)
            {
                dataArray[i] = 0;
            }
        }

        #region ICopyable<ByteArray> Members

        public virtual ByteArray Copy ()
        {
            byte[] data = new byte[dataArray.Length];
            dataArray.CopyTo(data, 0);

            return new ByteArray(data);
        }

        #endregion
    }

    public sealed class XZYByteArray : ByteArray, IDataArray3
    {
        private readonly int _xdim;
        private readonly int _ydim;
        private readonly int _zdim;

        public XZYByteArray (int xdim, int ydim, int zdim)
            : base(xdim * ydim * zdim)
        {
            _xdim = xdim;
            _ydim = ydim;
            _zdim = zdim;
        }

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

        public int this[int x, int y, int z]
        {
            get
            {
                int index = _ydim * (x * _zdim + z) + y;
                return dataArray[index];
            }

            set
            {
                int index = _ydim * (x * _zdim + z) + y;
                dataArray[index] = (byte)value;
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

    public sealed class YZXByteArray : ByteArray, IDataArray3
    {
        private readonly int _xdim;
        private readonly int _ydim;
        private readonly int _zdim;

        public YZXByteArray (int xdim, int ydim, int zdim)
            : base(xdim * ydim * zdim)
        {
            _xdim = xdim;
            _ydim = ydim;
            _zdim = zdim;
        }

        public YZXByteArray (int xdim, int ydim, int zdim, byte[] data)
            : base(data)
        {
            _xdim = xdim;
            _ydim = ydim;
            _zdim = zdim;

            if (xdim * ydim * zdim != data.Length) {
                throw new ArgumentException("Product of dimensions must equal length of data");
            }
        }

        public int this[int x, int y, int z]
        {
            get
            {
                int index = _xdim * (y * _zdim + z) + x;
                return dataArray[index];
            }

            set
            {
                int index = _xdim * (y * _zdim + z) + x;
                dataArray[index] = (byte)value;
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

        #region ICopyable<YZXByteArray> Members

        public override ByteArray Copy ()
        {
            byte[] data = new byte[dataArray.Length];
            dataArray.CopyTo(data, 0);

            return new YZXByteArray(_xdim, _ydim, _zdim, data);
        }

        #endregion
    }

    public sealed class ZXByteArray : ByteArray, IDataArray2
    {
        private readonly int _xdim;
        private readonly int _zdim;

        public ZXByteArray (int xdim, int zdim)
            : base(xdim * zdim)
        {
            _xdim = xdim;
            _zdim = zdim;
        }

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

        public int this[int x, int z]
        {
            get
            {
                int index = z * _xdim + x;
                return dataArray[index];
            }

            set
            {
                int index = z * _xdim + x;
                dataArray[index] = (byte)value;
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

    public class IntArray : IDataArray, ICopyable<IntArray>
    {
        protected readonly int[] dataArray;

        public IntArray (int length)
        {
            dataArray = new int[length];
        }

        public IntArray (int[] data)
        {
            dataArray = data;
        }

        public int this[int i]
        {
            get { return dataArray[i]; }
            set { dataArray[i] = value; }
        }

        public int Length
        {
            get { return dataArray.Length; }
        }

        public int DataWidth
        {
            get { return 32; }
        }

        public void Clear ()
        {
            for (int i = 0; i < dataArray.Length; i++) {
                dataArray[i] = 0;
            }
        }

        #region ICopyable<ByteArray> Members

        public virtual IntArray Copy ()
        {
            int[] data = new int[dataArray.Length];
            dataArray.CopyTo(data, 0);

            return new IntArray(data);
        }

        #endregion
    }

    public sealed class ZXIntArray : IntArray, IDataArray2
    {
        private readonly int _xdim;
        private readonly int _zdim;

        public ZXIntArray (int xdim, int zdim)
            : base(xdim * zdim)
        {
            _xdim = xdim;
            _zdim = zdim;
        }

        public ZXIntArray (int xdim, int zdim, int[] data)
            : base(data)
        {
            _xdim = xdim;
            _zdim = zdim;

            if (xdim * zdim != data.Length) {
                throw new ArgumentException("Product of dimensions must equal length of data");
            }
        }

        public int this[int x, int z]
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

        public override IntArray Copy ()
        {
            int[] data = new int[dataArray.Length];
            dataArray.CopyTo(data, 0);

            return new ZXIntArray(_xdim, _zdim, data);
        }

        #endregion
    }
}
