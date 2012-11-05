using System;
using MonoMac.Foundation;
using MonoMac.AppKit;
using NBTExplorer.Model;
using System.Collections.Generic;

namespace NBTExplorer.Mac
{
	public class TreeDataNode : NSObject
	{
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

		public bool HasChildren
		{
			get { return _children.Count > 0 || _dataNode.HasUnexpandedChildren; }
		}

		public List<TreeDataNode> Nodes 
		{
			get { return _children; }
		}
	}
}

