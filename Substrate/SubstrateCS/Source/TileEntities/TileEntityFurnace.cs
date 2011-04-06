using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Map.TileEntities
{
    using Substrate.Map.NBT;

    public class TileEntityFurnace : TileEntity, IItemContainer
    {
        public static readonly NBTCompoundNode FurnaceSchema = BaseSchema.MergeInto(new NBTCompoundNode("")
        {
            new NBTStringNode("id", "Furnace"),
            new NBTScalerNode("BurnTime", NBT_Type.TAG_SHORT),
            new NBTScalerNode("CookTime", NBT_Type.TAG_SHORT),
            new NBTListNode("Items", NBT_Type.TAG_COMPOUND, ItemCollection.ListSchema),
        });

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

        public TileEntityFurnace ()
            : base("Furnace")
        {
            _items = new ItemCollection(_CAPACITY);
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

        public override TileEntity LoadTree (NBT_Value tree)
        {
            NBT_Compound ctree = tree as NBT_Compound;
            if (ctree == null || base.LoadTree(tree) == null) {
                return null;
            }

            _burnTime = ctree["BurnTime"].ToNBTShort();
            _cookTime = ctree["CookTime"].ToNBTShort();

            NBT_List items = ctree["Items"].ToNBTList();
            _items = new ItemCollection(_CAPACITY).LoadTree(items);

            return this;
        }

        public override NBT_Value BuildTree ()
        {
            NBT_Compound tree = base.BuildTree() as NBT_Compound;
            tree["BurnTime"] = new NBT_Short(_burnTime);
            tree["CookTime"] = new NBT_Short(_cookTime);
            tree["Items"] = _items.BuildTree();

            return tree;
        }

        public override bool ValidateTree (NBT_Value tree)
        {
            return new NBTVerifier(tree, FurnaceSchema).Verify();
        }

        #endregion
    }
}
