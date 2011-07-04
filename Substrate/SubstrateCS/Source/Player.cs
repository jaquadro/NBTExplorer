using System;
using System.Collections.Generic;
using System.Text;
using Substrate.Core;
using Substrate.Nbt;

namespace Substrate
{
    /// <summary>
    /// Represents a Player from either single- or multi-player Minecraft.
    /// </summary>
    /// <remarks>Unlike <see cref="EntityTyped"/> objects, <see cref="Player"/> objects do not need to be added to chunks.  They
    /// are stored individually or within level data.</remarks>
    public class Player : Entity, INbtObject<Player>, ICopyable<Player>, IItemContainer
    {
        private static readonly SchemaNodeCompound _schema = Entity.Schema.MergeInto(new SchemaNodeCompound("")
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

        /// <summary>
        /// Gets or sets the number of ticks left in the player's "invincibility shield" after last struck.
        /// </summary>
        public int AttackTime
        {
            get { return _attackTime; }
            set { _attackTime = (short)value; }
        }

        /// <summary>
        /// Gets or sets the number of ticks that the player has been dead for.
        /// </summary>
        public int DeathTime
        {
            get { return _deathTime; }
            set { _deathTime = (short)value; }
        }

        /// <summary>
        /// Gets or sets the amount of the player's health.
        /// </summary>
        public int Health
        {
            get { return _health; }
            set { _health = (short)value; }
        }

        /// <summary>
        /// Gets or sets the player's Hurt Time value.
        /// </summary>
        public int HurtTime
        {
            get { return _hurtTime; }
            set { _hurtTime = (short)value; }
        }

        /// <summary>
        /// Gets or sets the dimension that the player is currently in.
        /// </summary>
        public int Dimension
        {
            get { return _dimension; }
            set { _dimension = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the player is sleeping in a bed.
        /// </summary>
        public bool IsSleeping
        {
            get { return _sleeping == 1; }
            set { _sleeping = (byte)(value ? 1 : 0); }
        }

        /// <summary>
        /// Gets or sets the player's Sleep Timer value.
        /// </summary>
        public int SleepTimer
        {
            get { return _sleepTimer; }
            set { _sleepTimer = (short)value; }
        }

        /// <summary>
        /// Gets or sets the player's personal spawn point, set by sleeping in beds.
        /// </summary>
        public SpawnPoint Spawn
        {
            get { return new SpawnPoint(_spawnX ?? 0, _spawnY ?? 0, _spawnZ ?? 0); }
            set
            {
                _spawnX = value.X;
                _spawnY = value.Y;
                _spawnZ = value.Z;
            }
        }

        /// <summary>
        /// Tests if the player currently has a personal spawn point.
        /// </summary>
        public bool HasSpawn
        {
            get { return _spawnX != null && _spawnY != null && _spawnZ != null; }
        }

        /// <summary>
        /// Gets or sets the name of the world that the player is currently within.
        /// </summary>
        public string World
        {
            get { return _world; }
            set { _world = value; }
        }

        /// <summary>
        /// Creates a new <see cref="Player"/> object with reasonable default values.
        /// </summary>
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

        /// <summary>
        /// Creates a copy of a <see cref="Player"/> object.
        /// </summary>
        /// <param name="p">The <see cref="Player"/> to copy fields from.</param>
        protected Player (Player p)
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

        /// <summary>
        /// Clears the player's personal spawn point.
        /// </summary>
        public void ClearSpawn ()
        {
            _spawnX = null;
            _spawnY = null;
            _spawnZ = null;
        }


        #region INBTObject<Player> Members

        /// <summary>
        /// Gets a <see cref="SchemaNode"/> representing the schema of a Player.
        /// </summary>
        public static new SchemaNodeCompound Schema
        {
            get { return _schema; }
        }

        /// <summary>
        /// Attempt to load a Player subtree into the <see cref="Player"/> without validation.
        /// </summary>
        /// <param name="tree">The root node of a Player subtree.</param>
        /// <returns>The <see cref="Player"/> returns itself on success, or null if the tree was unparsable.</returns>
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

        /// <summary>
        /// Attempt to load a Player subtree into the <see cref="Player"/> with validation.
        /// </summary>
        /// <param name="tree">The root node of a Player subtree.</param>
        /// <returns>The <see cref="Player"/> returns itself on success, or null if the tree failed validation.</returns>
        public virtual new Player LoadTreeSafe (TagNode tree)
        {
            if (!ValidateTree(tree)) {
                return null;
            }

            return LoadTree(tree);
        }

        /// <summary>
        /// Builds a Player subtree from the current data.
        /// </summary>
        /// <returns>The root node of a Player subtree representing the current data.</returns>
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

        /// <summary>
        /// Validate a Player subtree against a schema defintion.
        /// </summary>
        /// <param name="tree">The root node of a Player subtree.</param>
        /// <returns>Status indicating whether the tree was valid against the internal schema.</returns>
        public virtual new bool ValidateTree (TagNode tree)
        {
            return new NbtVerifier(tree, _schema).Verify();
        }

        #endregion


        #region ICopyable<Entity> Members

        /// <summary>
        /// Creates a deep-copy of the <see cref="Player"/>.
        /// </summary>
        /// <returns>A deep-copy of the <see cref="Player"/>.</returns>
        public virtual new Player Copy ()
        {
            return new Player(this);
        }

        #endregion


        #region IItemContainer Members

        /// <summary>
        /// Gets access to an <see cref="ItemCollection"/> representing the player's equipment and inventory.
        /// </summary>
        public ItemCollection Items
        {
            get { return _inventory; }
        }

        #endregion
    }
}