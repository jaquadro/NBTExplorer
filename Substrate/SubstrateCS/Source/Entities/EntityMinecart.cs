using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Entities
{
    using Substrate.NBT;

    public class EntityMinecart : Entity
    {
        public enum CartType
        {
            EMPTY = 0,
            CHEST = 1,
            FURNACE = 2,
        }

        public static readonly NBTCompoundNode MinecartSchema = BaseSchema.MergeInto(new NBTCompoundNode("")
        {
            new NBTStringNode("id", "Minecart"),
            new NBTScalerNode("Type", NBT_Type.TAG_BYTE),
        });

        private CartType _type;

        public CartType Type
        {
            get { return _type; }
        }

        public EntityMinecart ()
            : base("Minecart")
        {
        }

        public EntityMinecart (Entity e)
            : base(e)
        {
            EntityMinecart e2 = e as EntityMinecart;
            if (e2 != null) {
                _type = e2._type;
            }
        }


        #region INBTObject<Entity> Members

        public override Entity LoadTree (NBT_Value tree)
        {
            NBT_Compound ctree = tree as NBT_Compound;
            if (ctree == null || base.LoadTree(tree) == null) {
                return null;
            }

            _type = (CartType)ctree["Type"].ToNBTByte().Data;

            switch (_type) {
                case CartType.EMPTY:
                    return this;
                case CartType.CHEST:
                    return new EntityMinecartChest().LoadTreeSafe(tree);
                case CartType.FURNACE:
                    return new EntityMinecartFurnace().LoadTreeSafe(tree);
                default:
                    return this;
            }
        }

        public override NBT_Value BuildTree ()
        {
            NBT_Compound tree = base.BuildTree() as NBT_Compound;
            tree["Type"] = new NBT_Byte((byte)_type);

            return tree;
        }

        public override bool ValidateTree (NBT_Value tree)
        {
            return new NBTVerifier(tree, MinecartSchema).Verify();
        }

        #endregion


        #region ICopyable<Entity> Members

        public override Entity Copy ()
        {
            return new EntityMinecart(this);
        }

        #endregion
    }
}
