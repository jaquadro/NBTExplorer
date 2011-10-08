using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Entities
{
    using Substrate.Nbt;

    public class EntityCaveSpider : EntityMob
    {
        public static readonly SchemaNodeCompound CaveSpiderSchema = MobSchema.MergeInto(new SchemaNodeCompound("")
        {
            new SchemaNodeString("id", "CaveSpider"),
        });

        public EntityCaveSpider ()
            : base("CaveSpider")
        {
        }

        public EntityCaveSpider (TypedEntity e)
            : base(e)
        {
        }


        #region INBTObject<Entity> Members

        public override bool ValidateTree (TagNode tree)
        {
            return new NbtVerifier(tree, CaveSpiderSchema).Verify();
        }

        #endregion


        #region ICopyable<Entity> Members

        public override TypedEntity Copy ()
        {
            return new EntityCaveSpider(this);
        }

        #endregion
    }
}
