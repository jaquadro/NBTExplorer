using System;
using System.Collections.Generic;

namespace Substrate
{
    using Nbt;
    using TileEntities;

    /// <summary>
    /// Creates new instances of concrete <see cref="TileEntity"/> types from a dynamic registry.
    /// </summary>
    /// <remarks>This factory allows specific <see cref="TileEntity"/> objects to be generated as an NBT tree is parsed.  New types can be
    /// registered with the factory at any time, so that custom <see cref="TileEntity"/> types can be supported.  By default, the standard
    /// Tile Entities of Minecraft are registered with the factory at startup and bound to their respective 'id' fields.</remarks>
    public class TileEntityFactory
    {
        private static Dictionary<string, Type> _registry;

        /// <summary>
        /// Create a new instance of a concrete <see cref="TileEntity"/> type by name.
        /// </summary>
        /// <param name="type">The name that a concrete <see cref="TileEntity"/> type was registered with.</param>
        /// <returns>A new instance of a concrete <see cref="TileEntity"/> type, or null if no type was registered with the given name.</returns>
        public static TileEntity Create (string type)
        {
            Type t;
            if (!_registry.TryGetValue(type, out t)) {
                return null;
            }

            return Activator.CreateInstance(t) as TileEntity;
        }

        /// <summary>
        /// Create a new instance of a concrete <see cref="TileEntity"/> type by NBT node.
        /// </summary>
        /// <param name="tree">A <see cref="TagNodeCompound"/> representing a single Tile Entity, containing an 'id' field of the Tile Entity's registered name.</param>
        /// <returns>A new instance of a concrete <see cref="TileEntity"/> type, or null if no type was registered with the given name.</returns>
        public static TileEntity Create (TagNodeCompound tree)
        {
            string type = tree["id"].ToTagString();

            Type t;
            if (!_registry.TryGetValue(type, out t)) {
                return null;
            }

            TileEntity te = Activator.CreateInstance(t) as TileEntity;

            return te.LoadTreeSafe(tree);
        }

        /// <summary>
        /// Lookup a concrete <see cref="TileEntity"/> type by name.
        /// </summary>
        /// <param name="type">The name that a concrete <see cref="TileEntity"/> type was registered with.</param>
        /// <returns>The <see cref="Type"/> of a concrete <see cref="TileEntity"/> type, or null if no type was registered with the given name.</returns>
        public static Type Lookup (string type)
        {
            Type t;
            if (!_registry.TryGetValue(type, out t)) {
                return null;
            }

            return t;
        }

        /// <summary>
        /// Registers a new concrete <see cref="TileEntity"/> type with the <see cref="TileEntityFactory"/>, binding it to a given name.
        /// </summary>
        /// <param name="id">The name to bind to a concrete <see cref="TileEntity"/> type.</param>
        /// <param name="subtype">The <see cref="Type"/> of a concrete <see cref="TileEntity"/> type.</param>
        public static void Register (string id, Type subtype)
        {
            _registry[id] = subtype;
        }

        static TileEntityFactory ()
        {
            _registry = new Dictionary<string, Type>();

            _registry["Chest"] = typeof(TileEntityChest);
            _registry["Furnace"] = typeof(TileEntityFurnace);
            _registry["MobSpawner"] = typeof(TileEntityMobSpawner);
            _registry["Music"] = typeof(TileEntityMusic);
            _registry["RecordPlayer"] = typeof(TileEntityRecordPlayer);
            _registry["Sign"] = typeof(TileEntitySign);
            _registry["Trap"] = typeof(TileEntityTrap);
        }
    }

    /// <summary>
    /// An exception that is thrown when unknown TileEntity types are queried.
    /// </summary>
    public class UnknownTileEntityException : Exception 
    {
        public UnknownTileEntityException (string message)
            : base(message)
        { }
    }
}
