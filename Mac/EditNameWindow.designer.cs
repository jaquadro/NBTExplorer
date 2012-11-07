// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoMac.Foundation;

namespace NBTExplorer.Mac
{
	[Register ("EditNameWindowController")]
	partial class EditNameWindowController
	{
		[Outlet]
		MonoMac.AppKit.NSTextField _textField { get; set; }

		[Action ("OkayAction:")]
		partial void OkayAction (MonoMac.Foundation.NSObject sender);

		[Action ("CancelAction:")]
		partial void CancelAction (MonoMac.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (_textField != null) {
				_textField.Dispose ();
				_textField = null;
			}
		}
	}

	[Register ("EditNameWindow")]
	partial class EditNameWindow
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
