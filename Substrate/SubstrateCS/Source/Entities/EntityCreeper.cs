using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Entities
{
    using Substrate.NBT;

    public class EntityCreeper : EntityMob
    {
        public static readonly NBTCompoundNode CreeperSchema = MobSchema.MergeInto(new NBTCompoundNode("")
        {
            new NBTStringNode("id", "Creeper"),
        });

        public EntityCreeper ()
            : base("Creeper")
        {
        }

        public EntityCreeper (Entity e)
            : base(e)
        {
        }


        #region INBTObject<Entity> Members

        public override bool ValidateTree (NBT_Value tree)
        {
            return new NBTVerifier(tree, CreeperSchema).Verify();
        }

        #endregion


        #region ICopyable<Entity> Members

        public override Entity Copy ()
        {
            return new EntityCreeper(this);
        }

        #endregion
    }
}
