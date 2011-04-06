using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Map.Entities
{
    using Substrate.Map.NBT;

    public class EntityChicken : EntityMob
    {
        public static readonly NBTCompoundNode ChickenSchema = MobSchema.MergeInto(new NBTCompoundNode("")
        {
            new NBTStringNode("id", "Chicken"),
        });

        public EntityChicken ()
            : base("Chicken")
        {
        }

        public EntityChicken (Entity e)
            : base(e)
        {
        }


        #region INBTObject<Entity> Members

        public override bool ValidateTree (NBT_Value tree)
        {
            return new NBTVerifier(tree, ChickenSchema).Verify();
        }

        #endregion


        #region ICopyable<Entity> Members

        public override Entity Copy ()
        {
            return new EntityChicken(this);
        }

        #endregion
    }
}
