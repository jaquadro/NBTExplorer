
using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;

namespace NBTExplorer.Mac
{
	public partial class CancelFindWindowController : MonoMac.AppKit.NSWindowController
	{
		#region Constructors
		
		// Called when created from unmanaged code
		public CancelFindWindowController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public CancelFindWindowController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		
		// Call to load from the XIB/NIB file
		public CancelFindWindowController () : base ("CancelFindWindow")
		{
			Initialize ();
		}
		
		// Shared initialization code
		void Initialize ()
		{
		}
		
		#endregion
		
		//strongly typed window accessor
		public new CancelFindWindow Window
		{
			get {
				return (CancelFindWindow)base.Window;
			}
		}

		public void Accept ()
		{
			NSApplication.SharedApplication.StopModalWithCode((int)ModalResult.OK);
		}

		public void Cancel ()
		{
			NSApplication.SharedApplication.StopModalWithCode((int)ModalResult.Cancel);
		}

		partial void ActionCancel (MonoMac.Foundation.NSObject sender)
		{
			NSApplication.SharedApplication.StopModalWithCode((int)ModalResult.Cancel);
		}
	}
}

