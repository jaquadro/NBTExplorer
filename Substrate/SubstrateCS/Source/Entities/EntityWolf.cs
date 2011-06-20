using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Entities
{
    using Substrate.NBT;

    public class EntityWolf : EntityMob
    {
        public static readonly SchemaNodeCompound WolfSchema = MobSchema.MergeInto(new SchemaNodeCompound("")
        {
            new SchemaNodeString("id", "Wolf"),
            new SchemaNodeScaler("Owner", TagType.TAG_STRING),
            new SchemaNodeScaler("Sitting", TagType.TAG_BYTE),
            new SchemaNodeScaler("Angry", TagType.TAG_BYTE),
        });

        private string _owner;
        private bool _sitting;
        private bool _angry;

        public string Owner
        {
            get { return _owner; }
            set { _owner = value; }
        }

        public bool IsSitting
        {
            get { return _sitting; }
            set { _sitting = value; }
        }

        public bool IsAngry
        {
            get { return _angry; }
            set { _angry = value; }
        }

        public EntityWolf ()
            : base("Wolf")
        {
        }

        public EntityWolf (Entity e)
            : base(e)
        {
            EntityWolf e2 = e as EntityWolf;
            if (e2 != null) {
                _owner = e2._owner;
                _sitting = e2._sitting;
                _angry = e2._angry;
            }
        }


        #region INBTObject<Entity> Members

        public override Entity LoadTree (TagNode tree)
        {
            TagNodeCompound ctree = tree as TagNodeCompound;
            if (ctree == null || base.LoadTree(tree) == null) {
                return null;
            }

            _owner = ctree["Owner"].ToTagString();
            _sitting = ctree["Sitting"].ToTagByte() == 1;
            _angry = ctree["Angry"].ToTagByte() == 1;

            return this;
        }

        public override TagNode BuildTree ()
        {
            TagNodeCompound tree = base.BuildTree() as TagNodeCompound;
            tree["Owner"] = new TagNodeString(_owner);
            tree["Sitting"] = new TagNodeByte((byte)(_sitting ? 1 : 0));
            tree["Angry"] = new TagNodeByte((byte)(_angry ? 1 : 0));

            return tree;
        }

        public override bool ValidateTree (TagNode tree)
        {
            return new NBTVerifier(tree, WolfSchema).Verify();
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
