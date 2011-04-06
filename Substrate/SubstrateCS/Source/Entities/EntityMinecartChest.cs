using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Entities
{
    using Substrate.NBT;

    public class EntityMinecartChest : EntityMinecart, IItemContainer
    {
        public static readonly NBTCompoundNode MinecartChestSchema = MinecartSchema.MergeInto(new NBTCompoundNode("")
        {
            new NBTListNode("Items", NBT_Type.TAG_COMPOUND, ItemCollection.InventorySchema),
        });

        private static int _CAPACITY = 27;

        private ItemCollection _items;

        public EntityMinecartChest ()
            : base()
        {
            _items = new ItemCollection(_CAPACITY);
        }

        public EntityMinecartChest (Entity e)
            : base(e)
        {
            EntityMinecartChest e2 = e as EntityMinecartChest;
            if (e2 != null) {
                _items = e2._items.Copy();
            }
        }
            
        #region IItemContainer Members

        public ItemCollection  Items
        {
            get { return _items; }
        }

        #endregion


        #region INBTObject<Entity> Members

        public override Entity LoadTree (NBT_Value tree)
        {
            NBT_Compound ctree = tree as NBT_Compound;
            if (ctree == null || base.LoadTree(tree) == null) {
                return null;
            }

            NBT_List items = ctree["Items"].ToNBTList();
            _items = _items.LoadTree(items);

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
            return new NBTVerifier(tree, MinecartChestSchema).Verify();
        }

        #endregion


        #region ICopyable<Entity> Members

        public override Entity Copy ()
        {
            return new EntityMinecartChest(this);
        }

        #endregion
    }
}
