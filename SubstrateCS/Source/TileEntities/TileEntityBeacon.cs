using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.TileEntities
{
    using Substrate.Nbt;

    public class TileEntityBeacon : TileEntity
    {
        public static readonly SchemaNodeCompound BeaconSchema = TileEntity.Schema.MergeInto(new SchemaNodeCompound("")
        {
            new SchemaNodeString("id", TypeId),
            new SchemaNodeScaler("Levels", TagType.TAG_INT),
            new SchemaNodeScaler("Primary", TagType.TAG_INT),
            new SchemaNodeScaler("Secondary", TagType.TAG_INT),
        });

        public static string TypeId
        {
            get { return "Beacon"; }
        }

        private int _levels;
        private int _primary;
        private int _secondary;

        public int Levels
        {
            get { return _levels; }
            set { _levels = value; }
        }

        public int Primary
        {
            get { return _primary; }
            set { _primary = value; }
        }

        public int Secondary
        {
            get { return _secondary; }
            set { _secondary = value; }
        }

        protected TileEntityBeacon (string id)
            : base(id)
        {
        }

        public TileEntityBeacon ()
            : this(TypeId)
        {
        }

        public TileEntityBeacon (TileEntity te)
            : base(te)
        {
            TileEntityBeacon tes = te as TileEntityBeacon;
            if (tes != null) {
                _levels = tes._levels;
                _primary = tes._primary;
                _secondary = tes._secondary;
            }
        }


        #region ICopyable<TileEntity> Members

        public override TileEntity Copy ()
        {
            return new TileEntityBeacon(this);
        }

        #endregion


        #region INBTObject<TileEntity> Members

        public override TileEntity LoadTree (TagNode tree)
        {
            TagNodeCompound ctree = tree as TagNodeCompound;
            if (ctree == null || base.LoadTree(tree) == null) {
                return null;
            }

            _levels = ctree["Levels"].ToTagInt();
            _primary = ctree["Primary"].ToTagInt();
            _secondary = ctree["Secondary"].ToTagInt();

            return this;
        }

        public override TagNode BuildTree ()
        {
            TagNodeCompound tree = base.BuildTree() as TagNodeCompound;
            tree["Levels"] = new TagNodeInt(_levels);
            tree["Primary"] = new TagNodeInt(_primary);
            tree["Secondary"] = new TagNodeInt(_secondary);

            return tree;
        }

        public override bool ValidateTree (TagNode tree)
        {
            return new NbtVerifier(tree, BeaconSchema).Verify();
        }

        #endregion
    }
}
