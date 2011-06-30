using System;
using System.Collections.Generic;
using System.Text;
using Substrate.Core;
using Substrate.Nbt;

namespace Substrate
{
    public interface IItemContainer
    {
        ItemCollection Items { get; }
    }

    public class Item : INBTObject<Item>, ICopyable<Item>
    {
        public static readonly SchemaNodeCompound ItemSchema = new SchemaNodeCompound("")
        {
            new SchemaNodeScaler("id", TagType.TAG_SHORT),
            new SchemaNodeScaler("Damage", TagType.TAG_SHORT),
            new SchemaNodeScaler("Count", TagType.TAG_BYTE),
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

        public Item LoadTree (TagNode tree)
        {
            TagNodeCompound ctree = tree as TagNodeCompound;
            if (ctree == null) {
                return null;
            }

            _id = ctree["id"].ToTagShort();
            _count = ctree["Count"].ToTagByte();
            _damage = ctree["Damage"].ToTagShort();

            return this;
        }

        public Item LoadTreeSafe (TagNode tree)
        {
            if (!ValidateTree(tree)) {
                return null;
            }

            return LoadTree(tree);
        }

        public TagNode BuildTree ()
        {
            TagNodeCompound tree = new TagNodeCompound();
            tree["id"] = new TagNodeShort(_id);
            tree["Count"] = new TagNodeByte(_count);
            tree["Damage"] = new TagNodeShort(_damage);

            return tree;
        }

        public bool ValidateTree (TagNode tree)
        {
            return new NBTVerifier(tree, ItemSchema).Verify();
        }

        #endregion
    }

    public class ItemCollection : INBTObject<ItemCollection>, ICopyable<ItemCollection>
    {
        public static readonly SchemaNodeCompound InventorySchema = Item.ItemSchema.MergeInto(new SchemaNodeCompound("")
        {
            new SchemaNodeScaler("Slot", TagType.TAG_BYTE),
        });

        public static readonly SchemaNodeList ListSchema = new SchemaNodeList("", TagType.TAG_COMPOUND, InventorySchema);

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

        public ItemCollection LoadTree (TagNode tree)
        {
            TagNodeList ltree = tree as TagNodeList;
            if (ltree == null) {
                return null;
            }

            foreach (TagNodeCompound item in ltree) {
                int slot = item["Slot"].ToTagByte();
                _items[slot] = new Item().LoadTree(item);
            }

            return this;
        }

        public ItemCollection LoadTreeSafe (TagNode tree)
        {
            if (!ValidateTree(tree)) {
                return null;
            }

            return LoadTree(tree);
        }

        public TagNode BuildTree ()
        {
            TagNodeList list = new TagNodeList(TagType.TAG_COMPOUND);

            foreach (KeyValuePair<int, Item> item in _items) {
                TagNodeCompound itemtree = item.Value.BuildTree() as TagNodeCompound;
                itemtree["Slot"] = new TagNodeByte((byte)item.Key);
                list.Add(itemtree);
            }

            return list;
        }

        public bool ValidateTree (TagNode tree)
        {
            return new NBTVerifier(tree, ListSchema).Verify();
        }

        #endregion
    }
}
