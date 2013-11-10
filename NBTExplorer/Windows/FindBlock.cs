using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using NBTExplorer.Model;

namespace NBTExplorer.Windows
{
    public partial class FindBlock : Form
    {
        private class CoordinateGroup
        {
            public int? Region;
            public int? Chunk;
            public int? Block;
            public int? LocalChunk;
            public int? LocalBlock;

            public WatermarkTextBox RegionBox;
            public WatermarkTextBox ChunkBox;
            public WatermarkTextBox BlockBox;
            public WatermarkTextBox LocalChunkBox;
            public WatermarkTextBox LocalBlockBox;
        }

        private bool _inUpdate;

        private CoordinateGroup _groupX;
        private CoordinateGroup _groupZ;

        private DataNode _searchRoot;
        private DataNode _searchResult;

        public FindBlock (DataNode searchRoot)
        {
            InitializeComponent();

            _searchRoot = searchRoot;

            _groupX = new CoordinateGroup() {
                RegionBox = _regionXTextBox,
                ChunkBox = _chunkXTextBox,
                BlockBox = _blockXTextBox,
                LocalChunkBox = _localChunkXTextBox,
                LocalBlockBox = _localBlockXTextBox,
            };

            _groupZ = new CoordinateGroup() {
                RegionBox = _regionZTextBox,
                ChunkBox = _chunkZTextBox,
                BlockBox = _blockZTextBox,
                LocalChunkBox = _localChunkZTextBox,
                LocalBlockBox = _localBlockZTextBox,
            };

            ApplyRegion(_groupX, "0", true);
            ApplyRegion(_groupZ, "0", true);

            Validate();
        }

        public DataNode Result
        {
            get { return _searchResult; }
        }

        private DataNode Search (DataNode node)
        {
            if (node is DirectoryDataNode) {
                DirectoryDataNode dirNode = node as DirectoryDataNode;
                if (!dirNode.IsExpanded)
                    dirNode.Expand();

                foreach (var subNode in dirNode.Nodes) {
                    DataNode resultNode = Search(subNode);
                    if (resultNode != null)
                        return resultNode;
                }

                return null;
            }
            else if (node is RegionFileDataNode) {
                RegionFileDataNode regionNode = node as RegionFileDataNode;

                int rx, rz;
                if (!RegionFileDataNode.RegionCoordinates(regionNode.NodePathName, out rx, out rz))
                    return null;
                if (rx != _groupX.Region.Value || rz != _groupZ.Region.Value)
                    return null;

                if (!regionNode.IsExpanded)
                    regionNode.Expand();

                foreach (var subNode in regionNode.Nodes) {
                    DataNode resultNode = Search(subNode);
                    if (resultNode != null)
                        return resultNode;
                }

                return null;
            }
            else if (node is RegionChunkDataNode) {
                RegionChunkDataNode chunkNode = node as RegionChunkDataNode;
                if (chunkNode.X != _groupX.LocalChunk.Value || chunkNode.Z != _groupZ.LocalChunk.Value)
                    return null;

                return chunkNode;
            }

            return null;
        }

        private int Mod (int a, int b)
        {
            return ((a % b) + b) % b;
        }

        // Block Set

        private string RegionFromBlock (int block)
        {
            return ((block >> 4) >> 5).ToString();
        }

        private string ChunkFromBlock (int block)
        {
            return (block >> 4).ToString();
        }

        private string LocalChunkFromBlock (int block)
        {
            return Mod(block >> 4, 32).ToString();
        }

        private string LocalBlockFromBlock (int block)
        {
            return Mod(block, 16).ToString();
        }

        // Chunk Set

        private string RegionFromChunk (int chunk)
        {
            return (chunk >> 5).ToString();
        }

        private string BlockFromChunk (int chunk)
        {
            int start = chunk * 16;
            int end = (chunk + 1) * 16 - 1;

            return string.Format("({0} to {1})", start, end);
        }

        private string BlockFromChunk (int chunk, int localBlock)
        {
            return (chunk * 16 + localBlock).ToString();
        }

