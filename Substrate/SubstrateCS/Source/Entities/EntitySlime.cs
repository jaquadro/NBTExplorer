using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Entities
{
    using Substrate.NBT;

    public class EntitySlime : EntityMob
    {
        public static readonly NBTCompoundNode SlimeSchema = MobSchema.MergeInto(new NBTCompoundNode("")
        {
            new NBTStringNode("id", "Slime"),
            new NBTScalerNode("Size", TagType.TAG_INT),
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

        public EntitySlime (Entity e)
            : base(e)
        {
            EntitySlime e2 = e as EntitySlime;
            if (e2 != null) {
                _size = e2._size;
            }
        }


        #region INBTObject<Entity> Members

        public override Entity LoadTree (TagValue tree)
        {
            TagCompound ctree = tree as TagCompound;
            if (ctree == null || base.LoadTree(tree) == null) {
                return null;
            }

            _size = ctree["Size"].ToTagInt();

            return this;
        }

        public override TagValue BuildTree ()
        {
            TagCompound tree = base.BuildTree() as TagCompound;
            tree["Size"] = new TagInt(_size);

            return tree;
        }

        public override bool ValidateTree (TagValue tree)
        {
            return new NBTVerifier(tree, SlimeSchema).Verify();
        }

        #endregion


        #region ICopyable<Entity> Members

        public override Entity Copy ()
        {
            return new EntitySlime(this);
        }

        #endregion
    }
}
