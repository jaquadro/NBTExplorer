using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Entities
{
    using Substrate.NBT;

    public class EntityPrimedTnt : Entity
    {
        public static readonly NBTCompoundNode PrimedTntSchema = BaseSchema.MergeInto(new NBTCompoundNode("")
        {
            new NBTStringNode("id", "PrimedTnt"),
            new NBTScalerNode("Fuse", TagType.TAG_BYTE),
        });

        private byte _fuse;

        public int Fuse
        {
            get { return _fuse; }
            set { _fuse = (byte)value; }
        }

        public EntityPrimedTnt ()
            : base("PrimedTnt")
        {
        }

        public EntityPrimedTnt (Entity e)
            : base(e)
        {
            EntityPrimedTnt e2 = e as EntityPrimedTnt;
            if (e2 != null) {
                _fuse = e2._fuse;
            }
        }


        #region INBTObject<Entity> Members

        public override Entity LoadTree (TagValue tree)
        {
            TagCompound ctree = tree as TagCompound;
            if (ctree == null || base.LoadTree(tree) == null) {
                return null;
            }

            _fuse = ctree["Fuse"].ToTagByte();

            return this;
        }

        public override TagValue BuildTree ()
        {
            TagCompound tree = base.BuildTree() as TagCompound;
            tree["Fuse"] = new TagByte(_fuse);

            return tree;
        }

        public override bool ValidateTree (TagValue tree)
        {
            return new NBTVerifier(tree, PrimedTntSchema).Verify();
        }

        #endregion


        #region ICopyable<Entity> Members

        public override Entity Copy ()
        {
            return new EntityPrimedTnt(this);
        }

        #endregion
    }
}
