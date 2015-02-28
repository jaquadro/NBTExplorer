using System;
using System.Text.RegularExpressions;
using Substrate.Core;

namespace NBTExplorer.Model
{
    public class CubicRegionFile : RegionFile
    {
        private static Regex _namePattern = new Regex("r2\\.(-?[0-9]+)\\.(-?[0-9]+)\\.(-?[0-9]+)\\.mc[ar]$");

        private const int _sectorBytes = 256;
        private static byte[] _emptySector = new byte[_sectorBytes];

        public CubicRegionFile (string path)
            : base(path)
        { }

        protected override int SectorBytes
        {
            get { return _sectorBytes; }
        }

        protected override byte[] EmptySector
        {
            get { return _emptySector; }
        }

        public override RegionKey parseCoordinatesFromName ()
        {
            int x = 0;
            int z = 0;

            Match match = _namePattern.Match(fileName);
            if (!match.Success) {
                return RegionKey.InvalidRegion;
            }

            x = Convert.ToInt32(match.Groups[1].Value);
            z = Convert.ToInt32(match.Groups[3].Value);

            return new RegionKey(x, z);
        }
    }
}
