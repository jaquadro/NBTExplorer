// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoMac.Foundation;

namespace NBTExplorer.Mac
{
	[Register ("CreateNodeWindowController")]
	partial class CreateNodeWindowController
	{
		[Outlet]
		MonoMac.AppKit.NSTextField _nameFieldLabel { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField _sizeFieldLabel { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField _nameField { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField _sizeField { get; set; }

		[Action ("ActionOK:")]
		partial void ActionOK (MonoMac.Foundation.NSObject sender);

		[Action ("ActionCancel:")]
		partial void ActionCancel (MonoMac.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (_nameFieldLabel != null) {
				_nameFieldLabel.Dispose ();
				_nameFieldLabel = null;
			}

			if (_sizeFieldLabel != null) {
				_sizeFieldLabel.Dispose ();
				_sizeFieldLabel = null;
			}

			if (_nameField != null) {
				_nameField.Dispose ();
				_nameField = null;
			}

			if (_sizeField != null) {
				_sizeField.Dispose ();
				_sizeField = null;
			}
		}
	}

	[Register ("CreateNodeWindow")]
	partial class CreateNodeWindow
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
