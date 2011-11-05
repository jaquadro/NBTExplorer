using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Entities
{
    using Substrate.Nbt;

    public class EntityEnderEye : TypedEntity
    {
        public static readonly SchemaNodeCompound EnderEyeSchema = TypedEntity.Schema.MergeInto(new SchemaNodeCompound("")
        {
            new SchemaNodeString("id", "EyeOfEnderSignal"),
        });

        public EntityEnderEye ()
            : base("EyeOfEnderSignal")
        {
        }

        public EntityEnderEye (TypedEntity e)
            : base(e)
        {
        }


        #region INBTObject<Entity> Members

        public override bool ValidateTree (TagNode tree)
        {
            return new NbtVerifier(tree, EnderEyeSchema).Verify();
        }

        #endregion


        #region ICopyable<Entity> Members

        public override TypedEntity Copy ()
        {
            return new EntityEnderEye(this);
        }

        #endregion
    }
}
