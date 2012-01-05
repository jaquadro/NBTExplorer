using System;
using System.Collections.Generic;
using Substrate.Entities;
using Substrate.Nbt;

namespace Substrate
{
    /// <summary>
    /// Creates new instances of concrete <see cref="TypedEntity"/> types from a dynamic registry.
    /// </summary>
    /// <remarks>This factory allows specific <see cref="TypedEntity"/> objects to be generated as an NBT tree is parsed.  New types can be
    /// registered with the factory at any time, so that custom <see cref="TypedEntity"/> types can be supported.  By default, the standard
    /// Entities of Minecraft are registered with the factory at startup and bound to their respective 'id' fields.</remarks>
    public class EntityFactory
    {
        private static Dictionary<string, Type> _registry = new Dictionary<string, Type>();

        /// <summary>
        /// Create a new instance of a concrete <see cref="TypedEntity"/> type by name.
        /// </summary>
        /// <param name="type">The name that a concrete <see cref="TypedEntity"/> type was registered with.</param>
        /// <returns>A new instance of a concrete <see cref="TypedEntity"/> type, or null if no type was registered with the given name.</returns>
        public static TypedEntity Create (string type)
        {
            Type t;
            if (!_registry.TryGetValue(type, out t)) {
                return null;
            }

            return Activator.CreateInstance(t) as TypedEntity;
        }

        /// <summary>
        /// Create a new instance of a concrete <see cref="TypedEntity"/> type by NBT node.
        /// </summary>
        /// <param name="tree">A <see cref="TagNodeCompound"/> representing a single Entity, containing an 'id' field of the Entity's registered name.</param>
        /// <returns>A new instance of a concrete <see cref="TypedEntity"/> type, or null if no type was registered with the given name.</returns>
        public static TypedEntity Create (TagNodeCompound tree)
        {
            TagNode type;
            if (!tree.TryGetValue("id", out type)) {
                return null;
            }

            Type t;
            if (!_registry.TryGetValue(type.ToTagString(), out t)) {
                return null;
            }

            TypedEntity te = Activator.CreateInstance(t) as TypedEntity;

            return te.LoadTreeSafe(tree);
        }

        /// <summary>
        /// Creates a new instance of a nonspecific <see cref="TypedEntity"/> object by NBT node.
        /// </summary>
        /// <param name="tree">A <see cref="TagNodeCompound"/> representing a single Entity, containing an 'id' field.</param>
        /// <returns>A new instance of a <see cref="TypedEntity"/> object, or null if the entity is not typed.</returns>
        public static TypedEntity CreateGeneric (TagNodeCompound tree)
        {
            TagNode type;
            if (!tree.TryGetValue("id", out type)) {
                return null;
            }

            TypedEntity te = new TypedEntity(type.ToTagString().Data);

            return te.LoadTreeSafe(tree);
        }

        /// <summary>
        /// Lookup a concrete <see cref="TypedEntity"/> type by name.
        /// </summary>
        /// <param name="type">The name that a concrete <see cref="TypedEntity"/> type was registered with.</param>
        /// <returns>The <see cref="Type"/> of a concrete <see cref="TypedEntity"/> type, or null if no type was registered with the given name.</returns>
        public static Type Lookup (string type)
        {
            Type t;
            if (!_registry.TryGetValue(type, out t)) {
                return null;
            }

            return t;
        }

        /// <summary>
        /// Registers a new concrete <see cref="TypedEntity"/> type with the <see cref="EntityFactory"/>, binding it to a given name.
        /// </summary>
        /// <param name="id">The name to bind to a concrete <see cref="TypedEntity"/> type.</param>
        /// <param name="subtype">The <see cref="Type"/> of a concrete <see cref="TypedEntity"/> type.</param>
        public static void Register (string id, Type subtype)
        {
            _registry[id] = subtype;
        }

        /// <summary>
        /// Gets an enumerator over all registered Entities.
        /// </summary>
        public static IEnumerable<KeyValuePair<string, Type>> RegisteredEntities
        {
            get 
            {
                foreach (KeyValuePair<string, Type> kvp in _registry) {
                    yield return kvp;
                }
            }
        }

        static EntityFactory ()
        {
            _registry[EntityArrow.TypeId] = typeof(EntityArrow);
            _registry[EntityBlaze.TypeId] = typeof(EntityBlaze);
            _registry[EntityBoat.TypeId] = typeof(EntityBoat);
            _registry[EntityCaveSpider.TypeId] = typeof(EntityCaveSpider);
            _registry[EntityChicken.TypeId] = typeof(EntityChicken);
            _registry[EntityCow.TypeId] = typeof(EntityCow);
            _registry[EntityCreeper.TypeId] = typeof(EntityCreeper);
            _registry[EntityEgg.TypeId] = typeof(EntityEgg);
            _registry[EntityEnderDragon.TypeId] = typeof(EntityEnderDragon);
            _registry[EntityEnderman.TypeId] = typeof(EntityEnderman);
            _registry[EntityEnderEye.TypeId] = typeof(EntityEnderEye);
            _registry[EntityFallingSand.TypeId] = typeof(EntityFallingSand);
            _registry[EntityFireball.TypeId] = typeof(EntityFireball);
            _registry[EntityGhast.TypeId] = typeof(EntityGhast);
            _registry[EntityGiant.TypeId] = typeof(EntityGiant);
            _registry[EntityItem.TypeId] = typeof(EntityItem);
            _registry[EntityMagmaCube.TypeId] = typeof(EntityMagmaCube);
            _registry[EntityMinecart.TypeId] = typeof(EntityMinecart);
            _registry[EntityMob.TypeId] = typeof(EntityMob);
            _registry[EntityMonster.TypeId] = typeof(EntityMonster);
            _registry[EntityMooshroom.TypeId] = typeof(EntityMooshroom);
            _registry[EntityPainting.TypeId] = typeof(EntityPainting);
            _registry[EntityPig.TypeId] = typeof(EntityPig);
            _registry[EntityPigZombie.TypeId] = typeof(EntityPigZombie);
            _registry[EntityPrimedTnt.TypeId] = typeof(EntityPrimedTnt);
            _registry[EntitySheep.TypeId] = typeof(EntitySheep);
            _registry[EntitySilverfish.TypeId] = typeof(EntitySilverfish);
            _registry[EntitySkeleton.TypeId] = typeof(EntitySkeleton);
            _registry[EntitySlime.TypeId] = typeof(EntitySlime);
            _registry[EntitySmallFireball.TypeId] = typeof(EntitySmallFireball);
            _registry[EntitySnowball.TypeId] = typeof(EntitySnowball);
            _registry[EntitySnowman.TypeId] = typeof(EntitySnowman);
            _registry[EntitySpider.TypeId] = typeof(EntitySpider);
            _registry[EntitySquid.TypeId] = typeof(EntitySquid);
            _registry[EntityEnderPearl.TypeId] = typeof(EntityEnderPearl);
            _registry[EntityVillager.TypeId] = typeof(EntityVillager);
            _registry[EntityWolf.TypeId] = typeof(EntityWolf);
            _registry[EntityXPOrb.TypeId] = typeof(EntityXPOrb);
            _registry[EntityZombie.TypeId] = typeof(EntityZombie);
        }
    }
}