        private string LocalChunkFromChunk (int chunk)
        {
            return Mod(chunk, 32).ToString();
        }

        private string LocalBlockFromChunk (int chunk)
        {
            return "(0 to 15)";
        }
        
        // Region Set

        private string ChunkFromRegion (int region)
        {
            int start = region * 32;
            int end = (region + 1) * 32 - 1;

            return string.Format("({0} to {1})", start, end);
        }

        private string BlockFromRegion (int region)
        {
            int start = region * 32 * 16;
            int end = (region + 1) * 32 * 16 - 1;

            return string.Format("({0} to {1})", start, end);
        }

        private string LocalChunkFromRegion (int region)
        {
            return "(0 to 31)";
        }

        private string LocalBlockFromRegion (int region)
        {
            return "(0 to 15)";
        }

        // Local Chunk Set

        private string RegionFromLocalChunk (int localChunk)
        {
            return "(ANY)";
        }

        private string ChunkFromLocalChunk (int localChunk)
        {
            return "(ANY)";
        }

        private string ChunkFromLocalChunk (int region, int localChunk)
        {
            return (region * 32 + localChunk).ToString();
        }

        private string BlockFromLocalChunk (int localChunk)
        {
            return "(ANY)";
        }

        private string BlockFromLocalChunk (int region, int localChunk)
        {
            return BlockFromChunk(region * 32 + localChunk);
        }

        private string LocalBlockFromLocalChunk (int localChunk)
        {
            return "(0 to 15)";
        }

        private string LocalBlockFromLocalChunk (int region, int localChunk)
        {
            return "(0 to 15)";
        }

        // Local Block Set

        private string RegionFromLocalBlock (int localBlock)
        {
            return "(ANY)";
        }

        private string ChunkFromLocalBlock (int localBlock)
        {
            return "(ANY)";
        }

        private string ChunkFromLocalBlock (int region, int localChunk, int localBlock)
        {
            return (region * 32 + localChunk).ToString();
        }

        private string BlockFromLocalBlock (int localBlock)
        {
            return "(ANY)";
        }

        private string BlockFromLocalBlock (int region, int localChunk, int localBlock)
        {
            return (region * 32 * 16 + localChunk * 16 + localBlock).ToString();
        }

        private int? ParseInt (string value)
        {
            int parsedValue;
            if (int.TryParse(value, out parsedValue))
                return parsedValue;
            else
                return null;
        }

        private void ApplyValueToTextBox (WatermarkTextBox textBox, string value, bool primary)
        {
            if (primary)
                textBox.ApplyText(value);
            else
                textBox.ApplyWatermark(value);
        }

        private void ApplyRegion (CoordinateGroup group, string value, bool primary)
        {
            group.Region = ParseInt(value);
            ApplyValueToTextBox(group.RegionBox, value, primary);

            if (primary && group.Region.HasValue) {
                if (group.LocalChunk.HasValue && !group.LocalChunkBox.WatermarkActive) {
                    ApplyChunk(group, ChunkFromLocalChunk(group.Region.Value, group.LocalChunk.Value), false);
                    if (group.LocalBlock.HasValue && !group.LocalBlockBox.WatermarkActive)
                        ApplyBlock(group, BlockFromLocalBlock(group.Region.Value, group.LocalChunk.Value, group.LocalBlock.Value), false);
                    else {
                        ApplyBlock(group, BlockFromLocalChunk(group.Region.Value, group.LocalChunk.Value), false);
                        ApplyLocalBlock(group, LocalBlockFromLocalChunk(group.Region.Value, group.LocalChunk.Value), false);
                    }
                }
                else {
                    ApplyChunk(group, ChunkFromRegion(group.Region.Value), false);
                    ApplyBlock(group, BlockFromRegion(group.Region.Value), false);
                    ApplyLocalChunk(group, LocalChunkFromRegion(group.Region.Value), false);
                    ApplyLocalBlock(group, LocalBlockFromRegion(group.Region.Value), false);
                }
            }
        }

