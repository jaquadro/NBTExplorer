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
            new NBTScalerNode("Tile", TagType.TAG_BYTE),
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

        public override Entity LoadTree (TagValue tree)
        {
            TagCompound ctree = tree as TagCompound;
            if (ctree == null || base.LoadTree(tree) == null) {
                return null;
            }

            _tile = ctree["Tile"].ToTagByte();

            return this;
        }

        public override TagValue BuildTree ()
        {
            TagCompound tree = base.BuildTree() as TagCompound;
            tree["Tile"] = new TagByte(_tile);

            return tree;
        }

        public override bool ValidateTree (TagValue tree)
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
