using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Entities
{
    using Substrate.Nbt;

    public class EntitySmallFireball : EntityFireball
    {
        public static readonly SchemaNodeCompound SmallFireballSchema = FireballSchema.MergeInto(new SchemaNodeCompound("")
        {
            new SchemaNodeString("id", "SmallFireball"),
        });

        public EntitySmallFireball ()
            : base("SmallFireball")
        {
        }

        public EntitySmallFireball (TypedEntity e)
            : base(e)
        {
        }


        #region INBTObject<Entity> Members

        public override bool ValidateTree (TagNode tree)
        {
            return new NbtVerifier(tree, SmallFireballSchema).Verify();
        }

        #endregion


        #region ICopyable<Entity> Members

        public override TypedEntity Copy ()
        {
            return new EntitySmallFireball(this);
        }

        #endregion
    }
}
