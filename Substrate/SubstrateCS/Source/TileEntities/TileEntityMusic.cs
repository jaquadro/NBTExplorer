using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.TileEntities
{
    using Substrate.NBT;

    public class TileEntityMusic : TileEntity
    {
        public static readonly NBTCompoundNode MusicSchema = BaseSchema.MergeInto(new NBTCompoundNode("")
        {
            new NBTStringNode("id", "Music"),
            new NBTScalerNode("note", TagType.TAG_BYTE),
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

        public override TileEntity LoadTree (TagValue tree)
        {
            TagCompound ctree = tree as TagCompound;
            if (ctree == null || base.LoadTree(tree) == null) {
                return null;
            }

            _note = ctree["Note"].ToTagByte();

            return this;
        }

        public override TagValue BuildTree ()
        {
            TagCompound tree = base.BuildTree() as TagCompound;
            tree["Note"] = new TagByte(_note);

            return tree;
        }

        public override bool ValidateTree (TagValue tree)
        {
            return new NBTVerifier(tree, MusicSchema).Verify();
        }

        #endregion
    }
}
