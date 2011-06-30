using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.TileEntities
{
    using Substrate.Nbt;

    public class TileEntitySign : TileEntity
    {
        public static readonly SchemaNodeCompound SignSchema = TileEntity.Schema.MergeInto(new SchemaNodeCompound("")
        {
            new SchemaNodeString("id", "Sign"),
            new SchemaNodeScaler("Text1", TagType.TAG_STRING),
            new SchemaNodeScaler("Text2", TagType.TAG_STRING),
            new SchemaNodeScaler("Text3", TagType.TAG_STRING),
            new SchemaNodeScaler("Text4", TagType.TAG_STRING),
        });

        private string _text1 = "";
        private string _text2 = "";
        private string _text3 = "";
        private string _text4 = "";

        public string Text1
        {
            get { return _text1; }
            set { _text1 = value.Substring(0, 14); }
        }

        public string Text2
        {
            get { return _text2; }
            set { _text2 = value.Substring(0, 14); }
        }

        public string Text3
        {
            get { return _text3; }
            set { _text3 = value.Substring(0, 14); }
        }

        public string Text4
        {
            get { return _text4; }
            set { _text4 = value.Substring(0, 14); }
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

        public override TileEntity LoadTree (TagNode tree)
        {
            TagNodeCompound ctree = tree as TagNodeCompound;
            if (ctree == null || base.LoadTree(tree) == null) {
                return null;
            }

            _text1 = ctree["Text1"].ToTagString();
            _text2 = ctree["Text2"].ToTagString();
            _text3 = ctree["Text3"].ToTagString();
            _text4 = ctree["Text4"].ToTagString();

            return this;
        }

        public override TagNode BuildTree ()
        {
            TagNodeCompound tree = base.BuildTree() as TagNodeCompound;
            tree["Text1"] = new TagNodeString(_text1);
            tree["Text2"] = new TagNodeString(_text2);
            tree["Text3"] = new TagNodeString(_text3);
            tree["Text4"] = new TagNodeString(_text4);

            return tree;
        }

        public override bool ValidateTree (TagNode tree)
        {
            return new NbtVerifier(tree, SignSchema).Verify();
        }

        #endregion
    }
}
