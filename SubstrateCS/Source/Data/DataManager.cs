using System;
using System.Collections.Generic;

namespace Substrate.Data
{
    /// <summary>
    /// Provides a common interface for managing additional data resources in a world.
    /// </summary>
    public abstract class DataManager
    {
        /// <summary>
        /// Gets or sets the id of the next map to be created.
        /// </summary>
        public virtual int CurrentMapId 
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets an <see cref="IMapManager"/> for managing <see cref="Map"/> data resources.
        /// </summary>
        public IMapManager Maps 
        {
            get { return GetMapManager(); }
        }

        /// <summary>
        /// Gets an <see cref="IMapManager"/> for managing <see cref="Map"/> data resources.
        /// </summary>
        /// <returns>An <see cref="IMapManager"/> instance appropriate for the concrete <see cref="DataManager"/> instance.</returns>
        protected virtual IMapManager GetMapManager () 
        {
            return null;
        }

        /// <summary>
        /// Saves any metadata required by the world for managing data resources.
        /// </summary>
        /// <returns><c>true</c> on success, or <c>false</c> if data could not be saved.</returns>
        public virtual bool Save ()
        {
            return true;
        }
    }
    
    
}
