using System;
using System.Collections.Generic;
using System.Text;

namespace NBToolkit
{
    using Map;
    using Map.NBT;

    public interface IGenerator
    {
        bool Generate (BlockManager blockMan, Random rand, int x, int y, int z);
    }

    /**
     * This class is derived from Mojang sources, the algorithm in particular
     * is subject to Mojang copyright and restrictions.  Mathfix patch by
     * jaquadro.
     */

    public class NativeGenOre : IGenerator
    {
        private int _blockId = 0;
        private int _blockData = 0;
        private int _size = 0;

        private bool _mathFix = false;

        public NativeGenOre (int blockId, int size)
        {
            _blockId = blockId;
            _size = size;
        }

        public NativeGenOre (int blockId, int blockData, int size)
        {
            _blockId = blockId;
            _blockData = blockData;
            _size = size;
        }

        public bool MathFix
        {
            get
            {
                return _mathFix;
            }
            set
            {
                _mathFix = value;
            }
        }

        public bool Generate (BlockManager blockMan, Random rand, int x, int y, int z)
        {
            float rpi = (float)(rand.NextDouble() * Math.PI);

            double x1 = x + 8 + MathHelper.Sin(rpi) * _size / 8.0F;
            double x2 = x + 8 - MathHelper.Sin(rpi) * _size / 8.0F;
            double z1 = z + 8 + MathHelper.Cos(rpi) * _size / 8.0F;
            double z2 = z + 8 - MathHelper.Cos(rpi) * _size / 8.0F;

            double y1 = y + rand.Next(3) + 2;
            double y2 = y + rand.Next(3) + 2;

            for (int i = 0; i <= _size; i++) {
                double xPos = x1 + (x2 - x1) * i / _size;
                double yPos = y1 + (y2 - y1) * i / _size;
                double zPos = z1 + (z2 - z1) * i / _size;

                double fuzz = rand.NextDouble() * _size / 16.0D;
                double fuzzXZ = (MathHelper.Sin((float)(i * Math.PI / _size)) + 1.0F) * fuzz + 1.0D;
                double fuzzY = (MathHelper.Sin((float)(i * Math.PI / _size)) + 1.0F) * fuzz + 1.0D;

                int xStart, yStart, zStart, xEnd, yEnd, zEnd;

                if (_mathFix) {
                    xStart = (int)Math.Floor(xPos - fuzzXZ / 2.0D);
                    yStart = (int)Math.Floor(yPos - fuzzY / 2.0D);
                    zStart = (int)Math.Floor(zPos - fuzzXZ / 2.0D);

                    xEnd = (int)Math.Floor(xPos + fuzzXZ / 2.0D);
                    yEnd = (int)Math.Floor(yPos + fuzzY / 2.0D);
                    zEnd = (int)Math.Floor(zPos + fuzzXZ / 2.0D);
                }
                else {
                    xStart = (int)(xPos - fuzzXZ / 2.0D);
                    yStart = (int)(yPos - fuzzY / 2.0D);
                    zStart = (int)(zPos - fuzzXZ / 2.0D);

                    xEnd = (int)(xPos + fuzzXZ / 2.0D);
                    yEnd = (int)(yPos + fuzzY / 2.0D);
                    zEnd = (int)(zPos + fuzzXZ / 2.0D);
                }

                for (int ix = xStart; ix <= xEnd; ix++) {
                    double xThresh = (ix + 0.5D - xPos) / (fuzzXZ / 2.0D);
                    if (xThresh * xThresh < 1.0D) {
                        for (int iy = yStart; iy <= yEnd; iy++) {
                            double yThresh = (iy + 0.5D - yPos) / (fuzzY / 2.0D);
                            if (xThresh * xThresh + yThresh * yThresh < 1.0D) {
                                for (int iz = zStart; iz <= zEnd; iz++) {
                                    double zThresh = (iz + 0.5D - zPos) / (fuzzXZ / 2.0D);
                                    if (xThresh * xThresh + yThresh * yThresh + zThresh * zThresh < 1.0D) {
                                        BlockRef block = blockMan.GetBlockRef(ix, iy, iz);
                                        if (block != null) {
                                            block.ID = _blockId;
                                            block.Data = _blockData;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return true;
        }
    }

    /*public class LegacyGenOre {

        public bool Generate (BlockManager blockMan, Random rand, int x, int y, int z)
        {
            double x_scale = 0.25 + (rand.NextDouble() * 0.75);
            double y_scale = 0.25 + (rand.NextDouble() * 0.75);
            double z_scale = 0.25 + (rand.NextDouble() * 0.75);

            if (opt.OPT_VV) {
                Console.WriteLine("Selected scale: {0}, {1}, {2}", x_scale, y_scale, z_scale);
            }

            double x_len = (double)opt.OPT_SIZE / 8.0 * x_scale;
            double z_len = (double)opt.OPT_SIZE / 8.0 * z_scale;
            double y_len = ((double)opt.OPT_SIZE / 16.0 + 2.0) * y_scale;

            if (opt.OPT_VV) {
                Console.WriteLine("Selected length: {0}, {1}, {2}", x_len, y_len, z_len);
            }

            double xpos = x;
            double ypos = y;
            double zpos = z;

            if (opt.OPT_VV) {
                Console.WriteLine("Selected initial position: {0}, {1}, {2}", xpos, ypos, zpos);
            }

            int sample_size = 2 * (int)opt.OPT_SIZE;
            double fuzz = 0.25;

            double x_step = x_len / sample_size;
            double y_step = y_len / sample_size;
            double z_step = z_len / sample_size;

            for (int i = 0; i < sample_size; i++) {
                int tx = (int)Math.Floor(xpos + i * x_step);
                int ty = (int)Math.Floor(ypos + i * y_step);
                int tz = (int)Math.Floor(zpos + i * z_step);
                int txp = (int)Math.Floor(xpos + i * x_step + fuzz);
                int typ = (int)Math.Floor(ypos + i * y_step + fuzz);
                int tzp = (int)Math.Floor(zpos + i * z_step + fuzz);

                if (tx < 0) tx = 0;
                if (ty < 0) ty = 0;
                if (tz < 0) tz = 0;

                if (tx >= 16) tx = 15;
                if (ty >= 128) ty = 127;
                if (tz >= 16) tz = 15;

                if (txp < 0) txp = 0;
                if (typ < 0) typ = 0;
                if (tzp < 0) tzp = 0;

                if (txp >= 16) txp = 15;
                if (typ >= 128) typ = 127;
                if (tzp >= 16) tzp = 15;


                UpdateBlock(world, chunk, tx, ty, tz);
                UpdateBlock(world, chunk, txp, ty, tz);
                UpdateBlock(world, chunk, tx, typ, tz);
                UpdateBlock(world, chunk, tx, ty, tzp);
                UpdateBlock(world, chunk, txp, typ, tz);
                UpdateBlock(world, chunk, tx, typ, tzp);
                UpdateBlock(world, chunk, txp, ty, tzp);
                UpdateBlock(world, chunk, txp, typ, tzp);
            }
        }

    }*/
}
