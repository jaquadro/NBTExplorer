using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.TileEntities
{
    using Substrate.Nbt;

    public class TileEntityControl : TileEntity
    {
        public static readonly SchemaNodeCompound ControlSchema = TileEntity.Schema.MergeInto(new SchemaNodeCompound("")
        {
            new SchemaNodeString("id", TypeId),
            new SchemaNodeScaler("Command", TagType.TAG_STRING),
        });

        public static string TypeId
        {
            get { return "Control"; }
        }

        private string _command;

        public string Command
        {
            get { return _command; }
            set { _command = value; }
        }

        protected TileEntityControl (string id)
            : base(id)
        {
        }

        public TileEntityControl ()
            : this(TypeId)
        {
        }

        public TileEntityControl (TileEntity te)
            : base(te)
        {
            TileEntityControl tes = te as TileEntityControl;
            if (tes != null) {
                _command = tes._command;
            }
        }


        #region ICopyable<TileEntity> Members

        public override TileEntity Copy ()
        {
            return new TileEntityControl(this);
        }

        #endregion


        #region INBTObject<TileEntity> Members

        public override TileEntity LoadTree (TagNode tree)
        {
            TagNodeCompound ctree = tree as TagNodeCompound;
            if (ctree == null || base.LoadTree(tree) == null) {
                return null;
            }

            _command = ctree["Command"].ToTagString();

            return this;
        }

        public override TagNode BuildTree ()
        {
            TagNodeCompound tree = base.BuildTree() as TagNodeCompound;
            tree["Command"] = new TagNodeString(_command);

            return tree;
        }

        public override bool ValidateTree (TagNode tree)
        {
            return new NbtVerifier(tree, ControlSchema).Verify();
        }

        #endregion
    }
}
