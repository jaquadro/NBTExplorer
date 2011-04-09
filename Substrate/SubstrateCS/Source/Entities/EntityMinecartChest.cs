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
            new NBTListNode("Items", TagType.TAG_COMPOUND, ItemCollection.InventorySchema),
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

        public override Entity LoadTree (TagValue tree)
        {
            TagCompound ctree = tree as TagCompound;
            if (ctree == null || base.LoadTree(tree) == null) {
                return null;
            }

            TagList items = ctree["Items"].ToTagList();
            _items = _items.LoadTree(items);

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
