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
            new NBTScalerNode("Fuse", NBT_Type.TAG_BYTE),
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

        public override Entity LoadTree (NBT_Value tree)
        {
            NBT_Compound ctree = tree as NBT_Compound;
            if (ctree == null || base.LoadTree(tree) == null) {
                return null;
            }

            _fuse = ctree["Fuse"].ToNBTByte();

            return this;
        }

        public override NBT_Value BuildTree ()
        {
            NBT_Compound tree = base.BuildTree() as NBT_Compound;
            tree["Fuse"] = new NBT_Byte(_fuse);

            return tree;
        }

        public override bool ValidateTree (NBT_Value tree)
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
