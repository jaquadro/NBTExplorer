using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Core
{
    public interface IRegionContainer
    {
        bool RegionExists (int rx, int rz);

        Region GetRegion (int rx, int rz);
        Region CreateRegion (int rx, int rz);

        bool DeleteRegion (int rx, int rz);
    }

    public interface IRegionManager : IRegionContainer, IEnumerable<Region>
    {
    }
}
