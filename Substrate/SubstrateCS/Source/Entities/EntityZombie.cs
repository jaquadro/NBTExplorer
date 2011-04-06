using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Entities
{
    using Substrate.NBT;

    public class EntityZombie : EntityMob
    {
        public static readonly NBTCompoundNode ZombieSchema = MobSchema.MergeInto(new NBTCompoundNode("")
        {
            new NBTStringNode("id", "Zombie"),
        });

        public EntityZombie ()
            : base("Zombie")
        {
        }

        public EntityZombie (Entity e)
            : base(e)
        {
        }


        #region INBTObject<Entity> Members

        public override bool ValidateTree (NBT_Value tree)
        {
            return new NBTVerifier(tree, ZombieSchema).Verify();
        }

        #endregion


        #region ICopyable<Entity> Members

        public override Entity Copy ()
        {
            return new EntityZombie(this);
        }

        #endregion
    }
}
