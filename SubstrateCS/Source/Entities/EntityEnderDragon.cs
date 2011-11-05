using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Entities
{
    using Substrate.Nbt;

    public class EntityEnderDragon : EntityMob
    {
        public static readonly SchemaNodeCompound EnderDragonSchema = MobSchema.MergeInto(new SchemaNodeCompound("")
        {
            new SchemaNodeString("id", "EnderDragon"),
        });

        public EntityEnderDragon ()
            : base("EnderDragon")
        {
        }

        public EntityEnderDragon (TypedEntity e)
            : base(e)
        {
        }


        #region INBTObject<Entity> Members

        public override bool ValidateTree (TagNode tree)
        {
            return new NbtVerifier(tree, EnderDragonSchema).Verify();
        }

        #endregion


        #region ICopyable<Entity> Members

        public override TypedEntity Copy ()
        {
            return new EntityEnderDragon(this);
        }

        #endregion
    }
}
