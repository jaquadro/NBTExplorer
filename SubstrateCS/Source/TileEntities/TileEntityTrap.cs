using System;
using System.Collections.Generic;
using Substrate.Core;
using Substrate.Nbt;

namespace Substrate.TileEntities
{
    public class TileEntityTrap : TileEntity, IItemContainer
    {
        public static readonly SchemaNodeCompound TrapSchema = TileEntity.Schema.MergeInto(new SchemaNodeCompound("")
        {
            new SchemaNodeString("id", TypeId),
            new SchemaNodeList("Items", TagType.TAG_COMPOUND, ItemCollection.Schema),
        });

        public static string TypeId
        {
            get { return "Trap"; }
        }

        private const int _CAPACITY = 8;

        private ItemCollection _items;

        protected TileEntityTrap (string id)
            : base(id)
        {
            _items = new ItemCollection(_CAPACITY);
        }

        public TileEntityTrap ()
            : this(TypeId)
        {
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

        public override TileEntity LoadTree (TagNode tree)
        {
            TagNodeCompound ctree = tree as TagNodeCompound;
            if (ctree == null || base.LoadTree(tree) == null) {
                return null;
            }

            TagNodeList items = ctree["Items"].ToTagList();
            _items = new ItemCollection(_CAPACITY).LoadTree(items);

            return this;
        }

        public override TagNode BuildTree ()
        {
            TagNodeCompound tree = base.BuildTree() as TagNodeCompound;
            tree["Items"] = _items.BuildTree();

            return tree;
        }

        public override bool ValidateTree (TagNode tree)
        {
            return new NbtVerifier(tree, TrapSchema).Verify();
        }

        #endregion
    }
}
