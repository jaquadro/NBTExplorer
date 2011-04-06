using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Map.Utility
{
    public class NibbleArray : ICopyable<NibbleArray>
    {
        protected readonly byte[] _data = null;

        public NibbleArray (byte[] data)
        {
            _data = data;
        }

        public int this[int index]
        {
            get
            {
                int subs = index >> 1;
                if ((index & 1) == 0) {
                    return _data[subs] & 0x0F;
                }
                else {
                    return (_data[subs] >> 4) & 0x0F;
                }
            }

            set
            {
                int subs = index >> 1;
                if ((index & 1) == 0) {
                    _data[subs] = (byte)((_data[subs] & 0xF0) | (value & 0x0F));
                }
                else {
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

        #region ICopyable<NibbleArray> Members

        public NibbleArray Copy ()
        {
            byte[] data = new byte[_data.Length];
            _data.CopyTo(data, 0);

            return new NibbleArray(data);
        }

        #endregion
    }
}
