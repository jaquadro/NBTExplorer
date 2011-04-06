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
            new NBTScalerNode("Health", NBT_Type.TAG_SHORT),
            new NBTScalerNode("Age", NBT_Type.TAG_SHORT),
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

        public override Entity LoadTree (NBT_Value tree)
        {
            NBT_Compound ctree = tree as NBT_Compound;
            if (ctree == null || base.LoadTree(tree) == null) {
                return null;
            }

            _health = ctree["Health"].ToNBTShort();
            _age = ctree["Age"].ToNBTShort();

            _item = new Item().LoadTree(ctree["Item"]);

            return this;
        }

        public override NBT_Value BuildTree ()
        {
            NBT_Compound tree = base.BuildTree() as NBT_Compound;
            tree["Health"] = new NBT_Short(_health);
            tree["Age"] = new NBT_Short(_age);
            tree["Item"] = _item.BuildTree();

            return tree;
        }

        public override bool ValidateTree (NBT_Value tree)
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
