using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Entities
{
    using Substrate.Nbt;

    public class EntityCow : EntityMob
    {
        public static readonly SchemaNodeCompound CowSchema = MobSchema.MergeInto(new SchemaNodeCompound("")
        {
            new SchemaNodeString("id", TypeId),
        });

        public static new string TypeId
        {
            get { return "Cow"; }
        }

        protected EntityCow (string id)
            : base(id)
        {
        }

        public EntityCow ()
            : this(TypeId)
        {
        }

        public EntityCow (TypedEntity e)
            : base(e)
        {
        }


        #region INBTObject<Entity> Members

        public override bool ValidateTree (TagNode tree)
        {
            return new NbtVerifier(tree, CowSchema).Verify();
        }

        #endregion


        #region ICopyable<Entity> Members

        public override TypedEntity Copy ()
        {
            return new EntityCow(this);
        }

        #endregion
    }
}
