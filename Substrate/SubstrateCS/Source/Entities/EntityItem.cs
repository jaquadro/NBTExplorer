using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Entities
{
    using Substrate.Nbt;

    public class EntityItem : EntityTyped
    {
        public static readonly SchemaNodeCompound ItemSchema = EntityTyped.Schema.MergeInto(new SchemaNodeCompound("")
        {
            new SchemaNodeString("id", "Item"),
            new SchemaNodeScaler("Health", TagType.TAG_SHORT),
            new SchemaNodeScaler("Age", TagType.TAG_SHORT),
            new SchemaNodeCompound("Item", Item.ItemSchema),
        });

        private short _health;
        private short _age;

        private Item _item;

        public int Health
        {
            get { return _health; }
            set { _health = (short)value; }
        }

        public int Age
        {
            get { return _age; }
            set { _age = (short)value; }
        }

        public Item Item 
        {
            get { return _item; }
            set { _item = value; }
        }

        public EntityItem ()
            : base("Item")
        {
        }

        public EntityItem (EntityTyped e)
            : base(e)
        {
            EntityItem e2 = e as EntityItem;
            if (e2 != null) {
                _health = e2._health;
                _age = e2._age;
                _item = e2._item.Copy();
            }
        }


        #region INBTObject<Entity> Members

        public override EntityTyped LoadTree (TagNode tree)
        {
            TagNodeCompound ctree = tree as TagNodeCompound;
            if (ctree == null || base.LoadTree(tree) == null) {
                return null;
            }

            _health = ctree["Health"].ToTagShort();
            _age = ctree["Age"].ToTagShort();

            _item = new Item().LoadTree(ctree["Item"]);

            return this;
        }

        public override TagNode BuildTree ()
        {
            TagNodeCompound tree = base.BuildTree() as TagNodeCompound;
            tree["Health"] = new TagNodeShort(_health);
            tree["Age"] = new TagNodeShort(_age);
            tree["Item"] = _item.BuildTree();

            return tree;
        }

        public override bool ValidateTree (TagNode tree)
        {
            return new NBTVerifier(tree, ItemSchema).Verify();
        }

        #endregion


        #region ICopyable<Entity> Members

        public override EntityTyped Copy ()
        {
            return new EntityItem(this);
        }

        #endregion
    }
}
