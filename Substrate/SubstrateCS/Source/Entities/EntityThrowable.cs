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
            new NBTScalerNode("xTile", NBT_Type.TAG_SHORT),
            new NBTScalerNode("yTile", NBT_Type.TAG_SHORT),
            new NBTScalerNode("zTile", NBT_Type.TAG_SHORT),
            new NBTScalerNode("inTile", NBT_Type.TAG_BYTE),
            new NBTScalerNode("shake", NBT_Type.TAG_BYTE),
            new NBTScalerNode("inGround", NBT_Type.TAG_BYTE),
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

        public override Entity LoadTree (NBT_Value tree)
        {
            NBT_Compound ctree = tree as NBT_Compound;
            if (ctree == null || base.LoadTree(tree) == null) {
                return null;
            }

            _xTile = ctree["xTile"].ToNBTShort();
            _yTile = ctree["yTile"].ToNBTShort();
            _zTile = ctree["zTile"].ToNBTShort();
            _inTile = ctree["inTile"].ToNBTByte();
            _shake = ctree["shake"].ToNBTByte();
            _inGround = ctree["inGround"].ToNBTByte();

            return this;
        }

        public override NBT_Value BuildTree ()
        {
            NBT_Compound tree = base.BuildTree() as NBT_Compound;
            tree["xTile"] = new NBT_Short(_xTile);
            tree["yTile"] = new NBT_Short(_yTile);
            tree["zTile"] = new NBT_Short(_zTile);
            tree["inTile"] = new NBT_Byte(_inTile);
            tree["shake"] = new NBT_Byte(_shake);
            tree["inGround"] = new NBT_Byte(_inGround);

            return tree;
        }

        public override bool ValidateTree (NBT_Value tree)
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
