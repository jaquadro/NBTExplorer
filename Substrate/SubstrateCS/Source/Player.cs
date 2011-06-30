using System;
using System.Collections.Generic;
using System.Text;
using Substrate.Core;
using Substrate.Nbt;

namespace Substrate
{
    public class Player : Entity, INBTObject<Player>, ICopyable<Player>, IItemContainer
    {
        public static readonly SchemaNodeCompound PlayerSchema = Entity.Schema.MergeInto(new SchemaNodeCompound("")
        {
            new SchemaNodeScaler("AttackTime", TagType.TAG_SHORT),
            new SchemaNodeScaler("DeathTime", TagType.TAG_SHORT),
            new SchemaNodeScaler("Health", TagType.TAG_SHORT),
            new SchemaNodeScaler("HurtTime", TagType.TAG_SHORT),
            new SchemaNodeScaler("Dimension", TagType.TAG_INT),
            new SchemaNodeList("Inventory", TagType.TAG_COMPOUND, ItemCollection.InventorySchema),
            new SchemaNodeScaler("World", TagType.TAG_STRING, SchemaOptions.OPTIONAL),
            new SchemaNodeScaler("Sleeping", TagType.TAG_BYTE, SchemaOptions.CREATE_ON_MISSING),
            new SchemaNodeScaler("SleepTimer", TagType.TAG_SHORT, SchemaOptions.CREATE_ON_MISSING),
            new SchemaNodeScaler("SpawnX", TagType.TAG_INT, SchemaOptions.OPTIONAL),
            new SchemaNodeScaler("SpawnY", TagType.TAG_INT, SchemaOptions.OPTIONAL),
            new SchemaNodeScaler("SpawnZ", TagType.TAG_INT, SchemaOptions.OPTIONAL),
        });

        private const int _CAPACITY = 105;

        private short _attackTime;
        private short _deathTime;
        private short _health;
        private short _hurtTime;

        private int _dimension;
        private byte _sleeping;
        private short _sleepTimer;
        private int? _spawnX;
        private int? _spawnY;
        private int? _spawnZ;

        private string _world;

        private ItemCollection _inventory;

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

        public int Dimension
        {
            get { return _dimension; }
            set { _dimension = value; }
        }

        public bool IsSleeping
        {
            get { return _sleeping == 1; }
            set { _sleeping = (byte)(value ? 1 : 0); }
        }

        public int SleepTimer
        {
            get { return _sleepTimer; }
            set { _sleepTimer = (short)value; }
        }

        public int SpawnX
        {
            get { return _spawnX ?? 0; }
            set { _spawnX = value; }
        }

        public int SpawnY
        {
            get { return _spawnY ?? 0; }
            set { _spawnY = value; }
        }

        public int SpawnZ
        {
            get { return _spawnZ ?? 0; }
            set { _spawnZ = value; }
        }

        public string World
        {
            get { return _world; }
            set { _world = value; }
        }

        public Player ()
            : base()
        {
            _inventory = new ItemCollection(_CAPACITY);

            // Sane defaults
            _dimension = 0;
            _sleeping = 0;
            _sleepTimer = 0;

            Air = 300;
            Health = 20;
            Fire = -20;
        }

        public Player (Player p)
            : base(p)
        {
            _attackTime = p._attackTime;
            _deathTime = p._deathTime;
            _health = p._health;
            _hurtTime = p._hurtTime;

            _dimension = p._dimension;
            _sleeping = p._sleeping;
            _sleepTimer = p._sleepTimer;
            _spawnX = p._spawnX;
            _spawnY = p._spawnY;
            _spawnZ = p._spawnZ;
            _world = p._world;
            _inventory = p._inventory.Copy();
        }


        #region INBTObject<Player> Members

        public virtual new Player LoadTree (TagNode tree)
        {
            TagNodeCompound ctree = tree as TagNodeCompound;
            if (ctree == null || base.LoadTree(tree) == null) {
                return null;
            }

            _attackTime = ctree["AttackTime"].ToTagShort();
            _deathTime = ctree["DeathTime"].ToTagShort();
            _health = ctree["Health"].ToTagShort();
            _hurtTime = ctree["HurtTime"].ToTagShort();

            _dimension = ctree["Dimension"].ToTagInt();
            _sleeping = ctree["Sleeping"].ToTagByte();
            _sleepTimer = ctree["SleepTimer"].ToTagShort();

            if (ctree.ContainsKey("SpawnX")) {
                _spawnX = ctree["SpawnX"].ToTagInt();
            }
            if (ctree.ContainsKey("SpawnY")) {
                _spawnY = ctree["SpawnY"].ToTagInt();
            }
            if (ctree.ContainsKey("SpawnZ")) {
                _spawnZ = ctree["SpawnZ"].ToTagInt();
            }

            if (ctree.ContainsKey("World")) {
                _world = ctree["World"].ToTagString();
            }

            _inventory.LoadTree(ctree["Inventory"].ToTagList());

            return this;
        }

        public virtual new Player LoadTreeSafe (TagNode tree)
        {
            if (!ValidateTree(tree)) {
                return null;
            }

            return LoadTree(tree);
        }

        public virtual new TagNode BuildTree ()
        {
            TagNodeCompound tree = base.BuildTree() as TagNodeCompound;
            tree["AttackTime"] = new TagNodeShort(_attackTime);
            tree["DeathTime"] = new TagNodeShort(_deathTime);
            tree["Health"] = new TagNodeShort(_health);
            tree["HurtTime"] = new TagNodeShort(_hurtTime);

            tree["Dimension"] = new TagNodeInt(_dimension);
            tree["Sleeping"] = new TagNodeByte(_sleeping);
            tree["SleepTimer"] = new TagNodeShort(_sleepTimer);

            if (_spawnX != null && _spawnY != null && _spawnZ != null) {
                tree["SpawnX"] = new TagNodeInt(_spawnX ?? 0);
                tree["SpawnY"] = new TagNodeInt(_spawnY ?? 0);
                tree["SpawnZ"] = new TagNodeInt(_spawnZ ?? 0);
            }

            if (_world != null) {
                tree["World"] = new TagNodeString(_world);
            }

            tree["Inventory"] = _inventory.BuildTree();

            return tree;
        }

        public virtual new bool ValidateTree (TagNode tree)
        {
            return new NBTVerifier(tree, PlayerSchema).Verify();
        }

        #endregion


        #region ICopyable<Entity> Members

        public virtual new Player Copy ()
        {
            return new Player(this);
        }

        #endregion


        #region IItemContainer Members

        public ItemCollection Items
        {
            get { return _inventory; }
        }

        #endregion
    }
}