using System;
using System.Text.RegularExpressions;
using Substrate.Core;
using Substrate.Nbt;

namespace Substrate
{
    public class AnvilRegion : Region
    {
        private static Regex _namePattern = new Regex("r\\.(-?[0-9]+)\\.(-?[0-9]+)\\.mca$");

        public AnvilRegion (AnvilRegionManager rm, ChunkCache cache, int rx, int rz)
            : base(rm, cache, rx, rz)
        {
        }

        /// <inherits />
        public override string GetFileName ()
        {
            return "r." + _rx + "." + _rz + ".mca";

        }

        /// <inherits />
        public override string GetFilePath ()
        {
            return System.IO.Path.Combine(_regionMan.GetRegionPath(), GetFileName());
        }

        /// <summary>
        /// Tests if the given filename conforms to the general naming pattern for any region.
        /// </summary>
        /// <param name="filename">The filename to test.</param>
        /// <returns>True if the filename is a valid region name; false if it does not conform to the pattern.</returns>
        public static bool TestFileName (string filename)
        {
            Match match = _namePattern.Match(filename);
            if (!match.Success) {
                return false;
            }

            return true;
        }

        public static bool ParseFileName (string filename, out int x, out int z)
        {
            x = 0;
            z = 0;

            Match match = _namePattern.Match(filename);
            if (!match.Success) {
                return false;
            }

            x = Convert.ToInt32(match.Groups[1].Value);
            z = Convert.ToInt32(match.Groups[2].Value);
            return true;
        }

        /// <summary>
        /// Parses the given filename to extract encoded region coordinates.
        /// </summary>
        /// <param name="filename">The region filename to parse.</param>
        /// <param name="x">This parameter will contain the X-coordinate of a region.</param>
        /// <param name="z">This parameter will contain the Z-coordinate of a region.</param>
        /// <returns>True if the filename could be correctly parse; false otherwise.</returns>
        protected override bool ParseFileNameCore (string filename, out int x, out int z)
        {
            return ParseFileName(filename, out x, out z);
        }

        protected override IChunk CreateChunkCore (int cx, int cz)
        {
            return AnvilChunk.Create(cx, cz);
        }

        protected override IChunk CreateChunkVerifiedCore (NbtTree tree)
        {
            return AnvilChunk.CreateVerified(tree);
        }
    }
}
