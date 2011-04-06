using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.TileEntities
{
    using Substrate.NBT;

    public class TileEntityChest : TileEntity, IItemContainer
    {
        public static readonly NBTCompoundNode ChestSchema = BaseSchema.MergeInto(new NBTCompoundNode("")
        {
            new NBTStringNode("id", "Chest"),
            new NBTListNode("Items", NBT_Type.TAG_COMPOUND, ItemCollection.ListSchema),
        });

        private const int _CAPACITY = 27;

        private ItemCollection _items;

        public TileEntityChest ()
            : base("Chest")
        {
            _items = new ItemCollection(_CAPACITY);
        }

        public TileEntityChest (TileEntity te)
            : base(te)
        {
            TileEntityChest tec = te as TileEntityChest;
            if (tec != null) {
                _items = tec._items.Copy();
            }
            else {
                _items = new ItemCollection(_CAPACITY);
            }
        }

        #region ICopyable<TileEntity> Members

        public override TileEntity Copy ()
        {
            return new TileEntityChest(this);
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

            NBT_List items = ctree["Items"].ToNBTList();
            _items = new ItemCollection(_CAPACITY).LoadTree(items);

            return this;
        }

        public override NBT_Value BuildTree ()
        {
            NBT_Compound tree = base.BuildTree() as NBT_Compound;
            tree["Items"] = _items.BuildTree();

            return tree;
        }

        public override bool ValidateTree (NBT_Value tree)
        {
            return new NBTVerifier(tree, ChestSchema).Verify();
        }

        #endregion
    }
}
