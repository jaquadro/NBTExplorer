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
            new SchemaNodeString("id", TypeId),
            new SchemaNodeScaler("Anger", TagType.TAG_SHORT),
        });

        public static new string TypeId
        {
            get { return "PigZombie"; }
        }

        private short _anger;

        public int Anger
        {
            get { return _anger; }
            set { _anger = (short)value; }
        }

        protected EntityPigZombie (string id)
            : base(id)
        {
        }

        public EntityPigZombie ()
            : this(TypeId)
        {
        }

        public EntityPigZombie (TypedEntity e)
            : base(e)
        {
            EntityPigZombie e2 = e as EntityPigZombie;
            if (e2 != null) {
                _anger = e2._anger;
            }
        }


        #region INBTObject<Entity> Members

        public override TypedEntity LoadTree (TagNode tree)
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

        public override TypedEntity Copy ()
        {
            return new EntityPigZombie(this);
        }

        #endregion
    }
}
