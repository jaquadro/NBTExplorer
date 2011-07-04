using System;
using System.Collections.Generic;

namespace Substrate.Core
{
    /// <summary>
    /// A callback function to open a world and return it as an instance of a concrete derivative of <see cref="NbtWorld"/>.
    /// </summary>
    /// <param name="path">The path to the directory of the world to open.</param>
    /// <returns>An instance of a concrete derivative of <see cref="NbtWorld"/>.</returns>
    public delegate NbtWorld OpenWorldCallback (string path);

    /// <summary>
    /// Event arugments and response data for any handlers trying to determine if they can open a given world.
    /// </summary>
    public class OpenWorldEventArgs : EventArgs
    {
        private List<OpenWorldCallback> _handlers;
        private string _path;

        /// <summary>
        /// Create a new instance of event arguments.
        /// </summary>
        /// <param name="path">The path to the directory of a world.</param>
        public OpenWorldEventArgs (string path)
            : base()
        {
            _path = path;
            _handlers = new List<OpenWorldCallback>();
        }

        /// <summary>
        /// Gets the path to the directory of a world being investigated.
        /// </summary>
        public string Path
        {
            get { return _path; }
        }

        /// <summary>
        /// Adds an <see cref="OpenWorldCallback"/> delegate that can open a world and return a corresponding <see cref="NbtWorld"/> object.
        /// </summary>
        /// <param name="callback">The delegate to return to the code that raised the event.</param>
        public void AddHandler (OpenWorldCallback callback)
        {
            _handlers.Add(callback);
        }

        internal int HandlerCount
        {
            get { return _handlers.Count; }
        }

        internal ICollection<OpenWorldCallback> Handlers
        {
            get { return _handlers; }
        }
    }
}
