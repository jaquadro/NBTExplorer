using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using NBTExplorer.Windows;
using NBTExplorer.Model.Search;

namespace NBTExplorer.Controllers
{
    public class RuleTreeController
    {
        private TreeView _nodeTree;
        private IconRegistry _iconRegistry;

        private RootRule _rootData;

        public RuleTreeController (TreeView nodeTree)
        {
            _nodeTree = nodeTree;

            InitializeIconRegistry();
            ShowVirtualRoot = true;

            _rootData = new RootRule();

            RefreshTree();
        }

        private void InitializeIconRegistry ()
        {
            _iconRegistry = new IconRegistry();
            _iconRegistry.DefaultIcon = 4;

            _iconRegistry.Register(typeof(RootRule), 0);
            _iconRegistry.Register(typeof(UnionRule), 1);
            _iconRegistry.Register(typeof(IntersectRule), 2);
            _iconRegistry.Register(typeof(WildcardRule), 3);
        }

        public RootRule Root
        {
            get { return _rootData; }
        }

        public TreeView Tree
        {
            get { return _nodeTree; }
        }

        public bool ShowVirtualRoot { get; set; }

        public string VirtualRootDisplay
        {
            get { return _rootData.NodeDisplay; }
            /*set
            {
                _rootData.SetDisplayName(value);
                if (ShowVirtualRoot && _nodeTree.Nodes.Count > 0)
                    UpdateNodeText(_nodeTree.Nodes[0]);
            }*/
        }

        private TreeNode CreateNode (SearchRule rule)
        {
            TreeNode frontNode = new TreeNode(rule.NodeDisplay);
            frontNode.ImageIndex = _iconRegistry.Lookup(rule.GetType());
            frontNode.SelectedImageIndex = frontNode.ImageIndex;
            frontNode.Tag = rule;

            return frontNode;
        }

        private void ExpandNode (TreeNode node, bool recurse)
        {
            GroupRule rule = node.Tag as GroupRule;
            if (rule == null)
                return;

            foreach (var subRule in rule.Rules) {
                TreeNode subNode = CreateNode(subRule);
                node.Nodes.Add(subNode);

                if (recurse)
                    ExpandNode(subNode, recurse);
            }
        }

        private void RefreshTree ()
        {
            _nodeTree.Nodes.Clear();
            _nodeTree.Nodes.Add(CreateNode(_rootData));

            ExpandNode(_nodeTree.Nodes[0], true);
        }
    }
}
