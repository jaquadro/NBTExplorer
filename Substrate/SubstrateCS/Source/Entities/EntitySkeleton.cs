using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Entities
{
    using Substrate.NBT;

    public class EntitySkeleton : EntityMob
    {
        public static readonly NBTCompoundNode SkeletonSchema = MobSchema.MergeInto(new NBTCompoundNode("")
        {
            new NBTStringNode("id", "Skeleton"),
        });

        public EntitySkeleton ()
            : base("Skeleton")
        {
        }

        public EntitySkeleton (Entity e)
            : base(e)
        {
        }


        #region INBTObject<Entity> Members

        public override bool ValidateTree (NBT_Value tree)
        {
            return new NBTVerifier(tree, SkeletonSchema).Verify();
        }

        #endregion


        #region ICopyable<Entity> Members

        public override Entity Copy ()
        {
            return new EntitySkeleton(this);
        }

        #endregion
    }
}
