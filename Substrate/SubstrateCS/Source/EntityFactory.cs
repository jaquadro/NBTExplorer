using System;
using System.Collections.Generic;
using Substrate.Entities;
using Substrate.Nbt;

namespace Substrate
{
    /// <summary>
    /// Creates new instances of concrete <see cref="EntityTyped"/> types from a dynamic registry.
    /// </summary>
    /// <remarks>This factory allows specific <see cref="EntityTyped"/> objects to be generated as an NBT tree is parsed.  New types can be
    /// registered with the factory at any time, so that custom <see cref="EntityTyped"/> types can be supported.  By default, the standard
    /// Entities of Minecraft are registered with the factory at startup and bound to their respective 'id' fields.</remarks>
    public class EntityFactory
    {
        private static Dictionary<string, Type> _registry = new Dictionary<string, Type>();

        /// <summary>
        /// Create a new instance of a concrete <see cref="EntityTyped"/> type by name.
        /// </summary>
        /// <param name="type">The name that a concrete <see cref="EntityTyped"/> type was registered with.</param>
        /// <returns>A new instance of a concrete <see cref="EntityTyped"/> type, or null if no type was registered with the given name.</returns>
        public static EntityTyped Create (string type)
        {
            Type t;
            if (!_registry.TryGetValue(type, out t)) {
                return null;
            }

            return Activator.CreateInstance(t) as EntityTyped;
        }

        /// <summary>
        /// Create a new instance of a concrete <see cref="EntityTyped"/> type by NBT node.
        /// </summary>
        /// <param name="tree">A <see cref="TagNodeCompound"/> representing a single Entity, containing an 'id' field of the Entity's registered name.</param>
        /// <returns>A new instance of a concrete <see cref="EntityTyped"/> type, or null if no type was registered with the given name.</returns>
        public static EntityTyped Create (TagNodeCompound tree)
        {
            TagNode type;
            if (!tree.TryGetValue("id", out type)) {
                return null;
            }

            Type t;
            if (!_registry.TryGetValue(type.ToTagString(), out t)) {
                return null;
            }

            EntityTyped te = Activator.CreateInstance(t) as EntityTyped;

            return te.LoadTreeSafe(tree);
        }

        /// <summary>
        /// Lookup a concrete <see cref="EntityTyped"/> type by name.
        /// </summary>
        /// <param name="type">The name that a concrete <see cref="EntityTyped"/> type was registered with.</param>
        /// <returns>The <see cref="Type"/> of a concrete <see cref="EntityTyped"/> type, or null if no type was registered with the given name.</returns>
        public static Type Lookup (string type)
        {
            Type t;
            if (!_registry.TryGetValue(type, out t)) {
                return null;
            }

            return t;
        }

        /// <summary>
        /// Registers a new concrete <see cref="EntityTyped"/> type with the <see cref="EntityFactory"/>, binding it to a given name.
        /// </summary>
        /// <param name="id">The name to bind to a concrete <see cref="EntityTyped"/> type.</param>
        /// <param name="subtype">The <see cref="Type"/> of a concrete <see cref="EntityTyped"/> type.</param>
        public static void Register (string id, Type subtype)
        {
            _registry[id] = subtype;
        }

        static EntityFactory ()
        {
            _registry["Arrow"] = typeof(EntityArrow);
            _registry["Boat"] = typeof(EntityBoat);
            _registry["Chicken"] = typeof(EntityChicken);
            _registry["Cow"] = typeof(EntityCow);
            _registry["Creeper"] = typeof(EntityCreeper);
            _registry["Egg"] = typeof(EntityEgg);
            _registry["FallingSand"] = typeof(EntityFallingSand);
            _registry["Ghast"] = typeof(EntityGhast);
            _registry["Giant"] = typeof(EntityGiant);
            _registry["Item"] = typeof(EntityItem);
            _registry["Minecart"] = typeof(EntityMinecart);
            _registry["Mob"] = typeof(EntityMob);
            _registry["Monster"] = typeof(EntityMonster);
            _registry["Painting"] = typeof(EntityPainting);
            _registry["Pig"] = typeof(EntityPig);
            _registry["PigZombie"] = typeof(EntityPigZombie);
            _registry["PrimedTnt"] = typeof(EntityPrimedTnt);
            _registry["Sheep"] = typeof(EntitySheep);
            _registry["Skeleton"] = typeof(EntitySkeleton);
            _registry["Slime"] = typeof(EntitySlime);
            _registry["Snowball"] = typeof(EntitySnowball);
            _registry["Spider"] = typeof(EntitySpider);
            _registry["Squid"] = typeof(EntitySquid);
            _registry["Wolf"] = typeof(EntityWolf);
            _registry["Zombie"] = typeof(EntityZombie);
        }
    }
}
