using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Core
{
    public class CompositeDataArray3 : IDataArray3
    {
        private IDataArray3[] _sections;

        public CompositeDataArray3 (IDataArray3[] sections)
        {
            for (int i = 0; i < sections.Length; i++)
                if (sections[i] == null)
                    throw new ArgumentException("sections argument cannot have null entries.");

            for (int i = 0; i < sections.Length; i++) {
                if (sections[i].Length != sections[0].Length
                    || sections[i].XDim != sections[0].XDim
                    || sections[i].YDim != sections[0].YDim
                    || sections[i].ZDim != sections[0].ZDim)
                    throw new ArgumentException("All elements in sections argument must have same metrics.");
            }

            _sections = sections;
        }

        #region IByteArray3 Members

        public int this[int x, int y, int z]
        {
            get
            {
                int ydiv = y / _sections[0].YDim;
                int yrem = y - (ydiv * _sections[0].YDim);
                return _sections[ydiv][x, yrem, z];
            }

            set
            {
                int ydiv = y / _sections[0].YDim;
                int yrem = y - (ydiv * _sections[0].YDim);
                _sections[ydiv][x, yrem, z] = value;
            }
        }

        public int XDim
        {
            get { return _sections[0].XDim; }
        }

        public int YDim
        {
            get { return _sections[0].YDim * _sections.Length; }
        }

        public int ZDim
        {
            get { return _sections[0].ZDim; }
        }

        public int GetIndex (int x, int y, int z)
        {
            int ydiv = y / _sections[0].YDim;
            int yrem = y - (ydiv * _sections[0].YDim);
            return (ydiv * _sections[0].Length) + _sections[ydiv].GetIndex(x, yrem, z);
        }

        public void GetMultiIndex (int index, out int x, out int y, out int z)
        {
            int idiv = index / _sections[0].Length;
            int irem = index - (idiv * _sections[0].Length);
            _sections[idiv].GetMultiIndex(irem, out x, out y, out z);
            y += idiv * _sections[0].YDim;
        }

        #endregion

        #region IByteArray Members

        public int this[int i]
        {
            get
            {
                int idiv = i / _sections[0].Length;
                int irem = i - (idiv * _sections[0].Length);
                return _sections[idiv][irem];
            }
            set
            {
                int idiv = i / _sections[0].Length;
                int irem = i - (idiv * _sections[0].Length);
                _sections[idiv][irem] = value;
            }
        }

        public int Length
        {
            get { return _sections[0].Length * _sections.Length; }
        }

        public int DataWidth
        {
            get { return _sections[0].DataWidth; }
        }

        public void Clear ()
        {
            for (int i = 0; i < _sections.Length; i++)
                _sections[i].Clear();
        }

        #endregion
    }
}
