using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Entities
{
    using Substrate.NBT;

    public class EntitySheep : EntityMob
    {
        public static readonly NBTCompoundNode SheepSchema = MobSchema.MergeInto(new NBTCompoundNode("")
        {
            new NBTStringNode("id", "Sheep"),
            new NBTScalerNode("Sheared", TagType.TAG_BYTE),
            new NBTScalerNode("Color", TagType.TAG_BYTE, NBTOptions.CREATE_ON_MISSING),
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

        public override Entity LoadTree (TagValue tree)
        {
            TagCompound ctree = tree as TagCompound;
            if (ctree == null || base.LoadTree(tree) == null) {
                return null;
            }

            _sheared = ctree["Sheared"].ToTagByte() == 1;
            _color = ctree["Color"].ToTagByte();

            return this;
        }

        public override TagValue BuildTree ()
        {
            TagCompound tree = base.BuildTree() as TagCompound;
            tree["Sheared"] = new TagByte((byte)(_sheared ? 1 : 0));
            tree["Color"] = new TagByte(_color);

            return tree;
        }

        public override bool ValidateTree (TagValue tree)
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
