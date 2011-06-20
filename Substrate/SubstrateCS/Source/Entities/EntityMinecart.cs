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

        public static readonly SchemaNodeCompound MinecartSchema = BaseSchema.MergeInto(new SchemaNodeCompound("")
        {
            new SchemaNodeString("id", "Minecart"),
            new SchemaNodeScaler("Type", TagType.TAG_BYTE),
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

        public override Entity LoadTree (TagNode tree)
        {
            TagNodeCompound ctree = tree as TagNodeCompound;
            if (ctree == null || base.LoadTree(tree) == null) {
                return null;
            }

            _type = (CartType)ctree["Type"].ToTagByte().Data;

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

        public override TagNode BuildTree ()
        {
            TagNodeCompound tree = base.BuildTree() as TagNodeCompound;
            tree["Type"] = new TagNodeByte((byte)_type);

            return tree;
        }

        public override bool ValidateTree (TagNode tree)
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
