using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Entities
{
    using Substrate.Nbt;

    public class EntityPig : EntityMob
    {
        public static readonly SchemaNodeCompound PigSchema = MobSchema.MergeInto(new SchemaNodeCompound("")
        {
            new SchemaNodeString("id", "Pig"),
            new SchemaNodeScaler("Saddle", TagType.TAG_BYTE),
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

        public EntityPig (EntityTyped e)
            : base(e)
        {
            EntityPig e2 = e as EntityPig;
            if (e2 != null) {
                _saddle = e2._saddle;
            }
        }


        #region INBTObject<Entity> Members

        public override EntityTyped LoadTree (TagNode tree)
        {
            TagNodeCompound ctree = tree as TagNodeCompound;
            if (ctree == null || base.LoadTree(tree) == null) {
                return null;
            }

            _saddle = ctree["Saddle"].ToTagByte() == 1;

            return this;
        }

        public override TagNode BuildTree ()
        {
            TagNodeCompound tree = base.BuildTree() as TagNodeCompound;
            tree["Saddle"] = new TagNodeByte((byte)(_saddle ? 1 : 0));

            return tree;
        }

        public override bool ValidateTree (TagNode tree)
        {
            return new NBTVerifier(tree, PigSchema).Verify();
        }

        #endregion


        #region ICopyable<Entity> Members

        public override EntityTyped Copy ()
        {
            return new EntityPig(this);
        }

        #endregion
    }
}
