using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Entities
{
    using Substrate.Nbt;

    public class EntityBoat : TypedEntity
    {
        public static readonly SchemaNodeCompound BoatSchema = TypedEntity.Schema.MergeInto(new SchemaNodeCompound("")
        {
            new SchemaNodeString("id", TypeId),
        });

        public static string TypeId
        {
            get { return "Boat"; }
        }

        protected EntityBoat (string id)
            : base(id)
        {
        }

        public EntityBoat ()
            : this(TypeId)
        {
        }

        public EntityBoat (TypedEntity e)
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

        public override TypedEntity Copy ()
        {
            return new EntityBoat(this);
        }

        #endregion
    }
}
