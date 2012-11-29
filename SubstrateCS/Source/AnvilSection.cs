using System;
using System.Collections.Generic;
using System.Text;
using Substrate.Nbt;
using Substrate.Core;

namespace Substrate
{
    public class AnvilSection : INbtObject<AnvilSection>, ICopyable<AnvilSection>
    {
        public static SchemaNodeCompound SectionSchema = new SchemaNodeCompound()
        {
            new SchemaNodeArray("Blocks", 4096),
            new SchemaNodeArray("Data", 2048),
            new SchemaNodeArray("SkyLight", 2048),
            new SchemaNodeArray("BlockLight", 2048),
            new SchemaNodeScaler("Y", TagType.TAG_BYTE),
            new SchemaNodeArray("Add", 2048, SchemaOptions.OPTIONAL),
        };

        private const int XDIM = 16;
        private const int YDIM = 16;
        private const int ZDIM = 16;

        private const int MIN_Y = 0;
        private const int MAX_Y = 15;

        private TagNodeCompound _tree;

        private byte _y;
        private YZXByteArray _blocks;
        private YZXNibbleArray _data;
        private YZXNibbleArray _blockLight;
        private YZXNibbleArray _skyLight;
        private YZXNibbleArray _addBlocks;

        private AnvilSection ()
        {
        }

        public AnvilSection (int y)
        {
            if (y < MIN_Y || y > MAX_Y)
                throw new ArgumentOutOfRangeException();

            _y = (byte)y;
            BuildNbtTree();
        }

        public AnvilSection (TagNodeCompound tree)
        {
            LoadTree(tree);
        }

        public int Y
        {
            get { return _y; }
            set
            {
                if (value < MIN_Y || value > MAX_Y)
                    throw new ArgumentOutOfRangeException();

                _y = (byte)value;
                _tree["Y"].ToTagByte().Data = _y;
            }
        }

        public YZXByteArray Blocks
        {
            get { return _blocks; }
        }

        public YZXNibbleArray Data
        {
            get { return _data; }
        }

        public YZXNibbleArray BlockLight
        {
            get { return _blockLight; }
        }

        public YZXNibbleArray SkyLight
        {
            get { return _skyLight; }
        }

        public YZXNibbleArray AddBlocks
        {
            get { return _addBlocks; }
        }

        public bool CheckEmpty ()
        {
            return CheckBlocksEmpty() && CheckAddBlocksEmpty();
        }

        private bool CheckBlocksEmpty ()
        {
            for (int i = 0; i < _blocks.Length; i++)
                if (_blocks[i] != 0)
                    return false;
            return true;
        }

        private bool CheckAddBlocksEmpty ()
        {
            if (_addBlocks != null)
                for (int i = 0; i < _addBlocks.Length; i++)
                    if (_addBlocks[i] != 0)
                        return false;
            return true;
        }

        #region INbtObject<AnvilSection> Members

        public AnvilSection LoadTree (TagNode tree)
        {
            TagNodeCompound ctree = tree as TagNodeCompound;
            if (ctree == null) {
                return null;
            }

            _y = ctree["Y"] as TagNodeByte;

            _blocks = new YZXByteArray(XDIM, YDIM, ZDIM, ctree["Blocks"] as TagNodeByteArray);
            _data = new YZXNibbleArray(XDIM, YDIM, ZDIM, ctree["Data"] as TagNodeByteArray);
            _skyLight = new YZXNibbleArray(XDIM, YDIM, ZDIM, ctree["SkyLight"] as TagNodeByteArray);
            _blockLight = new YZXNibbleArray(XDIM, YDIM, ZDIM, ctree["BlockLight"] as TagNodeByteArray);

            if (!ctree.ContainsKey("Add"))
                ctree["Add"] = new TagNodeByteArray(new byte[2048]);
            _addBlocks = new YZXNibbleArray(XDIM, YDIM, ZDIM, ctree["Add"] as TagNodeByteArray);

            _tree = ctree;

            return this;
        }

        public AnvilSection LoadTreeSafe (TagNode tree)
        {
            if (!ValidateTree(tree)) {
                return null;
            }

            return LoadTree(tree);
        }

        public TagNode BuildTree ()
        {
            TagNodeCompound copy = new TagNodeCompound();
            foreach (KeyValuePair<string, TagNode> node in _tree) {
                copy.Add(node.Key, node.Value);
            }

            if (CheckAddBlocksEmpty())
                copy.Remove("Add");

            return copy;
        }

        public bool ValidateTree (TagNode tree)
        {
            NbtVerifier v = new NbtVerifier(tree, SectionSchema);
            return v.Verify();
        }

        #endregion

        #region ICopyable<AnvilSection> Members

        public AnvilSection Copy ()
        {
            return new AnvilSection().LoadTree(_tree.Copy());
        }

        #endregion

        private void BuildNbtTree ()
        {
            int elements3 = XDIM * YDIM * ZDIM;

            TagNodeByteArray blocks = new TagNodeByteArray(new byte[elements3]);
            TagNodeByteArray data = new TagNodeByteArray(new byte[elements3 >> 1]);
            TagNodeByteArray skyLight = new TagNodeByteArray(new byte[elements3 >> 1]);
            TagNodeByteArray blockLight = new TagNodeByteArray(new byte[elements3 >> 1]);
            TagNodeByteArray addBlocks = new TagNodeByteArray(new byte[elements3 >> 1]);

            _blocks = new YZXByteArray(XDIM, YDIM, ZDIM, blocks);
            _data = new YZXNibbleArray(XDIM, YDIM, ZDIM, data);
            _skyLight = new YZXNibbleArray(XDIM, YDIM, ZDIM, skyLight);
            _blockLight = new YZXNibbleArray(XDIM, YDIM, ZDIM, blockLight);
            _addBlocks = new YZXNibbleArray(XDIM, YDIM, ZDIM, addBlocks);

            TagNodeCompound tree = new TagNodeCompound();
            tree.Add("Y", new TagNodeByte(_y));
            tree.Add("Blocks", blocks);
            tree.Add("Data", data);
            tree.Add("SkyLight", skyLight);
            tree.Add("BlockLight", blockLight);
            tree.Add("Add", addBlocks);

            _tree = tree;
        }
    }
}
