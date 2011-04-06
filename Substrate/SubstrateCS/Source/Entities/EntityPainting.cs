using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Map.Entities
{
    using Substrate.Map.NBT;

    public class EntityPainting : Entity
    {
        public enum Direction
        {
            EAST = 0,
            NORTH = 1,
            WEST = 2,
            SOUTH = 3,
        }

        public static readonly NBTCompoundNode PaintingSchema = BaseSchema.MergeInto(new NBTCompoundNode("")
        {
            new NBTStringNode("id", "Painting"),
            new NBTScalerNode("Dir", NBT_Type.TAG_BYTE),
            new NBTScalerNode("TileX", NBT_Type.TAG_SHORT),
            new NBTScalerNode("TileY", NBT_Type.TAG_SHORT),
            new NBTScalerNode("TileZ", NBT_Type.TAG_SHORT),
            new NBTScalerNode("Motive", NBT_Type.TAG_STRING),
        });

        private Direction _dir;
        private string _motive;
        private short _xTile;
        private short _yTile;
        private short _zTile;

        public Direction Direction
        {
            get { return _dir; }
            set { _dir = value; }
        }

        public string Motive
        {
            get { return _motive; }
            set { _motive = value; }
        }

        public int TileX
        {
            get { return _xTile; }
            set { _xTile = (short)value; }
        }

        public int TileY
        {
            get { return _yTile; }
            set { _yTile = (short)value; }
        }

        public int TileZ
        {
            get { return _zTile; }
            set { _zTile = (short)value; }
        }

        public EntityPainting ()
            : base("Painting")
        {
        }

        public EntityPainting (Entity e)
            : base(e)
        {
            EntityPainting e2 = e as EntityPainting;
            if (e2 != null) {
                _xTile = e2._xTile;
                _yTile = e2._yTile;
                _zTile = e2._zTile;
                _dir = e2._dir;
                _motive = e2._motive;
            }
        }


        #region INBTObject<Entity> Members

        public override Entity LoadTree (NBT_Value tree)
        {
            NBT_Compound ctree = tree as NBT_Compound;
            if (ctree == null || base.LoadTree(tree) == null) {
                return null;
            }

            _dir = (Direction) ctree["Dir"].ToNBTByte().Data;
            _motive = ctree["Motive"].ToNBTString();
            _xTile = ctree["TileX"].ToNBTShort();
            _yTile = ctree["TileY"].ToNBTShort();
            _zTile = ctree["TileZ"].ToNBTShort();

            return this;
        }

        public override NBT_Value BuildTree ()
        {
            NBT_Compound tree = base.BuildTree() as NBT_Compound;
            tree["Dir"] = new NBT_Byte((byte)_dir);
            tree["Motive"] = new NBT_String(_motive);
            tree["TileX"] = new NBT_Short(_xTile);
            tree["TileY"] = new NBT_Short(_yTile);
            tree["TileZ"] = new NBT_Short(_zTile);

            return tree;
        }

        public override bool ValidateTree (NBT_Value tree)
        {
            return new NBTVerifier(tree, PaintingSchema).Verify();
        }

        #endregion


        #region ICopyable<Entity> Members

        public override Entity Copy ()
        {
            return new EntityPainting(this);
        }

        #endregion
    }
}
