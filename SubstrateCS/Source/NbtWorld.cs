using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Substrate.Core;
using Substrate.Nbt;
using Substrate.Data;

namespace Substrate
{
    /// <summary>
    /// An abstract representation of any conforming chunk-based world.
    /// </summary>
    /// <remarks><para>By default, NbtWorld registers handlers to check if a given world can be opened as an <see cref="AlphaWorld"/> or
    /// a <see cref="BetaWorld"/>, which are used by <see cref="NbtWorld"/>'s generic <see cref="Open(string)"/> method to automatically
    /// detect a world's type and open it.</para>
    /// <para>Advanced implementors can support loading other Nbt-compatible world formats by extending <see cref="NbtWorld"/> and registering
    /// an event handler with the <see cref="ResolveOpen"/> event, which will allow the generic <see cref="Open(string)"/> method to
    /// open worlds of the new format.</para></remarks>
    public abstract class NbtWorld
    {
        private const string _DATA_DIR = "data";

        private string _path;
        private string _dataDir;

        /// <summary>
        /// Creates a new instance of an <see cref="NbtWorld"/> object.
        /// </summary>
        protected NbtWorld () {
            _dataDir = _DATA_DIR;
        }

        /// <summary>
        /// Gets or sets the path to the directory containing the world.
        /// </summary>
        public string Path
        {
            get { return _path; }
            set { _path = value; }
        }

        /// <summary>
        /// Gets or sets the directory containing data resources, rooted in the world directory.
        /// </summary>
        public string DataDirectory
        {
            get { return _dataDir; }
            set { _dataDir = value; }
        }

        /// <summary>
        /// Gets a reference to this world's <see cref="Level"/> object.
        /// </summary>
        public abstract Level Level { get; }

        /// <summary>
        /// Gets an <see cref="IBlockManager"/> for the default dimension.
        /// </summary>
        /// <returns>An <see cref="IBlockManager"/> tied to the default dimension in this world.</returns>
        public IBlockManager GetBlockManager ()
        {
            return GetBlockManagerVirt(Dimension.DEFAULT);
        }

        /// <summary>
        /// Gets an <see cref="IBlockManager"/> for the given dimension.
        /// </summary>
        /// <param name="dim">The id of the dimension to look up.</param>
        /// <returns>An <see cref="IBlockManager"/> tied to the given dimension in this world.</returns>
        public IBlockManager GetBlockManager (int dim)
        {
            return GetBlockManagerVirt(dim);
        }

        public IBlockManager GetBlockManager (string dim)
        {
            return GetBlockManagerVirt(dim);
        }

        /// <summary>
        /// Gets an <see cref="IChunkManager"/> for the default dimension.
        /// </summary>
        /// <returns>An <see cref="IChunkManager"/> tied to the default dimension in this world.</returns>
        public IChunkManager GetChunkManager ()
        {
            return GetChunkManagerVirt(Dimension.DEFAULT);
        }

        /// <summary>
        /// Gets an <see cref="IChunkManager"/> for the given dimension.
        /// </summary>
        /// <param name="dim">The id of the dimension to look up.</param>
        /// <returns>An <see cref="IChunkManager"/> tied to the given dimension in this world.</returns>
        public IChunkManager GetChunkManager (int dim)
        {
            return GetChunkManagerVirt(dim);
        }

        public IChunkManager GetChunkManager (string dim)
        {
            return GetChunkManagerVirt(dim);
        }

        /// <summary>
        /// Gets an <see cref="IPlayerManager"/> for maanging players on multiplayer worlds.
        /// </summary>
        /// <returns>An <see cref="IPlayerManager"/> for this world.</returns>
        public IPlayerManager GetPlayerManager ()
        {
            return GetPlayerManagerVirt();
        }

        /// <summary>
        /// Gets a <see cref="DataManager"/> for managing data resources, such as maps.
        /// </summary>
        /// <returns>A <see cref="DataManager"/> for this world.</returns>
        public DataManager GetDataManager ()
        {
            return GetDataManagerVirt();
        }

        /// <summary>
        /// Attempts to determine the best matching world type of the given path, and open the world as that type.
        /// </summary>
        /// <param name="path">The path to the directory containing the world.</param>
        /// <returns>A concrete <see cref="NbtWorld"/> type, or null if the world cannot be opened or is ambiguos.</returns>
        public static NbtWorld Open (string path)
        {
            if (ResolveOpen == null) {
                return null;
            }

            OpenWorldEventArgs eventArgs = new OpenWorldEventArgs(path);
            ResolveOpen(null, eventArgs);

            if (eventArgs.HandlerCount != 1) {
                return null;
            }


            foreach (OpenWorldCallback callback in eventArgs.Handlers) {
                return callback(path);
            }

            return null;
        }

        /// <summary>
        /// Saves the world's <see cref="Level"/> data, and any <see cref="IChunk"/> objects known to have unsaved changes.
        /// </summary>
        public abstract void Save ();

        /// <summary>
        /// Raised when <see cref="Open"/> is called, used to find a concrete <see cref="NbtWorld"/> type that can open the world.
        /// </summary>
        protected static event EventHandler<OpenWorldEventArgs> ResolveOpen;

        #region Covariant Return-Type Helpers

        /// <summary>
        /// Virtual implementor of <see cref="GetBlockManager(int)"/>.
        /// </summary>
        /// <param name="dim">The given dimension to fetch an <see cref="IBlockManager"/> for.</param>
        /// <returns>An <see cref="IBlockManager"/> for the given dimension in the world.</returns>
        protected abstract IBlockManager GetBlockManagerVirt (int dim);

        /// <summary>
        /// Virtual implementor of <see cref="GetChunkManager(int)"/>.
        /// </summary>
        /// <param name="dim">The given dimension to fetch an <see cref="IChunkManager"/> for.</param>
        /// <returns>An <see cref="IChunkManager"/> for the given dimension in the world.</returns>
        protected abstract IChunkManager GetChunkManagerVirt (int dim);

        protected virtual IBlockManager GetBlockManagerVirt (string dim)
        {
            throw new NotImplementedException();
        }

        protected virtual IChunkManager GetChunkManagerVirt (string dim)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Virtual implementor of <see cref="GetPlayerManager"/>.
        /// </summary>
        /// <returns>An <see cref="IPlayerManager"/> for the given dimension in the world.</returns>
        protected abstract IPlayerManager GetPlayerManagerVirt ();

        /// <summary>
        /// Virtual implementor of <see cref="GetDataManager"/>
        /// </summary>
        /// <returns>A <see cref="DataManager"/> for the given dimension in the world.</returns>
        protected virtual DataManager GetDataManagerVirt ()
        {
            throw new NotImplementedException();
        }

        #endregion

        static NbtWorld ()
        {
            ResolveOpen += AnvilWorld.OnResolveOpen;
            ResolveOpen += BetaWorld.OnResolveOpen;
            ResolveOpen += AlphaWorld.OnResolveOpen;
        }
    }
}
