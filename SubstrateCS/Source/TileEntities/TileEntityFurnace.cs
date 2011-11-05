using System;
using System.Collections.Generic;
using Substrate.Core;
using Substrate.Nbt;

namespace Substrate.TileEntities
{
    public class TileEntityFurnace : TileEntity, IItemContainer
    {
        public static readonly SchemaNodeCompound FurnaceSchema = TileEntity.Schema.MergeInto(new SchemaNodeCompound("")
        {
            new SchemaNodeString("id", TypeId),
            new SchemaNodeScaler("BurnTime", TagType.TAG_SHORT),
            new SchemaNodeScaler("CookTime", TagType.TAG_SHORT),
            new SchemaNodeList("Items", TagType.TAG_COMPOUND, ItemCollection.Schema),
        });

        public static string TypeId
        {
            get { return "Furnace"; }
        }

        private const int _CAPACITY = 3;

        private short _burnTime;
        private short _cookTime;

        private ItemCollection _items;

        public int BurnTime
        {
            get { return _burnTime; }
            set { _burnTime = (short)value; }
        }

        public int CookTime
        {
            get { return _cookTime; }
            set { _cookTime = (short)value; }
        }

        protected TileEntityFurnace (string id)
            : base(id)
        {
            _items = new ItemCollection(_CAPACITY);
        }

        public TileEntityFurnace ()
            : this(TypeId)
        {
        }

        public TileEntityFurnace (TileEntity te)
            : base(te)
        {
            TileEntityFurnace tec = te as TileEntityFurnace;
            if (tec != null) {
                _cookTime = tec._cookTime;
                _burnTime = tec._burnTime;
                _items = tec._items.Copy();
            }
            else {
                _items = new ItemCollection(_CAPACITY);
            }
        }


        #region ICopyable<TileEntity> Members

        public override TileEntity Copy ()
        {
            return new TileEntityFurnace(this);
        }

        #endregion


        #region IItemContainer Members

        public ItemCollection Items
        {
            get { return _items; }
        }

        #endregion


        #region INBTObject<TileEntity> Members

        public override TileEntity LoadTree (TagNode tree)
        {
            TagNodeCompound ctree = tree as TagNodeCompound;
            if (ctree == null || base.LoadTree(tree) == null) {
                return null;
            }

            _burnTime = ctree["BurnTime"].ToTagShort();
            _cookTime = ctree["CookTime"].ToTagShort();

            TagNodeList items = ctree["Items"].ToTagList();
            _items = new ItemCollection(_CAPACITY).LoadTree(items);

            return this;
        }

        public override TagNode BuildTree ()
        {
            TagNodeCompound tree = base.BuildTree() as TagNodeCompound;
            tree["BurnTime"] = new TagNodeShort(_burnTime);
            tree["CookTime"] = new TagNodeShort(_cookTime);
            tree["Items"] = _items.BuildTree();

            return tree;
        }

        public override bool ValidateTree (TagNode tree)
        {
            return new NbtVerifier(tree, FurnaceSchema).Verify();
        }

        #endregion
    }
}
