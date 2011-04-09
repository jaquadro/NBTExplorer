using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Entities
{
    using Substrate.NBT;

    public class EntityCow : EntityMob
    {
        public static readonly NBTCompoundNode CowSchema = MobSchema.MergeInto(new NBTCompoundNode("")
        {
            new NBTStringNode("id", "Cow"),
        });

        public EntityCow ()
            : base("Cow")
        {
        }

        public EntityCow (Entity e)
            : base(e)
        {
        }


        #region INBTObject<Entity> Members

        public override bool ValidateTree (TagValue tree)
        {
            return new NBTVerifier(tree, CowSchema).Verify();
        }

        #endregion


        #region ICopyable<Entity> Members

        public override Entity Copy ()
        {
            return new EntityCow(this);
        }

        #endregion
    }
}
