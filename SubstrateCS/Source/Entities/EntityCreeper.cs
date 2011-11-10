using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Entities
{
    using Substrate.Nbt;

    public class EntityCreeper : EntityMob
    {
        public static readonly SchemaNodeCompound CreeperSchema = MobSchema.MergeInto(new SchemaNodeCompound("")
        {
            new SchemaNodeString("id", TypeId),
            new SchemaNodeScaler("powered", TagType.TAG_BYTE, SchemaOptions.OPTIONAL),
        });

        public static new string TypeId
        {
            get { return "Creeper"; }
        }

        private bool? _powered;

        public bool Powered
        {
            get { return _powered ?? false; }
            set { _powered = value; }
        }

        protected EntityCreeper (string id)
            : base(id)
        {
        }

        public EntityCreeper ()
            : this(TypeId)
        {
        }

        public EntityCreeper (TypedEntity e)
            : base(e)
        {
            EntityCreeper e2 = e as EntityCreeper;
            if (e2 != null) {
                _powered = e2._powered;
            }
        }


        #region INBTObject<Entity> Members

        public override TypedEntity LoadTree (TagNode tree)
        {
            TagNodeCompound ctree = tree as TagNodeCompound;
            if (ctree == null || base.LoadTree(tree) == null) {
                return null;
            }

            if (ctree.ContainsKey("powered")) {
                _powered = ctree["powered"].ToTagByte() == 1;
            }

            return this;
        }

        public override TagNode BuildTree ()
        {
            TagNodeCompound tree = base.BuildTree() as TagNodeCompound;

            if (_powered != null) {
                tree["powered"] = new TagNodeByte((byte)((_powered ?? false) ? 1 : 0));
            }

            return tree;
        }

        public override bool ValidateTree (TagNode tree)
        {
            return new NbtVerifier(tree, CreeperSchema).Verify();
        }

        #endregion


        #region ICopyable<Entity> Members

        public override TypedEntity Copy ()
        {
            return new EntityCreeper(this);
        }

        #endregion
    }
}
