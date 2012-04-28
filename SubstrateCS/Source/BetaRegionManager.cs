using System;
using System.IO;
using Substrate.Core;

namespace Substrate
{
    public class BetaRegionManager : RegionManager
    {
        public BetaRegionManager (string regionDir, ChunkCache cache)
            : base(regionDir, cache)
        {
        }

        protected override IRegion CreateRegionCore (int rx, int rz)
        {
            return new BetaRegion(this, _chunkCache, rx, rz);
        }

        protected override RegionFile CreateRegionFileCore (int rx, int rz)
        {
            string fp = "r." + rx + "." + rz + ".mcr";
            return new RegionFile(Path.Combine(_regionPath, fp));
        }

        protected override void DeleteRegionCore (IRegion region)
        {
            BetaRegion r = region as BetaRegion;
            if (r != null) {
                r.Dispose();
            }
        }

        public override IRegion GetRegion (string filename)
        {
            int rx, rz;
            if (!BetaRegion.ParseFileName(filename, out rx, out rz)) {
                throw new ArgumentException("Malformed region file name: " + filename, "filename");
            }

            return GetRegion(rx, rz);
        }
    }
}
