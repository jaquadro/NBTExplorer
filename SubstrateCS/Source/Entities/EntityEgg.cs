using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Entities
{
    using Substrate.Nbt;

    public class EntityEgg : EntityThrowable
    {
        public static readonly SchemaNodeCompound EggSchema = ThrowableSchema.MergeInto(new SchemaNodeCompound("")
        {
            new SchemaNodeString("id", TypeId),
        });

        public static string TypeId
        {
            get { return "Egg"; }
        }

        protected EntityEgg (string id)
            : base(id)
        {
        }

        public EntityEgg ()
            : this(TypeId)
        {
        }

        public EntityEgg (TypedEntity e)
            : base(e)
        {
        }


        #region INBTObject<Entity> Members

        public override bool ValidateTree (TagNode tree)
        {
            return new NbtVerifier(tree, EggSchema).Verify();
        }

        #endregion


        #region ICopyable<Entity> Members

        public override TypedEntity Copy ()
        {
            return new EntityEgg(this);
        }

        #endregion
    }
}