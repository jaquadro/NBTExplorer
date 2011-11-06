using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Entities
{
    using Substrate.Nbt;

    public class EntityPainting : TypedEntity
    {
        public enum DirectionType
        {
            EAST = 0,
            NORTH = 1,
            WEST = 2,
            SOUTH = 3,
        }

        public static readonly SchemaNodeCompound PaintingSchema = TypedEntity.Schema.MergeInto(new SchemaNodeCompound("")
        {
            new SchemaNodeString("id", TypeId),
            new SchemaNodeScaler("Dir", TagType.TAG_BYTE),
            new SchemaNodeScaler("TileX", TagType.TAG_INT),
            new SchemaNodeScaler("TileY", TagType.TAG_INT),
            new SchemaNodeScaler("TileZ", TagType.TAG_INT),
            new SchemaNodeScaler("Motive", TagType.TAG_STRING),
        });

        public static string TypeId
        {
            get { return "Painting"; }
        }

        private DirectionType _dir;
        private string _motive = "";
        private int _xTile;
        private int _yTile;
        private int _zTile;

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
            set { _xTile = value; }
        }

        public int TileY
        {
            get { return _yTile; }
            set { _yTile = value; }
        }

        public int TileZ
        {
            get { return _zTile; }
            set { _zTile = value; }
        }

        protected EntityPainting (string id)
            : base(id)
        {
        }

        public EntityPainting ()
            : this(TypeId)
        {
        }

        public EntityPainting (TypedEntity e)
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

        public override void MoveBy (int diffX, int diffY, int diffZ)
        {
            base.MoveBy(diffX, diffY, diffZ);

            _xTile += diffX;
            _yTile += diffY;
            _zTile += diffZ;
        }

        #region INBTObject<Entity> Members

        public override TypedEntity LoadTree (TagNode tree)
        {
            TagNodeCompound ctree = tree as TagNodeCompound;
            if (ctree == null || base.LoadTree(tree) == null) {
                return null;
            }

            _dir = (DirectionType) ctree["Dir"].ToTagByte().Data;
            _motive = ctree["Motive"].ToTagString();
            _xTile = ctree["TileX"].ToTagInt();
            _yTile = ctree["TileY"].ToTagInt();
            _zTile = ctree["TileZ"].ToTagInt();

            return this;
        }

        public override TagNode BuildTree ()
        {
            TagNodeCompound tree = base.BuildTree() as TagNodeCompound;
            tree["Dir"] = new TagNodeByte((byte)_dir);
            tree["Motive"] = new TagNodeString(_motive);
            tree["TileX"] = new TagNodeInt(_xTile);
            tree["TileY"] = new TagNodeInt(_yTile);
            tree["TileZ"] = new TagNodeInt(_zTile);

            return tree;
        }

        public override bool ValidateTree (TagNode tree)
        {
            return new NbtVerifier(tree, PaintingSchema).Verify();
        }

        #endregion


        #region ICopyable<Entity> Members

        public override TypedEntity Copy ()
        {
            return new EntityPainting(this);
        }

        #endregion
    }
}
