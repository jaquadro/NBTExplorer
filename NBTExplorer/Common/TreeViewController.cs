using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using NBTExplorer.Vendor.MultiSelectTreeView;

namespace NBTExplorer.Common
{
    abstract class TreeViewController
    { 
        public abstract TreeNodeCollectionController Nodes { get; }
        public abstract TreeNodeController SelectedNode { get; set; }
        public abstract List<TreeNodeController> SelectedNodes { get; }

        public abstract TreeNodeController CreateDefaultNode ();
    }

    public abstract class TreeNodeController : IEquatable<TreeNodeController>
    {
        public abstract ContextMenuStrip ContextMenu { get; set; }
        public abstract Image Icon { get; set; }
        public abstract bool IsExpanded { get; }
        public abstract TreeNodeCollectionController Nodes { get; }
        public abstract TreeNodeController Parent { get; }
        public abstract Image SelectedIcon { get; set; }
        public abstract object Tag { get; set; }
        public abstract string Text { get; set; }
        
        public abstract void Collapse ();
        public abstract void EnsureVisible();
        public abstract void Expand ();
        public abstract void ExpandAll ();
        public abstract void Remove ();

        public abstract bool Equals (TreeNodeController other);
    }

    abstract class TreeNodeCollectionController : IEnumerable<TreeNodeController>
    {
        public abstract TreeNodeController this[int index] { get; }
        public abstract int Count { get; }

        public abstract int Add (TreeNodeController node);
        public abstract void Clear ();
        public abstract void Remove (TreeNodeController node);

        public abstract IEnumerator<TreeNodeController> GetEnumerator ();

        public virtual int Add ()
        {
            return Add(new TreeNodeControllerWin());
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
        {
            return GetEnumerator();
        }
    }

    class TreeNodeControllerWin : TreeNodeController
    {
        private TreeNode _node;
        private TreeNodeCollectionControllerWin _collection;

        public TreeNodeControllerWin ()
        {
            _node = new TreeNode();
            _collection = new TreeNodeCollectionControllerWin(_node.Nodes);
        }

        public TreeNodeControllerWin (TreeNode node)
        {
            _node = node;
            _collection = new TreeNodeCollectionControllerWin(_node.Nodes);
        }

        public TreeNode Node
        {
            get { return _node; }
        }

        public override ContextMenuStrip ContextMenu
        {
            get { return _node.ContextMenuStrip; }
            set { _node.ContextMenuStrip = value; }
        }

        public override TreeNodeCollectionController Nodes
        {
            get { return _collection; }
        }

        public override object Tag
        {
            get { return _node.Tag; }
            set { _node.Tag = value; }
        }

        public override string Text
        {
            get { return _node.Text; }
            set { _node.Text = value; }
        }

        public override bool IsExpanded
        {
            get { return _node.IsExpanded; }
        }

        public override Image Icon
        {
            get 
            {
                if (_node.TreeView.ImageList == null)
                    return null;
                if (_node.ImageIndex < 0 || _node.ImageIndex >= _node.TreeView.ImageList.Images.Count)
                    return null;
                return _node.TreeView.ImageList.Images[_node.ImageIndex];
            }

            set
            {
                if (_node.TreeView.ImageList == null)
                    return;
                int index = -1;
                for (int i = 0; i < _node.TreeView.ImageList.Images.Count; i++) {
                    if (_node.TreeView.ImageList.Images[i] == value)
                        index = i;
                }
                if (index != -1)
                    _node.ImageIndex = index;
            }
        }

        public override Image SelectedIcon
        {
            get
            {
                if (_node.TreeView.ImageList == null)
                    return null;
                if (_node.SelectedImageIndex < 0 || _node.SelectedImageIndex >= _node.TreeView.ImageList.Images.Count)
                    return null;
                return _node.TreeView.ImageList.Images[_node.SelectedImageIndex];
            }

            set
            {
                if (_node.TreeView.ImageList == null)
                    return;
                int index = -1;
                for (int i = 0; i < _node.TreeView.ImageList.Images.Count; i++) {
                    if (_node.TreeView.ImageList.Images[i] == value)
                        index = i;
                }
                if (index != -1)
                    _node.SelectedImageIndex = index;
            }
        }

        public override void Collapse ()
        {
            _node.Collapse();
        }

        public override void EnsureVisible ()
        {
            _node.EnsureVisible();
        }

        public override void Expand ()
        {
            _node.Expand();
        }

        public override void ExpandAll ()
        {
            _node.ExpandAll();
        }

        public override void Remove ()
        {
            _node.Remove();
        }

        public override bool Equals (TreeNodeController other)
        {
            TreeNodeControllerWin nodewin = other as TreeNodeControllerWin;
            if (nodewin == null)
                return false;

            return _node.Equals(nodewin._node);
        }
    }

    class TreeNodeCollectionControllerWin : TreeNodeCollectionController
    {
        private TreeNodeCollection _collection;

        public TreeNodeCollectionControllerWin (TreeNodeCollection collection)
        {
            _collection = collection;
        }

        public override TreeNodeController this[int index]
        {
            get { return new TreeNodeControllerWin(_collection[index]); }
        }

        public override int Count
        {
            get { return _collection.Count; }
        }

        public override int Add (TreeNodeController node)
        {
            TreeNodeControllerWin nodewin = node as TreeNodeControllerWin;
            if (nodewin == null)
                throw new ArgumentException("Expected node to be a Windows concrete type");

            return _collection.Add(nodewin.Node);
        }

        public override void Clear ()
        {
            _collection.Clear();
        }

        public override void Remove (TreeNodeController node)
        {
            TreeNodeControllerWin nodewin = node as TreeNodeControllerWin;
            if (nodewin == null)
                throw new ArgumentException("Expected node to be a Windows concrete type");

            _collection.Remove(nodewin.Node);
        }

        public override IEnumerator<TreeNodeController> GetEnumerator ()
        {
            foreach (TreeNode node in _collection)
                yield return new TreeNodeControllerWin(node);
        }

    }

    class TreeViewControllerWin : TreeViewController
    {
        private TreeView _treeView;
        private MultiSelectTreeView _multiView;

        private TreeNodeCollectionController _collection;

        public TreeViewControllerWin (TreeView view)
        {
            _treeView = view;
            _multiView = view as MultiSelectTreeView;

            _collection = new TreeNodeCollectionControllerWin(_treeView.Nodes);
        }

        public override TreeNodeCollectionController Nodes
        {
            get { return _collection; }
        }

        public override TreeNodeController SelectedNode
        {
            get 
            {
                TreeNode selected = (_multiView != null) ? _multiView.SelectedNode : _treeView.SelectedNode;
                if (selected == null)
                    return null;
                return new TreeNodeControllerWin(selected);
            }

            set
            {
                TreeNodeControllerWin nodewin = value as TreeNodeControllerWin;
                if (nodewin == null)
                    throw new ArgumentException("Expected node to be a Windows concrete type");

                if (_multiView != null)
                    _multiView.SelectedNode = nodewin.Node;
                _treeView.SelectedNode = nodewin.Node;
            }
        }

        public override List<TreeNodeController> SelectedNodes
        {
            get { return null; }
        }

        public override TreeNodeController CreateDefaultNode ()
        {
            return new TreeNodeControllerWin();
        }
    }
}
