using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Entities
{
    using Substrate.NBT;

    public class EntitySnowball : EntityThrowable
    {
        public static readonly SchemaNodeCompound SnowballSchema = ThrowableSchema.MergeInto(new SchemaNodeCompound("")
        {
            new SchemaNodeString("id", "Snowball"),
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

        public override bool ValidateTree (TagNode tree)
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
