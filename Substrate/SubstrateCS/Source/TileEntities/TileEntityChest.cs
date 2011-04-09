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
            new NBTListNode("Items", TagType.TAG_COMPOUND, ItemCollection.ListSchema),
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

        public override TileEntity LoadTree (TagValue tree)
        {
            TagCompound ctree = tree as TagCompound;
            if (ctree == null || base.LoadTree(tree) == null) {
                return null;
            }

            TagList items = ctree["Items"].ToTagList();
            _items = new ItemCollection(_CAPACITY).LoadTree(items);

            return this;
        }

        public override TagValue BuildTree ()
        {
            TagCompound tree = base.BuildTree() as TagCompound;
            tree["Items"] = _items.BuildTree();

            return tree;
        }

        public override bool ValidateTree (TagValue tree)
        {
            return new NBTVerifier(tree, ChestSchema).Verify();
        }

        #endregion
    }
}
