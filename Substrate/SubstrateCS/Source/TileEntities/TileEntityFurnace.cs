using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.TileEntities
{
    using Substrate.NBT;

    public class TileEntityFurnace : TileEntity, IItemContainer
    {
        public static readonly NBTCompoundNode FurnaceSchema = BaseSchema.MergeInto(new NBTCompoundNode("")
        {
            new NBTStringNode("id", "Furnace"),
            new NBTScalerNode("BurnTime", TagType.TAG_SHORT),
            new NBTScalerNode("CookTime", TagType.TAG_SHORT),
            new NBTListNode("Items", TagType.TAG_COMPOUND, ItemCollection.InventorySchema),
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

        public override TileEntity LoadTree (TagValue tree)
        {
            TagCompound ctree = tree as TagCompound;
            if (ctree == null || base.LoadTree(tree) == null) {
                return null;
            }

            _burnTime = ctree["BurnTime"].ToTagShort();
            _cookTime = ctree["CookTime"].ToTagShort();

            TagList items = ctree["Items"].ToTagList();
            _items = new ItemCollection(_CAPACITY).LoadTree(items);

            return this;
        }

        public override TagValue BuildTree ()
        {
            TagCompound tree = base.BuildTree() as TagCompound;
            tree["BurnTime"] = new TagShort(_burnTime);
            tree["CookTime"] = new TagShort(_cookTime);
            tree["Items"] = _items.BuildTree();

            return tree;
        }

        public override bool ValidateTree (TagValue tree)
        {
            return new NBTVerifier(tree, FurnaceSchema).Verify();
        }

        #endregion
    }
}
