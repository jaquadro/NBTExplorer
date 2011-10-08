using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Core
{
    /// <summary>
    /// Provides a virtual deep copy capability to implementors.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICopyable <T>
    {
        /// <summary>
        /// Performs a virtual deep copy of the object instance.
        /// </summary>
        /// <returns>An independent copy of the object instance.</returns>
        T Copy ();
    }
}
