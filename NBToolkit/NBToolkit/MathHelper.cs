using System;
using System.Collections.Generic;
using System.Text;

/**
 * The following is adapted directly from Mojang sources and is
 * subject to Mojang copyright and restrictions.
 */

namespace NBToolkit
{
    class MathHelper
    {
        private static float[] trigTable = new float[65536];

        static MathHelper ()
        {
            for (int i = 0; i < 65536; i++) {
                trigTable[i] = (float)Math.Sin(i * Math.PI * 2.0D / 65536.0D);
            }
        }

        public static float Sin (float angle)
        {
            return trigTable[((int)(angle * 10430.378F) & 0xFFFF)];
        }

        public static float Cos (float angle)
        {
            return trigTable[((int)(angle * 10430.378F + 16384.0F) & 0xFFFF)];
        }
    }
}
