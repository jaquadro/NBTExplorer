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
            new NBTScalerNode("id", NBT_Type.TAG_SHORT),
            new NBTScalerNode("Damage", NBT_Type.TAG_SHORT),
            new NBTScalerNode("Count", NBT_Type.TAG_BYTE),
        };

        private short _id;
        private byte _count;
        private short _damage;

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

        public Item LoadTree (NBT_Value tree)
        {
            NBT_Compound ctree = tree as NBT_Compound;
            if (ctree == null) {
                return null;
            }

            _id = ctree["id"].ToNBTShort();
            _count = ctree["Count"].ToNBTByte();
            _damage = ctree["Damage"].ToNBTShort();

            return this;
        }

        public Item LoadTreeSafe (NBT_Value tree)
        {
            if (!ValidateTree(tree)) {
                return null;
            }

            return LoadTree(tree);
        }

        public NBT_Value BuildTree ()
        {
            NBT_Compound tree = new NBT_Compound();
            tree["id"] = new NBT_Short(_id);
            tree["Count"] = new NBT_Byte(_count);
            tree["Damage"] = new NBT_Short(_damage);

            return tree;
        }

        public bool ValidateTree (NBT_Value tree)
        {
            return new NBTVerifier(tree, ItemSchema).Verify();
        }

        #endregion
    }

    public class ItemCollection : INBTObject<ItemCollection>, ICopyable<ItemCollection>
    {
        public static readonly NBTCompoundNode InventorySchema = Item.ItemSchema.MergeInto(new NBTCompoundNode("")
        {
            new NBTScalerNode("Slot", NBT_Type.TAG_BYTE),
        });

        public static readonly NBTListNode ListSchema = new NBTListNode("", NBT_Type.TAG_COMPOUND, InventorySchema);

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

        public ItemCollection LoadTree (NBT_Value tree)
        {
            NBT_List ltree = tree as NBT_List;
            if (ltree == null) {
                return null;
            }

            foreach (NBT_Compound item in ltree) {
                int slot = item["Slot"].ToNBTByte();
                _items[slot] = new Item().LoadTree(item);
            }

            return this;
        }

        public ItemCollection LoadTreeSafe (NBT_Value tree)
        {
            if (!ValidateTree(tree)) {
                return null;
            }

            return LoadTree(tree);
        }

        public NBT_Value BuildTree ()
        {
            NBT_List list = new NBT_List(NBT_Type.TAG_COMPOUND);

            foreach (KeyValuePair<int, Item> item in _items) {
                NBT_Compound itemtree = item.Value.BuildTree() as NBT_Compound;
                itemtree["Slot"] = new NBT_Byte((byte)item.Key);
                list.Add(itemtree);
            }

            return list;
        }

        public bool ValidateTree (NBT_Value tree)
        {
            return new NBTVerifier(tree, ListSchema).Verify();
        }

        #endregion
    }
}
