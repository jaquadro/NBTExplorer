using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Entities
{
    using Substrate.NBT;

    public class EntityItem : Entity
    {
        public static readonly NBTCompoundNode ItemSchema = BaseSchema.MergeInto(new NBTCompoundNode("")
        {
            new NBTStringNode("id", "Item"),
            new NBTScalerNode("Health", TagType.TAG_SHORT),
            new NBTScalerNode("Age", TagType.TAG_SHORT),
            new NBTCompoundNode("Item", Item.ItemSchema),
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

        public EntityItem (Entity e)
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

        public override Entity LoadTree (TagValue tree)
        {
            TagCompound ctree = tree as TagCompound;
            if (ctree == null || base.LoadTree(tree) == null) {
                return null;
            }

            _health = ctree["Health"].ToTagShort();
            _age = ctree["Age"].ToTagShort();

            _item = new Item().LoadTree(ctree["Item"]);

            return this;
        }

        public override TagValue BuildTree ()
        {
            TagCompound tree = base.BuildTree() as TagCompound;
            tree["Health"] = new TagShort(_health);
            tree["Age"] = new TagShort(_age);
            tree["Item"] = _item.BuildTree();

            return tree;
        }

        public override bool ValidateTree (TagValue tree)
        {
            return new NBTVerifier(tree, ItemSchema).Verify();
        }

        #endregion


        #region ICopyable<Entity> Members

        public override Entity Copy ()
        {
            return new EntityItem(this);
        }

        #endregion
    }
}
