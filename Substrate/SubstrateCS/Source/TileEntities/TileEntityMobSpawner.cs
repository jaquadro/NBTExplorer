using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.TileEntities
{
    using Substrate.NBT;

    public class TileEntityMobSpawner : TileEntity
    {
        public static readonly NBTCompoundNode MobSpawnerSchema = BaseSchema.MergeInto(new NBTCompoundNode("")
        {
            new NBTStringNode("id", "MobSpawner"),
            new NBTScalerNode("EntityId", TagType.TAG_STRING),
            new NBTScalerNode("Delay", TagType.TAG_SHORT),
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

        public override TileEntity LoadTree (TagValue tree)
        {
            TagCompound ctree = tree as TagCompound;
            if (ctree == null || base.LoadTree(tree) == null) {
                return null;
            }

            _delay = ctree["Delay"].ToTagShort();
            _entityID = ctree["EntityID"].ToTagString();

            return this;
        }

        public override TagValue BuildTree ()
        {
            TagCompound tree = base.BuildTree() as TagCompound;
            tree["EntityID"] = new TagString(_entityID);
            tree["Delay"] = new TagShort(_delay);

            return tree;
        }

        public override bool ValidateTree (TagValue tree)
        {
            return new NBTVerifier(tree, MobSpawnerSchema).Verify();
        }

        #endregion
    }
}
