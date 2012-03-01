using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Substrate.Nbt;

namespace NBTExplorer.ViewModel
{
    public class ChunkNode
    {
        readonly TreeNode _node;
        readonly int _descriptionImageIndex;
        public int Index { get { return _node.Index; } }
        public string Text { get { return _node.Text; } }

        public TreeNode GetLevelNode()
        {
            if (ServerNode.NeedsExpand(_node))
                ServerNode.ExpandNode(_node, _descriptionImageIndex);
            var level = _node.Nodes.Cast<TreeNode>().FirstOrDefault(n => n.Text.StartsWith("Level"));
            return level;
        }
        public bool HasEntities()
        {
            var count = GetEntityCount();
            return count.HasValue && count.Value > 0;
        }
        public void EnsureEntitiesVisible()
        {
            var entityNode = GetEntityNode();
            if (entityNode != null)
                entityNode.EnsureVisible();
        }
        TreeNode GetEntityNode()
        {
            var level = GetLevelNode();
            if (level == null)
                return null;
            var entityNode = level.Nodes.Cast<TreeNode>().FirstOrDefault(n => n.Text.StartsWith("Entities"));
            return entityNode;
        }

        public int? GetEntityCount()
        {
            var entityNode = GetEntityNode();
            if (entityNode == null)
                return null;
            var text = entityNode.Text.Where(c => char.IsNumber(c)).Select(c => c.ToString()).Aggregate((c1, c2) => c1 + c2);

            var entityCount = int.Parse(text);
            return entityCount;
        }
        public TreeNode Node { get { return _node; } }
        public ChunkNode(TreeNode node,int descriptionImageIndex)
        {
            _node = node;
            _descriptionImageIndex = descriptionImageIndex;
        }
        public TreeNode Parent { get { return _node.Parent; } }
        internal void EnsureVisible()
        {
            _node.EnsureVisible();
        }

        internal void SelectEntityNode()
        {
            var entityNode = GetEntityNode();
            if(entityNode!=null)
            entityNode.TreeView.SelectedNode = entityNode;
        }
    }
}
