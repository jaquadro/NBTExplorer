using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Entities
{
    using Substrate.Nbt;

    public class EntityGhast : EntityMob
    {
        public static readonly SchemaNodeCompound GhastSchema = MobSchema.MergeInto(new SchemaNodeCompound("")
        {
            new SchemaNodeString("id", TypeId),
        });

        public static new string TypeId
        {
            get { return "Ghast"; }
        }

        protected EntityGhast (string id)
            : base(id)
        {
        }

        public EntityGhast ()
            : this(TypeId)
        {
        }

        public EntityGhast (TypedEntity e)
            : base(e)
        {
        }


        #region INBTObject<Entity> Members

        public override bool ValidateTree (TagNode tree)
        {
            return new NbtVerifier(tree, GhastSchema).Verify();
        }

        #endregion


        #region ICopyable<Entity> Members

        public override TypedEntity Copy ()
        {
            return new EntityGhast(this);
        }

        #endregion
    }
}
