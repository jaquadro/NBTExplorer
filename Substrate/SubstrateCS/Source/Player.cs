using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate
{
    using NBT;
    using Utility;

    public class Player : UntypedEntity, INBTObject<Player>, ICopyable<Player>, IItemContainer
    {
        public static readonly NBTCompoundNode PlayerSchema = UTBaseSchema.MergeInto(new NBTCompoundNode("")
        {
            new NBTScalerNode("Dimension", NBT_Type.TAG_INT),
            new NBTListNode("Inventory", NBT_Type.TAG_COMPOUND, ItemCollection.InventorySchema),
            new NBTScalerNode("World", NBT_Type.TAG_STRING, NBTOptions.OPTIONAL),
            new NBTScalerNode("Sleeping", NBT_Type.TAG_BYTE, NBTOptions.CREATE_ON_MISSING),
            new NBTScalerNode("SleepTimer", NBT_Type.TAG_SHORT, NBTOptions.CREATE_ON_MISSING),
            new NBTScalerNode("SpawnX", NBT_Type.TAG_INT, NBTOptions.OPTIONAL),
            new NBTScalerNode("SpawnY", NBT_Type.TAG_INT, NBTOptions.OPTIONAL),
            new NBTScalerNode("SpawnZ", NBT_Type.TAG_INT, NBTOptions.OPTIONAL),
        });

        private const int _CAPACITY = 105;

        private int _dimension;
        private byte _sleeping;
        private short _sleepTimer;
        private int? _spawnX;
        private int? _spawnY;
        private int? _spawnZ;

        private string _world;

        private ItemCollection _inventory;

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
        }

        public Player (Player p)
            : base(p)
        {
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

        public virtual new Player LoadTree (NBT_Value tree)
        {
            NBT_Compound ctree = tree as NBT_Compound;
            if (ctree == null || base.LoadTree(tree) == null) {
                return null;
            }

            _dimension = ctree["Dimension"].ToNBTInt();
            _sleeping = ctree["Sleeping"].ToNBTByte();
            _sleepTimer = ctree["SleepTimer"].ToNBTShort();

            if (ctree.ContainsKey("SpawnX")) {
                _spawnX = ctree["SpawnX"].ToNBTInt();
            }
            if (ctree.ContainsKey("SpawnY")) {
                _spawnY = ctree["SpawnY"].ToNBTInt();
            }
            if (ctree.ContainsKey("SpawnZ")) {
                _spawnZ = ctree["SpawnZ"].ToNBTInt();
            }

            if (ctree.ContainsKey("World")) {
                _world = ctree["World"].ToNBTString();
            }

            _inventory.LoadTree(ctree["Inventory"].ToNBTList());

            return this;
        }

        public virtual new Player LoadTreeSafe (NBT_Value tree)
        {
            if (!ValidateTree(tree)) {
                return null;
            }

            return LoadTree(tree);
        }

        public virtual new NBT_Value BuildTree ()
        {
            NBT_Compound tree = base.BuildTree() as NBT_Compound;
            tree["Dimension"] = new NBT_Int(_dimension);
            tree["Sleeping"] = new NBT_Byte(_sleeping);
            tree["SleepTimer"] = new NBT_Short(_sleepTimer);

            if (_spawnX != null && _spawnY != null && _spawnZ != null) {
                tree["SpawnX"] = new NBT_Int(_spawnX ?? 0);
                tree["SpawnY"] = new NBT_Int(_spawnY ?? 0);
                tree["SpawnZ"] = new NBT_Int(_spawnZ ?? 0);
            }

            if (_world != null) {
                tree["World"] = new NBT_String(_world);
            }

            tree["Inventory"] = _inventory.BuildTree();

            return tree;
        }

        public virtual new bool ValidateTree (NBT_Value tree)
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