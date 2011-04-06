using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Map.Entities
{
    using Substrate.Map.NBT;

    public class EntityPig : EntityMob
    {
        public static readonly NBTCompoundNode PigSchema = MobSchema.MergeInto(new NBTCompoundNode("")
        {
            new NBTStringNode("id", "Pig"),
            new NBTScalerNode("Saddle", NBT_Type.TAG_BYTE),
        });

        private bool _saddle;

        public bool HasSaddle
        {
            get { return _saddle; }
            set { _saddle = value; }
        }

        public EntityPig ()
            : base("Pig")
        {
        }

        public EntityPig (Entity e)
            : base(e)
        {
            EntityPig e2 = e as EntityPig;
            if (e2 != null) {
                _saddle = e2._saddle;
            }
        }


        #region INBTObject<Entity> Members

        public override Entity LoadTree (NBT_Value tree)
        {
            NBT_Compound ctree = tree as NBT_Compound;
            if (ctree == null || base.LoadTree(tree) == null) {
                return null;
            }

            _saddle = ctree["Saddle"].ToNBTByte() == 1;

            return this;
        }

        public override NBT_Value BuildTree ()
        {
            NBT_Compound tree = base.BuildTree() as NBT_Compound;
            tree["Saddle"] = new NBT_Byte((byte)(_saddle ? 1 : 0));

            return tree;
        }

        public override bool ValidateTree (NBT_Value tree)
        {
            return new NBTVerifier(tree, PigSchema).Verify();
        }

        #endregion


        #region ICopyable<Entity> Members

        public override Entity Copy ()
        {
            return new EntityPig(this);
        }

        #endregion
    }
}
