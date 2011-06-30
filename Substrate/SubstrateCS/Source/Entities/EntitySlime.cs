using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Entities
{
    using Substrate.NBT;

    public class EntitySlime : EntityMob
    {
        public static readonly SchemaNodeCompound SlimeSchema = MobSchema.MergeInto(new SchemaNodeCompound("")
        {
            new SchemaNodeString("id", "Slime"),
            new SchemaNodeScaler("Size", TagType.TAG_INT),
        });

        private int _size;

        public int Size
        {
            get { return _size; }
            set { _size = value; }
        }

        public EntitySlime ()
            : base("Slime")
        {
        }

        public EntitySlime (EntityTyped e)
            : base(e)
        {
            EntitySlime e2 = e as EntitySlime;
            if (e2 != null) {
                _size = e2._size;
            }
        }


        #region INBTObject<Entity> Members

        public override EntityTyped LoadTree (TagNode tree)
        {
            TagNodeCompound ctree = tree as TagNodeCompound;
            if (ctree == null || base.LoadTree(tree) == null) {
                return null;
            }

            _size = ctree["Size"].ToTagInt();

            return this;
        }

        public override TagNode BuildTree ()
        {
            TagNodeCompound tree = base.BuildTree() as TagNodeCompound;
            tree["Size"] = new TagNodeInt(_size);

            return tree;
        }

        public override bool ValidateTree (TagNode tree)
        {
            return new NBTVerifier(tree, SlimeSchema).Verify();
        }

        #endregion


        #region ICopyable<Entity> Members

        public override EntityTyped Copy ()
        {
            return new EntitySlime(this);
        }

        #endregion
    }
}
