using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Entities
{
    using Substrate.NBT;

    public class EntityWolf : EntityMob
    {
        public static readonly NBTCompoundNode WolfSchema = MobSchema.MergeInto(new NBTCompoundNode("")
        {
            new NBTStringNode("id", "Wolf"),
            new NBTScalerNode("Owner", NBT_Type.TAG_STRING),
            new NBTScalerNode("Sitting", NBT_Type.TAG_BYTE),
            new NBTScalerNode("Angry", NBT_Type.TAG_BYTE),
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

        public override Entity LoadTree (NBT_Value tree)
        {
            NBT_Compound ctree = tree as NBT_Compound;
            if (ctree == null || base.LoadTree(tree) == null) {
                return null;
            }

            _owner = ctree["Owner"].ToNBTString();
            _sitting = ctree["Sitting"].ToNBTByte() == 1;
            _angry = ctree["Angry"].ToNBTByte() == 1;

            return this;
        }

        public override NBT_Value BuildTree ()
        {
            NBT_Compound tree = base.BuildTree() as NBT_Compound;
            tree["Owner"] = new NBT_String(_owner);
            tree["Sitting"] = new NBT_Byte((byte)(_sitting ? 1 : 0));
            tree["Angry"] = new NBT_Byte((byte)(_angry ? 1 : 0));

            return tree;
        }

        public override bool ValidateTree (NBT_Value tree)
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
