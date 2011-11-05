using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Entities
{
    using Substrate.Nbt;

    public class EntityFireball : TypedEntity
    {
        public static readonly SchemaNodeCompound FireballSchema = TypedEntity.Schema.MergeInto(new SchemaNodeCompound("")
        {
            new SchemaNodeString("id", TypeId),
            new SchemaNodeScaler("xTile", TagType.TAG_SHORT),
            new SchemaNodeScaler("yTile", TagType.TAG_SHORT),
            new SchemaNodeScaler("zTile", TagType.TAG_SHORT),
            new SchemaNodeScaler("inTile", TagType.TAG_BYTE),
            new SchemaNodeScaler("inGround", TagType.TAG_BYTE),
        });

        public static string TypeId
        {
            get { return "Fireball"; }
        }

        private short _xTile;
        private short _yTile;
        private short _zTile;
        private byte _inTile;
        private byte _inGround;

        public int XTile
        {
            get { return _xTile; }
            set { _xTile = (short)value; }
        }

        public int YTile
        {
            get { return _yTile; }
            set { _yTile = (short)value; }
        }

        public int ZTile
        {
            get { return _zTile; }
            set { _zTile = (short)value; }
        }

        public int InTile
        {
            get { return _inTile; }
            set { _inTile = (byte)value; }
        }

        public bool IsInGround
        {
            get { return _inGround == 1; }
            set { _inGround = (byte)(value ? 1 : 0); }
        }

        protected EntityFireball (string id)
            : base(id)
        {
        }

        public EntityFireball ()
            : this(TypeId)
        {
        }

        public EntityFireball (TypedEntity e)
            : base(e)
        {
            EntityFireball e2 = e as EntityFireball;
            if (e2 != null) {
                _xTile = e2._xTile;
                _yTile = e2._yTile;
                _zTile = e2._zTile;
                _inTile = e2._inTile;
                _inGround = e2._inGround;
            }
        }


        #region INBTObject<Entity> Members

        public override TypedEntity LoadTree (TagNode tree)
        {
            TagNodeCompound ctree = tree as TagNodeCompound;
            if (ctree == null || base.LoadTree(tree) == null) {
                return null;
            }

            _xTile = ctree["xTile"].ToTagShort();
            _yTile = ctree["yTile"].ToTagShort();
            _zTile = ctree["zTile"].ToTagShort();
            _inTile = ctree["inTile"].ToTagByte();
            _inGround = ctree["inGround"].ToTagByte();

            return this;
        }

        public override TagNode BuildTree ()
        {
            TagNodeCompound tree = base.BuildTree() as TagNodeCompound;
            tree["xTile"] = new TagNodeShort(_xTile);
            tree["yTile"] = new TagNodeShort(_yTile);
            tree["zTile"] = new TagNodeShort(_zTile);
            tree["inTile"] = new TagNodeByte(_inTile);
            tree["inGround"] = new TagNodeByte(_inGround);

            return tree;
        }

        public override bool ValidateTree (TagNode tree)
        {
            return new NbtVerifier(tree, FireballSchema).Verify();
        }

        #endregion


        #region ICopyable<Entity> Members

        public override TypedEntity Copy ()
        {
            return new EntityFireball(this);
        }

        #endregion
    }
}
