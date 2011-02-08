using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Util
{
    public class Base36
    {
        static private String tokens = "0123456789abcdefghijklmnopqrstuvwxyz";
        static private int[] powers = { 1, 36, 36 * 36, 36 * 36 * 36, 36 * 36 * 36 * 36 };

        static public int ToInteger (String b36) {
            int res = 0;

            char[] b36a = b36.ToCharArray().Reverse().ToArray();
            for (int i = 0; i < b36a.Length; i++) {
                if (b36a[i] == '-') {
                    res *= -1;
                    break;
                }
                res += (powers[i] * tokens.IndexOf(b36a[i]));
            }

            return res;
        }
    }

    public struct Coord2
    {
        public int x;
        public int z;

        public Coord2 (int _x, int _z)
        {
            x = _x;
            z = _z;
        }
    }
}
