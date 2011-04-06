using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Map.Entities
{
    using Substrate.Map.NBT;

    public class EntitySheep : EntityMob
    {
        public static readonly NBTCompoundNode SheepSchema = MobSchema.MergeInto(new NBTCompoundNode("")
        {
            new NBTStringNode("id", "Pig"),
            new NBTScalerNode("Sheared", NBT_Type.TAG_BYTE),
            new NBTScalerNode("Color", NBT_Type.TAG_BYTE),
        });

        private bool _sheared;
        private byte _color;

        public bool IsSheared
        {
            get { return _sheared; }
            set { _sheared = value; }
        }

        public int Color
        {
            get { return _color; }
            set { _color = (byte)value; }
        }

        public EntitySheep ()
            : base("Sheep")
        {
        }

        public EntitySheep (Entity e)
            : base(e)
        {
            EntitySheep e2 = e as EntitySheep;
            if (e2 != null) {
                _sheared = e2._sheared;
                _color = e2._color;
            }
        }


        #region INBTObject<Entity> Members

        public override Entity LoadTree (NBT_Value tree)
        {
            NBT_Compound ctree = tree as NBT_Compound;
            if (ctree == null || base.LoadTree(tree) == null) {
                return null;
            }

            _sheared = ctree["Sheared"].ToNBTByte() == 1;
            _color = ctree["Color"].ToNBTByte();

            return this;
        }

        public override NBT_Value BuildTree ()
        {
            NBT_Compound tree = base.BuildTree() as NBT_Compound;
            tree["Sheared"] = new NBT_Byte((byte)(_sheared ? 1 : 0));
            tree["Color"] = new NBT_Byte(_color);

            return tree;
        }

        public override bool ValidateTree (NBT_Value tree)
        {
            return new NBTVerifier(tree, SheepSchema).Verify();
        }

        #endregion


        #region ICopyable<Entity> Members

        public override Entity Copy ()
        {
            return new EntitySheep(this);
        }

        #endregion
    }
}
