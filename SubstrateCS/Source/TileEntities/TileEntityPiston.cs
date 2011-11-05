using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.TileEntities
{
    using Substrate.Nbt;

    public class TileEntityPiston : TileEntity
    {
        public static readonly SchemaNodeCompound PistonSchema = TileEntity.Schema.MergeInto(new SchemaNodeCompound("")
        {
            new SchemaNodeString("id", TypeId),
            new SchemaNodeScaler("blockId", TagType.TAG_INT),
            new SchemaNodeScaler("blockData", TagType.TAG_INT),
            new SchemaNodeScaler("facing", TagType.TAG_INT),
            new SchemaNodeScaler("progress", TagType.TAG_FLOAT),
            new SchemaNodeScaler("extending", TagType.TAG_BYTE),
        });

        public static string TypeId
        {
            get { return "Piston"; }
        }

        private int? _record = null;

        private byte _extending;
        private int _blockId;
        private int _blockData;
        private int _facing;
        private float _progress;

        public bool Extending
        {
            get { return _extending != 0; }
            set { _extending = (byte)(value ? 1 : 0); }
        }

        public int BlockId
        {
            get { return _blockId; }
            set { _blockId = value; }
        }

        public int BlockData
        {
            get { return _blockData; }
            set { _blockData = value; }
        }

        public int Facing
        {
            get { return _facing; }
            set { _facing = value; }
        }

        public float Progress
        {
            get { return _progress; }
            set { _progress = value; }
        }

        protected TileEntityPiston (string id)
            : base(id)
        {
        }

        public TileEntityPiston ()
            : this(TypeId)
        {
        }

        public TileEntityPiston (TileEntity te)
            : base(te)
        {
            TileEntityPiston tes = te as TileEntityPiston;
            if (tes != null) {
                _blockId = tes._blockId;
                _blockData = tes._blockData;
                _facing = tes._facing;
                _progress = tes._progress;
                _extending = tes._extending;
            }
        }


        #region ICopyable<TileEntity> Members

        public override TileEntity Copy ()
        {
            return new TileEntityPiston(this);
        }

        #endregion


        #region INBTObject<TileEntity> Members

        public override TileEntity LoadTree (TagNode tree)
        {
            TagNodeCompound ctree = tree as TagNodeCompound;
            if (ctree == null || base.LoadTree(tree) == null) {
                return null;
            }

            _blockId = ctree["blockId"].ToTagInt();
            _blockData = ctree["blockData"].ToTagInt();
            _facing = ctree["facing"].ToTagInt();
            _progress = ctree["progress"].ToTagFloat();
            _extending = ctree["extending"].ToTagByte();

            return this;
        }

        public override TagNode BuildTree ()
        {
            TagNodeCompound tree = base.BuildTree() as TagNodeCompound;

            if (_record != null) {
                tree["blockId"] = new TagNodeInt(_blockId);
                tree["blockData"] = new TagNodeInt(_blockData);
                tree["facing"] = new TagNodeInt(_facing);
                tree["progress"] = new TagNodeFloat(_progress);
                tree["extending"] = new TagNodeByte(_extending);
            }

            return tree;
        }

        public override bool ValidateTree (TagNode tree)
        {
            return new NbtVerifier(tree, PistonSchema).Verify();
        }

        #endregion
    }
}