using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Entities
{
    using Substrate.NBT;

    public class EntityMinecartFurnace : EntityMinecart
    {
        public static readonly NBTCompoundNode MinecartFurnaceSchema = MinecartSchema.MergeInto(new NBTCompoundNode("")
        {
            new NBTScalerNode("PushX", NBT_Type.TAG_DOUBLE),
            new NBTScalerNode("PushZ", NBT_Type.TAG_DOUBLE),
            new NBTScalerNode("Fuel", NBT_Type.TAG_SHORT),
        });

        private double _pushX;
        private double _pushZ;
        private short _fuel;

        public double PushX
        {
            get { return _pushX; }
            set { _pushX = value; }
        }

        public double PushZ
        {
            get { return _pushZ; }
            set { _pushZ = value; }
        }

        public int Fuel
        {
            get { return _fuel; }
            set { _fuel = (short)value; }
        }

        public EntityMinecartFurnace ()
            : base()
        {
        }

        public EntityMinecartFurnace (Entity e)
            : base(e)
        {
            EntityMinecartFurnace e2 = e as EntityMinecartFurnace;
            if (e2 != null) {
                _pushX = e2._pushX;
                _pushZ = e2._pushZ;
                _fuel = e2._fuel;
            }
        }


        #region INBTObject<Entity> Members

        public override Entity LoadTree (NBT_Value tree)
        {
            NBT_Compound ctree = tree as NBT_Compound;
            if (ctree == null || base.LoadTree(tree) == null) {
                return null;
            }

            _pushX = ctree["PushX"].ToNBTDouble();
            _pushZ = ctree["PushZ"].ToNBTDouble();
            _fuel = ctree["Fuel"].ToNBTShort();

            return this;
        }

        public override NBT_Value BuildTree ()
        {
            NBT_Compound tree = base.BuildTree() as NBT_Compound;
            tree["PushX"] = new NBT_Double(_pushX);
            tree["PushZ"] = new NBT_Double(_pushZ);
            tree["Fuel"] = new NBT_Short(_fuel);

            return tree;
        }

        public override bool ValidateTree (NBT_Value tree)
        {
            return new NBTVerifier(tree, MinecartFurnaceSchema).Verify();
        }

        #endregion


        #region ICopyable<Entity> Members

        public override Entity Copy ()
        {
            return new EntityMinecartFurnace(this);
        }

        #endregion
    }
}
