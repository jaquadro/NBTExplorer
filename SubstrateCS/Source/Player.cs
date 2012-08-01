using System;
using System.Collections.Generic;
using System.Text;
using Substrate.Core;
using Substrate.Nbt;

namespace Substrate
{
    /// <summary>
    /// Encompases data to specify player abilities, especially mode-dependent abilities.
    /// </summary>
    /// <remarks>Whether or not any of these values are respected by the game client is dependent upon the active game mode.</remarks>
    public class PlayerAbilities : ICopyable<PlayerAbilities>
    {
        private bool _flying = false;
        private bool _instabuild = false;
        private bool _mayfly = false;
        private bool _invulnerable = false;
        private bool _maybuild = true;

        private float _walkSpeed = 0.1f;
        private float _flySpeed = 0.05f;

        /// <summary>
        /// Gets or sets whether the player is currently flying.
        /// </summary>
        public bool Flying
        {
            get { return _flying; }
            set { _flying = value; }
        }

        /// <summary>
        /// Gets or sets whether the player can instantly build or mine.
        /// </summary>
        public bool InstantBuild
        {
            get { return _instabuild; }
            set { _instabuild = value; }
        }

        /// <summary>
        /// Gets or sets whether the player is allowed to fly.
        /// </summary>
        public bool MayFly
        {
            get { return _mayfly; }
            set { _mayfly = value; }
        }

        /// <summary>
        /// Gets or sets whether the player can take damage.
        /// </summary>
        public bool Invulnerable
        {
            get { return _invulnerable; }
            set { _invulnerable = value; }
        }

        /// <summary>
        /// Gets or sets whether the player can create or destroy blocks.
        /// </summary>
        public bool MayBuild 
        {
            get { return _maybuild; }
            set { _maybuild = value; }
        }

        /// <summary>
        /// Gets or sets the player's walking speed.  Always 0.1.
        /// </summary>
        public float FlySpeed
        {
            get { return _flySpeed; }
            set { _flySpeed = value; }
        }

        /// <summary>
        /// Gets or sets the player's flying speed.  Always 0.05.
        /// </summary>
        public float WalkSpeed
        {
            get { return _walkSpeed; }
            set { _walkSpeed = value; }
        }

        #region ICopyable<PlayerAbilities> Members

        /// <inheritdoc />
        public PlayerAbilities Copy ()
        {
            PlayerAbilities pa = new PlayerAbilities();
            pa._flying = _flying;
            pa._instabuild = _instabuild;
            pa._mayfly = _mayfly;
            pa._invulnerable = _invulnerable;
            pa._maybuild = _maybuild;
            pa._walkSpeed = _walkSpeed;
            pa._flySpeed = _flySpeed;

            return pa;
        }

        #endregion
    }

    public enum PlayerGameType
    {
        Survival = 0,
        Creative = 1,
        Adventure = 2,
    }

