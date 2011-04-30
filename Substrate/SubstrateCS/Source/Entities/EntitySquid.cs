using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Entities
{
    using Substrate.NBT;

    public class EntitySquid : EntityMob
    {
        public static readonly NBTCompoundNode SquidSchema = MobSchema.MergeInto(new NBTCompoundNode("")
        {
            new NBTStringNode("id", "Squid"),
        });

        public EntitySquid ()
            : base("Squid")
        {
        }

        public EntitySquid (Entity e)
            : base(e)
        {
        }


        #region INBTObject<Entity> Members

        public override bool ValidateTree (TagValue tree)
        {
            return new NBTVerifier(tree, SquidSchema).Verify();
        }

        #endregion


        #region ICopyable<Entity> Members

        public override Entity Copy ()
        {
            return new EntitySquid(this);
        }

        #endregion
    }
}
