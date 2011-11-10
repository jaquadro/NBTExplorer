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
            new SchemaNodeString("id", TypeId),
        });

        public static new string TypeId
        {
            get { return "Skeleton"; }
        }

        protected EntitySkeleton (string id)
            : base(id)
        {
        }

        public EntitySkeleton ()
            : this(TypeId)
        {
        }

        public EntitySkeleton (TypedEntity e)
            : base(e)
        {
        }


        #region INBTObject<Entity> Members

        public override bool ValidateTree (TagNode tree)
        {
            return new NbtVerifier(tree, SkeletonSchema).Verify();
        }

        #endregion


        #region ICopyable<Entity> Members

        public override TypedEntity Copy ()
        {
            return new EntitySkeleton(this);
        }

        #endregion
    }
}
