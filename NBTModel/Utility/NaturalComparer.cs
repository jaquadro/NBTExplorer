using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace NBTExplorer.Utility
{
    // NaturalComparer implementation by Justin.Jones
    // Licensed under The Code Project Open License (CPOL) (http://www.codeproject.com/info/cpol10.aspx)

    public class NaturalComparer : Comparer<string>, IDisposable
    {
        private Dictionary<string, string[]> table;

        public NaturalComparer ()
        {
            table = new Dictionary<string, string[]>();
        }

        public void Dispose ()
        {
            table.Clear();
            table = null;
        }

        public override int Compare (string x, string y)
        {
            if (x == y) {
                return 0;
            }
            string[] x1, y1;
            if (!table.TryGetValue(x, out x1)) {
                x1 = Regex.Split(x.Replace(" ", ""), "(-?[0-9]+)");
                table.Add(x, x1);
            }
            if (!table.TryGetValue(y, out y1)) {
                y1 = Regex.Split(y.Replace(" ", ""), "(-?[0-9]+)");
                table.Add(y, y1);
            }

            for (int i = 0; i < x1.Length && i < y1.Length; i++) {
                if (x1[i] != y1[i]) {
                    return PartCompare(x1[i], y1[i]);
                }
            }
            if (y1.Length > x1.Length) {
                return 1;
            }
            else if (x1.Length > y1.Length) {
                return -1;
            }
            else {
                return 0;
            }
        }

        private static int PartCompare (string left, string right)
        {
            int x, y;
            if (!int.TryParse(left, out x)) {
                return left.CompareTo(right);
            }

            if (!int.TryParse(right, out y)) {
                return left.CompareTo(right);
            }

            return x.CompareTo(y);
        }
    } 
}
