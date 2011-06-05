using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.TileEntities
{
    using Substrate.NBT;

    public class TileEntityRecordPlayer : TileEntity
    {
        public static readonly NBTCompoundNode RecordPlayerSchema = BaseSchema.MergeInto(new NBTCompoundNode("")
        {
            new NBTStringNode("id", "RecordPlayer"),
            new NBTScalerNode("Record", TagType.TAG_INT, NBTOptions.OPTIONAL),
        });

        private int? _record = null;

        public int? Record
        {
            get { return _record; }
            set { _record = value; }
        }

        public TileEntityRecordPlayer ()
            : base("RecordPlayer")
        {
        }

        public TileEntityRecordPlayer (TileEntity te)
            : base(te)
        {
            TileEntityRecordPlayer tes = te as TileEntityRecordPlayer;
            if (tes != null) {
                _record = tes._record;
            }
        }


        #region ICopyable<TileEntity> Members

        public override TileEntity Copy ()
        {
            return new TileEntityRecordPlayer(this);
        }

        #endregion


        #region INBTObject<TileEntity> Members

        public override TileEntity LoadTree (TagValue tree)
        {
            TagCompound ctree = tree as TagCompound;
            if (ctree == null || base.LoadTree(tree) == null) {
                return null;
            }

            if (ctree.ContainsKey("Record")) {
                _record = ctree["Record"].ToTagInt();
            }

            return this;
        }

        public override TagValue BuildTree ()
        {
            TagCompound tree = base.BuildTree() as TagCompound;

            if (_record != null) {
                tree["Record"] = new TagInt((int)_record);
            }

            return tree;
        }

        public override bool ValidateTree (TagValue tree)
        {
            return new NBTVerifier(tree, RecordPlayerSchema).Verify();
        }

        #endregion
    }
}
