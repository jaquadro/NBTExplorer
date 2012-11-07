// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoMac.Foundation;

namespace NBTExplorer.Mac
{
	[Register ("EditStringWindowController")]
	partial class EditStringWindowController
	{
		[Outlet]
		MonoMac.AppKit.NSTextView _textView { get; set; }

		[Action ("ActionOK:")]
		partial void ActionOK (MonoMac.Foundation.NSObject sender);

		[Action ("ActionCancel:")]
		partial void ActionCancel (MonoMac.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (_textView != null) {
				_textView.Dispose ();
				_textView = null;
			}
		}
	}

	[Register ("EditStringWindow")]
	partial class EditStringWindow
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
