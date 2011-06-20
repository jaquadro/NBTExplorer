using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Entities
{
    using Substrate.NBT;

    public class EntityGiant : EntityMob
    {
        public static readonly SchemaNodeCompound GiantSchema = MobSchema.MergeInto(new SchemaNodeCompound("")
        {
            new SchemaNodeString("id", "Giant"),
        });

        public EntityGiant ()
            : base("Giant")
        {
        }

        public EntityGiant (Entity e)
            : base(e)
        {
        }


        #region INBTObject<Entity> Members

        public override bool ValidateTree (TagNode tree)
        {
            return new NBTVerifier(tree, GiantSchema).Verify();
        }

        #endregion


        #region ICopyable<Entity> Members

        public override Entity Copy ()
        {
            return new EntityGiant(this);
        }

        #endregion
    }
}
