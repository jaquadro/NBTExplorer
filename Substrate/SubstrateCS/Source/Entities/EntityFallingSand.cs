using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Entities
{
    using Substrate.NBT;

    public class EntityFallingSand : Entity
    {
        public static readonly NBTCompoundNode FallingSandSchema = BaseSchema.MergeInto(new NBTCompoundNode("")
        {
            new NBTStringNode("id", "FallingSand"),
            new NBTScalerNode("Tile", NBT_Type.TAG_BYTE),
        });

        private byte _tile;

        public int Tile
        {
            get { return _tile; }
            set { _tile = (byte)value; }
        }

        public EntityFallingSand ()
            : base("PrimedTnt")
        {
        }

        public EntityFallingSand (Entity e)
            : base(e)
        {
            EntityFallingSand e2 = e as EntityFallingSand;
            if (e2 != null) {
                _tile = e2._tile;
            }
        }


        #region INBTObject<Entity> Members

        public override Entity LoadTree (NBT_Value tree)
        {
            NBT_Compound ctree = tree as NBT_Compound;
            if (ctree == null || base.LoadTree(tree) == null) {
                return null;
            }

            _tile = ctree["Tile"].ToNBTByte();

            return this;
        }

        public override NBT_Value BuildTree ()
        {
            NBT_Compound tree = base.BuildTree() as NBT_Compound;
            tree["Tile"] = new NBT_Byte(_tile);

            return tree;
        }

        public override bool ValidateTree (NBT_Value tree)
        {
            return new NBTVerifier(tree, FallingSandSchema).Verify();
        }

        #endregion


        #region ICopyable<Entity> Members

        public override Entity Copy ()
        {
            return new EntityFallingSand(this);
        }

        #endregion
    }
}
