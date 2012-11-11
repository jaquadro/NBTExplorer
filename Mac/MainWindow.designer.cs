// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoMac.Foundation;

namespace NBTExplorer
{
	[Register ("MainWindow")]
	partial class MainWindow
	{
		[Outlet]
		MonoMac.AppKit.NSToolbar _toolbar { get; set; }

		[Outlet]
		MonoMac.AppKit.NSToolbarItem _toolbarOpenFolder { get; set; }

		[Outlet]
		MonoMac.AppKit.NSToolbarItem _toolbarSave { get; set; }

		[Outlet]
		MonoMac.AppKit.NSToolbarItem _toolbarRename { get; set; }

		[Outlet]
		MonoMac.AppKit.NSToolbarItem _toolbarEdit { get; set; }

		[Outlet]
		MonoMac.AppKit.NSToolbarItem _toolbarDelete { get; set; }

		[Outlet]
		MonoMac.AppKit.NSToolbarItem _toolbarByte { get; set; }

		[Outlet]
		MonoMac.AppKit.NSToolbarItem _toolbarShort { get; set; }

		[Outlet]
		MonoMac.AppKit.NSToolbarItem _toolbarInt { get; set; }

		[Outlet]
		MonoMac.AppKit.NSToolbarItem _toolbarLong { get; set; }

		[Outlet]
		MonoMac.AppKit.NSToolbarItem _toolbarFloat { get; set; }

		[Outlet]
		MonoMac.AppKit.NSToolbarItem _toolbarDouble { get; set; }

		[Outlet]
		MonoMac.AppKit.NSToolbarItem _toolbarByteArray { get; set; }

		[Outlet]
		MonoMac.AppKit.NSToolbarItem _toolbarIntArray { get; set; }

		[Outlet]
		MonoMac.AppKit.NSToolbarItem _toolbarString { get; set; }

		[Outlet]
		MonoMac.AppKit.NSToolbarItem _toolbarList { get; set; }

		[Outlet]
		MonoMac.AppKit.NSToolbarItem _toolbarCompound { get; set; }

		[Outlet]
		MonoMac.AppKit.NSScrollView _mainScrollView { get; set; }

		[Outlet]
		MonoMac.AppKit.NSOutlineView _mainOutlineView { get; set; }

		[Action ("ActionOpenFolder:")]
		partial void ActionOpenFolder (MonoMac.Foundation.NSObject sender);

		[Action ("ActionSave:")]
		partial void ActionSave (MonoMac.Foundation.NSObject sender);

		[Action ("ActionRename:")]
		partial void ActionRename (MonoMac.Foundation.NSObject sender);

		[Action ("ActionEdit:")]
		partial void ActionEdit (MonoMac.Foundation.NSObject sender);

		[Action ("ActionDelete:")]
		partial void ActionDelete (MonoMac.Foundation.NSObject sender);

		[Action ("ActionInsertByte:")]
		partial void ActionInsertByte (MonoMac.Foundation.NSObject sender);

		[Action ("ActionInsertShort:")]
		partial void ActionInsertShort (MonoMac.Foundation.NSObject sender);

		[Action ("ActionInsertInt:")]
		partial void ActionInsertInt (MonoMac.Foundation.NSObject sender);

		[Action ("ActionInsertLong:")]
		partial void ActionInsertLong (MonoMac.Foundation.NSObject sender);

		[Action ("ActionInsertFloat:")]
		partial void ActionInsertFloat (MonoMac.Foundation.NSObject sender);

		[Action ("ActionInsertDouble:")]
		partial void ActionInsertDouble (MonoMac.Foundation.NSObject sender);

		[Action ("ActionInsertByteArray:")]
		partial void ActionInsertByteArray (MonoMac.Foundation.NSObject sender);

		[Action ("ActionInsertIntArray:")]
		partial void ActionInsertIntArray (MonoMac.Foundation.NSObject sender);

		[Action ("ActionInsertString:")]
		partial void ActionInsertString (MonoMac.Foundation.NSObject sender);

		[Action ("ActionInsertList:")]
		partial void ActionInsertList (MonoMac.Foundation.NSObject sender);

		[Action ("ActionInsertCompound:")]
		partial void ActionInsertCompound (MonoMac.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (_toolbar != null) {
				_toolbar.Dispose ();
				_toolbar = null;
			}

			if (_toolbarOpenFolder != null) {
				_toolbarOpenFolder.Dispose ();
				_toolbarOpenFolder = null;
			}

			if (_toolbarSave != null) {
				_toolbarSave.Dispose ();
				_toolbarSave = null;
			}

			if (_toolbarRename != null) {
				_toolbarRename.Dispose ();
				_toolbarRename = null;
			}

			if (_toolbarEdit != null) {
				_toolbarEdit.Dispose ();
				_toolbarEdit = null;
			}

			if (_toolbarDelete != null) {
				_toolbarDelete.Dispose ();
				_toolbarDelete = null;
			}

			if (_toolbarByte != null) {
				_toolbarByte.Dispose ();
				_toolbarByte = null;
			}

			if (_toolbarShort != null) {
				_toolbarShort.Dispose ();
				_toolbarShort = null;
			}

			if (_toolbarInt != null) {
				_toolbarInt.Dispose ();
				_toolbarInt = null;
			}

			if (_toolbarLong != null) {
				_toolbarLong.Dispose ();
				_toolbarLong = null;
			}

			if (_toolbarFloat != null) {
				_toolbarFloat.Dispose ();
				_toolbarFloat = null;
			}

			if (_toolbarDouble != null) {
				_toolbarDouble.Dispose ();
				_toolbarDouble = null;
			}

			if (_toolbarByteArray != null) {
				_toolbarByteArray.Dispose ();
				_toolbarByteArray = null;
			}

			if (_toolbarIntArray != null) {
				_toolbarIntArray.Dispose ();
				_toolbarIntArray = null;
			}

			if (_toolbarString != null) {
				_toolbarString.Dispose ();
				_toolbarString = null;
			}

			if (_toolbarList != null) {
				_toolbarList.Dispose ();
				_toolbarList = null;
			}

			if (_toolbarCompound != null) {
				_toolbarCompound.Dispose ();
				_toolbarCompound = null;
			}

			if (_mainScrollView != null) {
				_mainScrollView.Dispose ();
				_mainScrollView = null;
			}

			if (_mainOutlineView != null) {
				_mainOutlineView.Dispose ();
				_mainOutlineView = null;
			}
		}
	}

	[Register ("MainWindowController")]
	partial class MainWindowController
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
