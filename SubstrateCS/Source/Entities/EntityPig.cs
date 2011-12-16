using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Entities
{
    using Substrate.Nbt;

    public class EntityPig : EntityAnimal
    {
        public static readonly SchemaNodeCompound PigSchema = AnimalSchema.MergeInto(new SchemaNodeCompound("")
        {
            new SchemaNodeString("id", TypeId),
            new SchemaNodeScaler("Saddle", TagType.TAG_BYTE),
        });

        public static new string TypeId
        {
            get { return "Pig"; }
        }

        private bool _saddle;

        public bool HasSaddle
        {
            get { return _saddle; }
            set { _saddle = value; }
        }

        protected EntityPig (string id)
            : base(id)
        {
        }

        public EntityPig ()
            : this(TypeId)
        {
        }

        public EntityPig (TypedEntity e)
            : base(e)
        {
            EntityPig e2 = e as EntityPig;
            if (e2 != null) {
                _saddle = e2._saddle;
            }
        }


        #region INBTObject<Entity> Members

        public override TypedEntity LoadTree (TagNode tree)
        {
            TagNodeCompound ctree = tree as TagNodeCompound;
            if (ctree == null || base.LoadTree(tree) == null) {
                return null;
            }

            _saddle = ctree["Saddle"].ToTagByte() == 1;

            return this;
        }

        public override TagNode BuildTree ()
        {
            TagNodeCompound tree = base.BuildTree() as TagNodeCompound;
            tree["Saddle"] = new TagNodeByte((byte)(_saddle ? 1 : 0));

            return tree;
        }

        public override bool ValidateTree (TagNode tree)
        {
            return new NbtVerifier(tree, PigSchema).Verify();
        }

        #endregion


        #region ICopyable<Entity> Members

        public override TypedEntity Copy ()
        {
            return new EntityPig(this);
        }

        #endregion
    }
}