        private void ApplyChunk (CoordinateGroup group, string value, bool primary)
        {
            group.Chunk = ParseInt(value);
            ApplyValueToTextBox(group.ChunkBox, value, primary);

            if (primary && group.Chunk.HasValue) {
                ApplyRegion(group, RegionFromChunk(group.Chunk.Value), false);
                ApplyLocalChunk(group, LocalChunkFromChunk(group.Chunk.Value), false);
                if (group.LocalBlock.HasValue && !group.LocalBlockBox.WatermarkActive)
                    ApplyBlock(group, BlockFromChunk(group.Chunk.Value, group.LocalBlock.Value), false);
                else {
                    ApplyBlock(group, BlockFromChunk(group.Chunk.Value), false);
                    ApplyLocalBlock(group, LocalBlockFromChunk(group.Chunk.Value), false);
                }
            }
        }

        private void ApplyBlock (CoordinateGroup group, string value, bool primary)
        {
            group.Block = ParseInt(value);
            ApplyValueToTextBox(group.BlockBox, value, primary);

            if (primary && group.Block.HasValue) {
                ApplyRegion(group, RegionFromBlock(group.Block.Value), false);
                ApplyChunk(group, ChunkFromBlock(group.Block.Value), false);
                ApplyLocalChunk(group, LocalChunkFromBlock(group.Block.Value), false);
                ApplyLocalBlock(group, LocalBlockFromBlock(group.Block.Value), false);
            }
        }

        private void ApplyLocalChunk (CoordinateGroup group, string value, bool primary)
        {
            group.LocalChunk = ParseInt(value);
            ApplyValueToTextBox(group.LocalChunkBox, value, primary);

            if (primary && group.LocalChunk.HasValue) {
                if (group.Region.HasValue) {
                    ApplyChunk(group, ChunkFromLocalChunk(group.Region.Value, group.LocalChunk.Value), false);
                    if (group.LocalBlock.HasValue && !group.LocalBlockBox.WatermarkActive)
                        ApplyBlock(group, BlockFromLocalBlock(group.Region.Value, group.LocalChunk.Value, group.LocalBlock.Value), false);
                    else {
                        ApplyBlock(group, BlockFromLocalChunk(group.Region.Value, group.LocalChunk.Value), false);
                        ApplyLocalBlock(group, LocalBlockFromLocalChunk(group.Region.Value, group.LocalChunk.Value), false);
                    }                    
                }
                else {
                    ApplyRegion(group, RegionFromLocalChunk(group.LocalChunk.Value), false);
                    ApplyChunk(group, ChunkFromLocalChunk(group.LocalChunk.Value), false);
                    ApplyBlock(group, BlockFromLocalChunk(group.LocalChunk.Value), false);
                    ApplyLocalBlock(group, LocalBlockFromLocalChunk(group.LocalChunk.Value), false);
                }
            }
        }

        private void ApplyLocalBlock (CoordinateGroup group, string value, bool primary)
        {
            group.LocalBlock = ParseInt(value);
            ApplyValueToTextBox(group.LocalBlockBox, value, primary);

            if (primary && group.LocalBlock.HasValue) {
                if (group.Region.HasValue && group.LocalChunk.HasValue && !group.LocalChunkBox.WatermarkActive) {
                    ApplyChunk(group, ChunkFromLocalBlock(group.Region.Value, group.LocalChunk.Value, group.LocalBlock.Value), false);
                    ApplyBlock(group, BlockFromLocalBlock(group.Region.Value, group.LocalChunk.Value, group.LocalBlock.Value), false);
                }
                else {
                    ApplyRegion(group, RegionFromLocalBlock(group.LocalBlock.Value), false);
                    ApplyChunk(group, ChunkFromLocalBlock(group.LocalBlock.Value), false);
                    ApplyBlock(group, BlockFromLocalBlock(group.LocalBlock.Value), false);
                }
            }
        }

        private void Validate ()
        {
            if (_groupX.LocalChunk.HasValue && _groupZ.LocalChunk.HasValue)
                _findButton.Enabled = true;
            else
                _findButton.Enabled = false;
        }

