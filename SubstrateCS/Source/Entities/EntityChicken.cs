using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Entities
{
    using Substrate.Nbt;

    public class EntityChicken : EntityAnimal
    {
        public static readonly SchemaNodeCompound ChickenSchema = AnimalSchema.MergeInto(new SchemaNodeCompound("")
        {
            new SchemaNodeString("id", TypeId),
        });

        public static new string TypeId
        {
            get { return "Chicken"; }
        }

        protected EntityChicken (string id)
            : base(id)
        {
        }

        public EntityChicken ()
            : this(TypeId)
        {
        }

        public EntityChicken (TypedEntity e)
            : base(e)
        {
        }


        #region INBTObject<Entity> Members

        public override bool ValidateTree (TagNode tree)
        {
            return new NbtVerifier(tree, ChickenSchema).Verify();
        }

        #endregion


        #region ICopyable<Entity> Members

        public override TypedEntity Copy ()
        {
            return new EntityChicken(this);
        }

        #endregion
    }
}
