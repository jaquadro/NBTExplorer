using System;
using System.Collections.Generic;
using System.Text;
using Substrate.Core;
using Substrate.Nbt;

namespace Substrate
{
    /// <summary>
    /// Represents an item (or item stack) within an item slot.
    /// </summary>
    public class Item : INbtObject<Item>, ICopyable<Item>
    {
        private static readonly SchemaNodeCompound _schema = new SchemaNodeCompound("")
        {
            new SchemaNodeScaler("id", TagType.TAG_SHORT),
            new SchemaNodeScaler("Damage", TagType.TAG_SHORT),
            new SchemaNodeScaler("Count", TagType.TAG_BYTE),
        };

        private short _id;
        private byte _count;
        private short _damage;

        /// <summary>
        /// Constructs an empty <see cref="Item"/> instance.
        /// </summary>
        public Item ()
        {
        }

        /// <summary>
        /// Constructs an <see cref="Item"/> instance representing the given item id.
        /// </summary>
        /// <param name="id">An item id.</param>
        public Item (int id)
        {
            _id = (short)id;
        }

        #region Properties

        /// <summary>
        /// Gets an <see cref="ItemInfo"/> entry for this item's type.
        /// </summary>
        public ItemInfo Info
        {
            get { return ItemInfo.ItemTable[_id]; }
        }

        /// <summary>
        /// Gets or sets the current type (id) of the item.
        /// </summary>
        public int ID
        {
            get { return _id; }
            set { _id = (short)value; }
        }

        /// <summary>
        /// Gets or sets the damage value of the item.
        /// </summary>
        /// <remarks>The damage value may represent a generic data value for some items.</remarks>
        public int Damage
        {
            get { return _damage; }
            set { _damage = (short)value; }
        }

        /// <summary>
        /// Gets or sets the number of this item stacked together in an item slot.
        /// </summary>
        public int Count
        {
            get { return _count; }
            set { _count = (byte)value; }
        }

        /// <summary>
        /// Gets a <see cref="SchemaNode"/> representing the schema of an item.
        /// </summary>
        public static SchemaNodeCompound Schema
        {
            get { return _schema; }
        }

        #endregion

        #region ICopyable<Item> Members

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public Item LoadTreeSafe (TagNode tree)
        {
            if (!ValidateTree(tree)) {
                return null;
            }

            return LoadTree(tree);
        }

        /// <inheritdoc/>
        public TagNode BuildTree ()
        {
            TagNodeCompound tree = new TagNodeCompound();
            tree["id"] = new TagNodeShort(_id);
            tree["Count"] = new TagNodeByte(_count);
            tree["Damage"] = new TagNodeShort(_damage);

            return tree;
        }

        /// <inheritdoc/>
        public bool ValidateTree (TagNode tree)
        {
            return new NbtVerifier(tree, _schema).Verify();
        }

        #endregion
    }

    /// <summary>
    /// Represents a collection of items, such as a chest or an inventory.
    /// </summary>
    /// <remarks>ItemCollections have a limited number of slots that depends on where they are used.</remarks>
    public class ItemCollection : INbtObject<ItemCollection>, ICopyable<ItemCollection>
    {
        private static readonly SchemaNodeCompound _schema = Item.Schema.MergeInto(new SchemaNodeCompound("")
        {
            new SchemaNodeScaler("Slot", TagType.TAG_BYTE),
        });

        private static readonly SchemaNodeList _listSchema = new SchemaNodeList("", TagType.TAG_COMPOUND, _schema);

        private Dictionary<int, Item> _items;
        private int _capacity;

        /// <summary>
        /// Constructs an <see cref="ItemCollection"/> with at most <paramref name="capacity"/> item slots.
        /// </summary>
        /// <param name="capacity">The upper bound on item slots available.</param>
        /// <remarks>The <paramref name="capacity"/> parameter does not necessarily indicate the true capacity of an item collection.
        /// The player object, for example, contains a conventional inventory, a range of invalid slots, and then equipment.  Capacity in
        /// this case would refer to the highest equipment slot.</remarks>
        public ItemCollection (int capacity)
        {
            _capacity = capacity;
            _items = new Dictionary<int, Item>();
        }

        #region Properties

        /// <summary>
        /// Gets the capacity of the item collection.
        /// </summary>
        public int Capacity
        {
            get { return _capacity; }
        }

        /// <summary>
        /// Gets the current number of item slots actually used in the collection.
        /// </summary>
        public int Count
        {
            get { return _items.Count; }
        }

        /// <summary>
        /// Gets or sets an item in a given item slot.
        /// </summary>
        /// <param name="slot">The item slot to query or insert an item or item stack into.</param>
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

        /// <summary>
        /// Gets a <see cref="SchemaNode"/> representing the schema of an item collection.
        /// </summary>
        public static SchemaNodeCompound Schema
        {
            get { return _schema; }
        }

        #endregion

        /// <summary>
        /// Checks if an item exists in the given item slot.
        /// </summary>
        /// <param name="slot">The item slot to check.</param>
        /// <returns>True if an item or stack of items exists in the given slot.</returns>
        public bool ItemExists (int slot)
        {
            return _items.ContainsKey(slot);
        }

        /// <summary>
        /// Removes an item from the given item slot, if it exists.
        /// </summary>
        /// <param name="slot">The item slot to clear.</param>
        /// <returns>True if an item was removed; false otherwise.</returns>
        public bool Clear (int slot)
        {
            return _items.Remove(slot);
        }

        /// <summary>
        /// Removes all items from the item collection.
        /// </summary>
        public void ClearAllItems ()
        {
            _items.Clear();
        }

        #region ICopyable<ItemCollection> Members

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public ItemCollection LoadTreeSafe (TagNode tree)
        {
            if (!ValidateTree(tree)) {
                return null;
            }

            return LoadTree(tree);
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public bool ValidateTree (TagNode tree)
        {
            return new NbtVerifier(tree, _listSchema).Verify();
        }

        #endregion
    }
}
