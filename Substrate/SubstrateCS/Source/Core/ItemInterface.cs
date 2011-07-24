using System;

namespace Substrate.Core
{
    /// <summary>
    /// An interface that exposes an <see cref="ItemCollection"/> for the object.
    /// </summary>
    public interface IItemContainer
    {
        /// <summary>
        /// Gets an <see cref="ItemCollection"/> associated with the object.
        /// </summary>
        ItemCollection Items { get; }
    }
}
