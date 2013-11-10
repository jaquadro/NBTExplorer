// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoMac.Foundation;

namespace NBTExplorer.Mac
{
	[Register ("FindWindowController")]
	partial class FindWindowController
	{
		[Outlet]
		MonoMac.AppKit.NSButton _nameToggle { get; set; }

		[Outlet]
		MonoMac.AppKit.NSButton _valueToggle { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField _nameField { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField _valueField { get; set; }

		[Action ("ActionFind:")]
		partial void ActionFind (MonoMac.Foundation.NSObject sender);

		[Action ("ActionCancel:")]
		partial void ActionCancel (MonoMac.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (_nameToggle != null) {
				_nameToggle.Dispose ();
				_nameToggle = null;
			}

			if (_valueToggle != null) {
				_valueToggle.Dispose ();
				_valueToggle = null;
			}

			if (_nameField != null) {
				_nameField.Dispose ();
				_nameField = null;
			}

			if (_valueField != null) {
				_valueField.Dispose ();
				_valueField = null;
			}
		}
	}

	[Register ("FindWindow")]
	partial class FindWindow
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
