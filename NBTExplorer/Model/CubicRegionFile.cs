using Substrate.Core;

namespace NBTExplorer.Model
{
    public class CubicRegionFile : RegionFile
    {
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
    }
}
