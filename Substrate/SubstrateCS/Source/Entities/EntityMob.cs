using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Entities
{
    using Substrate.Nbt;

    /// <summary>
    /// Encompasses data in the "ActiveEffects" compound attribute of mob entity types
    /// </summary>
    public class ActiveEffects
    {
        private byte _id;
        private byte _amplifier;
        private int _duration;

        public int Id
        {
            get { return _id; }
            set { _id = (byte)value; }
        }

        public int Amplifier
        {
            get { return _amplifier; }
            set { _amplifier = (byte)value; }
        }

        public int Duration
        {
            get { return _duration; }
            set { _duration = value; }
        }
    }

    public class EntityMob : TypedEntity
    {
        public static readonly SchemaNodeCompound MobSchema = TypedEntity.Schema.MergeInto(new SchemaNodeCompound("")
        {
            new SchemaNodeString("id", "Mob"),
            new SchemaNodeScaler("AttackTime", TagType.TAG_SHORT),
            new SchemaNodeScaler("DeathTime", TagType.TAG_SHORT),
            new SchemaNodeScaler("Health", TagType.TAG_SHORT),
            new SchemaNodeScaler("HurtTime", TagType.TAG_SHORT),
            new SchemaNodeCompound("ActiveEffects", SchemaOptions.OPTIONAL)
            {
                new SchemaNodeScaler("Id", TagType.TAG_BYTE),
                new SchemaNodeScaler("Amplifier", TagType.TAG_BYTE),
                new SchemaNodeScaler("Duration", TagType.TAG_INT),
            },
        });

        private short _attackTime;
        private short _deathTime;
        private short _health;
        private short _hurtTime;

        private ActiveEffects _activeEffects;

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

        public ActiveEffects ActiveEffects
        {
            get { return _activeEffects; }
            set { _activeEffects = value; }
        }

        public EntityMob ()
            : base("Mob")
        {
        }

        public EntityMob (string id)
            : base(id)
        {
        }

        public EntityMob (TypedEntity e)
            : base(e)
        {
            EntityMob e2 = e as EntityMob;
            if (e2 != null) {
                _attackTime = e2._attackTime;
                _deathTime = e2._deathTime;
                _health = e2._health;
                _hurtTime = e2._hurtTime;
                _activeEffects = e2._activeEffects;
            }
        }


        #region INBTObject<Entity> Members

        public override TypedEntity LoadTree (TagNode tree)
        {
            TagNodeCompound ctree = tree as TagNodeCompound;
            if (ctree == null || base.LoadTree(tree) == null) {
                return null;
            }

            _attackTime = ctree["AttackTime"].ToTagShort();
            _deathTime = ctree["DeathTime"].ToTagShort();
            _health = ctree["Health"].ToTagShort();
            _hurtTime = ctree["HurtTime"].ToTagShort();

            if (ctree.ContainsKey("ActiveEffects")) {
                TagNodeCompound ae = ctree["ActiveEffects"].ToTagCompound();

                _activeEffects = new ActiveEffects();
                _activeEffects.Id = ae["Id"].ToTagByte();
                _activeEffects.Amplifier = ae["Amplifier"].ToTagByte();
                _activeEffects.Duration = ae["Duration"].ToTagInt();
            }

            return this;
        }

        public override TagNode BuildTree ()
        {
            TagNodeCompound tree = base.BuildTree() as TagNodeCompound;
            tree["AttackTime"] = new TagNodeShort(_attackTime);
            tree["DeathTime"] = new TagNodeShort(_deathTime);
            tree["Health"] = new TagNodeShort(_health);
            tree["HurtTime"] = new TagNodeShort(_hurtTime);

            if (_activeEffects != null) {
                TagNodeCompound ae = new TagNodeCompound();
                ae["Id"] = new TagNodeByte((byte)_activeEffects.Id);
                ae["Amplifier"] = new TagNodeByte((byte)_activeEffects.Amplifier);
                ae["Duration"] = new TagNodeInt(_activeEffects.Duration);

                tree["ActiveEffects"] = ae;
            }

            return tree;
        }

        public override bool ValidateTree (TagNode tree)
        {
            return new NbtVerifier(tree, MobSchema).Verify();
        }

        #endregion


        #region ICopyable<Entity> Members

        public override TypedEntity Copy ()
        {
            return new EntityMob(this);
        }

        #endregion
    }
}
