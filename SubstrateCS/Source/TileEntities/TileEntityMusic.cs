using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.TileEntities
{
    using Substrate.Nbt;

    public class TileEntityMusic : TileEntity
    {
        public static readonly SchemaNodeCompound MusicSchema = TileEntity.Schema.MergeInto(new SchemaNodeCompound("")
        {
            new SchemaNodeString("id", TypeId),
            new SchemaNodeScaler("note", TagType.TAG_BYTE),
        });

        public static string TypeId
        {
            get { return "Music"; }
        }

        private byte _note;

        public int Note
        {
            get { return _note; }
            set { _note = (byte)value; }
        }

        protected TileEntityMusic (string id)
            : base(id)
        {
        }

        public TileEntityMusic ()
            : this(TypeId)
        {
        }

        public TileEntityMusic (TileEntity te)
            : base(te)
        {
            TileEntityMusic tes = te as TileEntityMusic;
            if (tes != null) {
                _note = tes._note;
            }
        }


        #region ICopyable<TileEntity> Members

        public override TileEntity Copy ()
        {
            return new TileEntityMusic(this);
        }

        #endregion


        #region INBTObject<TileEntity> Members

        public override TileEntity LoadTree (TagNode tree)
        {
            TagNodeCompound ctree = tree as TagNodeCompound;
            if (ctree == null || base.LoadTree(tree) == null) {
                return null;
            }

            _note = ctree["note"].ToTagByte();

            return this;
        }

        public override TagNode BuildTree ()
        {
            TagNodeCompound tree = base.BuildTree() as TagNodeCompound;
            tree["note"] = new TagNodeByte(_note);

            return tree;
        }

        public override bool ValidateTree (TagNode tree)
        {
            return new NbtVerifier(tree, MusicSchema).Verify();
        }

        #endregion
    }
}
