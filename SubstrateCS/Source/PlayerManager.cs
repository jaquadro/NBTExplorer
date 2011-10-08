using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Substrate.Core;
using Substrate.Nbt;
using System.Text.RegularExpressions;
using System.Collections;

namespace Substrate
{
    /// <summary>
    /// Functions to manage multiple <see cref="Player"/> entities and files in multiplayer settings.
    /// </summary>
    /// <remarks>This manager is intended for player files stored in standard compressed NBT format.</remarks>
    public class PlayerManager : IPlayerManager, IEnumerable<Player>
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
                Player p = new Player().LoadTreeSafe(GetPlayerTree(name).Root);
                p.Name = name;
                return p;
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
        /// Saves a <see cref="Player"/> object's data back to file given the name set in the <see cref="Player"/> object.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> object containing the data to write back.</param>
        /// <exception cref="PlayerIOException">Thrown when the manager cannot write out the player.</exception>
        public void SetPlayer (Player player)
        {
            SetPlayer(player.Name, player);
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

        #region IEnumerable<Player> Members

        /// <summary>
        /// Gets an enumerator that iterates through all the chunks in the world.
        /// </summary>
        /// <returns>An enumerator for this manager.</returns>
        public IEnumerator<Player> GetEnumerator ()
        {
            return new Enumerator(this);
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
        {
            return new Enumerator(this);
        }

        #endregion

        private class Enumerator : IEnumerator<Player>
        {
            protected PlayerManager _pm;
            protected Queue<string> _names;

            protected Player _curPlayer;

            public Enumerator (PlayerManager cfm)
            {
                _pm = cfm;
                _names = new Queue<string>();

                if (!Directory.Exists(_pm._playerPath)) {
                    throw new DirectoryNotFoundException();
                }

                Reset();
            }

            public bool MoveNext ()
            {
                if (_names.Count == 0) {
                    return false;
                }

                string name = _names.Dequeue();
                _curPlayer = _pm.GetPlayer(name);
                _curPlayer.Name = name;

                return true;
            }

            public void Reset ()
            {
                _names.Clear();
                _curPlayer = null;

                string[] files = Directory.GetFiles(_pm._playerPath);
                foreach (string file in files) {
                    string basename = Path.GetFileName(file);

                    if (!ParseFileName(basename)) {
                        continue;
                    }

                    _names.Enqueue(PlayerFile.NameFromFilename(basename));
                }
            }

            void IDisposable.Dispose () { }

            object IEnumerator.Current
            {
                get
                {
                    return Current;
                }
            }

            Player IEnumerator<Player>.Current
            {
                get
                {
                    return Current;
                }
            }

            public Player Current
            {
                get
                {
                    if (_curPlayer == null) {
                        throw new InvalidOperationException();
                    }
                    return _curPlayer;
                }
            }

            private bool ParseFileName (string filename)
            {
                Match match = _namePattern.Match(filename);
                if (!match.Success) {
                    return false;
                }

                return true;
            }

            protected static Regex _namePattern = new Regex(".+\\.dat$");
        }
    }
}
