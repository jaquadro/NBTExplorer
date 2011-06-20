using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Entities
{
    using Substrate.NBT;

    public class EntityEgg : EntityThrowable
    {
        public static readonly SchemaNodeCompound EggSchema = ThrowableSchema.MergeInto(new SchemaNodeCompound("")
        {
            new SchemaNodeString("id", "Egg"),
        });

        public EntityEgg ()
            : base("Egg")
        {
        }

        public EntityEgg (Entity e)
            : base(e)
        {
        }


        #region INBTObject<Entity> Members

        public override bool ValidateTree (TagNode tree)
        {
            return new NBTVerifier(tree, EggSchema).Verify();
        }

        #endregion


        #region ICopyable<Entity> Members

        public override Entity Copy ()
        {
            return new EntityEgg(this);
        }

        #endregion
    }
}