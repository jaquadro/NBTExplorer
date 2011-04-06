using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Map.TileEntities
{
    using Substrate.Map.NBT;

    public class TileEntityMobSpawner : TileEntity
    {
        public static readonly NBTCompoundNode MobSpawnerSchema = BaseSchema.MergeInto(new NBTCompoundNode("")
        {
            new NBTStringNode("id", "MobSpawner"),
            new NBTScalerNode("EntityId", NBT_Type.TAG_STRING),
            new NBTScalerNode("Delay", NBT_Type.TAG_SHORT),
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

        public override TileEntity LoadTree (NBT_Value tree)
        {
            NBT_Compound ctree = tree as NBT_Compound;
            if (ctree == null || base.LoadTree(tree) == null) {
                return null;
            }

            _delay = ctree["Delay"].ToNBTShort();
            _entityID = ctree["EntityID"].ToNBTString();

            return this;
        }

        public override NBT_Value BuildTree ()
        {
            NBT_Compound tree = base.BuildTree() as NBT_Compound;
            tree["EntityID"] = new NBT_String(_entityID);
            tree["Delay"] = new NBT_Short(_delay);

            return tree;
        }

        public override bool ValidateTree (NBT_Value tree)
        {
            return new NBTVerifier(tree, MobSpawnerSchema).Verify();
        }

        #endregion
    }
}
