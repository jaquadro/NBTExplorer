using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.TileEntities
{
    using Substrate.Nbt;

    public class TileEntityEndPortal : TileEntity
    {
        public static readonly SchemaNodeCompound EndPortalSchema = TileEntity.Schema.MergeInto(new SchemaNodeCompound("")
        {
            new SchemaNodeString("id", TypeId),
        });

        public static string TypeId
        {
            get { return "Airportal"; }
        }

        protected TileEntityEndPortal (string id)
            : base(id)
        {
        }

        public TileEntityEndPortal ()
            : this(TypeId)
        {
        }

        public TileEntityEndPortal (TileEntity te)
            : base(te)
        {
        }


        #region ICopyable<TileEntity> Members

        public override TileEntity Copy ()
        {
            return new TileEntityEndPortal(this);
        }

        #endregion


        #region INBTObject<TileEntity> Members

        public override TileEntity LoadTree (TagNode tree)
        {
            TagNodeCompound ctree = tree as TagNodeCompound;
            if (ctree == null || base.LoadTree(tree) == null) {
                return null;
            }

            return this;
        }

        public override TagNode BuildTree ()
        {
            TagNodeCompound tree = base.BuildTree() as TagNodeCompound;

            return tree;
        }

        public override bool ValidateTree (TagNode tree)
        {
            return new NbtVerifier(tree, EndPortalSchema).Verify();
        }

        #endregion
    }
}
