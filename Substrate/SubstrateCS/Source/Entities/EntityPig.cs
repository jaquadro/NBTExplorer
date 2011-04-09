using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Entities
{
    using Substrate.NBT;

    public class EntityPig : EntityMob
    {
        public static readonly NBTCompoundNode PigSchema = MobSchema.MergeInto(new NBTCompoundNode("")
        {
            new NBTStringNode("id", "Pig"),
            new NBTScalerNode("Saddle", TagType.TAG_BYTE),
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

        public override Entity LoadTree (TagValue tree)
        {
            TagCompound ctree = tree as TagCompound;
            if (ctree == null || base.LoadTree(tree) == null) {
                return null;
            }

            _saddle = ctree["Saddle"].ToTagByte() == 1;

            return this;
        }

        public override TagValue BuildTree ()
        {
            TagCompound tree = base.BuildTree() as TagCompound;
            tree["Saddle"] = new TagByte((byte)(_saddle ? 1 : 0));

            return tree;
        }

        public override bool ValidateTree (TagValue tree)
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
