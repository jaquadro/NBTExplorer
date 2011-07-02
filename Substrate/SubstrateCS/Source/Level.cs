using System;
using System.IO;
using Substrate.Core;
using Substrate.Nbt;

namespace Substrate
{
    public class Level : INbtObject<Level>, ICopyable<Level>
    {
        public static SchemaNodeCompound LevelSchema = new SchemaNodeCompound()
        {
            new SchemaNodeCompound("Data")
            {
                new SchemaNodeScaler("Time", TagType.TAG_LONG),
                new SchemaNodeScaler("LastPlayed", TagType.TAG_LONG, SchemaOptions.CREATE_ON_MISSING),
                new SchemaNodeCompound("Player", Player.Schema, SchemaOptions.OPTIONAL),
                new SchemaNodeScaler("SpawnX", TagType.TAG_INT),
                new SchemaNodeScaler("SpawnY", TagType.TAG_INT),
                new SchemaNodeScaler("SpawnZ", TagType.TAG_INT),
                new SchemaNodeScaler("SizeOnDisk", TagType.TAG_LONG, SchemaOptions.CREATE_ON_MISSING),
                new SchemaNodeScaler("RandomSeed", TagType.TAG_LONG),
                new SchemaNodeScaler("version", TagType.TAG_INT, SchemaOptions.OPTIONAL),
                new SchemaNodeScaler("LevelName", TagType.TAG_STRING, SchemaOptions.OPTIONAL),
                new SchemaNodeScaler("raining", TagType.TAG_BYTE, SchemaOptions.OPTIONAL),
                new SchemaNodeScaler("thundering", TagType.TAG_BYTE, SchemaOptions.OPTIONAL),
                new SchemaNodeScaler("rainTime", TagType.TAG_INT, SchemaOptions.OPTIONAL),
                new SchemaNodeScaler("thunderTime", TagType.TAG_INT, SchemaOptions.OPTIONAL),
            },
        };

        private INBTWorld _world;

        private long _time;
        private long _lastPlayed;

        private Player _player;

        private int _spawnX;
        private int _spawnY;
        private int _spawnZ;

        private long _sizeOnDisk;
        private long _randomSeed;
        private int? _version;
        private string _name;

        private byte? _raining;
        private byte? _thundering;
        private int? _rainTime;
        private int? _thunderTime;

        public long Time
        {
            get { return _time; }
            set { _time = value; }
        }

        public long LastPlayed
        {
            get { return _lastPlayed; }
        }

        public Player Player
        {
            get { return _player; }
            set
            {
                _player = value;
                _player.World = _name;
            }
        }

        public int SpawnX
        {
            get { return _spawnX; }
            set { _spawnX = value; }
        }

        public int SpawnY
        {
            get { return _spawnY; }
            set { _spawnY = value; }
        }

        public int SpawnZ
        {
            get { return _spawnZ; }
            set { _spawnZ = value; }
        }

        public long SizeOnDisk
        {
            get { return _sizeOnDisk; }
        }

        public long RandomSeed
        {
            get { return _randomSeed; }
            set { _randomSeed = value; }
        }

        public int Version
        {
            get { return _version ?? 0; }
            set { _version = value; }
        }

        public string LevelName
        {
            get { return _name; }
            set
            {
                _name = value;
                if (_player != null) {
                    _player.World = value;
                }
            }
        }

        public bool IsRaining
        {
            get { return (_raining ?? 0) == 1; }
            set { _raining = value ? (byte)1 : (byte)0; }
        }

        public bool IsThundering
        {
            get { return (_thundering ?? 0) == 1; }
            set { _thundering = value ? (byte)1 : (byte)0; }
        }

        public int RainTime
        {
            get { return _rainTime ?? 0; }
            set { _rainTime = value; }
        }

        public int ThunderTime
        {
            get { return _thunderTime ?? 0; }
            set { _thunderTime = value; }
        }

        public Level (INBTWorld world)
        {
            _world = world;

            // Sane defaults
            _time = 0;
            _lastPlayed = 0;
            _spawnX = 0;
            _spawnY = 64;
            _spawnZ = 0;
            _sizeOnDisk = 0;
            _randomSeed = new Random().Next();
            _version = 19132;
            _name = "Untitled";
        }

        public Level (Level p)
        {
            _world = p._world;

            _time = p._time;
            _lastPlayed = p._lastPlayed;
            _spawnX = p._spawnX;
            _spawnY = p._spawnY;
            _spawnZ = p._spawnZ;
            _sizeOnDisk = p._sizeOnDisk;
            _randomSeed = p._randomSeed;
            _version = p._version;
            _name = p._name;

            _raining = p._raining;
            _thundering = p._thundering;
            _rainTime = p._rainTime;
            _thunderTime = p._thunderTime;

            if (p._player != null) {
                _player = p._player.Copy();
            }
        }

