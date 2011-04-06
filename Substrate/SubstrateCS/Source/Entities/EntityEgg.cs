using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Map.Entities
{
    using Substrate.Map.NBT;

    public class EntityEgg : EntityThrowable
    {
        public static readonly NBTCompoundNode EggSchema = ThrowableSchema.MergeInto(new NBTCompoundNode("")
        {
            new NBTStringNode("id", "Egg"),
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

        public override bool ValidateTree (NBT_Value tree)
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