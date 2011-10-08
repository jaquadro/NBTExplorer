using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Entities
{
    using Substrate.Nbt;

    public class EntityMonster : EntityMob
    {
        public static readonly SchemaNodeCompound MonsterSchema = MobSchema.MergeInto(new SchemaNodeCompound("")
        {
            new SchemaNodeString("id", "Monster"),
        });

        public EntityMonster ()
            : base("Monster")
        {
        }

        public EntityMonster (TypedEntity e)
            : base(e)
        {
        }


        #region INBTObject<Entity> Members

        public override bool ValidateTree (TagNode tree)
        {
            return new NbtVerifier(tree, MonsterSchema).Verify();
        }

        #endregion


        #region ICopyable<Entity> Members

        public override TypedEntity Copy ()
        {
            return new EntityMonster(this);
        }

        #endregion
    }
}
