using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Entities
{
    using Substrate.Nbt;

    public enum VillagerProfession
    {
        Farmer = 0,
        Librarian = 1,
        Priest = 2,
        Smith = 3,
        Butcher = 4,
    }

    public class EntityVillager : EntityMob
    {
        public static readonly SchemaNodeCompound VillagerSchema = MobSchema.MergeInto(new SchemaNodeCompound("")
        {
            new SchemaNodeString("id", TypeId),
            new SchemaNodeScaler("Profession", TagType.TAG_INT),
        });

        public static new string TypeId
        {
            get { return "Villager"; }
        }

        private int _profession;

        public VillagerProfession Profession
        {
            get { return (VillagerProfession)_profession; }
            set { _profession = (int)value; }
        }

        protected EntityVillager (string id)
            : base(id)
        {
        }

        public EntityVillager ()
            : this(TypeId)
        {
        }

        public EntityVillager (TypedEntity e)
            : base(e)
        {
            EntityVillager e2 = e as EntityVillager;
            if (e2 != null) {
                _profession = e2._profession;
            }
        }


        #region INBTObject<Entity> Members

        public override TypedEntity LoadTree (TagNode tree)
        {
            TagNodeCompound ctree = tree as TagNodeCompound;
            if (ctree == null || base.LoadTree(tree) == null) {
                return null;
            }

            _profession = ctree["Profession"].ToTagInt();

            return this;
        }

        public override TagNode BuildTree ()
        {
            TagNodeCompound tree = base.BuildTree() as TagNodeCompound;
            tree["Profession"] = new TagNodeInt(_profession);

            return tree;
        }

        public override bool ValidateTree (TagNode tree)
        {
            return new NbtVerifier(tree, VillagerSchema).Verify();
        }

        #endregion


        #region ICopyable<Entity> Members

        public override TypedEntity Copy ()
        {
            return new EntityVillager(this);
        }

        #endregion
    }
}
