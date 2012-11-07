
using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;
using NBTExplorer.Mac;
using System.IO;
using NBTExplorer.Model;
using Substrate.Nbt;

namespace NBTExplorer
{
	public partial class MainWindow : MonoMac.AppKit.NSWindow
	{
		#region Constructors
		
		// Called when created from unmanaged code
		public MainWindow (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public MainWindow (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		
		// Shared initialization code
		void Initialize ()
		{
			InitializeIconRegistry();
			FormHandlers.Register();
		}

		private AppDelegate _appDelegate;

		private NBTExplorer.Mac.IconRegistry _iconRegistry;

		private void InitializeIconRegistry ()
		{
			_iconRegistry = new NBTExplorer.Mac.IconRegistry();
			_iconRegistry.DefaultIcon = NSImage.ImageNamed("question-white.png");
			
			_iconRegistry.Register(typeof(TagByteDataNode), NSImage.ImageNamed("document-attribute-b.png"));
			_iconRegistry.Register(typeof(TagShortDataNode), NSImage.ImageNamed("document-attribute-s.png"));
			_iconRegistry.Register(typeof(TagIntDataNode), NSImage.ImageNamed("document-attribute-i.png"));
			_iconRegistry.Register(typeof(TagLongDataNode), NSImage.ImageNamed("document-attribute-l.png"));
			_iconRegistry.Register(typeof(TagFloatDataNode), NSImage.ImageNamed("document-attribute-f.png"));
			_iconRegistry.Register(typeof(TagDoubleDataNode), NSImage.ImageNamed("document-attribute-d.png"));
			_iconRegistry.Register(typeof(TagByteArrayDataNode), NSImage.ImageNamed("edit-code.png"));
			_iconRegistry.Register(typeof(TagStringDataNode), NSImage.ImageNamed("edit-small-caps.png"));
			_iconRegistry.Register(typeof(TagListDataNode), NSImage.ImageNamed("edit-list.png"));
			_iconRegistry.Register(typeof(TagCompoundDataNode), NSImage.ImageNamed("box.png"));
			_iconRegistry.Register(typeof(RegionChunkDataNode), NSImage.ImageNamed("wooden-box.png"));
			_iconRegistry.Register(typeof(DirectoryDataNode), NSImage.ImageNamed("folder-open"));
			_iconRegistry.Register(typeof(RegionFileDataNode), NSImage.ImageNamed("block.png"));
			_iconRegistry.Register(typeof(CubicRegionDataNode), NSImage.ImageNamed("block.png"));
			_iconRegistry.Register(typeof(NbtFileDataNode), NSImage.ImageNamed("wooden-box.png"));
			_iconRegistry.Register(typeof(TagIntArrayDataNode), NSImage.ImageNamed("edit-code-i.png"));
		}

		public AppDelegate AppDelegate
		{
			get { return _appDelegate; }
			set { _appDelegate = value; }
		}
		
		#endregion

		private TreeDataSource _dataSource;

		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();

			_dataSource = new TreeDataSource();
			_mainOutlineView.DataSource = _dataSource;
			_mainOutlineView.Delegate = new MyDelegate(this);

			string[] args = Environment.GetCommandLineArgs();
			if (args.Length > 2) {
				string[] paths = new string[args.Length - 1];
				Array.Copy(args, 1, paths, 0, paths.Length);
				OpenPaths(paths);
			}
			else {
				OpenMinecraftDirectory();
			}
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

			public override void ItemDidCollapse (NSNotification notification)
			{
				TreeDataNode node = notification.UserInfo ["NSObject"] as TreeDataNode;
				if (node != null) {
					_main.CollapseNode(node);
				}
			}

			public override void WillDisplayCell (NSOutlineView outlineView, NSObject cell, NSTableColumn tableColumn, NSObject item)
			{
				ImageAndTextCell c = cell as ImageAndTextCell;
				TreeDataNode node = item as TreeDataNode;

				c.Title = node.CombinedName;
				c.Image = _main._iconRegistry.Lookup(node.Data.GetType());
				//c.StringValue = node.Name;
				//throw new System.NotImplementedException ();
			}
		}

		#region Actions

		partial void ActionOpenFolder (MonoMac.Foundation.NSObject sender)
		{
			OpenFolder ();
		}

		#endregion

		private string _openFolderPath = null;

		private void OpenFolder ()
		{
			NSOpenPanel opanel = new NSOpenPanel ();
			opanel.CanChooseDirectories = true;
			opanel.CanChooseFiles = false;

			if (_openFolderPath != null)
				opanel.DirectoryUrl = new NSUrl (_openFolderPath, true);

			if (opanel.RunModal () == (int)NSPanelButtonType.Ok) {
				_openFolderPath = opanel.DirectoryUrl.AbsoluteString;
				OpenPaths(new string[] { opanel.DirectoryUrl.Path });
			}

			UpdateUI();
		}

		private void OpenMinecraftDirectory ()
		{
			try {
				string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
				path = Path.Combine(path, "Library", "Application Support");
				path = Path.Combine(path, "minecraft", "saves");

				if (!Directory.Exists(path)) {
					path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
				}
				
				OpenPaths(new string[] { path });
			}
			catch (Exception e) {
				//NSAlert.WithMessage("Could not open default Minecraft save directory", "OK", null, null, null).RunModal();
				Console.WriteLine(e.Message);
				
				try {
					OpenPaths(new string[] { Directory.GetCurrentDirectory() });
				}
				catch (Exception) {
					//MessageBox.Show("Could not open current directory, this tool is probably not compatible with your platform.");
					Console.WriteLine(e.Message);
					NSApplication.SharedApplication.Terminate(this);
				}
			}
			
			UpdateUI();
		}

		private void OpenPaths (string[] paths)
		{
			_dataSource.Nodes.Clear ();
			_mainOutlineView.ReloadData ();

			foreach (string path in paths) {
				if (Directory.Exists (path)) {
					DirectoryDataNode node = new DirectoryDataNode (path);
					_dataSource.Nodes.Add (new TreeDataNode (node));

					// AddPathToHistory(Settings.Default.RecentDirectories, path);
				} else if (File.Exists (path)) {
					DataNode node = null;
				}
			}

			if (_dataSource.Nodes.Count > 0) {
				_mainOutlineView.ExpandItem(_dataSource.Nodes[0]);
			}

			_mainOutlineView.ReloadData();

			UpdateUI();
			// UpdateOpenMenu();

			/*_nodeTree.Nodes.Clear();
			
			foreach (string path in paths) {
				if (Directory.Exists(path)) {
					DirectoryDataNode node = new DirectoryDataNode(path);
					_nodeTree.Nodes.Add(CreateUnexpandedNode(node));
					
					AddPathToHistory(Settings.Default.RecentDirectories, path);
				}
				else if (File.Exists(path)) {
					DataNode node = null;
					foreach (var item in FileTypeRegistry.RegisteredTypes) {
						if (item.Value.NamePatternTest(path))
							node = item.Value.NodeCreate(path);
					}
					
					if (node != null) {
						_nodeTree.Nodes.Add(CreateUnexpandedNode(node));
						AddPathToHistory(Settings.Default.RecentFiles, path);
					}
				}
			}
			
			if (_nodeTree.Nodes.Count > 0) {
				_nodeTree.Nodes[0].Expand();
			}
			
			UpdateUI();
			UpdateOpenMenu();*/
		}

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
					node.AddNode (new TreeDataNode (child));
				}
			}
		}

		private void CollapseNode (TreeDataNode node)
		{
			if (node == null || !node.IsExpanded)
				return;

			/*Console.WriteLine("Collapse Node: " + node.Data.NodeDisplay);
			
			DataNode backNode = node.Data;
			if (backNode.IsModified)
				return;
			
			backNode.Collapse();

			node.IsExpanded = false;
			node.Nodes.Clear();*/
		}

		public void ActionEditValue ()
		{
			TreeDataNode node = _mainOutlineView.ItemAtRow(_mainOutlineView.SelectedRow) as TreeDataNode;
			if (node != null)
				EditNode(node);
		}

		public void ActionRenameValue ()
		{
			TreeDataNode node = _mainOutlineView.ItemAtRow(_mainOutlineView.SelectedRow) as TreeDataNode;
			if (node != null)
				RenameNode(node);
		}

		public void ActionDeleteValue ()
		{
			TreeDataNode node = _mainOutlineView.ItemAtRow(_mainOutlineView.SelectedRow) as TreeDataNode;
			if (node != null)
				DeleteNode(node);
		}

		private void EditNode (TreeDataNode node)
		{
			if (node == null)
				return;

			if (!node.Data.CanEditNode)
				return;

			if (node.Data.EditNode()) {
				//node.Text = node.Data.NodeDisplay;
				UpdateUI(node.Data);
			}
		}

		private void RenameNode (TreeDataNode node)
		{
			if (node == null)
				return;

			if (!node.Data.CanRenameNode)
				return;

			if (node.Data.RenameNode()) {
				//node.Text = dataNode.NodeDisplay;
				UpdateUI(node.Data);
			}
		}

		private void DeleteNode (TreeDataNode node)
		{
			if (node == null)
				return;

			if (!node.Data.CanDeleteNode)
				return;

			if (node.Data.DeleteNode()) {
				UpdateUI(node.Parent.Data);
				//UpdateNodeText(node.Parent);
				TreeDataNode parent = node.Parent;
				node.Remove();

				_mainOutlineView.ReloadItem(parent, true);
			}
		}

		private void UpdateUI ()
		{
			if (_appDelegate == null)
				return;

			TreeDataNode selected = _mainOutlineView.ItemAtRow(_mainOutlineView.SelectedRow) as TreeDataNode;
			if (selected != null) {
				UpdateUI(selected.Data);
			}
			else {
				//_appDelegate.MenuSave.Enabled = CheckModifications();
				_appDelegate.MenuFind.Enabled = false;
				//_appDelegate.MenuFindNext.Enabled = _searchState != null;
			}
		}

		private void UpdateUI (DataNode node)
		{
			if (_appDelegate == null || node == null)
				return;

			_appDelegate.MenuInsertByte.Enabled = node.CanCreateTag(TagType.TAG_BYTE);
			_appDelegate.MenuInsertShort.Enabled = node.CanCreateTag(TagType.TAG_SHORT);
			_appDelegate.MenuInsertInt.Enabled = node.CanCreateTag(TagType.TAG_INT);
			_appDelegate.MenuInsertLong.Enabled = node.CanCreateTag(TagType.TAG_LONG);
			_appDelegate.MenuInsertFloat.Enabled = node.CanCreateTag(TagType.TAG_FLOAT);
			_appDelegate.MenuInsertDouble.Enabled = node.CanCreateTag(TagType.TAG_DOUBLE);
			_appDelegate.MenuInsertByteArray.Enabled = node.CanCreateTag(TagType.TAG_BYTE_ARRAY);
			_appDelegate.MenuInsertIntArray.Enabled = node.CanCreateTag(TagType.TAG_INT_ARRAY);
			_appDelegate.MenuInsertString.Enabled = node.CanCreateTag(TagType.TAG_STRING);
			_appDelegate.MenuInsertList.Enabled = node.CanCreateTag(TagType.TAG_LIST);
			_appDelegate.MenuInsertCompound.Enabled = node.CanCreateTag(TagType.TAG_COMPOUND);

			//_appDelegate.MenuSave.Enabled = CheckModifications();
			_appDelegate.MenuCopy.Enabled = node.CanCopyNode;
			_appDelegate.MenuCut.Enabled = node.CanCutNode;
			_appDelegate.MenuPaste.Enabled = node.CanPasteIntoNode;
			_appDelegate.MenuDelete.Enabled = node.CanDeleteNode;
			_appDelegate.MenuEditValue.Enabled = node.CanEditNode;
			_appDelegate.MenuRename.Enabled = node.CanRenameNode;
			_appDelegate.MenuFind.Enabled = node.CanSearchNode;
			//_appDelegate.MenuFindNext.Enabled = _searchState != null;
		}
	}
}

