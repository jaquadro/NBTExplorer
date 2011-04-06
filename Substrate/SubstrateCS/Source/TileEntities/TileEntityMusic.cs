using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Map.TileEntities
{
    using Substrate.Map.NBT;

    public class TileEntityMusic : TileEntity
    {
        public static readonly NBTCompoundNode MusicSchema = BaseSchema.MergeInto(new NBTCompoundNode("")
        {
            new NBTStringNode("id", "Music"),
            new NBTScalerNode("note", NBT_Type.TAG_BYTE),
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

        public override TileEntity LoadTree (NBT_Value tree)
        {
            NBT_Compound ctree = tree as NBT_Compound;
            if (ctree == null || base.LoadTree(tree) == null) {
                return null;
            }

            _note = ctree["Note"].ToNBTByte();

            return this;
        }

        public override NBT_Value BuildTree ()
        {
            NBT_Compound tree = base.BuildTree() as NBT_Compound;
            tree["Note"] = new NBT_Byte(_note);

            return tree;
        }

        public override bool ValidateTree (NBT_Value tree)
        {
            return new NBTVerifier(tree, MusicSchema).Verify();
        }

        #endregion
    }
}
