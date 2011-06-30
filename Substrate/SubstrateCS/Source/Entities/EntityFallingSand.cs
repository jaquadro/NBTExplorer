using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Entities
{
    using Substrate.Nbt;

    public class EntityFallingSand : EntityTyped
    {
        public static readonly SchemaNodeCompound FallingSandSchema = EntityTyped.Schema.MergeInto(new SchemaNodeCompound("")
        {
            new SchemaNodeString("id", "FallingSand"),
            new SchemaNodeScaler("Tile", TagType.TAG_BYTE),
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

        public EntityFallingSand (EntityTyped e)
            : base(e)
        {
            EntityFallingSand e2 = e as EntityFallingSand;
            if (e2 != null) {
                _tile = e2._tile;
            }
        }


        #region INBTObject<Entity> Members

        public override EntityTyped LoadTree (TagNode tree)
        {
            TagNodeCompound ctree = tree as TagNodeCompound;
            if (ctree == null || base.LoadTree(tree) == null) {
                return null;
            }

            _tile = ctree["Tile"].ToTagByte();

            return this;
        }

        public override TagNode BuildTree ()
        {
            TagNodeCompound tree = base.BuildTree() as TagNodeCompound;
            tree["Tile"] = new TagNodeByte(_tile);

            return tree;
        }

        public override bool ValidateTree (TagNode tree)
        {
            return new NBTVerifier(tree, FallingSandSchema).Verify();
        }

        #endregion


        #region ICopyable<Entity> Members

        public override EntityTyped Copy ()
        {
            return new EntityFallingSand(this);
        }

        #endregion
    }
}
