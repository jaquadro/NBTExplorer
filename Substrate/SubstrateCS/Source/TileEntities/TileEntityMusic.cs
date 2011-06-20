using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.TileEntities
{
    using Substrate.NBT;

    public class TileEntityMusic : TileEntity
    {
        public static readonly SchemaNodeCompound MusicSchema = BaseSchema.MergeInto(new SchemaNodeCompound("")
        {
            new SchemaNodeString("id", "Music"),
            new SchemaNodeScaler("note", TagType.TAG_BYTE),
        });

        private byte _note;

        public int Note
        {
            get { return _note; }
            set { _note = (byte)value; }
        }

        public TileEntityMusic ()
            : base("Music")
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
            return new NBTVerifier(tree, MusicSchema).Verify();
        }

        #endregion
    }
}
