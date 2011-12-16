using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Entities
{
    using Substrate.Nbt;

    public class EntitySheep : EntityAnimal
    {
        public static readonly SchemaNodeCompound SheepSchema = AnimalSchema.MergeInto(new SchemaNodeCompound("")
        {
            new SchemaNodeString("id", TypeId),
            new SchemaNodeScaler("Sheared", TagType.TAG_BYTE),
            new SchemaNodeScaler("Color", TagType.TAG_BYTE, SchemaOptions.CREATE_ON_MISSING),
        });

        public static new string TypeId
        {
            get { return "Sheep"; }
        }

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

        protected EntitySheep (string id)
            : base(id)
        {
        }

        public EntitySheep ()
            : this(TypeId)
        {
        }

        public EntitySheep (TypedEntity e)
            : base(e)
        {
            EntitySheep e2 = e as EntitySheep;
            if (e2 != null) {
                _sheared = e2._sheared;
                _color = e2._color;
            }
        }


        #region INBTObject<Entity> Members

        public override TypedEntity LoadTree (TagNode tree)
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
            return new NbtVerifier(tree, SheepSchema).Verify();
        }

        #endregion


        #region ICopyable<Entity> Members

        public override TypedEntity Copy ()
        {
            return new EntitySheep(this);
        }

        #endregion
    }
}
