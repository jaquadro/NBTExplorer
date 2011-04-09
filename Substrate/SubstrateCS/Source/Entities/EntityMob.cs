using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Entities
{
    using Substrate.NBT;

    public class EntityMob : Entity
    {
        public static readonly NBTCompoundNode MobSchema = BaseSchema.MergeInto(new NBTCompoundNode("")
        {
            new NBTStringNode("id", "Mob"),
            new NBTScalerNode("AttackTime", TagType.TAG_SHORT),
            new NBTScalerNode("DeathTime", TagType.TAG_SHORT),
            new NBTScalerNode("Health", TagType.TAG_SHORT),
            new NBTScalerNode("HurtTime", TagType.TAG_SHORT),
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

        public EntityMob (Entity e)
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

        public override Entity LoadTree (TagValue tree)
        {
            TagCompound ctree = tree as TagCompound;
            if (ctree == null || base.LoadTree(tree) == null) {
                return null;
            }

            _attackTime = ctree["AttackTime"].ToTagShort();
            _deathTime = ctree["DeathTime"].ToTagShort();
            _health = ctree["Health"].ToTagShort();
            _hurtTime = ctree["HurtTime"].ToTagShort();

            return this;
        }

        public override TagValue BuildTree ()
        {
            TagCompound tree = base.BuildTree() as TagCompound;
            tree["AttackTime"] = new TagShort(_attackTime);
            tree["DeathTime"] = new TagShort(_deathTime);
            tree["Health"] = new TagShort(_health);
            tree["HurtTime"] = new TagShort(_hurtTime);

            return tree;
        }

        public override bool ValidateTree (TagValue tree)
        {
            return new NBTVerifier(tree, MobSchema).Verify();
        }

        #endregion


        #region ICopyable<Entity> Members

        public override Entity Copy ()
        {
            return new EntityMob(this);
        }

        #endregion
    }
}
