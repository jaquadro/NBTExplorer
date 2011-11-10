using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Entities
{
    using Substrate.Nbt;

    public class EntityMinecartFurnace : EntityMinecart
    {
        public static readonly SchemaNodeCompound MinecartFurnaceSchema = MinecartSchema.MergeInto(new SchemaNodeCompound("")
        {
            new SchemaNodeScaler("PushX", TagType.TAG_DOUBLE),
            new SchemaNodeScaler("PushZ", TagType.TAG_DOUBLE),
            new SchemaNodeScaler("Fuel", TagType.TAG_SHORT),
        });

        public static new string TypeId
        {
            get { return EntityMinecart.TypeId; }
        }

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

        protected EntityMinecartFurnace (string id)
            : base(id)
        {
        }

        public EntityMinecartFurnace ()
            : base()
        {
        }

        public EntityMinecartFurnace (TypedEntity e)
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

        public override TypedEntity LoadTree (TagNode tree)
        {
            TagNodeCompound ctree = tree as TagNodeCompound;
            if (ctree == null || base.LoadTree(tree) == null) {
                return null;
            }

            _pushX = ctree["PushX"].ToTagDouble();
            _pushZ = ctree["PushZ"].ToTagDouble();
            _fuel = ctree["Fuel"].ToTagShort();

            return this;
        }

        public override TagNode BuildTree ()
        {
            TagNodeCompound tree = base.BuildTree() as TagNodeCompound;
            tree["PushX"] = new TagNodeDouble(_pushX);
            tree["PushZ"] = new TagNodeDouble(_pushZ);
            tree["Fuel"] = new TagNodeShort(_fuel);

            return tree;
        }

        public override bool ValidateTree (TagNode tree)
        {
            return new NbtVerifier(tree, MinecartFurnaceSchema).Verify();
        }

        #endregion


        #region ICopyable<Entity> Members

        public override TypedEntity Copy ()
        {
            return new EntityMinecartFurnace(this);
        }

        #endregion
    }
}
