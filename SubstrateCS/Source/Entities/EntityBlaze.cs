using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Entities
{
    using Substrate.Nbt;

    public class EntityBlaze : EntityMob
    {
        public static readonly SchemaNodeCompound BlazeSchema = MobSchema.MergeInto(new SchemaNodeCompound("")
        {
            new SchemaNodeString("id", TypeId),
        });

        public static new string TypeId
        {
            get { return "Blaze"; }
        }

        protected EntityBlaze (string id)
            : base(id)
        {
        }

        public EntityBlaze ()
            : this(TypeId)
        {
        }

        public EntityBlaze (TypedEntity e)
            : base(e)
        {
        }


        #region INBTObject<Entity> Members

        public override bool ValidateTree (TagNode tree)
        {
            return new NbtVerifier(tree, BlazeSchema).Verify();
        }

        #endregion


        #region ICopyable<Entity> Members

        public override TypedEntity Copy ()
        {
            return new EntityBlaze(this);
        }

        #endregion
    }
}
