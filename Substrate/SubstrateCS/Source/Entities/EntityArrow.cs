using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Entities
{
    using Substrate.Nbt;

    public class EntityArrow : EntityThrowable
    {
        public static readonly SchemaNodeCompound ArrowSchema = ThrowableSchema.MergeInto(new SchemaNodeCompound("")
        {
            new SchemaNodeString("id", "Arrow"),
        });

        public EntityArrow ()
            : base("Arrow")
        {
        }

        public EntityArrow (TypedEntity e)
            : base(e)
        {
        }


        #region INBTObject<Entity> Members

        public override bool ValidateTree (TagNode tree)
        {
            return new NbtVerifier(tree, ArrowSchema).Verify();
        }

        #endregion


        #region ICopyable<Entity> Members

        public override TypedEntity Copy ()
        {
            return new EntityArrow(this);
        }

        #endregion
    }
}
