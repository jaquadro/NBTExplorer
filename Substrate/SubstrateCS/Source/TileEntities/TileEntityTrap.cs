using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Map.TileEntities
{
    using Substrate.Map.NBT;

    public class TileEntityTrap : TileEntity, IItemContainer
    {
        public static readonly NBTCompoundNode TrapSchema = BaseSchema.MergeInto(new NBTCompoundNode("")
        {
            new NBTStringNode("id", "Trap"),
            new NBTListNode("Items", NBT_Type.TAG_COMPOUND, ItemCollection.ListSchema),
        });

        private const int _CAPACITY = 8;

        private ItemCollection _items;

        public TileEntityTrap ()
            : base("Trap")
        {
            _items = new ItemCollection(_CAPACITY);
        }

        public TileEntityTrap (TileEntity te)
            : base(te)
        {
            TileEntityTrap tec = te as TileEntityTrap;
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
            return new TileEntityTrap(this);
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
            return new NBTVerifier(tree, TrapSchema).Verify();
        }

        #endregion
    }
}
