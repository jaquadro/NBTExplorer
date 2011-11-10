using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Entities
{
    using Substrate.Nbt;

    public class EntityGiant : EntityMob
    {
        public static readonly SchemaNodeCompound GiantSchema = MobSchema.MergeInto(new SchemaNodeCompound("")
        {
            new SchemaNodeString("id", TypeId),
        });

        public static new string TypeId
        {
            get { return "Giant"; }
        }

        protected EntityGiant (string id)
            : base(id)
        {
        }

        public EntityGiant ()
            : this(TypeId)
        {
        }

        public EntityGiant (TypedEntity e)
            : base(e)
        {
        }


        #region INBTObject<Entity> Members

        public override bool ValidateTree (TagNode tree)
        {
            return new NbtVerifier(tree, GiantSchema).Verify();
        }

        #endregion


        #region ICopyable<Entity> Members

        public override TypedEntity Copy ()
        {
            return new EntityGiant(this);
        }

        #endregion
    }
}
