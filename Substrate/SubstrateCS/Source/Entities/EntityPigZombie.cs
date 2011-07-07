using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Entities
{
    using Substrate.Nbt;

    public class EntityPigZombie : EntityMob
    {
        public static readonly SchemaNodeCompound PigZombieSchema = MobSchema.MergeInto(new SchemaNodeCompound("")
        {
            new SchemaNodeString("id", "PigZombie"),
            new SchemaNodeScaler("Anger", TagType.TAG_SHORT),
        });

        private short _anger;

        public int Anger
        {
            get { return _anger; }
            set { _anger = (short)value; }
        }

        public EntityPigZombie ()
            : base("PigZombie")
        {
        }

        public EntityPigZombie (EntityTyped e)
            : base(e)
        {
            EntityPigZombie e2 = e as EntityPigZombie;
            if (e2 != null) {
                _anger = e2._anger;
            }
        }


        #region INBTObject<Entity> Members

        public override EntityTyped LoadTree (TagNode tree)
        {
            TagNodeCompound ctree = tree as TagNodeCompound;
            if (ctree == null || base.LoadTree(tree) == null) {
                return null;
            }

            _anger = ctree["Anger"].ToTagShort();

            return this;
        }

        public override TagNode BuildTree ()
        {
            TagNodeCompound tree = base.BuildTree() as TagNodeCompound;
            tree["Anger"] = new TagNodeShort(_anger);

            return tree;
        }

        public override bool ValidateTree (TagNode tree)
        {
            return new NbtVerifier(tree, PigZombieSchema).Verify();
        }

        #endregion


        #region ICopyable<Entity> Members

        public override EntityTyped Copy ()
        {
            return new EntityPigZombie(this);
        }

        #endregion
    }
}
