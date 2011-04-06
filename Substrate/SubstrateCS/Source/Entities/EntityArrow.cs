using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Map.Entities
{
    using Substrate.Map.NBT;

    public class EntityArrow : EntityThrowable
    {
        public static readonly NBTCompoundNode ArrowSchema = ThrowableSchema.MergeInto(new NBTCompoundNode("")
        {
            new NBTStringNode("id", "Arrow"),
        });

        public EntityArrow ()
            : base("Arrow")
        {
        }

        public EntityArrow (Entity e)
            : base(e)
        {
        }


        #region INBTObject<Entity> Members

        public override bool ValidateTree (NBT_Value tree)
        {
            return new NBTVerifier(tree, ArrowSchema).Verify();
        }

        #endregion


        #region ICopyable<Entity> Members

        public override Entity Copy ()
        {
            return new EntityArrow(this);
        }

        #endregion
    }
}
