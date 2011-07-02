using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Core
{
    /// <summary>
    /// An interface of basic manipulations on an abstract data store for player data.
    /// </summary>
    public interface IPlayerManager
    {
        /// <summary>
        /// Gets a <see cref="Player"/> object for the given player from the underlying data store.
        /// </summary>
        /// <param name="name">The name of the player to fetch.</param>
        /// <returns>A <see cref="Player"/> object for the given player, or null if the player could not be found.</returns>
        Player GetPlayer (string name);

        /// <summary>
        /// Saves a <see cref="Player"/> object's data back to the underlying data store for the given player.
        /// </summary>
        /// <param name="name">The name of the player to write back data for.</param>
        /// <param name="player">The <see cref="Player"/> object containing data to write back.</param>
        void SetPlayer (string name, Player player);

        /// <summary>
        /// Checks if a player exists in the underlying data store.
        /// </summary>
        /// <param name="name">The name of the player to look up.</param>
        /// <returns>True if player data was found; false otherwise.</returns>
        bool PlayerExists (string name);

        /// <summary>
        /// Deletes a player with the given name from the underlying data store.
        /// </summary>
        /// <param name="name">The name of the player to delete.</param>
        void DeletePlayer (string name);
    }
}
