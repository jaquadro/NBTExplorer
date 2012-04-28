using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Core
{

    public interface IRegionContainer
    {
        /// <summary>
        /// Determines if a region exists at the given coordinates.
        /// </summary>
        /// <param name="rx">The global X-coordinate of a region.</param>
        /// <param name="rz">The global Z-coordinate of a region.</param>
        /// <returns>True if a region exists at the given global region coordinates; false otherwise.</returns>
        bool RegionExists (int rx, int rz);

        /// <summary>
        /// Gets an <see cref="IRegion"/> for the given region filename.
        /// </summary>
        /// <param name="filename">The filename of the region to get.</param>
        /// <returns>A <see cref="IRegion"/> corresponding to the coordinates encoded in the filename.</returns>
        IRegion GetRegion (int rx, int rz);

        /// <summary>
        /// Creates a new empty region at the given coordinates, if no region exists.
        /// </summary>
        /// <param name="rx">The global X-coordinate of a region.</param>
        /// <param name="rz">The global Z-coordinate of a region.</param>
        /// <returns>A new empty <see cref="IRegion"/> object for the given coordinates, or an existing <see cref="IRegion"/> if one exists.</returns>
        IRegion CreateRegion (int rx, int rz);

        /// <summary>
        /// Deletes a region at the given coordinates.
        /// </summary>
        /// <param name="rx">The global X-coordinate of a region.</param>
        /// <param name="rz">The global Z-coordinate of a region.</param>
        /// <returns>True if a region was deleted; false otherwise.</returns>
        bool DeleteRegion (int rx, int rz);
    }

    public interface IRegionManager : IRegionContainer, IEnumerable<IRegion>
    {

    }
}
