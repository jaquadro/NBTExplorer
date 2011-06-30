using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Entities
{
    using Substrate.Nbt;

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

        public EntityGiant (EntityTyped e)
            : base(e)
        {
        }


        #region INBTObject<Entity> Members

        public override bool ValidateTree (TagNode tree)
        {
            return new NbtVerifier(tree, GiantSchema).Verify();
        }

        #endregion


        #region ICopyable<Entity> Members

        public override EntityTyped Copy ()
        {
            return new EntityGiant(this);
        }

        #endregion
    }
}