        public void SetDefaultPlayer ()
        {
            _player = new Player();
            _player.World = _name;

            _player.Position.X = _spawnX;
            _player.Position.Y = _spawnY + 1.7;
            _player.Position.Z = _spawnZ;
        }

        public bool Save ()
        {
            if (_world == null) {
                return false;
            }

            NBTFile nf = new NBTFile(Path.Combine(_world.WorldPath, "level.dat"));
            Stream zipstr = nf.GetDataOutputStream();
            if (zipstr == null) {
                return false;
            }

            new NbtTree(BuildTree() as TagNodeCompound).WriteTo(zipstr);
            zipstr.Close();

            return true;
        }


        #region INBTObject<Player> Members

        public virtual Level LoadTree (TagNode tree)
        {
            TagNodeCompound dtree = tree as TagNodeCompound;
            if (dtree == null) {
                return null;
            }

            _version = null;
            _raining = null;
            _rainTime = null;
            _thundering = null;
            _thunderTime = null;

            TagNodeCompound ctree = dtree["Data"].ToTagCompound();

            _time = ctree["Time"].ToTagLong();
            _lastPlayed = ctree["LastPlayed"].ToTagLong();

            if (ctree.ContainsKey("Player")) {
                _player = new Player().LoadTree(ctree["Player"]);
            }

            _spawnX = ctree["SpawnX"].ToTagInt();
            _spawnY = ctree["SpawnY"].ToTagInt();
            _spawnZ = ctree["SpawnZ"].ToTagInt();

            _sizeOnDisk = ctree["SizeOnDisk"].ToTagLong();
            _randomSeed = ctree["RandomSeed"].ToTagLong();

            if (ctree.ContainsKey("version")) {
                _version = ctree["version"].ToTagInt();
            }
            if (ctree.ContainsKey("LevelName")) {
                _name = ctree["LevelName"].ToTagString();
            }

            if (ctree.ContainsKey("raining")) {
                _raining = ctree["raining"].ToTagByte();
            }
            if (ctree.ContainsKey("thundering")) {
                _thundering = ctree["thundering"].ToTagByte();
            }
            if (ctree.ContainsKey("rainTime")) {
                _rainTime = ctree["rainTime"].ToTagInt();
            }
            if (ctree.ContainsKey("thunderTime")) {
                _thunderTime = ctree["thunderTime"].ToTagInt();
            }

            return this;
        }

        public virtual Level LoadTreeSafe (TagNode tree)
        {
            if (!ValidateTree(tree)) {
                return null;
            }

            return LoadTree(tree);
        }

        public virtual TagNode BuildTree ()
        {
            TagNodeCompound data = new TagNodeCompound();
            data["Time"] = new TagNodeLong(_time);
            data["LastPlayed"] = new TagNodeLong(_lastPlayed);

            if (_player != null) {
                data["Player"] = _player.BuildTree();
            }

            data["SpawnX"] = new TagNodeInt(_spawnX);
            data["SpawnY"] = new TagNodeInt(_spawnY);
            data["SpawnZ"] = new TagNodeInt(_spawnZ);
            data["SizeOnDisk"] = new TagNodeLong(_sizeOnDisk);
            data["RandomSeed"] = new TagNodeLong(_randomSeed);

            if (_version != null && _version != 0) {
                data["version"] = new TagNodeInt(_version ?? 0);
            }

            if (_name != null) {
                data["LevelName"] = new TagNodeString(_name);
            }

            if (_raining != null) {
                data["raining"] = new TagNodeByte(_raining ?? 0);
            }
            if (_thundering != null) {
                data["thundering"] = new TagNodeByte(_thundering ?? 0);
            }
            if (_rainTime != null) {
                data["rainTime"] = new TagNodeInt(_rainTime ?? 0);
            }
            if (_thunderTime != null) {
                data["thunderTime"] = new TagNodeInt(_thunderTime ?? 0);
            }

            TagNodeCompound tree = new TagNodeCompound();
            tree.Add("Data", data);

            return tree;
        }

        public virtual bool ValidateTree (TagNode tree)
        {
            return new NbtVerifier(tree, LevelSchema).Verify();
        }

        #endregion


        #region ICopyable<Entity> Members

        public virtual Level Copy ()
        {
            return new Level(this);
        }

        #endregion
    }
}