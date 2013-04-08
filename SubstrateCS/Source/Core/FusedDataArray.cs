using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Core
{
    public class FusedDataArray3 : IDataArray3
    {
        private IDataArray3 _array0;
        private IDataArray3 _array1;

        private int _mask1;

        public FusedDataArray3 (IDataArray3 array0, IDataArray3 array1)
        {
            if (array0 == null || array1 == null)
                throw new ArgumentException("arguments cannot be null");

            if (array0.XDim != array1.XDim || array0.YDim != array1.YDim || array0.ZDim != array1.ZDim)
                throw new ArgumentException("array0 and array1 must have matching dimensions");

            _array0 = array0;
            _array1 = array1;

            _mask1 = (1 << _array1.DataWidth) - 1;
        }

        public int this[int x, int y, int z]
        {
            get { return (_array0[x, y, z] << _array1.DataWidth) + _array1[x, y, z]; }
            set
            {
                _array0[x, y, z] = value >> _array1.DataWidth;
                _array1[x, y, z] = value & _mask1;
            }
        }

        public int XDim
        {
            get { return _array1.XDim; }
        }

        public int YDim
        {
            get { return _array1.YDim; }
        }

        public int ZDim
        {
            get { return _array1.ZDim; }
        }

        public int GetIndex (int x, int y, int z)
        {
            return _array1.GetIndex(x, y, z);
        }

        public void GetMultiIndex (int index, out int x, out int y, out int z)
        {
            _array1.GetMultiIndex(index, out x, out y, out z);
        }

        public int this[int i]
        {
            get { return (_array0[i] << _array1.DataWidth) + _array1[i]; }
            set
            {
                _array0[i] = value >> _array1.DataWidth;
                _array1[i] = value & _mask1;
            }
        }

        public int Length
        {
            get { return _array1.Length; }
        }

        public int DataWidth
        {
            get { return _array0.DataWidth + _array1.DataWidth; }
        }

        public void Clear ()
        {
            _array0.Clear();
            _array1.Clear();
        }
    }
}
