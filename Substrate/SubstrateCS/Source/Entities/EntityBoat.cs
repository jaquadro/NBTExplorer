using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Entities
{
    using Substrate.Nbt;

    public class EntityBoat : EntityTyped
    {
        public static readonly SchemaNodeCompound BoatSchema = EntityTyped.Schema.MergeInto(new SchemaNodeCompound("")
        {
            new SchemaNodeString("id", "Boat"),
        });

        public EntityBoat ()
            : base("Boat")
        {
        }

        public EntityBoat (EntityTyped e)
            : base(e)
        {
        }


        #region INBTObject<Entity> Members

        public override bool ValidateTree (TagNode tree)
        {
            return new NbtVerifier(tree, BoatSchema).Verify();
        }

        #endregion


        #region ICopyable<Entity> Members

        public override EntityTyped Copy ()
        {
            return new EntityBoat(this);
        }

        #endregion
    }
}
