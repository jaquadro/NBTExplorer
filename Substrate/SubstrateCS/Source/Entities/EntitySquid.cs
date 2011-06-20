using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Entities
{
    using Substrate.NBT;

    public class EntitySquid : EntityMob
    {
        public static readonly SchemaNodeCompound SquidSchema = MobSchema.MergeInto(new SchemaNodeCompound("")
        {
            new SchemaNodeString("id", "Squid"),
        });

        public EntitySquid ()
            : base("Squid")
        {
        }

        public EntitySquid (Entity e)
            : base(e)
        {
        }


        #region INBTObject<Entity> Members

        public override bool ValidateTree (TagNode tree)
        {
            return new NBTVerifier(tree, SquidSchema).Verify();
        }

        #endregion


        #region ICopyable<Entity> Members

        public override Entity Copy ()
        {
            return new EntitySquid(this);
        }

        #endregion
    }
}
