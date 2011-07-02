using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Substrate.Core;
using Substrate.Nbt;

namespace Substrate
{
    /// <summary>
    /// Functions to manage multiple <see cref="Player"/> entities and files in multiplayer settings.
    /// </summary>
    /// <remarks>This manager is intended for player files stored in standard compressed NBT format.</remarks>
    public class PlayerManager : IPlayerManager
    {
        private string _playerPath;

        /// <summary>
        /// Create a new <see cref="PlayerManager"/> for a given file path.
        /// </summary>
        /// <param name="playerDir">Path to a directory containing player data files.</param>
        public PlayerManager (string playerDir)
        {
            _playerPath = playerDir;
        }

        /// <summary>
        /// Gets a <see cref="PlayerFile"/> representing the backing NBT data stream.
        /// </summary>
        /// <param name="name">The name of the player to fetch.</param>
        /// <returns>A <see cref="PlayerFile"/> for the given player.</returns>
        protected PlayerFile GetPlayerFile (string name)
        {
            return new PlayerFile(_playerPath, name);
        }

        /// <summary>
        /// Gets a raw <see cref="NbtTree"/> of data for the given player.
        /// </summary>
        /// <param name="name">The name of the player to fetch.</param>
        /// <returns>An <see cref="NbtTree"/> containing the given player's raw data.</returns>
        /// <exception cref="NbtIOException">Thrown when the manager cannot read in an NBT data stream.</exception>
        public NbtTree GetPlayerTree (string name)
        {
            PlayerFile pf = GetPlayerFile(name);
            Stream nbtstr = pf.GetDataInputStream();
            if (nbtstr == null) {
                throw new NbtIOException("Failed to initialize NBT data stream for input.");
            }

            return new NbtTree(nbtstr);
        }

        /// <summary>
        /// Saves a raw <see cref="NbtTree"/> representing a player to the given player's file.
        /// </summary>
        /// <param name="name">The name of the player to write data to.</param>
        /// <param name="tree">The player's data as an <see cref="NbtTree"/>.</param>
        /// <exception cref="NbtIOException">Thrown when the manager cannot initialize an NBT data stream for output.</exception>
        public void SetPlayerTree (string name, NbtTree tree)
        {
            PlayerFile pf = GetPlayerFile(name);
            Stream zipstr = pf.GetDataOutputStream();
            if (zipstr == null) {
                throw new NbtIOException("Failed to initialize NBT data stream for output.");
            }

            tree.WriteTo(zipstr);
            zipstr.Close();
        }

        /// <summary>
        /// Gets a <see cref="Player"/> object for the given player.
        /// </summary>
        /// <param name="name">The name of the player to fetch.</param>
        /// <returns>A <see cref="Player"/> object for the given player, or null if the player could not be found.</returns>
        /// <exception cref="PlayerIOException">Thrown when the manager cannot read in a player that should exist.</exception>
        public Player GetPlayer (string name)
        {
            if (!PlayerExists(name)) {
                return null;
            }

            try {
                return new Player().LoadTreeSafe(GetPlayerTree(name).Root);
            }
            catch (Exception ex) {
                PlayerIOException pex = new PlayerIOException("Could not load player", ex);
                pex.Data["PlayerName"] = name;
                throw pex;
            }
        }

        /// <summary>
        /// Saves a <see cref="Player"/> object's data back to the given player's file.
        /// </summary>
        /// <param name="name">The name of the player to write back to.</param>
        /// <param name="player">The <see cref="Player"/> object containing data to write back.</param>
        /// <exception cref="PlayerIOException">Thrown when the manager cannot write out the player.</exception>
        public void SetPlayer (string name, Player player)
        {
            try {
                SetPlayerTree(name, new NbtTree(player.BuildTree() as TagNodeCompound));
            }
            catch (Exception ex) {
                PlayerIOException pex = new PlayerIOException("Could not save player", ex);
                pex.Data["PlayerName"] = name;
                throw pex;
            }
        }

        /// <summary>
        /// Checks if data for a player with the given name exists.
        /// </summary>
        /// <param name="name">The name of the player to look up.</param>
        /// <returns>True if player data was found; false otherwise.</returns>
        public bool PlayerExists (string name)
        {
            return new PlayerFile(_playerPath, name).Exists();
        }

        /// <summary>
        /// Deletes a player with the given name from the underlying data store.
        /// </summary>
        /// <param name="name">The name of the player to delete.</param>
        /// <exception cref="PlayerIOException">Thrown when the manager cannot delete the player.</exception>
        public void DeletePlayer (string name)
        {
            try {
                new PlayerFile(_playerPath, name).Delete();
            }
            catch (Exception ex) {
                PlayerIOException pex = new PlayerIOException("Could not save player", ex);
                pex.Data["PlayerName"] = name;
                throw pex;
            }
        }
    }
}
