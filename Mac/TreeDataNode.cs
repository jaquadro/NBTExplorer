using System;
using MonoMac.Foundation;
using MonoMac.AppKit;
using NBTExplorer.Model;
using System.Collections.Generic;

namespace NBTExplorer.Mac
{
	public class TreeDataNode : NSObject
	{
		private TreeDataNode _parent;
		private DataNode _dataNode;
		private List<TreeDataNode> _children;
		private bool _expanded;

		public TreeDataNode (DataNode node)
		{
			_dataNode = node;
			_children = new List<TreeDataNode>();
		}

		public DataNode Data
		{
			get { return _dataNode; }
		}

		public string CombinedName 
		{
			get { return _dataNode.NodeDisplay; }
		}

		public string Name 
		{
			get { return _dataNode.NodeName; }
		}

		public bool IsExpanded
		{
			get { return _expanded; }
			set { _expanded = value; }
		}

		public void Remove ()
		{
			if (_parent != null)
				_parent.RemoveNode(this);
		}

		public TreeDataNode Parent
		{
			get { return _parent; }
		}

		public bool HasChildren
		{
			get { return _children.Count > 0 || _dataNode.Nodes.Count > 0 || _dataNode.HasUnexpandedChildren; }
		}

		public void AddNode (TreeDataNode node)
		{
			node._parent = this;
			_children.Add(node);
		}

		public void RemoveNode (TreeDataNode node)
		{
			if (_children.Contains (node)) {
				_children.Remove(node);
				node._parent = null;
			}
		}

		public List<TreeDataNode> Nodes 
		{
			get { return _children; }
		}
	}
}

