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
        public static NBTCompoundNode LevelSchema = new NBTCompoundNode()
        {
            new NBTCompoundNode("Data")
            {
                new NBTScalerNode("Time", NBT_Type.TAG_LONG),
                new NBTScalerNode("LastPlayed", NBT_Type.TAG_LONG),
                new NBTCompoundNode("Player", Player.PlayerSchema),
                new NBTScalerNode("SpawnX", NBT_Type.TAG_INT),
                new NBTScalerNode("SpawnY", NBT_Type.TAG_INT),
                new NBTScalerNode("SpawnZ", NBT_Type.TAG_INT),
                new NBTScalerNode("SizeOnDisk", NBT_Type.TAG_LONG),
                new NBTScalerNode("RandomSeed", NBT_Type.TAG_LONG),
                new NBTScalerNode("Version", NBT_Type.TAG_INT, NBTOptions.OPTIONAL),
                new NBTScalerNode("LevelName", NBT_Type.TAG_STRING, NBTOptions.OPTIONAL),
            },
        };

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

            new NBT_Tree(BuildTree() as NBT_Compound).WriteTo(zipstr);
            zipstr.Close();

            return true;
        }


        #region INBTObject<Player> Members

        public virtual Level LoadTree (NBT_Value tree)
        {
            NBT_Compound dtree = tree as NBT_Compound;
            if (dtree == null) {
                return null;
            }

            NBT_Compound ctree = dtree["Data"].ToNBTCompound();

            _time = ctree["Time"].ToNBTLong();
            _lastPlayed = ctree["LastPlayed"].ToNBTLong();

            _player = new Player().LoadTree(ctree["Player"]);

            _spawnX = ctree["SpawnX"].ToNBTInt();
            _spawnY = ctree["SpawnY"].ToNBTInt();
            _spawnZ = ctree["SpawnZ"].ToNBTInt();

            _sizeOnDisk = ctree["SizeOnDisk"].ToNBTLong();
            _randomSeed = ctree["RandomSeed"].ToNBTLong();

            if (ctree.ContainsKey("Version")) {
                _version = ctree["Version"].ToNBTInt();
            }

            if (ctree.ContainsKey("LevelName")) {
                _name = ctree["LevelName"].ToNBTString();
            }

            return this;
        }

        public virtual Level LoadTreeSafe (NBT_Value tree)
        {
            if (!ValidateTree(tree)) {
                return null;
            }

            return LoadTree(tree);
        }

        public virtual NBT_Value BuildTree ()
        {
            NBT_Compound data = new NBT_Compound();
            data["Time"] = new NBT_Long(_time);
            data["LastPlayed"] = new NBT_Long(_lastPlayed);
            data["Player"] = _player.BuildTree();
            data["SpawnX"] = new NBT_Int(_spawnX);
            data["SpawnY"] = new NBT_Int(_spawnY);
            data["SpawnZ"] = new NBT_Int(_spawnZ);
            data["SizeOnDisk"] = new NBT_Long(_sizeOnDisk);
            data["RandomSeed"] = new NBT_Long(_randomSeed);

            if (_version != null) {
                data["Version"] = new NBT_Int(_version ?? 0);
            }

            if (_name != null) {
                data["LevelName"] = new NBT_String(_name);
            }

            NBT_Compound tree = new NBT_Compound();
            tree.Add("Data", data);

            return tree;
        }

        public virtual bool ValidateTree (NBT_Value tree)
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