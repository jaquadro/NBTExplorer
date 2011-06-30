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
            new SchemaNodeString("id", "MobSpawner"),
            new SchemaNodeScaler("EntityId", TagType.TAG_STRING),
            new SchemaNodeScaler("Delay", TagType.TAG_SHORT),
        });

        private short _delay;
        private string _entityID;

        public int Delay
        {
            get { return _delay; }
            set { _delay = (short)value; }
        }

        public string EntityID
        {
            get { return _entityID; }
            set { _entityID = value; }
        }

        public TileEntityMobSpawner ()
            : base("MobSpawner")
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
            _entityID = ctree["EntityID"].ToTagString();

            return this;
        }

        public override TagNode BuildTree ()
        {
            TagNodeCompound tree = base.BuildTree() as TagNodeCompound;
            tree["EntityID"] = new TagNodeString(_entityID);
            tree["Delay"] = new TagNodeShort(_delay);

            return tree;
        }

        public override bool ValidateTree (TagNode tree)
        {
            return new NBTVerifier(tree, MobSpawnerSchema).Verify();
        }

        #endregion
    }
}
