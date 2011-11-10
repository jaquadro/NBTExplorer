using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Entities
{
    using Substrate.Nbt;

    // XXX: BUG ALERT
    // MC 1.8.1 Enderman data names "carried" and "carriedData" are inconsistent.  These values are subject to change
    // in the future, when the differences are reconciled.

    public class EntityEnderman : EntityMob
    {
        public static readonly SchemaNodeCompound EndermanSchema = MobSchema.MergeInto(new SchemaNodeCompound("")
        {
            new SchemaNodeString("id", TypeId),
            new SchemaNodeScaler("carried", TagType.TAG_SHORT, SchemaOptions.CREATE_ON_MISSING),
            new SchemaNodeScaler("carriedData", TagType.TAG_SHORT, SchemaOptions.CREATE_ON_MISSING),
        });

        public static new string TypeId
        {
            get { return "Enderman"; }
        }

        private short _carried;
        private short _carryingData;

        public int Carried
        {
            get { return _carried; }
            set { _carried = (short)value; }
        }

        public int CarryingData
        {
            get { return _carryingData; }
            set { _carryingData = (short)value; }
        }

        protected EntityEnderman (string id)
            : base(id)
        {
        }

        public EntityEnderman ()
            : this(TypeId)
        {
        }

        public EntityEnderman (TypedEntity e)
            : base(e)
        {
            EntityEnderman e2 = e as EntityEnderman;
            if (e2 != null) {
                _carried = e2._carried;
                _carryingData = e2._carryingData;
            }
        }


        #region INBTObject<Entity> Members

        public override TypedEntity LoadTree (TagNode tree)
        {
            TagNodeCompound ctree = tree as TagNodeCompound;
            if (ctree == null || base.LoadTree(tree) == null) {
                return null;
            }

            _carried = ctree["carried"].ToTagShort();
            _carryingData = ctree["carriedData"].ToTagShort();

            return this;
        }

        public override TagNode BuildTree ()
        {
            TagNodeCompound tree = base.BuildTree() as TagNodeCompound;
            tree["carried"] = new TagNodeShort(_carried);
            tree["carriedData"] = new TagNodeShort(_carryingData);

            return tree;
        }

        public override bool ValidateTree (TagNode tree)
        {
            return new NbtVerifier(tree, EndermanSchema).Verify();
        }

        #endregion


        #region ICopyable<Entity> Members

        public override TypedEntity Copy ()
        {
            return new EntityEnderman(this);
        }

        #endregion
    }
}
