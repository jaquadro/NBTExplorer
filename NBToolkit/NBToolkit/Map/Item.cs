using System;
using System.Collections.Generic;
using System.Text;

namespace NBToolkit.Map
{
    using NBT;

    public interface IItemContainer
    {
        ItemCollection Items { get; }
    }

    public class Item
    {
        protected NBT_Compound _tree;

        public NBT_Compound Root
        {
            get { return _tree; }
        }

        public int ID
        {
            get { return _tree["id"].ToNBTShort(); }
            set { _tree["id"].ToNBTShort().Data = (short)value; }
        }

        public int Damage
        {
            get { return _tree["Damage"].ToNBTShort(); }
            set { _tree["Count"].ToNBTShort().Data = (short)value; }
        }

        public int Count
        {
            get { return _tree["Count"].ToNBTByte(); }
            set { _tree["Count"].ToNBTByte().Data = (byte) value; }
        }

        public Item (NBT_Compound tree)
        {
            _tree = tree;
        }
    }

    public class ItemCollection
    {
        protected NBT_List _list;
        protected int _capacity;

        public ItemCollection (NBT_List list, int capacity)
        {
            _list = list;
            _capacity = capacity;
        }

        public int Capacity
        {
            get { return _capacity; }
        }

        public int Count
        {
            get { return _list.Count; }
        }

        public Item this [int slot]
        {
            get
            {
                foreach (NBT_Compound tag in _list) {
                    if (tag["Slot"].ToNBTByte() == slot) {
                        return new Item(tag);
                    }
                }
                return null;
            }

            set
            {
                if (slot < 0 || slot >= _capacity) {
                    return;
                }
                foreach (NBT_Compound tag in _list) {
                    if (tag["Slot"].ToNBTByte() == slot) {
                        _list.Remove(tag);
                        break;
                    }
                }
                _list.Add(value.Root);
            }
        }

        public int GetItemID (int slot)
        {
            NBT_Compound tag = FindTag(slot);
            if (tag == null) {
                return 0;
            }

            return tag["id"].ToNBTShort();
        }

        public int GetItemCount (int slot)
        {
            NBT_Compound tag = FindTag(slot);
            if (tag == null) {
                return 0;
            }

            return tag["Count"].ToNBTByte();
        }

        public int GetItemDamage (int slot)
        {
            NBT_Compound tag = FindTag(slot);
            if (tag == null) {
                return 0;
            }

            return tag["Damage"].ToNBTShort();
        }

        public bool SetItemID (int slot, int id)
        {
            NBT_Compound tag = FindTag(slot);
            if (tag == null) {
                return false;
            }

            tag["id"].ToNBTShort().Data = (short) id;
            return true;
        }

        public bool SetItemCount (int slot, int count)
        {
            NBT_Compound tag = FindTag(slot);
            if (tag == null) {
                return false;
            }

            tag["Count"].ToNBTByte().Data = (byte) count;
            return true;
        }

        public bool SetItemDamage (int slot, int damage)
        {
            NBT_Compound tag = FindTag(slot);
            if (tag == null) {
                return false;
            }

            tag["Damage"].ToNBTShort().Data = (short)damage;
            return true;
        }

        public bool ClearItem (int slot)
        {
            foreach (NBT_Compound tag in _list) {
                if (tag["Slot"].ToNBTByte() == slot) {
                    _list.Remove(tag);
                    return true;
                }
            }

            return false;
        }

        public bool ItemExists (int slot)
        {
            return FindTag(slot) != null;
        }

        public void ClearAllItems ()
        {
            _list.Clear();
        }

        private NBT_Compound FindTag (int slot)
        {
            foreach (NBT_Compound tag in _list) {
                if (tag["Slot"].ToNBTByte() == slot) {
                    return tag;
                }
            }
            return null;
        }
    }
}
