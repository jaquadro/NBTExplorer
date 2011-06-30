using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Entities
{
    using Substrate.NBT;

    public class EntityMob : EntityTyped
    {
        public static readonly SchemaNodeCompound MobSchema = EntityTyped.Schema.MergeInto(new SchemaNodeCompound("")
        {
            new SchemaNodeString("id", "Mob"),
            new SchemaNodeScaler("AttackTime", TagType.TAG_SHORT),
            new SchemaNodeScaler("DeathTime", TagType.TAG_SHORT),
            new SchemaNodeScaler("Health", TagType.TAG_SHORT),
            new SchemaNodeScaler("HurtTime", TagType.TAG_SHORT),
        });

        private short _attackTime;
        private short _deathTime;
        private short _health;
        private short _hurtTime;

        public int AttackTime
        {
            get { return _attackTime; }
            set { _attackTime = (short)value; }
        }

        public int DeathTime
        {
            get { return _deathTime; }
            set { _deathTime = (short)value; }
        }

        public int Health
        {
            get { return _health; }
            set { _health = (short)value; }
        }

        public int HurtTime
        {
            get { return _hurtTime; }
            set { _hurtTime = (short)value; }
        }

        public EntityMob ()
            : base("Mob")
        {
        }

        public EntityMob (string id)
            : base(id)
        {
        }

        public EntityMob (EntityTyped e)
            : base(e)
        {
            EntityMob e2 = e as EntityMob;
            if (e2 != null) {
                _attackTime = e2._attackTime;
                _deathTime = e2._deathTime;
                _health = e2._health;
                _hurtTime = e2._hurtTime;
            }
        }


        #region INBTObject<Entity> Members

        public override EntityTyped LoadTree (TagNode tree)
        {
            TagNodeCompound ctree = tree as TagNodeCompound;
            if (ctree == null || base.LoadTree(tree) == null) {
                return null;
            }

            _attackTime = ctree["AttackTime"].ToTagShort();
            _deathTime = ctree["DeathTime"].ToTagShort();
            _health = ctree["Health"].ToTagShort();
            _hurtTime = ctree["HurtTime"].ToTagShort();

            return this;
        }

        public override TagNode BuildTree ()
        {
            TagNodeCompound tree = base.BuildTree() as TagNodeCompound;
            tree["AttackTime"] = new TagNodeShort(_attackTime);
            tree["DeathTime"] = new TagNodeShort(_deathTime);
            tree["Health"] = new TagNodeShort(_health);
            tree["HurtTime"] = new TagNodeShort(_hurtTime);

            return tree;
        }

        public override bool ValidateTree (TagNode tree)
        {
            return new NBTVerifier(tree, MobSchema).Verify();
        }

        #endregion


        #region ICopyable<Entity> Members

        public override EntityTyped Copy ()
        {
            return new EntityMob(this);
        }

        #endregion
    }
}
