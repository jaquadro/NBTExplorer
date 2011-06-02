using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate
{
    using NBT;
    using Utility;

    public interface IItemContainer
    {
        ItemCollection Items { get; }
    }

    public class Item : INBTObject<Item>, ICopyable<Item>
    {
        public static readonly NBTCompoundNode ItemSchema = new NBTCompoundNode("")
        {
            new NBTScalerNode("id", TagType.TAG_SHORT),
            new NBTScalerNode("Damage", TagType.TAG_SHORT),
            new NBTScalerNode("Count", TagType.TAG_BYTE),
        };

        private short _id;
        private byte _count;
        private short _damage;

        public ItemInfo Info
        {
            get { return ItemInfo.ItemTable[_id]; }
        }

        public int ID
        {
            get { return _id; }
            set { _id = (short)value; }
        }

        public int Damage
        {
            get { return _damage; }
            set { _damage = (short)value; }
        }

        public int Count
        {
            get { return _count; }
            set { _count = (byte)value; }
        }

        public Item () 
        {
        }

        public Item (int id)
        {
            _id = (short)id;
        }

        #region ICopyable<Item> Members

        public Item Copy ()
        {
            Item item = new Item();
            item._id = _id;
            item._count = _count;
            item._damage = _damage;

            return item;
        }

        #endregion

        #region INBTObject<Item> Members

        public Item LoadTree (TagValue tree)
        {
            TagCompound ctree = tree as TagCompound;
            if (ctree == null) {
                return null;
            }

            _id = ctree["id"].ToTagShort();
            _count = ctree["Count"].ToTagByte();
            _damage = ctree["Damage"].ToTagShort();

            return this;
        }

        public Item LoadTreeSafe (TagValue tree)
        {
            if (!ValidateTree(tree)) {
                return null;
            }

            return LoadTree(tree);
        }

        public TagValue BuildTree ()
        {
            TagCompound tree = new TagCompound();
            tree["id"] = new TagShort(_id);
            tree["Count"] = new TagByte(_count);
            tree["Damage"] = new TagShort(_damage);

            return tree;
        }

        public bool ValidateTree (TagValue tree)
        {
            return new NBTVerifier(tree, ItemSchema).Verify();
        }

        #endregion
    }

    public class ItemCollection : INBTObject<ItemCollection>, ICopyable<ItemCollection>
    {
        public static readonly NBTCompoundNode InventorySchema = Item.ItemSchema.MergeInto(new NBTCompoundNode("")
        {
            new NBTScalerNode("Slot", TagType.TAG_BYTE),
        });

        public static readonly NBTListNode ListSchema = new NBTListNode("", TagType.TAG_COMPOUND, InventorySchema);

        protected Dictionary<int, Item> _items;
        protected int _capacity;

        public ItemCollection (int capacity)
        {
            _capacity = capacity;
            _items = new Dictionary<int, Item>();
        }

        public int Capacity
        {
            get { return _capacity; }
        }

        public int Count
        {
            get { return _items.Count; }
        }

        public Item this [int slot]
        {
            get
            {
                Item item;
                _items.TryGetValue(slot, out item);
                return item;
            }

            set
            {
                if (slot < 0 || slot >= _capacity) {
                    return;
                }
                _items[slot] = value;
            }
        }

        public bool ItemExists (int slot)
        {
            return _items.ContainsKey(slot);
        }

        public bool Clear (int slot)
        {
            return _items.Remove(slot);
        }

        public void ClearAllItems ()
        {
            _items.Clear();
        }

        #region ICopyable<ItemCollection> Members

        public ItemCollection Copy ()
        {
            ItemCollection ic = new ItemCollection(_capacity);
            foreach (KeyValuePair<int, Item> item in _items) {
                ic[item.Key] = item.Value.Copy();
            }
            return ic;
        }

        #endregion

        #region INBTObject<ItemCollection> Members

        public ItemCollection LoadTree (TagValue tree)
        {
            TagList ltree = tree as TagList;
            if (ltree == null) {
                return null;
            }

            foreach (TagCompound item in ltree) {
                int slot = item["Slot"].ToTagByte();
                _items[slot] = new Item().LoadTree(item);
            }

            return this;
        }

        public ItemCollection LoadTreeSafe (TagValue tree)
        {
            if (!ValidateTree(tree)) {
                return null;
            }

            return LoadTree(tree);
        }

        public TagValue BuildTree ()
        {
            TagList list = new TagList(TagType.TAG_COMPOUND);

            foreach (KeyValuePair<int, Item> item in _items) {
                TagCompound itemtree = item.Value.BuildTree() as TagCompound;
                itemtree["Slot"] = new TagByte((byte)item.Key);
                list.Add(itemtree);
            }

            return list;
        }

        public bool ValidateTree (TagValue tree)
        {
            return new NBTVerifier(tree, ListSchema).Verify();
        }

        #endregion
    }
}
