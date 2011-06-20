using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate
{
    using NBT;
    using Utility;

    public class TileEntity : INBTObject<TileEntity>, ICopyable<TileEntity>
    {
        public static readonly SchemaNodeCompound BaseSchema = new SchemaNodeCompound("")
        {
            new SchemaNodeScaler("id", TagType.TAG_STRING),
            new SchemaNodeScaler("x", TagType.TAG_INT),
            new SchemaNodeScaler("y", TagType.TAG_INT),
            new SchemaNodeScaler("z", TagType.TAG_INT),
        };

        private string _id;
        private int _x;
        private int _y;
        private int _z;

        public string ID
        {
            get { return _id; }
        }

        public int X
        {
            get { return _x; }
            set { _x = value; }
        }

        public int Y
        {
            get { return _y; }
            set { _y = value; }
        }

        public int Z
        {
            get { return _z; }
            set { _z = value; }
        }

        public TileEntity (string id)
        {
            _id = id;
        }

        public TileEntity (TileEntity te)
        {
            _id = te._id;
            _x = te._x;
            _y = te._y;
            _z = te._z;
        }

        public bool LocatedAt (int x, int y, int z)
        {
            return _x == x && _y == y && _z == z;
        }


        #region ICopyable<TileEntity> Members

        public virtual TileEntity Copy ()
        {
            return new TileEntity(this);
        }

        #endregion


        #region INBTObject<TileEntity> Members

        public virtual TileEntity LoadTree (TagNode tree)
        {
            TagNodeCompound ctree = tree as TagNodeCompound;
            if (ctree == null) {
                return null;
            }

            _id = ctree["id"].ToTagString();
            _x = ctree["x"].ToTagInt();
            _y = ctree["y"].ToTagInt();
            _z = ctree["z"].ToTagInt();

            return this;
        }

        public virtual TileEntity LoadTreeSafe (TagNode tree)
        {
            if (!ValidateTree(tree)) {
                return null;
            }

            return LoadTree(tree);
        }

        public virtual TagNode BuildTree ()
        {
            TagNodeCompound tree = new TagNodeCompound();
            tree["id"] = new TagNodeString(_id);
            tree["x"] = new TagNodeInt(_x);
            tree["y"] = new TagNodeInt(_y);
            tree["z"] = new TagNodeInt(_z);

            return tree;
        }

        public virtual bool ValidateTree (TagNode tree)
        {
            return new NBTVerifier(tree, BaseSchema).Verify();
        }

        #endregion
    }

}
