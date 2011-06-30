using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Entities
{
    using Substrate.NBT;

    public class EntityCreeper : EntityMob
    {
        public static readonly SchemaNodeCompound CreeperSchema = MobSchema.MergeInto(new SchemaNodeCompound("")
        {
            new SchemaNodeString("id", "Creeper"),
        });

        public EntityCreeper ()
            : base("Creeper")
        {
        }

        public EntityCreeper (EntityTyped e)
            : base(e)
        {
        }


        #region INBTObject<Entity> Members

        public override bool ValidateTree (TagNode tree)
        {
            return new NBTVerifier(tree, CreeperSchema).Verify();
        }

        #endregion


        #region ICopyable<Entity> Members

        public override EntityTyped Copy ()
        {
            return new EntityCreeper(this);
        }

        #endregion
    }
}
