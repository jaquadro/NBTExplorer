using Substrate.Core;
using System;
using System.Text.RegularExpressions;

namespace NBTExplorer.Model
{
    public class CubicRegionFile : RegionFile
    {
        private const int _sectorBytes = 256;
        private static readonly Regex _namePattern = new Regex("r2\\.(-?[0-9]+)\\.(-?[0-9]+)\\.(-?[0-9]+)\\.mc[ar]$");
        private static readonly byte[] _emptySector = new byte[_sectorBytes];

        public CubicRegionFile(string path)
            : base(path)
        {
        }

        protected override int SectorBytes => _sectorBytes;

        protected override byte[] EmptySector => _emptySector;

        public override RegionKey parseCoordinatesFromName()
        {
            var x = 0;
            var z = 0;

            var match = _namePattern.Match(fileName);
            if (!match.Success) return RegionKey.InvalidRegion;

            x = Convert.ToInt32(match.Groups[1].Value);
            z = Convert.ToInt32(match.Groups[3].Value);

            return new RegionKey(x, z);
        }
    }
}