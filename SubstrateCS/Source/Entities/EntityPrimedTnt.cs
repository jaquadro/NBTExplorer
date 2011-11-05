using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Entities
{
    using Substrate.Nbt;

    public class EntityPrimedTnt : TypedEntity
    {
        public static readonly SchemaNodeCompound PrimedTntSchema = TypedEntity.Schema.MergeInto(new SchemaNodeCompound("")
        {
            new SchemaNodeString("id", TypeId),
            new SchemaNodeScaler("Fuse", TagType.TAG_BYTE),
        });

        public static string TypeId
        {
            get { return "PrimedTnt"; }
        }

        private byte _fuse;

        public int Fuse
        {
            get { return _fuse; }
            set { _fuse = (byte)value; }
        }

        protected EntityPrimedTnt (string id)
            : base(id)
        {
        }

        public EntityPrimedTnt ()
            : this(TypeId)
        {
        }

        public EntityPrimedTnt (TypedEntity e)
            : base(e)
        {
            EntityPrimedTnt e2 = e as EntityPrimedTnt;
            if (e2 != null) {
                _fuse = e2._fuse;
            }
        }


        #region INBTObject<Entity> Members

        public override TypedEntity LoadTree (TagNode tree)
        {
            TagNodeCompound ctree = tree as TagNodeCompound;
            if (ctree == null || base.LoadTree(tree) == null) {
                return null;
            }

            _fuse = ctree["Fuse"].ToTagByte();

            return this;
        }

        public override TagNode BuildTree ()
        {
            TagNodeCompound tree = base.BuildTree() as TagNodeCompound;
            tree["Fuse"] = new TagNodeByte(_fuse);

            return tree;
        }

        public override bool ValidateTree (TagNode tree)
        {
            return new NbtVerifier(tree, PrimedTntSchema).Verify();
        }

        #endregion


        #region ICopyable<Entity> Members

        public override TypedEntity Copy ()
        {
            return new EntityPrimedTnt(this);
        }

        #endregion
    }
}
