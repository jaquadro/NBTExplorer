using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate
{
    using NBT;
    using Utility;
    using System.IO;

    public class Level : INBTObject<Level>, ICopyable<Level>
    {
        public static NBTCompoundNode LevelSchema;

        private World _world;

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
            set { _player = value; }
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
        }

        public string LevelName
        {
            get { return _name; }
            set { _name = value; }
        }

        public Level (World world)
        {
            _world = world;

            LevelSchema = new NBTCompoundNode()
            {
                new NBTCompoundNode("Data")
                {
                    new NBTScalerNode("Time", TagType.TAG_LONG),
                    new NBTScalerNode("LastPlayed", TagType.TAG_LONG),
                    new NBTCompoundNode("Player", Player.PlayerSchema, NBTOptions.OPTIONAL),
                    new NBTScalerNode("SpawnX", TagType.TAG_INT),
                    new NBTScalerNode("SpawnY", TagType.TAG_INT),
                    new NBTScalerNode("SpawnZ", TagType.TAG_INT),
                    new NBTScalerNode("SizeOnDisk", TagType.TAG_LONG),
                    new NBTScalerNode("RandomSeed", TagType.TAG_LONG),
                    new NBTScalerNode("version", TagType.TAG_INT, NBTOptions.OPTIONAL),
                    new NBTScalerNode("LevelName", TagType.TAG_STRING, NBTOptions.OPTIONAL),
                },
            };
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

            if (p._player != null) {
                _player = p._player.Copy();
            }
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

            new NBT_Tree(BuildTree() as TagCompound).WriteTo(zipstr);
            zipstr.Close();

            return true;
        }


        #region INBTObject<Player> Members

        public virtual Level LoadTree (TagValue tree)
        {
            TagCompound dtree = tree as TagCompound;
            if (dtree == null) {
                return null;
            }

            TagCompound ctree = dtree["Data"].ToTagCompound();

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

            return this;
        }

        public virtual Level LoadTreeSafe (TagValue tree)
        {
            if (!ValidateTree(tree)) {
                return null;
            }

            return LoadTree(tree);
        }

        public virtual TagValue BuildTree ()
        {
            TagCompound data = new TagCompound();
            data["Time"] = new TagLong(_time);
            data["LastPlayed"] = new TagLong(_lastPlayed);

            if (_player != null) {
                data["Player"] = _player.BuildTree();
            }

            data["SpawnX"] = new TagInt(_spawnX);
            data["SpawnY"] = new TagInt(_spawnY);
            data["SpawnZ"] = new TagInt(_spawnZ);
            data["SizeOnDisk"] = new TagLong(_sizeOnDisk);
            data["RandomSeed"] = new TagLong(_randomSeed);

            if (_version != null) {
                data["version"] = new TagInt(_version ?? 0);
            }

            if (_name != null) {
                data["LevelName"] = new TagString(_name);
            }

            TagCompound tree = new TagCompound();
            tree.Add("Data", data);

            return tree;
        }

        public virtual bool ValidateTree (TagValue tree)
        {
            return new NBTVerifier(tree, LevelSchema).Verify();
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