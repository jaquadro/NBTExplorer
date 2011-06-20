using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate
{
    using NBT;
    using Entities;

    public class EntityFactory
    {
        private static Dictionary<string, Type> _registry;

        public static Entity Create (string type)
        {
            Type t;
            if (!_registry.TryGetValue(type, out t)) {
                return null;
            }

            return Activator.CreateInstance(t) as Entity;
        }

        public static Entity Create (TagNodeCompound tree)
        {
            TagNode type;
            if (!tree.TryGetValue("id", out type)) {
                return null;
            }

            Type t;
            if (!_registry.TryGetValue(type.ToTagString(), out t)) {
                return null;
            }

            Entity te = Activator.CreateInstance(t) as Entity;

            return te.LoadTreeSafe(tree);
        }

        public static Type Lookup (string type)
        {
            Type t;
            if (!_registry.TryGetValue(type, out t)) {
                return null;
            }

            return t;
        }

        public static void Register (string id, Type subtype)
        {
            _registry[id] = subtype;
        }

        static EntityFactory ()
        {
            _registry = new Dictionary<string, Type>();

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
