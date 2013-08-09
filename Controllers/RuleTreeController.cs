using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using NBTExplorer.Windows;
using NBTExplorer.Model.Search;
using Substrate.Nbt;
using NBTExplorer.Windows.Search;
using System.Drawing;

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
            _iconRegistry.DefaultIcon = 15;

            _iconRegistry.Register(typeof(RootRule), 16);
            _iconRegistry.Register(typeof(UnionRule), 19);
            _iconRegistry.Register(typeof(IntersectRule), 18);
            _iconRegistry.Register(typeof(WildcardRule), 17);
            _iconRegistry.Register(typeof(ByteTagRule), 0);
            _iconRegistry.Register(typeof(ShortTagRule), 1);
            _iconRegistry.Register(typeof(IntTagRule), 2);
            _iconRegistry.Register(typeof(LongTagRule), 3);
            _iconRegistry.Register(typeof(FloatTagRule), 4);
            _iconRegistry.Register(typeof(DoubleTagRule), 5);
            _iconRegistry.Register(typeof(StringTagRule), 7);
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

        public void DeleteSelection ()
        {
            DeleteNode(SelectedNode);
        }

        public void DeleteNode (TreeNode node)
        {
            if (node == null || !(node.Tag is SearchRule))
                return;

            SearchRule dataNode = node.Tag as SearchRule;
            //if (!dataNode.CanDeleteNode)
            //    return;

            /*if (dataNode.DeleteNode()) {
                UpdateNodeText(node.Parent);
                node.Remove();

                RemoveNodeFromSelection(node);
                OnSelectionInvalidated();
            }*/
        }

        private TreeNode SelectedNode
        {
            get
            {
                return _nodeTree.SelectedNode;
            }
        }

        private TreeNode CreateIntegralNode<T, K> (string typeName)
            where K : TagNode
            where T : IntegralTagRule<K>, new()
        {
            T rule = new T();

            using (ValueRuleForm form = new ValueRuleForm(SearchRule.NumericOpStrings) {
                Text = "Edit " + typeName + " Tag Rule",
            }) {
                if (form.ShowDialog() == DialogResult.OK) {
                    rule.Name = form.TagName;
                    rule.Value = form.TagValueAsLong;
                    rule.Operator = form.Operator;
                }
                else
                    return null;
            }

            TreeNode node = CreateNode(rule);
            node.Text = rule.NodeDisplay;

            return node;
        }

        private TreeNode CreateFloatNode<T, K> (string typeName)
            where K : TagNode
            where T : FloatTagRule<K>, new()
        {
            T rule = new T();

            using (ValueRuleForm form = new ValueRuleForm(SearchRule.NumericOpStrings) {
                Text = "Edit " + typeName + " Tag Rule",
            }) {
                if (form.ShowDialog() == DialogResult.OK) {
                    rule.Name = form.TagName;
                    rule.Value = form.TagValueAsDouble;
                    rule.Operator = form.Operator;
                }
                else
                    return null;
            }

            TreeNode node = CreateNode(rule);
            node.Text = rule.NodeDisplay;

            return node;
        }

        private TreeNode CreateStringNode (string typeName)
        {
            StringTagRule rule = new StringTagRule();

            using (StringRuleForm form = new StringRuleForm(SearchRule.StringOpStrings) {
                Text = "Edit " + typeName + " Tag Rule",
            }) {
                if (form.ShowDialog() == DialogResult.OK) {
                    rule.Name = form.TagName;
                    rule.Value = form.TagValue;
                    rule.Operator = form.Operator;
                }
                else
                    return null;
            }

            TreeNode node = CreateNode(rule);
            node.Text = rule.NodeDisplay;

            return node;
        }

        private TreeNode CreateWildcardNode (string typeName)
        {
            WildcardRule rule = new WildcardRule();

            using (WildcardRuleForm form = new WildcardRuleForm(SearchRule.WildcardOpStrings) {
                Text = "Edit " + typeName + " Rule",
            }) {
                if (form.ShowDialog() == DialogResult.OK) {
                    rule.Name = form.TagName;
                    rule.Value = form.TagValue;
                    rule.Operator = form.Operator;
                }
                else
                    return null;
            }

            TreeNode node = CreateNode(rule);
            node.Text = rule.NodeDisplay;

            return node;
        }

        public void CreateNode (TreeNode node, TagType type)
        {
            if (node == null || !(node.Tag is GroupRule))
                return;

            GroupRule dataNode = node.Tag as GroupRule;
            TreeNode newNode = null;

            switch (type) {
                case TagType.TAG_BYTE:
                    newNode = CreateIntegralNode<ByteTagRule, TagNodeByte>("Byte");
                    break;
                case TagType.TAG_SHORT:
                    newNode = CreateIntegralNode<ShortTagRule, TagNodeShort>("Short");
                    break;
                case TagType.TAG_INT:
                    newNode = CreateIntegralNode<IntTagRule, TagNodeInt>("Int");
                    break;
                case TagType.TAG_LONG:
                    newNode = CreateIntegralNode<LongTagRule, TagNodeLong>("Long");
                    break;
                case TagType.TAG_FLOAT:
                    newNode = CreateFloatNode<FloatTagRule, TagNodeFloat>("Float");
                    break;
                case TagType.TAG_DOUBLE:
                    newNode = CreateFloatNode<DoubleTagRule, TagNodeDouble>("Double");
                    break;
                case TagType.TAG_STRING:
                    newNode = CreateStringNode("String");
                    break;
            }

            if (newNode != null) {
                node.Nodes.Add(newNode);
                dataNode.Rules.Add(newNode.Tag as SearchRule);

                node.Expand();
            }

            /*if (dataNode.CreateNode(type)) {
                node.Text = dataNode.NodeDisplay;
                RefreshChildNodes(node, dataNode);
                OnSelectionInvalidated();
            }*/
        }

        public void CreateWildcardNode (TreeNode node)
        {
            if (node == null || !(node.Tag is GroupRule))
                return;

            GroupRule dataNode = node.Tag as GroupRule;

            TreeNode newNode = CreateWildcardNode("Wildcard");

            if (newNode != null) {
                node.Nodes.Add(newNode);
                dataNode.Rules.Add(newNode.Tag as SearchRule);

                node.Expand();
            }
        }

        public void CreateWildcardNode ()
        {
            CreateWildcardNode(SelectedNode);
        }

        public void CreateUnionNode (TreeNode node)
        {
            if (node == null || !(node.Tag is GroupRule))
                return;

            GroupRule dataNode = node.Tag as GroupRule;

            TreeNode newNode = CreateNode(new UnionRule());
            node.Nodes.Add(newNode);
            dataNode.Rules.Add(newNode.Tag as SearchRule);

            node.Expand();
        }

        public void CreateUnionNode ()
        {
            CreateUnionNode(SelectedNode);
        }

        public void CreateIntersectNode (TreeNode node)
        {
            if (node == null || !(node.Tag is GroupRule))
                return;

            GroupRule dataNode = node.Tag as GroupRule;

            TreeNode newNode = CreateNode(new IntersectRule());
            node.Nodes.Add(newNode);
            dataNode.Rules.Add(newNode.Tag as SearchRule);

            node.Expand();
        }

        public void CreateIntersectNode ()
        {
            CreateIntersectNode(SelectedNode);
        }

        public void CreateNode (TagType type)
        {
            if (_nodeTree.SelectedNode == null)
                return;

            CreateNode(_nodeTree.SelectedNode, type);
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

            _nodeTree.ExpandAll();
        }
    }
}
