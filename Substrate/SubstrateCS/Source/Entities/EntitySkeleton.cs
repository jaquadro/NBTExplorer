using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Entities
{
    using Substrate.Nbt;

    public class EntitySkeleton : EntityMob
    {
        public static readonly SchemaNodeCompound SkeletonSchema = MobSchema.MergeInto(new SchemaNodeCompound("")
        {
            new SchemaNodeString("id", "Skeleton"),
        });

        public EntitySkeleton ()
            : base("Skeleton")
        {
        }

        public EntitySkeleton (EntityTyped e)
            : base(e)
        {
        }


        #region INBTObject<Entity> Members

        public override bool ValidateTree (TagNode tree)
        {
            return new NBTVerifier(tree, SkeletonSchema).Verify();
        }

        #endregion


        #region ICopyable<Entity> Members

        public override EntityTyped Copy ()
        {
            return new EntitySkeleton(this);
        }

        #endregion
    }
}
