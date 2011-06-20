using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Entities
{
    using Substrate.NBT;

    public class EntityBoat : Entity
    {
        public static readonly SchemaNodeCompound BoatSchema = BaseSchema.MergeInto(new SchemaNodeCompound("")
        {
            new SchemaNodeString("id", "Boat"),
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

        public override bool ValidateTree (TagNode tree)
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
