using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Map
{
    using NBT;
    using Utility;

    public class TileEntity : INBTObject<TileEntity>, ICopyable<TileEntity>
    {
        public static readonly NBTCompoundNode BaseSchema = new NBTCompoundNode("")
        {
            new NBTScalerNode("id", NBT_Type.TAG_STRING),
            new NBTScalerNode("x", NBT_Type.TAG_INT),
            new NBTScalerNode("y", NBT_Type.TAG_INT),
            new NBTScalerNode("z", NBT_Type.TAG_INT),
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

        public virtual TileEntity LoadTree (NBT_Value tree)
        {
            NBT_Compound ctree = tree as NBT_Compound;
            if (ctree == null) {
                return null;
            }

            _id = ctree["id"].ToNBTString();
            _x = ctree["x"].ToNBTInt();
            _y = ctree["y"].ToNBTInt();
            _z = ctree["z"].ToNBTInt();

            return this;
        }

        public virtual TileEntity LoadTreeSafe (NBT_Value tree)
        {
            if (!ValidateTree(tree)) {
                return null;
            }

            return LoadTree(tree);
        }

        public virtual NBT_Value BuildTree ()
        {
            NBT_Compound tree = new NBT_Compound();
            tree["id"] = new NBT_String(_id);
            tree["x"] = new NBT_Int(_x);
            tree["y"] = new NBT_Int(_y);
            tree["z"] = new NBT_Int(_z);

            return tree;
        }

        public virtual bool ValidateTree (NBT_Value tree)
        {
            return new NBTVerifier(tree, BaseSchema).Verify();
        }

        #endregion
    }

}
