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
		MonoMac.AppKit.NSScrollView _mainScrollView { get; set; }

		[Outlet]
		MonoMac.AppKit.NSOutlineView _mainOutlineView { get; set; }

		[Action ("ActionOpenFolder:")]
		partial void ActionOpenFolder (MonoMac.Foundation.NSObject sender);

		[Action ("ActionSave:")]
		partial void ActionSave (MonoMac.Foundation.NSObject sender);
		
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
