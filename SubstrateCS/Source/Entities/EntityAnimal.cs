using System;
using System.Collections.Generic;
using System.Text;
using Substrate.Nbt;

namespace Substrate.Entities
{
    public class EntityAnimal : EntityMob
    {
        public static readonly SchemaNodeCompound AnimalSchema = MobSchema.MergeInto(new SchemaNodeCompound("")
        {
            new SchemaNodeScaler("Age", TagType.TAG_INT, SchemaOptions.CREATE_ON_MISSING),
            new SchemaNodeScaler("InLove", TagType.TAG_INT, SchemaOptions.CREATE_ON_MISSING),
        });

        private int _age;
        private int _inLove;

        public int Age
        {
            get { return _age; }
            set { _age = value; }
        }

        public int InLove
        {
            get { return _inLove; }
            set { _inLove = value; }
        }

        protected EntityAnimal (string id)
            : base(id)
        {
        }

        public EntityAnimal ()
            : this(TypeId)
        {
        }

        public EntityAnimal (TypedEntity e)
            : base(e)
        {
            EntityAnimal e2 = e as EntityAnimal;
            if (e2 != null) {
                _age = e2._age;
                _inLove = e2._inLove;
            }
        }


        #region INBTObject<Entity> Members

        public override TypedEntity LoadTree (TagNode tree)
        {
            TagNodeCompound ctree = tree as TagNodeCompound;
            if (ctree == null || base.LoadTree(tree) == null) {
                return null;
            }

            _age = ctree["Age"].ToTagInt();
            _inLove = ctree["InLove"].ToTagInt();

            return this;
        }

        public override TagNode BuildTree ()
        {
            TagNodeCompound tree = base.BuildTree() as TagNodeCompound;
            tree["Age"] = new TagNodeInt(_age);
            tree["InLove"] = new TagNodeInt(_inLove);

            return tree;
        }

        public override bool ValidateTree (TagNode tree)
        {
            return new NbtVerifier(tree, AnimalSchema).Verify();
        }

        #endregion


        #region ICopyable<Entity> Members

        public override TypedEntity Copy ()
        {
            return new EntityAnimal(this);
        }

        #endregion
    }
}