        private void _regionXTextBox_TextChanged (object sender, EventArgs e)
        {
            int region = 0;
            if (_inUpdate || !int.TryParse(_regionXTextBox.Text, out region))
                return;

            _inUpdate = true;
            ApplyRegion(_groupX, _regionXTextBox.Text, true);
            _inUpdate = false;

            Validate();
        }

        private void _regionZTextBox_TextChanged (object sender, EventArgs e)
        {
            int region = 0;
            if (_inUpdate || !int.TryParse(_regionZTextBox.Text, out region))
                return;

            _inUpdate = true;
            ApplyRegion(_groupZ, _regionZTextBox.Text, true);
            _inUpdate = false;

            Validate();
        }

        private void _chunkXTextBox_TextChanged (object sender, EventArgs e)
        {
            int chunk = 0;
            if (_inUpdate || !int.TryParse(_chunkXTextBox.Text, out chunk))
                return;

            _inUpdate = true;
            ApplyChunk(_groupX, _chunkXTextBox.Text, true);
            _inUpdate = false;

            Validate();
        }

        private void _chunkZTextBox_TextChanged (object sender, EventArgs e)
        {
            int chunk = 0;
            if (_inUpdate || !int.TryParse(_chunkZTextBox.Text, out chunk))
                return;

            _inUpdate = true;
            ApplyChunk(_groupZ, _chunkZTextBox.Text, true);
            _inUpdate = false;

            Validate();
        }

        private void _blockXTextBox_TextChanged (object sender, EventArgs e)
        {
            int block = 0;
            if (_inUpdate || !int.TryParse(_blockXTextBox.Text, out block))
                return;

            _inUpdate = true;
            ApplyBlock(_groupX, _blockXTextBox.Text, true);
            _inUpdate = false;

            Validate();
        }

        private void _blockZTextBox_TextChanged (object sender, EventArgs e)
        {
            int block = 0;
            if (_inUpdate || !int.TryParse(_blockZTextBox.Text, out block))
                return;

            _inUpdate = true;
            ApplyBlock(_groupZ, _blockZTextBox.Text, true);
            _inUpdate = false;

            Validate();
        }

        private void _localChunkXTextBox_TextChanged (object sender, EventArgs e)
        {
            int localChunk = 0;
            if (_inUpdate || !int.TryParse(_localChunkXTextBox.Text, out localChunk))
                return;

            _inUpdate = true;
            ApplyLocalChunk(_groupX, _localChunkXTextBox.Text, true);
            _inUpdate = false;

            Validate();
        }

        private void _localChunkZTextBox_TextChanged (object sender, EventArgs e)
        {
            int localChunk = 0;
            if (_inUpdate || !int.TryParse(_localChunkZTextBox.Text, out localChunk))
                return;

            _inUpdate = true;
            ApplyLocalChunk(_groupZ, _localChunkZTextBox.Text, true);
            _inUpdate = false;

            Validate();
        }

        private void _localBlockXTextBox_TextChanged (object sender, EventArgs e)
        {
            int localBlock = 0;
            if (_inUpdate || !int.TryParse(_localBlockXTextBox.Text, out localBlock))
                return;

            _inUpdate = true;
            ApplyLocalBlock(_groupX, _localBlockXTextBox.Text, true);
            _inUpdate = false;

            Validate();
        }

        private void _localBlockZTextBox_TextChanged (object sender, EventArgs e)
        {
            int localBlock = 0;
            if (_inUpdate || !int.TryParse(_localBlockZTextBox.Text, out localBlock))
                return;

            _inUpdate = true;
            ApplyLocalBlock(_groupZ, _localBlockZTextBox.Text, true);
            _inUpdate = false;

            Validate();
        }

        private void _findButton_Click (object sender, EventArgs e)
        {
            _searchResult = Search(_searchRoot);

            DialogResult = DialogResult.OK;
            Close();
        }

        private void _cancelButton_Click (object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
