// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoMac.Foundation;

namespace NBTExplorer.Mac
{
	[Register ("EditValueWindowController")]
	partial class EditValueWindowController
	{
		[Outlet]
		MonoMac.AppKit.NSTextField _valueField { get; set; }

		[Action ("ActionOK:")]
		partial void ActionOK (MonoMac.Foundation.NSObject sender);

		[Action ("ActionCancel:")]
		partial void ActionCancel (MonoMac.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (_valueField != null) {
				_valueField.Dispose ();
				_valueField = null;
			}
		}
	}

	[Register ("EditValueWindow")]
	partial class EditValueWindow
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
