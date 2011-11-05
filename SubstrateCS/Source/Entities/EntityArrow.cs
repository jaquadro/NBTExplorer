using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Entities
{
    using Substrate.Nbt;

    public class EntityArrow : EntityThrowable
    {
        public static readonly SchemaNodeCompound ArrowSchema = ThrowableSchema.MergeInto(new SchemaNodeCompound("")
        {
            new SchemaNodeString("id", TypeId),
            new SchemaNodeScaler("inData", TagType.TAG_BYTE, SchemaOptions.CREATE_ON_MISSING),
            new SchemaNodeScaler("player", TagType.TAG_BYTE, SchemaOptions.CREATE_ON_MISSING),
        });

        public static string TypeId
        {
            get { return "Arrow"; }
        }

        private byte _inData;
        private byte _player;

        public int InData
        {
            get { return _inData; }
            set { _inData = (byte)value; }
        }

        public bool IsPlayerArrow
        {
            get { return _player != 0; }
            set { _player = (byte)(value ? 1 : 0); }
        }

        protected EntityArrow (string id)
            : base(id)
        {
        }

        public EntityArrow ()
            : this(TypeId)
        {
        }

        public EntityArrow (TypedEntity e)
            : base(e)
        {
            EntityArrow e2 = e as EntityArrow;
            if (e2 != null) {
                _inData = e2._inData;
                _player = e2._player;
            }
        }


        #region INBTObject<Entity> Members

        public override TypedEntity LoadTree (TagNode tree)
        {
            TagNodeCompound ctree = tree as TagNodeCompound;
            if (ctree == null || base.LoadTree(tree) == null) {
                return null;
            }

            _inData = ctree["inData"].ToTagByte();
            _player = ctree["player"].ToTagByte();

            return this;
        }

        public override TagNode BuildTree ()
        {
            TagNodeCompound tree = base.BuildTree() as TagNodeCompound;
            tree["inData"] = new TagNodeShort(_inData);
            tree["player"] = new TagNodeShort(_player);

            return tree;
        }

        public override bool ValidateTree (TagNode tree)
        {
            return new NbtVerifier(tree, ArrowSchema).Verify();
        }

        #endregion


        #region ICopyable<Entity> Members

        public override TypedEntity Copy ()
        {
            return new EntityArrow(this);
        }

        #endregion
    }
}
