using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Map
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

            return Activator.CreateInstance(t, new object[] { type} ) as Entity;
        }

        public static Entity Create (NBT_Compound tree)
        {
            string type = tree["id"].ToNBTString();

            Type t;
            if (!_registry.TryGetValue(type, out t)) {
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

            _registry["Chicken"] = typeof(EntityChicken);
            _registry["Cow"] = typeof(EntityCow);
            _registry["Creeper"] = typeof(EntityCreeper);
            _registry["Ghast"] = typeof(EntityGhast);
            _registry["Giant"] = typeof(EntityGiant);
            _registry["Mob"] = typeof(EntityMob);
            _registry["Monster"] = typeof(EntityMonster);
            _registry["Pig"] = typeof(EntityPig);
            _registry["PigZombie"] = typeof(EntityPigZombie);
            _registry["Sheep"] = typeof(EntitySheep);
            _registry["Skeleton"] = typeof(EntitySkeleton);
            _registry["Slime"] = typeof(EntitySlime);
            _registry["Spider"] = typeof(EntitySpider);
            _registry["Wolf"] = typeof(EntityWolf);
            _registry["Zombie"] = typeof(EntityZombie);
        }
    }
}
