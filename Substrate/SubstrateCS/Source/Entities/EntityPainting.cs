using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Entities
{
    using Substrate.NBT;

    public class EntityPainting : Entity
    {
        public enum DirectionType
        {
            EAST = 0,
            NORTH = 1,
            WEST = 2,
            SOUTH = 3,
        }

        public static readonly SchemaNodeCompound PaintingSchema = BaseSchema.MergeInto(new SchemaNodeCompound("")
        {
            new SchemaNodeString("id", "Painting"),
            new SchemaNodeScaler("Dir", TagType.TAG_BYTE),
            new SchemaNodeScaler("TileX", TagType.TAG_SHORT),
            new SchemaNodeScaler("TileY", TagType.TAG_SHORT),
            new SchemaNodeScaler("TileZ", TagType.TAG_SHORT),
            new SchemaNodeScaler("Motive", TagType.TAG_STRING),
        });

        private DirectionType _dir;
        private string _motive;
        private short _xTile;
        private short _yTile;
        private short _zTile;

        public DirectionType Direction
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

        public override Entity LoadTree (TagNode tree)
        {
            TagNodeCompound ctree = tree as TagNodeCompound;
            if (ctree == null || base.LoadTree(tree) == null) {
                return null;
            }

            _dir = (DirectionType) ctree["Dir"].ToTagByte().Data;
            _motive = ctree["Motive"].ToTagString();
            _xTile = ctree["TileX"].ToTagShort();
            _yTile = ctree["TileY"].ToTagShort();
            _zTile = ctree["TileZ"].ToTagShort();

            return this;
        }

        public override TagNode BuildTree ()
        {
            TagNodeCompound tree = base.BuildTree() as TagNodeCompound;
            tree["Dir"] = new TagNodeByte((byte)_dir);
            tree["Motive"] = new TagNodeString(_motive);
            tree["TileX"] = new TagNodeShort(_xTile);
            tree["TileY"] = new TagNodeShort(_yTile);
            tree["TileZ"] = new TagNodeShort(_zTile);

            return tree;
        }

        public override bool ValidateTree (TagNode tree)
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
