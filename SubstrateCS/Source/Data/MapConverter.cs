using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace Substrate.Data
{
    /// <summary>
    /// Represents a range of color index values that pertains to an individual group of blocks.
    /// </summary>
    public enum ColorGroup
    {
        Unexplored = 0,
        Grass = 1,
        Sand = 2,
        Other = 3,
        Lava = 4,
        Ice = 5,
        Iron = 6,
        Leaves = 7,
        Snow = 8,
        Clay = 9,
        Dirt = 10,
        Stone = 11,
        Water = 12,
        Wood = 13,
    }

    /// <summary>
    /// A utility class for converting <see cref="Map"/> colors and data.
    /// </summary>
    public class MapConverter
    {
        private static Color[] _defaultColorIndex;

        private Color[] _colorIndex;
        private ColorGroup[] _blockIndex;

        private int _groupSize = 4;

        private Vector3[] _labIndex;

        /// <summary>
        /// Creates a new <see cref="MapConverter"/> with a Minecraft-default color-index and block-index.
        /// </summary>
        public MapConverter ()
        {
            _colorIndex = new Color[256];
            _labIndex = new Vector3[256];
            _defaultColorIndex.CopyTo(_colorIndex, 0);

            RefreshColorCache();

            // Setup default block index
            _blockIndex = new ColorGroup[4096];
            for (int i = 0; i < _blockIndex.Length; i++) {
                _blockIndex[i] = ColorGroup.Other;
            }

            _blockIndex[BlockInfo.Grass.ID] = ColorGroup.Grass;
            _blockIndex[BlockInfo.TallGrass.ID] = ColorGroup.Grass;
            _blockIndex[BlockInfo.Sand.ID] = ColorGroup.Sand;
            _blockIndex[BlockInfo.Gravel.ID] = ColorGroup.Sand;
            _blockIndex[BlockInfo.Sandstone.ID] = ColorGroup.Sand;
            _blockIndex[BlockInfo.Lava.ID] = ColorGroup.Lava;
            _blockIndex[BlockInfo.StationaryLava.ID] = ColorGroup.Lava;
            _blockIndex[BlockInfo.Ice.ID] = ColorGroup.Ice;
            _blockIndex[BlockInfo.Leaves.ID] = ColorGroup.Leaves;
            _blockIndex[BlockInfo.YellowFlower.ID] = ColorGroup.Leaves;
            _blockIndex[BlockInfo.RedRose.ID] = ColorGroup.Leaves;
            _blockIndex[BlockInfo.Snow.ID] = ColorGroup.Snow;
            _blockIndex[BlockInfo.SnowBlock.ID] = ColorGroup.Snow;
            _blockIndex[BlockInfo.ClayBlock.ID] = ColorGroup.Clay;
            _blockIndex[BlockInfo.Dirt.ID] = ColorGroup.Dirt;
            _blockIndex[BlockInfo.Stone.ID] = ColorGroup.Stone;
            _blockIndex[BlockInfo.Cobblestone.ID] = ColorGroup.Stone;
            _blockIndex[BlockInfo.CoalOre.ID] = ColorGroup.Stone;
            _blockIndex[BlockInfo.IronOre.ID] = ColorGroup.Stone;
            _blockIndex[BlockInfo.GoldOre.ID] = ColorGroup.Stone;
            _blockIndex[BlockInfo.DiamondOre.ID] = ColorGroup.Stone;
            _blockIndex[BlockInfo.RedstoneOre.ID] = ColorGroup.Stone;
            _blockIndex[BlockInfo.LapisOre.ID] = ColorGroup.Stone;
            _blockIndex[BlockInfo.Water.ID] = ColorGroup.Water;
            _blockIndex[BlockInfo.StationaryWater.ID] = ColorGroup.Water;
            _blockIndex[BlockInfo.Wood.ID] = ColorGroup.Wood;
        }

        /// <summary>
        /// Gets or sets the number of color levels within each color group.  The Minecraft default is 4.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the property is assigned a non-positive value.</exception>
        public int ColorGroupSize
        {
            get { return _groupSize; }
            set
            {
                if (value <= 0) {
                    throw new ArgumentOutOfRangeException("The ColorGroupSize property must be a positive number.");
                }
                _groupSize = value;
            }
        }

        /// <summary>
        /// Gets the color index table, used to translate map color index values to RGB color values.
        /// </summary>
        public Color[] ColorIndex
        {
            get { return _colorIndex; }
        }

        /// <summary>
        /// Gets the block index table, used to translate block IDs to <see cref="ColorGroup"/>s that represent them.
        /// </summary>
        public ColorGroup[] BlockIndex
        {
            get { return _blockIndex; }
        }

        /// <summary>
        /// Returns the baseline color index associated with the <see cref="ColorGroup"/> currently representing the given block ID.
        /// </summary>
        /// <param name="blockId">The ID of a block.</param>
        /// <returns>A color index value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="blockId"/> is out of its normal range.</exception>
        public int BlockToColorIndex (int blockId)
        {
            return BlockToColorIndex(blockId, 0);
        }

        /// <summary>
        /// Returns a color index associated with the <see cref="ColorGroup"/> currently representing the given block ID and based on the given level.
        /// </summary>
        /// <param name="blockId">The ID of a block.</param>
        /// <param name="level">The color level to select from within the derived <see cref="ColorGroup"/>.</param>
        /// <returns>A color index value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when either <paramref name="blockId"/> or <paramref name="level"/> are out of their normal ranges.</exception>
        public int BlockToColorIndex (int blockId, int level) {
            if (level < 0 || level >= _groupSize) {
                throw new ArgumentOutOfRangeException("level", level, "Argument 'level' must be in range [0, " + (_groupSize - 1) + "]");
            }
            if (blockId < 0 || blockId >= 4096) {
                throw new ArgumentOutOfRangeException("blockId");
            }

            return (int)_blockIndex[blockId] * _groupSize + level;
        }

        /// <summary>
        /// Returns a <see cref="Color"/> value assocated with the <see cref="ColorGroup"/> currently representing the given block ID.
        /// </summary>
        /// <param name="blockId">The ID of a block.</param>
        /// <returns>A <see cref="Color"/> value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="blockId"/> is out of its normal range.</exception>
        /// <exception cref="InvalidOperationException">Thrown when <paramref name="blockId"/> maps to an invalid color index.</exception>
        public Color BlockToColor (int blockId)
        {
            return BlockToColor(blockId, 0);
        }

        /// <summary>
        /// Returns a <see cref="Color"/> value associated with the <see cref="ColorGroup"/> currently representing the given block ID and based on the given level.
        /// </summary>
        /// <param name="blockId">The ID of a block.</param>
        /// <param name="level">The color level to select from within the derived <see cref="ColorGroup"/>.</param>
        /// <returns>A <see cref="Color"/> value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when either <paramref name="blockId"/> or <paramref name="level"/> are out of their normal ranges.</exception>
        /// <exception cref="InvalidOperationException">Thrown when <paramref name="blockId"/> maps to an invalid color index.</exception>
        public Color BlockToColor (int blockId, int level)
        {
            int ci = BlockToColorIndex(blockId, level);
            if (ci < 0 || ci >= 256) {
                throw new InvalidOperationException("The specified Block ID mapped to an invalid color index.");
            }

            return _colorIndex[ci];
        }

        /// <summary>
        /// Returns the <see cref="ColorGroup"/> that a particular color index is part of.
        /// </summary>
        /// <param name="colorIndex">A color index value.</param>
        /// <returns>A <see cref="ColorGroup"/> value.</returns>
        public ColorGroup ColorIndexToGroup (int colorIndex)
        {
            int group = colorIndex / _groupSize;

            try {
                return (ColorGroup)group;
            }
            catch {
                return ColorGroup.Other;
            }
        }

        /// <summary>
        /// Returns the baseline color index within a particular <see cref="ColorGroup"/>.
        /// </summary>
        /// <param name="group">A <see cref="ColorGroup"/> value.</param>
        /// <returns>The baseline (level = 0) color index value for the given <see cref="ColorGroup"/>.</returns>
        public int GroupToColorIndex (ColorGroup group)
        {
            return GroupToColorIndex(group, 0);
        }

        /// <summary>
        /// Returns the color index for a given <see cref="ColorGroup"/> and group level.
        /// </summary>
        /// <param name="group">A <see cref="ColorGroup"/> value.</param>
        /// <param name="level">A level value within the <see cref="ColorGroup"/>.</param>
        /// <returns>The color index value for the given <see cref="ColorGroup"/> and group level.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the <paramref name="level"/> is out of range with respect to the current <see cref="ColorGroupSize"/> parameter.</exception>
        public int GroupToColorIndex (ColorGroup group, int level)
        {
            if (level < 0 || level >= _groupSize) {
                throw new ArgumentOutOfRangeException("level", level, "Argument 'level' must be in range [0, " + (_groupSize - 1) + "]");
            }

            return (int)group * _groupSize + level;
        }

        /// <summary>
        /// Returns the baseline <see cref="Color"/> within a particular <see cref="ColorGroup"/>.
        /// </summary>
        /// <param name="group">A <see cref="ColorGroup"/> value.</param>
        /// <returns>The baseline (level = 0) <see cref="Color"/> for the given <see cref="ColorGroup"/>.</returns>
        public Color GroupToColor (ColorGroup group)
        {
            return GroupToColor(group, 0);
        }

        /// <summary>
        /// Returns the <see cref="Color"/> for a given <see cref="ColorGroup"/> and group level.
        /// </summary>
        /// <param name="group">A <see cref="ColorGroup"/> value.</param>
        /// <param name="level">A level value within the <see cref="ColorGroup"/> and group level.</param>
        /// <returns>The <see cref="Color"/> for the given <see cref="ColorGroup"/> and group level.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the <paramref name="level"/> is out of range with respect to the current <see cref="ColorGroupSize"/> parameter.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the <paramref name="group"/> and <paramref name="level"/> map to an invalid color index.</exception>
        public Color GroupToColor (ColorGroup group, int level)
        {
            int ci = GroupToColorIndex(group, level);
            if (ci < 0 || ci >= 256) {
                throw new InvalidOperationException("The specified group mapped to an invalid color index.");
            }

            return _colorIndex[ci];
        }

        /// <summary>
        /// Rebuilds the internal color conversion tables.  Should be called after modifying the <see cref="ColorIndex"/> table.
        /// </summary>
        public void RefreshColorCache ()
        {
            for (int i = 0; i < _colorIndex.Length; i++) {
                _labIndex[i] = RgbToLab(_colorIndex[i]);
            }
        }

        /// <summary>
        /// Given a <see cref="Color"/>, returns the index of the closest matching color from the color index table.
        /// </summary>
        /// <param name="color">The source <see cref="Color"/>.</param>
        /// <returns>The closest matching color index value.</returns>
        /// <remarks>This method performs color comparisons in the CIELAB color space, to find the best match according to human perception.</remarks>
        public int NearestColorIndex (Color color)
        {
            double min = double.MaxValue;
            int minIndex = 0;

            Vector3 cr = RgbToLab(color);

            for (int i = 0; i < _colorIndex.Length; i++) {
                if (_colorIndex[i].A == 0) {
                    continue;
                }

                double x = cr.X - _labIndex[i].X;
                double y = cr.Y - _labIndex[i].Y;
                double z = cr.Z - _labIndex[i].Z;

                double err = x * x + y * y + z * z;
                if (err < min) {
                    min = err;
                    minIndex = i;
                }
            }

            return minIndex;
        }

        /// <summary>
        /// Given a <see cref="Color"/>, returns the cloest matching <see cref="Color"/> from the color index table.
        /// </summary>
        /// <param name="color">The source <see cref="Color"/>.</param>
        /// <returns>The closest matching <see cref="Color"/>.</returns>
        /// <remarks>This method performs color comparisons in the CIELAB color space, to find the best match according to human perception.</remarks>
        public Color NearestColor (Color color)
        {
            return _colorIndex[NearestColorIndex(color)];
        }

        /// <summary>
        /// Fills a <see cref="Map"/>'s color data using nearest-matching colors from a source <see cref="Bitmap"/>.
        /// </summary>
        /// <param name="map">The <see cref="Map"/> to modify.</param>
        /// <param name="bmp">The source <see cref="Bitmap"/>.</param>
        /// <exception cref="InvalidOperationException">Thrown when the <paramref name="map"/> and <paramref name="bmp"/> objects have different dimensions.</exception>
        public void BitmapToMap (Map map, Bitmap bmp) 
        {
            if (map.Width != bmp.Width || map.Height != bmp.Height) {
                throw new InvalidOperationException("The source map and bitmap must have the same dimensions.");
            }

            for (int x = 0; x < map.Width; x++) {
                for (int z = 0; z < map.Height; z++) {
                    Color c = bmp.GetPixel(x, z);
                    map[x, z] = (byte)NearestColorIndex(c);
                }
            }
        }

        /// <summary>
        /// Creates a 32bpp <see cref="Bitmap"/> from a <see cref="Map"/>.
        /// </summary>
        /// <param name="map">The source <see cref="Map"/> object.</param>
        /// <returns>A 32bpp <see cref="Bitmap"/> with the same dimensions and pixel data as the source <see cref="Map"/>.</returns>
        public Bitmap MapToBitmap (Map map)
        {
            Bitmap bmp = new Bitmap(map.Width, map.Height, PixelFormat.Format32bppArgb);

            for (int x = 0; x < map.Width; x++) {
                for (int z = 0; z < map.Height; z++) {
                    Color c = _colorIndex[map[x, z]];
                    bmp.SetPixel(x, z, c);
                }
            }

            return bmp;
        }

        private Vector3 RgbToXyz (Color color)
        {
            double r = color.R / 255.0;
            double g = color.G / 255.0;
            double b = color.B / 255.0;

            r = (r > 0.04045)
                ? Math.Pow((r + 0.055) / 1.055, 2.4)
                : r / 12.92;
            g = (g > 0.04045)
                ? Math.Pow((g + 0.055) / 1.055, 2.4)
                : g / 12.92;
            b = (b > 0.04045)
                ? Math.Pow((b + 0.055) / 1.055, 2.4)
                : b / 12.92;

            r *= 100;
            g *= 100;
            b *= 100;

            Vector3 xyz = new Vector3();

            xyz.X = r * 0.4124 + g * 0.3576 + b * 0.1805;
            xyz.Y = r * 0.2126 + g * 0.7152 + b * 0.0722;
            xyz.Z = r * 0.0193 + g * 0.1192 + b * 0.9505;

            return xyz;
        }

        private Vector3 XyzToLab (Vector3 xyz)
        {
            double x = xyz.X / 95.047;
            double y = xyz.Y / 100.0;
            double z = xyz.Z / 108.883;

            x = (x > 0.008856)
                ? Math.Pow(x, 1.0 / 3.0)
                : (7.787 * x) + (16.0 / 116.0);
            y = (y > 0.008856)
                ? Math.Pow(y, 1.0 / 3.0)
                : (7.787 * y) + (16.0 / 116.0);
            z = (z > 0.008856)
                ? Math.Pow(z, 1.0 / 3.0)
                : (7.787 * z) + (16.0 / 116.0);

            Vector3 lab = new Vector3();

            lab.X = (116 * y) - 16;
            lab.Y = 500 * (x - y);
            lab.Z = 200 * (y - z);

            return lab;
        }

        private Vector3 RgbToLab (Color rgb)
        {
            return XyzToLab(RgbToXyz(rgb));
        }

        static MapConverter ()
        {
            _defaultColorIndex = new Color[] {
                Color.FromArgb(0, 0, 0, 0),         // Unexplored
                Color.FromArgb(0, 0, 0, 0),
                Color.FromArgb(0, 0, 0, 0),
                Color.FromArgb(0, 0, 0, 0),
                Color.FromArgb(89, 125, 39),        // Grass
                Color.FromArgb(109, 153, 48),
                Color.FromArgb(127, 178, 56),
                Color.FromArgb(109, 153, 48),
                Color.FromArgb(174, 164, 115),      // Sand/Gravel
                Color.FromArgb(213, 201, 140),
                Color.FromArgb(247, 233, 163),
                Color.FromArgb(213, 201, 140),
                Color.FromArgb(117, 117, 117),      // Other
                Color.FromArgb(144, 144, 144),
                Color.FromArgb(167, 167, 167),
                Color.FromArgb(144, 144, 144),
                Color.FromArgb(180, 0, 0),          // Lava
                Color.FromArgb(220, 0, 0),
                Color.FromArgb(255, 0, 0),
                Color.FromArgb(220, 0, 0),
                Color.FromArgb(112, 112, 180),      // Ice
                Color.FromArgb(138, 138, 220),
                Color.FromArgb(160, 160, 255),
                Color.FromArgb(138, 138, 220),
                Color.FromArgb(117, 117, 117),      // Iron
                Color.FromArgb(144, 144, 144),
                Color.FromArgb(167, 167, 167),
                Color.FromArgb(144, 144, 144),
                Color.FromArgb(0, 87, 0),           // Leaves/Flowers
                Color.FromArgb(0, 106, 0),
                Color.FromArgb(0, 124, 0),
                Color.FromArgb(0, 106, 0),
                Color.FromArgb(180, 180, 180),      // Snow
                Color.FromArgb(220, 220, 220),
                Color.FromArgb(255, 255, 255),
                Color.FromArgb(220, 220, 220),
                Color.FromArgb(115, 118, 129),      // Clay
                Color.FromArgb(141, 144, 158),
                Color.FromArgb(164, 168, 184),
                Color.FromArgb(141, 144, 158),
                Color.FromArgb(129, 74, 33),        // Dirt
                Color.FromArgb(157, 91, 40),
                Color.FromArgb(183, 106, 47),
                Color.FromArgb(157, 91, 40),
                Color.FromArgb(79, 79, 79),         // Stone/Cobblestone/Ore
                Color.FromArgb(96, 96, 96),
                Color.FromArgb(112, 112, 112),
                Color.FromArgb(96, 96, 96),
                Color.FromArgb(45, 45, 180),        // Water
                Color.FromArgb(55, 55, 220),
                Color.FromArgb(64, 64, 255),
                Color.FromArgb(55, 55, 220),
                Color.FromArgb(73, 58, 35),         // Log/Tree/Wood
                Color.FromArgb(89, 71, 43),
                Color.FromArgb(104, 83, 50),
                Color.FromArgb(89, 71, 43),
            };
        }
    }
}
