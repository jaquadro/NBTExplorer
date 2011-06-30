using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Entities
{
    using Substrate.Nbt;

    public class EntitySheep : EntityMob
    {
        public static readonly SchemaNodeCompound SheepSchema = MobSchema.MergeInto(new SchemaNodeCompound("")
        {
            new SchemaNodeString("id", "Sheep"),
            new SchemaNodeScaler("Sheared", TagType.TAG_BYTE),
            new SchemaNodeScaler("Color", TagType.TAG_BYTE, SchemaOptions.CREATE_ON_MISSING),
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

        public EntitySheep (EntityTyped e)
            : base(e)
        {
            EntitySheep e2 = e as EntitySheep;
            if (e2 != null) {
                _sheared = e2._sheared;
                _color = e2._color;
            }
        }


        #region INBTObject<Entity> Members

        public override EntityTyped LoadTree (TagNode tree)
        {
            TagNodeCompound ctree = tree as TagNodeCompound;
            if (ctree == null || base.LoadTree(tree) == null) {
                return null;
            }

            _sheared = ctree["Sheared"].ToTagByte() == 1;
            _color = ctree["Color"].ToTagByte();

            return this;
        }

        public override TagNode BuildTree ()
        {
            TagNodeCompound tree = base.BuildTree() as TagNodeCompound;
            tree["Sheared"] = new TagNodeByte((byte)(_sheared ? 1 : 0));
            tree["Color"] = new TagNodeByte(_color);

            return tree;
        }

        public override bool ValidateTree (TagNode tree)
        {
            return new NBTVerifier(tree, SheepSchema).Verify();
        }

        #endregion


        #region ICopyable<Entity> Members

        public override EntityTyped Copy ()
        {
            return new EntitySheep(this);
        }

        #endregion
    }
}