    /// <summary>
    /// Represents a Player from either single- or multi-player Minecraft.
    /// </summary>
    /// <remarks>Unlike <see cref="TypedEntity"/> objects, <see cref="Player"/> objects do not need to be added to chunks.  They
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
            new SchemaNodeList("Inventory", TagType.TAG_COMPOUND, ItemCollection.Schema),
            //new SchemaNodeList("EnderItems", TagType.TAG_COMPOUND, ItemCollection.Schema, SchemaOptions.OPTIONAL),
            new SchemaNodeScaler("World", TagType.TAG_STRING, SchemaOptions.OPTIONAL),
            new SchemaNodeScaler("Sleeping", TagType.TAG_BYTE, SchemaOptions.CREATE_ON_MISSING),
            new SchemaNodeScaler("SleepTimer", TagType.TAG_SHORT, SchemaOptions.CREATE_ON_MISSING),
            new SchemaNodeScaler("SpawnX", TagType.TAG_INT, SchemaOptions.OPTIONAL),
            new SchemaNodeScaler("SpawnY", TagType.TAG_INT, SchemaOptions.OPTIONAL),
            new SchemaNodeScaler("SpawnZ", TagType.TAG_INT, SchemaOptions.OPTIONAL),
            new SchemaNodeScaler("foodLevel", TagType.TAG_INT, SchemaOptions.OPTIONAL),
            new SchemaNodeScaler("foodTickTimer", TagType.TAG_INT, SchemaOptions.OPTIONAL),
            new SchemaNodeScaler("foodExhaustionLevel", TagType.TAG_FLOAT, SchemaOptions.OPTIONAL),
            new SchemaNodeScaler("foodSaturationLevel", TagType.TAG_FLOAT, SchemaOptions.OPTIONAL),
            new SchemaNodeScaler("XpP", TagType.TAG_FLOAT, SchemaOptions.OPTIONAL),
            new SchemaNodeScaler("XpLevel", TagType.TAG_INT, SchemaOptions.OPTIONAL),
            new SchemaNodeScaler("XpTotal", TagType.TAG_INT, SchemaOptions.OPTIONAL),
            new SchemaNodeScaler("Score", TagType.TAG_INT, SchemaOptions.OPTIONAL),
            new SchemaNodeScaler("PlayerGameType", TagType.TAG_INT, SchemaOptions.OPTIONAL),
            new SchemaNodeCompound("abilities", new SchemaNodeCompound("") {
                new SchemaNodeScaler("flying", TagType.TAG_BYTE),
                new SchemaNodeScaler("instabuild", TagType.TAG_BYTE),
                new SchemaNodeScaler("mayfly", TagType.TAG_BYTE),
                new SchemaNodeScaler("invulnerable", TagType.TAG_BYTE),
                new SchemaNodeScaler("mayBuild", TagType.TAG_BYTE, SchemaOptions.OPTIONAL),
                new SchemaNodeScaler("walkSpeed", TagType.TAG_FLOAT, SchemaOptions.OPTIONAL),
                new SchemaNodeScaler("flySpeed", TagType.TAG_FLOAT, SchemaOptions.OPTIONAL),
            }, SchemaOptions.OPTIONAL),
        });

        private const int _CAPACITY = 105;
        private const int _ENDER_CAPACITY = 27;

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

        private int? _foodLevel;
        private int? _foodTickTimer;
        private float? _foodExhaustion;
        private float? _foodSaturation;
        private float? _xpP;
        private int? _xpLevel;
        private int? _xpTotal;
        private int? _score;

        private string _world;
        private string _name;


        private PlayerAbilities _abilities;
        private PlayerGameType? _gameType;

        private ItemCollection _inventory;
        private ItemCollection _enderItems;

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

        public PlayerGameType GameType
        {
            get { return _gameType ?? PlayerGameType.Survival; }
            set { _gameType = value; }
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
        /// Gets or sets the name that is used when the player is read or written from a <see cref="PlayerManager"/>.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Gets or sets the player's score.
        /// </summary>
        public int Score
        {
            get { return _score ?? 0; }
            set { _score = value; }
        }

        /// <summary>
        /// Gets or sets the player's XP Level.
        /// </summary>
        public int XPLevel
        {
            get { return _xpLevel ?? 0; }
            set { _xpLevel = value; }
        }

        /// <summary>
        /// Gets or sets the amount of the player's XP points.
        /// </summary>
        public int XPTotal
        {
            get { return _xpTotal ?? 0; }
            set { _xpTotal = value; }
        }

        /// <summary>
        /// Gets or sets the hunger level of the player.  Valid values range 0 - 20.
        /// </summary>
        public int HungerLevel
        {
            get { return _foodLevel ?? 0; }
            set { _foodLevel = value; }
        }

        /// <summary>
        /// Gets or sets the player's hunger saturation level, which is reserve food capacity above <see cref="HungerLevel"/>.
        /// </summary>
        public float HungerSaturationLevel
        {
            get { return _foodSaturation ?? 0; }
            set { _foodSaturation = value; }
        }

        /// <summary>
        /// Gets or sets the counter towards the next hunger point decrement.  Valid values range 0.0 - 4.0.
        /// </summary>
        public float HungerExhaustionLevel
        {
            get { return _foodExhaustion ?? 0; }
            set { _foodExhaustion = value; }
        }

        /// <summary>
        /// Gets or sets the timer used to periodically heal or damage the player based on <see cref="HungerLevel"/>.  Valid values range 0 - 80.
        /// </summary>
        public int HungerTimer
        {
            get { return _foodTickTimer ?? 0; }
            set { _foodTickTimer = value; }
        }

        /// <summary>
        /// Gets the state of the player's abilities.
        /// </summary>
        public PlayerAbilities Abilities
        {
            get { return _abilities; }
        }

        /// <summary>
        /// Creates a new <see cref="Player"/> object with reasonable default values.
        /// </summary>
        public Player ()
            : base()
        {
            _inventory = new ItemCollection(_CAPACITY);
            _enderItems = new ItemCollection(_ENDER_CAPACITY);
            _abilities = new PlayerAbilities();

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
            _gameType = p._gameType;
            _sleeping = p._sleeping;
            _sleepTimer = p._sleepTimer;
            _spawnX = p._spawnX;
            _spawnY = p._spawnY;
            _spawnZ = p._spawnZ;
            _world = p._world;
            _inventory = p._inventory.Copy();
            _enderItems = p._inventory.Copy();

            _foodLevel = p._foodLevel;
            _foodTickTimer = p._foodTickTimer;
            _foodSaturation = p._foodSaturation;
            _foodExhaustion = p._foodExhaustion;
            _xpP = p._xpP;
            _xpLevel = p._xpLevel;
            _xpTotal = p._xpTotal;
            _abilities = p._abilities.Copy();
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

        private bool AbilitiesSet ()
        {
            return _abilities.Flying
                || _abilities.InstantBuild
                || _abilities.MayFly
                || _abilities.Invulnerable;
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

            if (ctree.ContainsKey("foodLevel")) {
                _foodLevel = ctree["foodLevel"].ToTagInt();
            }
            if (ctree.ContainsKey("foodTickTimer")) {
                _foodTickTimer = ctree["foodTickTimer"].ToTagInt();
            }
            if (ctree.ContainsKey("foodExhaustionLevel")) {
                _foodExhaustion = ctree["foodExhaustionLevel"].ToTagFloat();
            }
            if (ctree.ContainsKey("foodSaturationLevel")) {
                _foodSaturation = ctree["foodSaturationLevel"].ToTagFloat();
            }
            if (ctree.ContainsKey("XpP")) {
                _xpP = ctree["XpP"].ToTagFloat();
            }
            if (ctree.ContainsKey("XpLevel")) {
                _xpLevel = ctree["XpLevel"].ToTagInt();
            }
            if (ctree.ContainsKey("XpTotal")) {
                _xpTotal = ctree["XpTotal"].ToTagInt();
            }
            if (ctree.ContainsKey("Score")) {
                _score = ctree["Score"].ToTagInt();
            }

            if (ctree.ContainsKey("abilities")) {
                TagNodeCompound pb = ctree["abilities"].ToTagCompound();

                _abilities = new PlayerAbilities();
                _abilities.Flying = pb["flying"].ToTagByte().Data == 1;
                _abilities.InstantBuild = pb["instabuild"].ToTagByte().Data == 1;
                _abilities.MayFly = pb["mayfly"].ToTagByte().Data == 1;
                _abilities.Invulnerable = pb["invulnerable"].ToTagByte().Data == 1;

                if (pb.ContainsKey("mayBuild"))
                    _abilities.MayBuild = pb["mayBuild"].ToTagByte().Data == 1;
                if (pb.ContainsKey("walkSpeed"))
                    _abilities.WalkSpeed = pb["walkSpeed"].ToTagFloat();
                if (pb.ContainsKey("flySpeed"))
                    _abilities.FlySpeed = pb["flySpeed"].ToTagFloat();
            }

            if (ctree.ContainsKey("PlayerGameType")) {
                _gameType = (PlayerGameType)ctree["PlayerGameType"].ToTagInt().Data;
            }

            _inventory.LoadTree(ctree["Inventory"].ToTagList());

            if (ctree.ContainsKey("EnderItems")) {
                if (ctree["EnderItems"].ToTagList().Count > 0)
                    _enderItems.LoadTree(ctree["EnderItems"].ToTagList());
            }

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
            else {
                tree.Remove("SpawnX");
                tree.Remove("SpawnY");
                tree.Remove("SpawnZ");
            }

            if (_world != null) {
                tree["World"] = new TagNodeString(_world);
            }

            if (_foodLevel != null)
                tree["foodLevel"] = new TagNodeInt(_foodLevel ?? 0);
            if (_foodTickTimer != null)
                tree["foodTickTimer"] = new TagNodeInt(_foodTickTimer ?? 0);
            if (_foodExhaustion != null)
                tree["foodExhaustionLevel"] = new TagNodeFloat(_foodExhaustion ?? 0);
            if (_foodSaturation != null)
                tree["foodSaturation"] = new TagNodeFloat(_foodSaturation ?? 0);
            if (_xpP != null)
                tree["XpP"] = new TagNodeFloat(_xpP ?? 0);
            if (_xpLevel != null)
                tree["XpLevel"] = new TagNodeInt(_xpLevel ?? 0);
            if (_xpTotal != null)
                tree["XpTotal"] = new TagNodeInt(_xpTotal ?? 0);
            if (_score != null)
                tree["Score"] = new TagNodeInt(_score ?? 0);

            if (_gameType != null)
                tree["PlayerGameType"] = new TagNodeInt((int)(_gameType ?? PlayerGameType.Survival));

            if (AbilitiesSet()) {
                TagNodeCompound pb = new TagNodeCompound();
                pb["flying"] = new TagNodeByte(_abilities.Flying ? (byte)1 : (byte)0);
                pb["instabuild"] = new TagNodeByte(_abilities.InstantBuild ? (byte)1 : (byte)0);
                pb["mayfly"] = new TagNodeByte(_abilities.MayFly ? (byte)1 : (byte)0);
                pb["invulnerable"] = new TagNodeByte(_abilities.Invulnerable ? (byte)1 : (byte)0);
                pb["mayBuild"] = new TagNodeByte(_abilities.MayBuild ? (byte)1 : (byte)0);
                pb["walkSpeed"] = new TagNodeFloat(_abilities.WalkSpeed);
                pb["flySpeed"] = new TagNodeFloat(_abilities.FlySpeed);

                tree["abilities"] = pb;
            }

            tree["Inventory"] = _inventory.BuildTree();
            tree["EnderItems"] = _enderItems.BuildTree();

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

        public ItemCollection EnderItems
        {
            get { return _enderItems; }
        }
    }
}