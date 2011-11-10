using System;

namespace Substrate.Data
{
    /// <summary>
    /// An interface of basic manipulations on an abstract data store for map data.
    /// </summary>
    public interface IMapManager
    {
        /// <summary>
        /// Gets a <see cref="Map"/> object for the given map id from the underlying data store.
        /// </summary>
        /// <param name="id">The id of a map data resource.</param>
        /// <returns>A <see cref="Map"/> object for the given map id, or <c>null</c> if the map doesn't exist.</returns>
        Map GetMap (int id);

        /// <summary>
        /// Saves a <see cref="Map"/> object's data back to the underlying data store for the given map id.
        /// </summary>
        /// <param name="id">The id of the map to write back data for.</param>
        /// <param name="map">The <see cref="Map"/> object containing data to write back.</param>
        void SetMap (int id, Map map);

        /// <summary>
        /// Checks if a map exists in the underlying data store.
        /// </summary>
        /// <param name="id">The id of the map to look up.</param>
        /// <returns>True if map data was found; false otherwise.</returns>
        bool MapExists (int id);

        /// <summary>
        /// Deletes a map with the given id from the underlying data store.
        /// </summary>
        /// <param name="id">The id of the map to delete.</param>
        void DeleteMap (int id);
    }
}
