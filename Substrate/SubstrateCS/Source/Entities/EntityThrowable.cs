using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Entities
{
    using Substrate.NBT;

    public class EntityThrowable : Entity
    {
        public static readonly NBTCompoundNode ThrowableSchema = BaseSchema.MergeInto(new NBTCompoundNode("")
        {
            new NBTScalerNode("xTile", TagType.TAG_SHORT),
            new NBTScalerNode("yTile", TagType.TAG_SHORT),
            new NBTScalerNode("zTile", TagType.TAG_SHORT),
            new NBTScalerNode("inTile", TagType.TAG_BYTE),
            new NBTScalerNode("shake", TagType.TAG_BYTE),
            new NBTScalerNode("inGround", TagType.TAG_BYTE),
        });

        private short _xTile;
        private short _yTile;
        private short _zTile;
        private byte _inTile;
        private byte _shake;
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

        public bool IsInTile
        {
            get { return _inTile == 1; }
            set { _inTile = (byte)(value ? 1 : 0); }
        }

        public int Shake
        {
            get { return _shake; }
            set { _shake = (byte)value; }
        }

        public bool IsInGround
        {
            get { return _inGround == 1; }
            set { _inGround = (byte)(value ? 1 : 0); }
        }

        public EntityThrowable (string id)
            : base(id)
        {
        }

        public EntityThrowable (Entity e)
            : base(e)
        {
            EntityThrowable e2 = e as EntityThrowable;
            if (e2 != null) {
                _xTile = e2._xTile;
                _yTile = e2._yTile;
                _zTile = e2._zTile;
                _inTile = e2._inTile;
                _inGround = e2._inGround;
                _shake = e2._shake;
            }
        }


        #region INBTObject<Entity> Members

        public override Entity LoadTree (TagValue tree)
        {
            TagCompound ctree = tree as TagCompound;
            if (ctree == null || base.LoadTree(tree) == null) {
                return null;
            }

            _xTile = ctree["xTile"].ToTagShort();
            _yTile = ctree["yTile"].ToTagShort();
            _zTile = ctree["zTile"].ToTagShort();
            _inTile = ctree["inTile"].ToTagByte();
            _shake = ctree["shake"].ToTagByte();
            _inGround = ctree["inGround"].ToTagByte();

            return this;
        }

        public override TagValue BuildTree ()
        {
            TagCompound tree = base.BuildTree() as TagCompound;
            tree["xTile"] = new TagShort(_xTile);
            tree["yTile"] = new TagShort(_yTile);
            tree["zTile"] = new TagShort(_zTile);
            tree["inTile"] = new TagByte(_inTile);
            tree["shake"] = new TagByte(_shake);
            tree["inGround"] = new TagByte(_inGround);

            return tree;
        }

        public override bool ValidateTree (TagValue tree)
        {
            return new NBTVerifier(tree, ThrowableSchema).Verify();
        }

        #endregion


        #region ICopyable<Entity> Members

        public override Entity Copy ()
        {
            return new EntityThrowable(this);
        }

        #endregion
    }
}
