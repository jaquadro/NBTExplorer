using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Core
{
    internal static class Base36
    {
        private const string _alphabet = "0123456789abcdefghijklmnopqrstuvwxyz";

        public static string Encode (long input)
        {
            if (input == 0) {
                return "0";
            }

            bool neg = (input < 0);
            if (neg) {
                input *= -1;
            }

            Stack<char> result = new Stack<char>();
            while (input != 0) {
                result.Push(_alphabet[(int)(input % 36)]);
                input /= 36;
            }

            string ret = new string(result.ToArray());
            if (neg) {
                ret = '-' + ret;
            }

            return ret;
        }

        public static long Decode (string input)
        {
            if (input.Length == 0) {
                throw new ArgumentOutOfRangeException("input", input);
            }

            bool neg = false;
            if (input[0] == '-') {
                neg = true;
                input = input.Substring(1);
            }

            input = input.ToLower();
            long result = 0;

            int pos = 0;
            for (int i = input.Length - 1; i >= 0; i--) {
                result += _alphabet.IndexOf(input[i]) * (long)Math.Pow(36, pos);
                pos++;
            }

            if (neg) {
                result *= -1;
            }
            return result;
        }
    }

    internal static class Base16
    {
        private const string _alphabet = "0123456789abcdef";

        public static string Encode (byte[] input, int stride = 0, char strideChar = ' ')
        {
            List<char> result = new List<char>();

            for (int i = 0; i < input.Length; i++) {
                int hi = (input[i] >> 4) & 0xF;
                int lo = input[i] & 0xF;

                result.Add(_alphabet[hi]);
                if (stride > 0 && (((i + 1) * 2 - 1) % stride) == 0) {
                    result.Add(strideChar);
                }

                result.Add(_alphabet[lo]);
                if (stride > 0 && (((i + 1) * 2) % stride) == 0) {
                    result.Add(strideChar);
                }
            }

            return new string(result.ToArray());
        }
    }
}
