
using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;
using NBTExplorer.Mac;
using System.IO;
using NBTExplorer.Model;
using Substrate.Nbt;
using System.Threading;

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
			Delegate = new WindowDelegate(this);
			InitializeIconRegistry();
			FormHandlers.Register();
			NbtClipboardController.Initialize(new NbtClipboardControllerMac());
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
			set 
			{ 
				_appDelegate = value;
				UpdateUI ();
			}
		}
		
		#endregion

		private TreeDataSource _dataSource;

		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();

			NSApplication.SharedApplication.MainMenu.AutoEnablesItems = false;

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

		public class WindowDelegate : NSWindowDelegate
		{
			private MainWindow _main;

			public WindowDelegate (MainWindow main) {
				_main = main;
			}

			public override bool WindowShouldClose (NSObject sender)
			{
				//Settings.Default.RecentFiles = Settings.Default.RecentFiles;
				//Settings.Default.Save();
				return _main.ConfirmExit();
			}
		}

		public class MyDelegate : NSOutlineViewDelegate
		{
			private MainWindow _main;

			public MyDelegate (MainWindow main)
			{
				_main = main;
			}

			public override void SelectionDidChange (NSNotification notification)
			{
				TreeDataNode node = _main.SelectedNode;
				if (node != null)
					_main.UpdateUI(node.Data);
			}

			public override void ItemWillExpand (NSNotification notification)
			{
				TreeDataNode node = notification.UserInfo ["NSObject"] as TreeDataNode;
				if (node != null) {
					//Console.WriteLine ("Preparing to expand: " + node.Data.NodeDisplay);
					_main.ExpandNode(node);
				}
			}

			public override void ItemDidExpand (NSNotification notification)
			{
				TreeDataNode node = notification.UserInfo ["NSObject"] as TreeDataNode;
				if (node != null) {
					//Console.WriteLine("Finished Expanding: " + node.Data.NodeDisplay);
				}
			}

			public override void ItemWillCollapse (NSNotification notification)
			{
				TreeDataNode node = notification.UserInfo ["NSObject"] as TreeDataNode;
				if (node != null) {
					//if (node.Data.NodeDisplay == "saves") // The root node
						//Console.WriteLine ("Uh-oh...");
					//Console.WriteLine("Preparing to collapse: " + node.Data.NodeDisplay);
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

		partial void ActionOpenFolder (NSObject sender)
		{
			OpenFolder ();
		}

		partial void ActionSave (NSObject sender)
		{
			ActionSave ();
		}

		partial void ActionRename (MonoMac.Foundation.NSObject sender)
		{
			ActionRenameValue();
		}

		partial void ActionEdit (MonoMac.Foundation.NSObject sender)
		{
			ActionEditValue();
		}

		partial void ActionDelete (MonoMac.Foundation.NSObject sender)
		{
			ActionDeleteValue();
		}

		partial void ActionInsertByte (MonoMac.Foundation.NSObject sender)
		{
			ActionInsertByteTag();
		}

		partial void ActionInsertShort (MonoMac.Foundation.NSObject sender)
		{
			ActionInsertShortTag();
		}

		partial void ActionInsertInt (MonoMac.Foundation.NSObject sender)
		{
			ActionInsertIntTag();
		}

		partial void ActionInsertLong (MonoMac.Foundation.NSObject sender)
		{
			ActionInsertLongTag();
		}

		partial void ActionInsertFloat (MonoMac.Foundation.NSObject sender)
		{
			ActionInsertFloatTag();
		}

		partial void ActionInsertDouble (MonoMac.Foundation.NSObject sender)
		{
			ActionInsertDoubleTag();
		}

		partial void ActionInsertByteArray (MonoMac.Foundation.NSObject sender)
		{
			ActionInsertByteArrayTag();
		}

		partial void ActionInsertIntArray (MonoMac.Foundation.NSObject sender)
		{
			ActionInsertIntArrayTag();
		}

		partial void ActionInsertString (MonoMac.Foundation.NSObject sender)
		{
			ActionInsertStringTag();
		}

		partial void ActionInsertList (MonoMac.Foundation.NSObject sender)
		{
			ActionInsertListTag();
		}

		partial void ActionInsertCompound (MonoMac.Foundation.NSObject sender)
		{
			ActionInsertCompoundTag();
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

		private void OpenFile ()
		{
			NSOpenPanel opanel = new NSOpenPanel ();
			opanel.CanChooseDirectories = false;
			opanel.CanChooseFiles = true;
			//opanel.AllowsMultipleSelection = true;

			if (opanel.RunModal() == (int)NSPanelButtonType.Ok) {
				List<string> paths = new List<string>();
				foreach (var url in opanel.Urls)
					paths.Add(url.Path);
				OpenPaths(paths);
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
				NSAlert.WithMessage("Operation Failed", "OK", null, null, "Could not open default Minecraft save directory").RunModal();
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

		private void OpenPaths (IEnumerable<string> paths)
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

					foreach (var item in FileTypeRegistry.RegisteredTypes) {
						if (item.Value.NamePatternTest(path))
							node = item.Value.NodeCreate(path);
					}
					
					if (node != null) {
						_dataSource.Nodes.Add(new TreeDataNode(node));
						//AddPathToHistory(Settings.Default.RecentFiles, path);
					}
				}
			}

			if (_dataSource.Nodes.Count > 0) {
				_mainOutlineView.ExpandItem(_dataSource.Nodes[0]);
			}

			_mainOutlineView.ReloadData();

			UpdateUI();
		}

		private void ExpandNode (TreeDataNode node)
		{
			if (node == null || node.IsExpanded)
				return;

			//Console.WriteLine ("Expand Node: " + node.Data.NodeDisplay);

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

			//Console.WriteLine("Collapse Node: " + node.Data.NodeDisplay);
			
			DataNode backNode = node.Data;
			if (backNode.IsModified)
				return;
			
			backNode.Collapse();

			node.IsExpanded = false;
			node.Nodes.Clear();
		}

		private void RefreshChildNodes (TreeDataNode node, DataNode dataNode)
		{
			Dictionary<DataNode, TreeDataNode> currentNodes = new Dictionary<DataNode, TreeDataNode>();
			foreach (TreeDataNode child in node.Nodes) {
				currentNodes.Add(child.Data, child);
			}
			
			node.Nodes.Clear();
			foreach (DataNode child in dataNode.Nodes) {
				if (!currentNodes.ContainsKey(child))
					node.Nodes.Add(new TreeDataNode(child));
				else
					node.Nodes.Add(currentNodes[child]);
			}
			
			//foreach (TreeDataNode child in node.Nodes)
			//	child.ContextMenuStrip = BuildNodeContextMenu(child.Tag as DataNode);
			
			if (node.Nodes.Count == 0 && dataNode.HasUnexpandedChildren) {
				ExpandNode(node);
				_mainOutlineView.ExpandItem(node);
				//node.Expand();
			}

			_mainOutlineView.ReloadItem(node, true);
		}

		private void CreateNode (TreeDataNode node, TagType type)
		{
			if (node == null)
				return;

			if (!node.Data.CanCreateTag(type))
				return;

			if (node.Data.CreateNode(type)) {
				//node.Text = dataNode.NodeDisplay;
				RefreshChildNodes(node, node.Data);
				UpdateUI(node.Data);
			}
		}

		private TreeDataNode SelectedNode
		{
			get { return _mainOutlineView.ItemAtRow (_mainOutlineView.SelectedRow) as TreeDataNode; }
		}

		public void ActionOpen ()
		{
			if (ConfirmOpen())
				OpenFile();
		}

		public void ActionOpenFolder ()
		{
			if (ConfirmOpen ())
				OpenFolder ();
		}

		public void ActionOpenMinecraft ()
		{
			if (ConfirmOpen ())
				OpenMinecraftDirectory();
		}

		public void ActionSave ()
		{
			Save ();
		}

		public void ActionCopy ()
		{
			CopyNode (SelectedNode);
		}

		public void ActionCut ()
		{
			CutNode (SelectedNode);
		}

		public void ActionPaste ()
		{
			PasteNode (SelectedNode);
		}

		public void ActionEditValue ()
		{
			EditNode(SelectedNode);
		}

		public void ActionRenameValue ()
		{
			RenameNode(SelectedNode);
		}

		public void ActionDeleteValue ()
		{
			DeleteNode(SelectedNode);
		}

		public void ActionMoveNodeUp ()
		{
			MoveNodeUp(SelectedNode);
		}

		public void ActionMoveNodeDown ()
		{
			MoveNodeDown (SelectedNode);
		}

		public void ActionFind ()
		{
			SearchNode (SelectedNode);
		}

		public void ActionFindNext ()
		{
			SearchNextNode();
		}

		public void ActionInsertByteTag ()
		{
			CreateNode (SelectedNode, TagType.TAG_BYTE);
		}

		public void ActionInsertShortTag ()
		{
			CreateNode (SelectedNode, TagType.TAG_SHORT);
		}

		public void ActionInsertIntTag ()
		{
			CreateNode (SelectedNode, TagType.TAG_INT);
		}

		public void ActionInsertLongTag ()
		{
			CreateNode (SelectedNode, TagType.TAG_LONG);
		}

		public void ActionInsertFloatTag ()
		{
			CreateNode (SelectedNode, TagType.TAG_FLOAT);
		}

		public void ActionInsertDoubleTag ()
		{
			CreateNode (SelectedNode, TagType.TAG_DOUBLE);
		}

		public void ActionInsertByteArrayTag ()
		{
			CreateNode (SelectedNode, TagType.TAG_BYTE_ARRAY);
		}

		public void ActionInsertIntArrayTag ()
		{
			CreateNode (SelectedNode, TagType.TAG_INT_ARRAY);
		}

		public void ActionInsertStringTag ()
		{
			CreateNode (SelectedNode, TagType.TAG_STRING);
		}

		public void ActionInsertListTag ()
		{
			CreateNode (SelectedNode, TagType.TAG_LIST);
		}

		public void ActionInsertCompoundTag ()
		{
			CreateNode (SelectedNode, TagType.TAG_COMPOUND);
		}

		private void CopyNode (TreeDataNode node)
		{
			if (node == null)
				return;

			if (!node.Data.CanCopyNode)
				return;
			
			node.Data.CopyNode();
		}
		
		private void CutNode (TreeDataNode node)
		{
			if (node == null)
				return;

			if (!node.Data.CanCutNode)
				return;

			if (node.Data.CutNode()) {
				TreeDataNode parent = node.Parent;
				UpdateUI(parent.Data);
				node.Remove();
				_mainOutlineView.ReloadItem(parent, true);
			}
		}
		
		private void PasteNode (TreeDataNode node)
		{
			if (node == null)
				return;

			if (!node.Data.CanPasteIntoNode)
				return;

			if (node.Data.PasteNode()) {
				//node.Text = dataNode.NodeDisplay;
				RefreshChildNodes(node, node.Data);
				UpdateUI(node.Data);
			}
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

		private void MoveNodeUp (TreeDataNode node)
		{
			if (node == null)
				return;

			if (!node.Data.CanMoveNodeUp)
				return;

			node.Data.ChangeRelativePosition(-1);
			RefreshChildNodes(node.Parent, node.Data.Parent);
		}
		
		private void MoveNodeDown (TreeDataNode node)
		{
			if (node == null)
				return;
			
			if (!node.Data.CanMoveNodeDown)
				return;
			
			node.Data.ChangeRelativePosition(1);
			RefreshChildNodes(node.Parent, node.Data.Parent);
		}
		
		private void Save ()
		{
			foreach (TreeDataNode node in _dataSource.Nodes) {
				if (node.Data != null)
					node.Data.Save();
			}
			
			UpdateUI();
		}

		private static ModalResult RunWindow (NSWindowController controller)
		{
			int response = NSApplication.SharedApplication.RunModalForWindow (controller.Window);
			controller.Window.Close();
			controller.Window.OrderOut(null);
			
			if (!Enum.IsDefined(typeof(ModalResult), response))
				response = 0;
			
			return (ModalResult)response;
		}

		private CancelFindWindowController _searchForm;
		private SearchStateMac _searchState;
		
		private void SearchNode (TreeDataNode node)
		{
			if (node == null)
				return;

			if (!node.Data.CanSearchNode)
				return;

			FindWindowController form = new FindWindowController();
			if (RunWindow (form) != ModalResult.OK)
				return;

			_searchState = new SearchStateMac(this) {
				RootNode = node.Data,
				SearchName = form.NameToken,
				SearchValue = form.ValueToken,
				DiscoverCallback = SearchDiscoveryCallback,
				CollapseCallback = SearchCollapseCallback,
				EndCallback = SearchEndCallback,
			};
			
			SearchNextNode();
		}
		
		private void SearchNextNode ()
		{
			if (_searchState == null)
				return;
			
			SearchWorker worker = new SearchWorker (_searchState);
			
			Thread t = new Thread (new ThreadStart (worker.Run));
			t.IsBackground = true;
			t.Start ();

			_searchForm = new CancelFindWindowController ();
			if (RunWindow (_searchForm) == ModalResult.Cancel) {
				worker.Cancel();
				_searchState = null;
			}
			
			t.Join();
		}
		
		private void SearchDiscoveryCallback (DataNode node)
		{
			Console.WriteLine ("Discovery: " + node.NodeDisplay);
			TreeDataNode frontNode = FindFrontNode(node);
			Console.WriteLine ("  Front Node: " + frontNode.Data.NodeDisplay);
			_mainOutlineView.SelectRow (_mainOutlineView.RowForItem(frontNode), false);
			_mainOutlineView.ScrollRowToVisible(_mainOutlineView.RowForItem(frontNode));
			//_nodeTree.SelectedNode = FindFrontNode(node);
			
			if (_searchForm != null) {
				_searchForm.Accept();
				_searchForm = null;
			}
		}
		
		private void SearchCollapseCallback (DataNode node)
		{
			CollapseBelow(node);
		}
		
		private void SearchEndCallback (DataNode node)
		{
			_searchForm.Cancel();
			_searchForm = null;

			NSAlert.WithMessage("End of Results", "OK", null, null, "").RunModal();
		}
		
		private TreeDataNode GetRootFromDataNodePath (DataNode node, out Stack<DataNode> hierarchy)
		{
			hierarchy = new Stack<DataNode>();
			while (node != null) {
				hierarchy.Push(node);
				node = node.Parent;
			}
			
			DataNode rootDataNode = hierarchy.Pop();
			TreeDataNode frontNode = null;
			foreach (TreeDataNode child in _dataSource.Nodes) {
				if (child.Data == rootDataNode)
					frontNode = child;
			}
			
			return frontNode;
		}
		
		private TreeDataNode FindFrontNode (DataNode node)
		{
			Stack<DataNode> hierarchy;
			TreeDataNode frontNode = GetRootFromDataNodePath(node, out hierarchy);
			
			if (frontNode == null)
				return null;
			
			while (hierarchy.Count > 0) {
				if (!frontNode.IsExpanded) {
					_mainOutlineView.ExpandItem(frontNode);
					_mainOutlineView.ReloadItem(frontNode);
				}
				
				DataNode childData = hierarchy.Pop();
				foreach (TreeDataNode childFront in frontNode.Nodes) {
					if (childFront.Data == childData) {
						frontNode = childFront;
						break;
					}
				}
			}
			
			return frontNode;
		}
		
		private void CollapseBelow (DataNode node)
		{
			Stack<DataNode> hierarchy;
			TreeDataNode frontNode = GetRootFromDataNodePath (node, out hierarchy);
			
			if (frontNode == null)
				return;
			
			while (hierarchy.Count > 0) {
				if (!frontNode.IsExpanded)
					return;
				
				DataNode childData = hierarchy.Pop ();
				foreach (TreeDataNode childFront in frontNode.Nodes) {
					if (childFront.Data == childData) {
						frontNode = childFront;
						break;
					}
				}
			}
			
			if (frontNode.IsExpanded) {
				_mainOutlineView.CollapseItem (frontNode);
				frontNode.IsExpanded = false;
			}
		}
		
		private bool ConfirmExit ()
		{
			if (CheckModifications()) {
				int id = NSAlert.WithMessage("Unsaved Changes", "OK", "Cancel", "", "You currently have unsaved changes.  Close anyway?").RunModal();
				if (id != 1)
					return false;
			}
			
			return true;
		}

		private bool ConfirmOpen ()
		{
			if (CheckModifications()) {
				int id = NSAlert.WithMessage("Unsaved Changes", "OK", "Cancel", "", "You currently have unsaved changes.  Open new location anyway?").RunModal();
				if (id != 1)
					return false;
			}
			
			return true;
		}

		private bool CheckModifications ()
		{
			foreach (TreeDataNode node in _dataSource.Nodes) {
				if (node.Data != null && node.Data.IsModified)
					return true;
			}
			
			return false;
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
				UpdateUI(new DataNode());

				_appDelegate.MenuSave.Enabled = CheckModifications();
				_appDelegate.MenuFind.Enabled = false;
				_appDelegate.MenuFindNext.Enabled = _searchState != null;

				_toolbarSave.Enabled = _appDelegate.MenuSave.Enabled;
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

			_appDelegate.MenuSave.Enabled = CheckModifications();
			_appDelegate.MenuCopy.Enabled = node.CanCopyNode && NbtClipboardController.IsInitialized;
			_appDelegate.MenuCut.Enabled = node.CanCutNode && NbtClipboardController.IsInitialized;
			_appDelegate.MenuPaste.Enabled = node.CanPasteIntoNode && NbtClipboardController.IsInitialized;
			_appDelegate.MenuDelete.Enabled = node.CanDeleteNode;
			_appDelegate.MenuEditValue.Enabled = node.CanEditNode;
			_appDelegate.MenuRename.Enabled = node.CanRenameNode;
			_appDelegate.MenuMoveUp.Enabled = node.CanMoveNodeUp;
			_appDelegate.MenuMoveDown.Enabled = node.CanMoveNodeDown;
			_appDelegate.MenuFind.Enabled = node.CanSearchNode;
			_appDelegate.MenuFindNext.Enabled = _searchState != null;

			_toolbarByte.Enabled = _appDelegate.MenuInsertByte.Enabled;
			_toolbarShort.Enabled = _appDelegate.MenuInsertShort.Enabled;
			_toolbarInt.Enabled = _appDelegate.MenuInsertInt.Enabled;
			_toolbarLong.Enabled = _appDelegate.MenuInsertLong.Enabled;
			_toolbarFloat.Enabled = _appDelegate.MenuInsertFloat.Enabled;
			_toolbarDouble.Enabled = _appDelegate.MenuInsertDouble.Enabled;
			_toolbarByteArray.Enabled = _appDelegate.MenuInsertByteArray.Enabled;
			_toolbarIntArray.Enabled = _appDelegate.MenuInsertIntArray.Enabled;
			_toolbarString.Enabled = _appDelegate.MenuInsertString.Enabled;
			_toolbarList.Enabled = _appDelegate.MenuInsertList.Enabled;
			_toolbarCompound.Enabled = _appDelegate.MenuInsertCompound.Enabled;

			_toolbarSave.Enabled = _appDelegate.MenuSave.Enabled;
			_toolbarDelete.Enabled = _appDelegate.MenuDelete.Enabled;
			_toolbarEdit.Enabled = _appDelegate.MenuEditValue.Enabled;
			_toolbarRename.Enabled = _appDelegate.MenuRename.Enabled;
		}

		/*private void UpdateOpenMenu ()
		{
			try {
				if (Settings.Default.RecentDirectories == null)
					Settings.Default.RecentDirectories = new StringCollection();
				if (Settings.Default.RecentFiles == null)
					Settings.Default.RecentFiles = new StringCollection();
			}
			catch {
				return;
			}
			
			_menuItemRecentFolders.DropDown = BuildRecentEntriesDropDown(Settings.Default.RecentDirectories);
			_menuItemRecentFiles.DropDown = BuildRecentEntriesDropDown(Settings.Default.RecentFiles);
		}
		
		private ToolStripDropDown BuildRecentEntriesDropDown (StringCollection list)
		{
			if (list == null || list.Count == 0)
				return new ToolStripDropDown();
			
			ToolStripDropDown menu = new ToolStripDropDown();
			foreach (string entry in list) {
				ToolStripMenuItem item = new ToolStripMenuItem("&" + (menu.Items.Count + 1) + " " + entry);
				item.Tag = entry;
				item.Click += _menuItemRecentPaths_Click;
				
				menu.Items.Add(item);
			}
			
			return menu;
		}
		
		private void AddPathToHistory (StringCollection list, string entry)
		{
			foreach (string item in list) {
				if (item == entry) {
					list.Remove(item);
					break;
				}
			}
			
			while (list.Count >= 5)
				list.RemoveAt(list.Count - 1);
			
			list.Insert(0, entry);
		}*/
	}
}

