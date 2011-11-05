using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Entities
{
    using Substrate.Nbt;

    public class EntitySnowball : EntityThrowable
    {
        public static readonly SchemaNodeCompound SnowballSchema = ThrowableSchema.MergeInto(new SchemaNodeCompound("")
        {
            new SchemaNodeString("id", TypeId),
        });

        public static string TypeId
        {
            get { return "Snowball"; }
        }

        protected EntitySnowball (string id)
            : base(id)
        {
        }

        public EntitySnowball ()
            : this(TypeId)
        {
        }

        public EntitySnowball (TypedEntity e)
            : base(e)
        {
        }


        #region INBTObject<Entity> Members

        public override bool ValidateTree (TagNode tree)
        {
            return new NbtVerifier(tree, SnowballSchema).Verify();
        }

        #endregion


        #region ICopyable<Entity> Members

        public override TypedEntity Copy ()
        {
            return new EntitySnowball(this);
        }

        #endregion
    }
}
