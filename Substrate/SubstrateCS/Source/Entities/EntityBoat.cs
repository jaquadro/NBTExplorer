using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Entities
{
    using Substrate.NBT;

    public class EntityBoat : Entity
    {
        public static readonly NBTCompoundNode BoatSchema = BaseSchema.MergeInto(new NBTCompoundNode("")
        {
            new NBTStringNode("id", "Boat"),
        });

        public EntityBoat ()
            : base("Boat")
        {
        }

        public EntityBoat (Entity e)
            : base(e)
        {
        }


        #region INBTObject<Entity> Members

        public override bool ValidateTree (NBT_Value tree)
        {
            return new NBTVerifier(tree, BoatSchema).Verify();
        }

        #endregion


        #region ICopyable<Entity> Members

        public override Entity Copy ()
        {
            return new EntityBoat(this);
        }

        #endregion
    }
}
