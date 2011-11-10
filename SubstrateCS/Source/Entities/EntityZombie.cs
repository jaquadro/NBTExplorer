using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Entities
{
    using Substrate.Nbt;

    public class EntityZombie : EntityMob
    {
        public static readonly SchemaNodeCompound ZombieSchema = MobSchema.MergeInto(new SchemaNodeCompound("")
        {
            new SchemaNodeString("id", TypeId),
        });

        public static new string TypeId
        {
            get { return "Zombie"; }
        }

        protected EntityZombie (string id)
            : base(id)
        {
        }

        public EntityZombie ()
            : this(TypeId)
        {
        }

        public EntityZombie (TypedEntity e)
            : base(e)
        {
        }


        #region INBTObject<Entity> Members

        public override bool ValidateTree (TagNode tree)
        {
            return new NbtVerifier(tree, ZombieSchema).Verify();
        }

        #endregion


        #region ICopyable<Entity> Members

        public override TypedEntity Copy ()
        {
            return new EntityZombie(this);
        }

        #endregion
    }
}
