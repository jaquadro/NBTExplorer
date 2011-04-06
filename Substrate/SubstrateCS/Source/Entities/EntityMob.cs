using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Map.Entities
{
    using Substrate.Map.NBT;

    public class EntityMob : Entity
    {
        public static readonly NBTCompoundNode MobSchema = BaseSchema.MergeInto(new NBTCompoundNode("")
        {
            new NBTStringNode("id", "Mob"),
            new NBTScalerNode("AttackTime", NBT_Type.TAG_SHORT),
            new NBTScalerNode("DeathTime", NBT_Type.TAG_SHORT),
            new NBTScalerNode("Health", NBT_Type.TAG_SHORT),
            new NBTScalerNode("HurtTime", NBT_Type.TAG_SHORT),
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

        public override Entity LoadTree (NBT_Value tree)
        {
            NBT_Compound ctree = tree as NBT_Compound;
            if (ctree == null || base.LoadTree(tree) == null) {
                return null;
            }

            _attackTime = ctree["AttackTime"].ToNBTShort();
            _deathTime = ctree["DeathTime"].ToNBTShort();
            _health = ctree["Health"].ToNBTShort();
            _hurtTime = ctree["HurtTime"].ToNBTShort();

            return this;
        }

        public override NBT_Value BuildTree ()
        {
            NBT_Compound tree = base.BuildTree() as NBT_Compound;
            tree["AttackTime"] = new NBT_Short(_attackTime);
            tree["DeathTime"] = new NBT_Short(_deathTime);
            tree["Health"] = new NBT_Short(_health);
            tree["HurtTime"] = new NBT_Short(_hurtTime);

            return tree;
        }

        public override bool ValidateTree (NBT_Value tree)
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
