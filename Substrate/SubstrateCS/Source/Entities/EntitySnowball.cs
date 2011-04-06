using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Map.Entities
{
    using Substrate.Map.NBT;

    public class EntitySnowball : EntityThrowable
    {
        public static readonly NBTCompoundNode SnowballSchema = ThrowableSchema.MergeInto(new NBTCompoundNode("")
        {
            new NBTStringNode("id", "Snowball"),
        });

        public EntitySnowball ()
            : base("Snowball")
        {
        }

        public EntitySnowball (Entity e)
            : base(e)
        {
        }


        #region INBTObject<Entity> Members

        public override bool ValidateTree (NBT_Value tree)
        {
            return new NBTVerifier(tree, SnowballSchema).Verify();
        }

        #endregion


        #region ICopyable<Entity> Members

        public override Entity Copy ()
        {
            return new EntitySnowball(this);
        }

        #endregion
    }
}
