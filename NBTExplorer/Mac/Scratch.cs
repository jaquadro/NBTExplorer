using System;

namespace NBTExplorer.Mac.Test
{
	public partial class MainWindow : MonoMac.AppKit.NSWindow
	{
		// ...



		[Outlet]
		MonoMac.AppKit.NSOutlineView _mainOutlineView { get; set; }
		
		private TreeDataSource _dataSource;
		
		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();
			
			_dataSource = new TreeDataSource();
			_mainOutlineView.DataSource = _dataSource;
			_mainOutlineView.Delegate = new MyDelegate(this);
		}
		
		public class MyDelegate : NSOutlineViewDelegate
		{
			private MainWindow _main;
			
			public MyDelegate (MainWindow main)
			{
				_main = main;
			}
			
			public override void ItemWillExpand (NSNotification notification)
			{
				TreeDataNode node = notification.UserInfo ["NSObject"] as TreeDataNode;
				if (node != null) {
					Console.WriteLine ("Preparing to expand: " + node.Data.NodeDisplay);
					_main.ExpandNode(node);
				}
			}
			
			public override void ItemDidExpand (NSNotification notification)
			{
				TreeDataNode node = notification.UserInfo ["NSObject"] as TreeDataNode;
				if (node != null) {
					Console.WriteLine("Finished Expanding: " + node.Data.NodeDisplay);
				}
			}
			
			public override void ItemWillCollapse (NSNotification notification)
			{
				TreeDataNode node = notification.UserInfo ["NSObject"] as TreeDataNode;
				if (node != null) {
					if (node.Data.NodeDisplay == "saves") // The root node
						Console.WriteLine ("Uh-oh...");
					Console.WriteLine("Preparing to collapse: " + node.Data.NodeDisplay);
				}
			}
		}
		
		// ...
		
		private void ExpandNode (TreeDataNode node)
		{
			if (node == null || node.IsExpanded)
				return;
			
			Console.WriteLine ("Expand Node: " + node.Data.NodeDisplay);
			
			node.IsExpanded = true;
			node.Nodes.Clear ();
			
			DataNode backNode = node.Data;
			if (!backNode.IsExpanded) {
				backNode.Expand ();
			}
			
			foreach (DataNode child in backNode.Nodes) {
				if (child != null) {
					node.Nodes.Add (new TreeDataNode (child));
				}
			}
		}
	}

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
			if (nodeItem != null)
				return nodeItem.HasChildren;
			
			return false;
		}
	}

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

