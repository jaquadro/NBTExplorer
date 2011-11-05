using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Entities
{
    using Substrate.Nbt;

    public class EntityXPOrb : TypedEntity
    {
        public static readonly SchemaNodeCompound XPOrbSchema = TypedEntity.Schema.MergeInto(new SchemaNodeCompound("")
        {
            new SchemaNodeString("id", TypeId),
            new SchemaNodeScaler("Health", TagType.TAG_SHORT),
            new SchemaNodeScaler("Age", TagType.TAG_SHORT),
            new SchemaNodeScaler("Value", TagType.TAG_SHORT),
        });

        public static string TypeId
        {
            get { return "XPOrb"; }
        }

        private short _health;
        private short _age;
        private short _value;

        public int Health
        {
            get { return _health; }
            set { _health = (short)(value & 0xFF); }
        }

        public int Age
        {
            get { return _age; }
            set { _age = (short)value; }
        }

        public int Value
        {
            get { return _value; }
            set { _value = (short)value; }
        }

        protected EntityXPOrb (string id)
            : base(id)
        {
        }

        public EntityXPOrb ()
            : this(TypeId)
        {
        }

        public EntityXPOrb (TypedEntity e)
            : base(e)
        {
            EntityXPOrb e2 = e as EntityXPOrb;
            if (e2 != null) {
                _health = e2._health;
                _age = e2._age;
                _value = e2._value;
            }
        }


        #region INBTObject<Entity> Members

        public override TypedEntity LoadTree (TagNode tree)
        {
            TagNodeCompound ctree = tree as TagNodeCompound;
            if (ctree == null || base.LoadTree(tree) == null) {
                return null;
            }

            _health = ctree["Health"].ToTagShort();
            _age = ctree["Age"].ToTagShort();
            _value = ctree["Value"].ToTagShort();

            return this;
        }

        public override TagNode BuildTree ()
        {
            TagNodeCompound tree = base.BuildTree() as TagNodeCompound;
            tree["Health"] = new TagNodeShort(_health);
            tree["Age"] = new TagNodeShort(_age);
            tree["Value"] = new TagNodeShort(_value);

            return tree;
        }

        public override bool ValidateTree (TagNode tree)
        {
            return new NbtVerifier(tree, XPOrbSchema).Verify();
        }

        #endregion


        #region ICopyable<Entity> Members

        public override TypedEntity Copy ()
        {
            return new EntityXPOrb(this);
        }

        #endregion
    }
}
