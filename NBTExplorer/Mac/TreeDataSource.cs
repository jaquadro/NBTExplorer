using System;
using System.Collections.Generic;
using MonoMac.AppKit;
using MonoMac.Foundation;

namespace NBTExplorer.Mac
{
	public class TreeDataSource : NSOutlineViewDataSource
	{
		private List<TreeDataNode> _nodes;

		public TreeDataSource ()
		{
			_nodes = new List<TreeDataNode>();
		}

		public List<TreeDataNode> Nodes
		{
			get { return _nodes; }
		}

		public override int GetChildrenCount (NSOutlineView outlineView, NSObject item)
		{
			if (item is TreeDataNode) {
				TreeDataNode nodeItem = item as TreeDataNode;
				return nodeItem.Nodes.Count;
			}

			return _nodes.Count;
		}

		public override NSObject GetObjectValue (NSOutlineView outlineView, NSTableColumn forTableColumn, NSObject byItem)
		{
			TreeDataNode nodeItem = byItem as TreeDataNode;
			if (nodeItem == null)
				return null;

			return (NSString)nodeItem.CombinedName;
		}

		public override NSObject GetChild (NSOutlineView outlineView, int childIndex, NSObject ofItem)
		{
			TreeDataNode nodeItem = ofItem as TreeDataNode;
			if (nodeItem != null) {
				return nodeItem.Nodes [childIndex];
			}

			return Nodes[childIndex];
		}

		public override bool ItemExpandable (NSOutlineView outlineView, NSObject item)
		{
			TreeDataNode nodeItem = item as TreeDataNode;
			if (nodeItem != null) {
				return nodeItem.HasChildren;
			}

			return false;
		}
	}
}

