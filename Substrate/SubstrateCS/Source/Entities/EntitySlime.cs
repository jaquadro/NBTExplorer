using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Map.Entities
{
    using Substrate.Map.NBT;

    public class EntitySlime : EntityMob
    {
        public static readonly NBTCompoundNode SlimeSchema = MobSchema.MergeInto(new NBTCompoundNode("")
        {
            new NBTStringNode("id", "Slime"),
            new NBTScalerNode("Size", NBT_Type.TAG_INT),
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

        public override Entity LoadTree (NBT_Value tree)
        {
            NBT_Compound ctree = tree as NBT_Compound;
            if (ctree == null || base.LoadTree(tree) == null) {
                return null;
            }

            _size = ctree["Size"].ToNBTInt();

            return this;
        }

        public override NBT_Value BuildTree ()
        {
            NBT_Compound tree = base.BuildTree() as NBT_Compound;
            tree["Size"] = new NBT_Int(_size);

            return tree;
        }

        public override bool ValidateTree (NBT_Value tree)
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
