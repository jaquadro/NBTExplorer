using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Map.Entities
{
    using Substrate.Map.NBT;

    public class EntityGhast : EntityMob
    {
        public static readonly NBTCompoundNode GhastSchema = MobSchema.MergeInto(new NBTCompoundNode("")
        {
            new NBTStringNode("id", "Ghast"),
        });

        public EntityGhast ()
            : base("Ghast")
        {
        }

        public EntityGhast (Entity e)
            : base(e)
        {
        }


        #region INBTObject<Entity> Members

        public override bool ValidateTree (NBT_Value tree)
        {
            return new NBTVerifier(tree, GhastSchema).Verify();
        }

        #endregion


        #region ICopyable<Entity> Members

        public override Entity Copy ()
        {
            return new EntityGhast(this);
        }

        #endregion
    }
}
