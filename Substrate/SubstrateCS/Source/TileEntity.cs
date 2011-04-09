using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate
{
    using NBT;
    using Utility;

    public class TileEntity : INBTObject<TileEntity>, ICopyable<TileEntity>
    {
        public static readonly NBTCompoundNode BaseSchema = new NBTCompoundNode("")
        {
            new NBTScalerNode("id", TagType.TAG_STRING),
            new NBTScalerNode("x", TagType.TAG_INT),
            new NBTScalerNode("y", TagType.TAG_INT),
            new NBTScalerNode("z", TagType.TAG_INT),
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

        public virtual TileEntity LoadTree (TagValue tree)
        {
            TagCompound ctree = tree as TagCompound;
            if (ctree == null) {
                return null;
            }

            _id = ctree["id"].ToTagString();
            _x = ctree["x"].ToTagInt();
            _y = ctree["y"].ToTagInt();
            _z = ctree["z"].ToTagInt();

            return this;
        }

        public virtual TileEntity LoadTreeSafe (TagValue tree)
        {
            if (!ValidateTree(tree)) {
                return null;
            }

            return LoadTree(tree);
        }

        public virtual TagValue BuildTree ()
        {
            TagCompound tree = new TagCompound();
            tree["id"] = new TagString(_id);
            tree["x"] = new TagInt(_x);
            tree["y"] = new TagInt(_y);
            tree["z"] = new TagInt(_z);

            return tree;
        }

        public virtual bool ValidateTree (TagValue tree)
        {
            return new NBTVerifier(tree, BaseSchema).Verify();
        }

        #endregion
    }

}
