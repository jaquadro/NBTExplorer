using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.TileEntities
{
    using Substrate.NBT;

    public class TileEntitySign : TileEntity
    {
        public static readonly NBTCompoundNode SignSchema = BaseSchema.MergeInto(new NBTCompoundNode("")
        {
            new NBTStringNode("id", "Sign"),
            new NBTScalerNode("Text1", NBT_Type.TAG_STRING),
            new NBTScalerNode("Text2", NBT_Type.TAG_STRING),
            new NBTScalerNode("Text3", NBT_Type.TAG_STRING),
            new NBTScalerNode("Text4", NBT_Type.TAG_STRING),
        });

        private string _text1 = "";
        private string _text2 = "";
        private string _text3 = "";
        private string _text4 = "";

        public string Text1
        {
            get { return _text1; }
            set { _text1 = value.Substring(0, 12); }
        }

        public string Text2
        {
            get { return _text2; }
            set { _text2 = value.Substring(0, 12); }
        }

        public string Text3
        {
            get { return _text3; }
            set { _text3 = value.Substring(0, 12); }
        }

        public string Text4
        {
            get { return _text4; }
            set { _text4 = value.Substring(0, 12); }
        }

        public TileEntitySign ()
            : base("Sign")
        {
        }

        public TileEntitySign (TileEntity te)
            : base(te)
        {
            TileEntitySign tes = te as TileEntitySign;
            if (tes != null) {
                _text1 = tes._text1;
                _text2 = tes._text2;
                _text3 = tes._text3;
                _text4 = tes._text4;
            }
        }


        #region ICopyable<TileEntity> Members

        public override TileEntity Copy ()
        {
            return new TileEntitySign(this);
        }

        #endregion


        #region INBTObject<TileEntity> Members

        public override TileEntity LoadTree (NBT_Value tree)
        {
            NBT_Compound ctree = tree as NBT_Compound;
            if (ctree == null || base.LoadTree(tree) == null) {
                return null;
            }

            _text1 = ctree["Text1"].ToNBTString();
            _text2 = ctree["Text2"].ToNBTString();
            _text3 = ctree["Text3"].ToNBTString();
            _text4 = ctree["Text4"].ToNBTString();

            return this;
        }

        public override NBT_Value BuildTree ()
        {
            NBT_Compound tree = base.BuildTree() as NBT_Compound;
            tree["Text1"] = new NBT_String(_text1);
            tree["Text2"] = new NBT_String(_text2);
            tree["Text3"] = new NBT_String(_text3);
            tree["Text4"] = new NBT_String(_text4);

            return tree;
        }

        public override bool ValidateTree (NBT_Value tree)
        {
            return new NBTVerifier(tree, SignSchema).Verify();
        }

        #endregion
    }
}
