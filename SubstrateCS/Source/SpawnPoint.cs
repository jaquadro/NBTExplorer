using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate
{
    /// <summary>
    /// Represents the spawn point of a player or world.
    /// </summary>
    /// <remarks><see cref="SpawnPoint"/> values are immutable.  To change an existing spawn point, create a new instance with
    /// the new coordinate(s).  Since some spawn points are optional in Minecraft, this helps safegaurd against saving a partial
    /// spawn point.</remarks>
    public struct SpawnPoint : IEquatable<SpawnPoint>
    {
        private readonly int _x;
        private readonly int _y;
        private readonly int _z;

        /// <summary>
        /// Gets the global X-coordinate of the spawn point (in blocks).
        /// </summary>
        public int X
        {
            get { return _x; }
        }

        /// <summary>
        /// Gets the global Y-coordinate of the spawn point (in blocks).
        /// </summary>
        public int Y
        {
            get { return _y; }
        }

        /// <summary>
        /// Gets the global Z-coordinate of the spawn point (in blocks).
        /// </summary>
        public int Z
        {
            get { return _z; }
        }

        /// <summary>
        /// Creates a new spawn point.
        /// </summary>
        /// <param name="x">The global X-coordinate of the spawn point.</param>
        /// <param name="y">The global Y-coordinate of the spawn point.</param>
        /// <param name="z">The global Z-coordinate of the spawn point.</param>
        public SpawnPoint (int x, int y, int z)
        {
            _x = x;
            _y = y;
            _z = z;
        }

        /// <summary>
        /// Checks if two <see cref="SpawnPoint"/> objects are considered equal.
        /// </summary>
        /// <param name="spawn">A <see cref="SpawnPoint"/> to compare against.</param>
        /// <returns>True if the two <see cref="SpawnPoint"/> objects are equal; false otherwise.</returns>
        public bool Equals (SpawnPoint spawn)
        {
            return this._x == spawn._x && this._y == spawn._y && this._z == spawn._z;
        }

        /// <summary>
        /// Checks if two <see cref="SpawnPoint"/> objects are considered equal.
        /// </summary>
        /// <param name="o">An to compare against.</param>
        /// <returns>True if the two <see cref="SpawnPoint"/> objects are equal; false otherwise.</returns>
        public override bool Equals (Object o)
        {
            if (o is SpawnPoint) {
                return this == (SpawnPoint)o;
            }
            else {
                return false;
            }
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance.</returns>
        public override int GetHashCode ()
        {
            int hash = 23;
            hash = hash * 37 + _x;
            hash = hash * 37 + _y;
            hash = hash * 37 + _z;
            return hash;
        }

        /// <summary>
        /// Checks if two <see cref="SpawnPoint"/> objects are considered equal.
        /// </summary>
        /// <param name="k1">The first <see cref="SpawnPoint"/> in the comparison.</param>
        /// <param name="k2">The second <see cref="SpawnPoint"/> in the comparison.</param>
        /// <returns>True if the two <see cref="SpawnPoint"/> objects are equal; false otherwise.</returns>
        public static bool operator == (SpawnPoint k1, SpawnPoint k2)
        {
            return k1._x == k2._x && k1._y == k2._y && k1._z == k2._z;
        }

        /// <summary>
        /// Checks if two <see cref="SpawnPoint"/> objects are considered unequal.
        /// </summary>
        /// <param name="k1">The first <see cref="SpawnPoint"/> in the comparison.</param>
        /// <param name="k2">The second <see cref="SpawnPoint"/> in the comparison.</param>
        /// <returns>True if the two <see cref="SpawnPoint"/> objects are not equal; false otherwise.</returns>
        public static bool operator != (SpawnPoint k1, SpawnPoint k2)
        {
            return k1._x != k2._x || k1._y != k2._y || k1._z != k2._z;
        }

        /// <summary>
        /// Returns a string representation of the <see cref="SpawnPoint"/>.
        /// </summary>
        /// <returns>A string representing this <see cref="SpawnPoint"/>.</returns>
        public override string ToString ()
        {
            return "(" + _x + ", " + _y + ", " + _z + ")";
        }
    }
}
