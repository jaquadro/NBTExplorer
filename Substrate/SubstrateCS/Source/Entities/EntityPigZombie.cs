using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Entities
{
    using Substrate.NBT;

    public class EntityPigZombie : EntityMob
    {
        public static readonly SchemaNodeCompound PigZombieSchema = MobSchema.MergeInto(new SchemaNodeCompound("")
        {
            new SchemaNodeString("id", "PigZombie"),
        });

        public EntityPigZombie ()
            : base("PigZombie")
        {
        }

        public EntityPigZombie (Entity e)
            : base(e)
        {
        }


        #region INBTObject<Entity> Members

        public override bool ValidateTree (TagNode tree)
        {
            return new NBTVerifier(tree, PigZombieSchema).Verify();
        }

        #endregion


        #region ICopyable<Entity> Members

        public override Entity Copy ()
        {
            return new EntityPigZombie(this);
        }

        #endregion
    }
}
