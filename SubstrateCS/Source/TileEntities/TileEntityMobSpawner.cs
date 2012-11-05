using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.TileEntities
{
    using Substrate.Nbt;

    public class TileEntityMobSpawner : TileEntity
    {
        public static readonly SchemaNodeCompound MobSpawnerSchema = TileEntity.Schema.MergeInto(new SchemaNodeCompound("")
        {
            new SchemaNodeString("id", TypeId),
            new SchemaNodeScaler("EntityId", TagType.TAG_STRING),
            new SchemaNodeScaler("Delay", TagType.TAG_SHORT),
			new SchemaNodeScaler("MaxSpawnDelay", TagType.TAG_SHORT, SchemaOptions.CREATE_ON_MISSING),
			new SchemaNodeScaler("MinSpawnDelay", TagType.TAG_SHORT, SchemaOptions.CREATE_ON_MISSING),
			new SchemaNodeScaler("SpawnCount", TagType.TAG_SHORT),
			Entity.Schema.MergeInto(new SchemaNodeCompound("SpawnData", SchemaOptions.OPTIONAL))
        });

        public static string TypeId
        {
            get { return "MobSpawner"; }
        }

        private short _delay;
        private string _entityID;
		private short _maxDelay;
		private short _minDelay;
		private short _spawnCount;
		private Entity _spawnData;

        public int Delay
        {
            get { return _delay; }
            set { _delay = (short)value; }
        }

		public Entity SpawnData
		{
			get { return _spawnData; }
			set { _spawnData = value; }
		}

        public string EntityID
        {
            get { return _entityID; }
            set { _entityID = value; }
        }

		public short MaxSpawnDelay
		{
			get { return _maxDelay; }
			set { _maxDelay = value; }
		}

		public short MinSpawnDelay
		{
			get { return _minDelay; }
			set { _minDelay = value; }
		}

		public short SpawnCount
		{
			get { return _spawnCount; }
			set { _spawnCount = value; }
		}

        protected TileEntityMobSpawner (string id)
            : base(id)
        {
        }

        public TileEntityMobSpawner ()
            : this(TypeId)
        {
        }

        public TileEntityMobSpawner (TileEntity te)
            : base(te)
        {
            TileEntityMobSpawner tes = te as TileEntityMobSpawner;
            if (tes != null) {
                _delay = tes._delay;
                _entityID = tes._entityID;
            }
        }


        #region ICopyable<TileEntity> Members

        public override TileEntity Copy ()
        {
            return new TileEntityMobSpawner(this);
        }

        #endregion


        #region INBTObject<TileEntity> Members

        public override TileEntity LoadTree (TagNode tree)
        {
            TagNodeCompound ctree = tree as TagNodeCompound;
            if (ctree == null || base.LoadTree(tree) == null) {
                return null;
            }

            _delay = ctree["Delay"].ToTagShort();
            _entityID = ctree["EntityId"].ToTagString();
			_maxDelay = ctree["MaxSpawnDelay"].ToTagShort();
			_minDelay = ctree["MinSpawnDelay"].ToTagShort();
			_spawnCount = ctree["SpawnCount"].ToTagShort();
			_spawnData = new Entity().LoadTree(ctree["SpawnData"].ToTagCompound());

            return this;
        }

        public override TagNode BuildTree ()
        {
            TagNodeCompound tree = base.BuildTree() as TagNodeCompound;
            tree["EntityId"] = new TagNodeString(_entityID);
            tree["Delay"] = new TagNodeShort(_delay);
			tree["MaxSpawnDelay"] = new TagNodeShort(_maxDelay);
			tree["MinSpawnDelay"] = new TagNodeShort(_minDelay);
			tree["SpawnCount"] = new TagNodeShort(_spawnCount);
			tree["SpawnData"] = _spawnData.BuildTree();
            return tree;
        }

        public override bool ValidateTree (TagNode tree)
        {
            return new NbtVerifier(tree, MobSpawnerSchema).Verify();
        }

        #endregion
    }
}
