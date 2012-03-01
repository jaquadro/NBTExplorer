using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace NBTExplorer.ViewModel
{
    public class RegionNode
    {
        readonly TreeNode _node;
        readonly int _descriptionImageIndex;
        public int Index { get { return _node.Index; } }
        public string Text { get { return _node.Text; } }

        public RegionNode(TreeNode node,int descriptionImageIndex)
        {
            Debug.Assert(node.Text.EndsWith(".mcr"));

            _node = node;
            _descriptionImageIndex = descriptionImageIndex;
        }

        public IEnumerable<ChunkNode> FindChunks(Func<ChunkNode,bool> predicate)
        {
            foreach (var chunk in _node.Nodes.Cast<TreeNode>().Where(c => c.Text.StartsWith("Chunk")).Select(tn=> new ChunkNode(tn,_descriptionImageIndex)))
            {
                if (predicate == null || predicate(chunk))
                    yield return  chunk;
            }
        }
    }
}
