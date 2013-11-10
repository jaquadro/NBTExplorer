
using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;

namespace NBTExplorer.Mac
{
	public partial class FindWindowController : MonoMac.AppKit.NSWindowController
	{
		#region Constructors
		
		// Called when created from unmanaged code
		public FindWindowController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public FindWindowController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		
		// Call to load from the XIB/NIB file
		public FindWindowController () : base ("FindWindow")
		{
			Initialize ();
		}
		
		// Shared initialization code
		void Initialize ()
		{
		}
		
		#endregion
		
		//strongly typed window accessor
		public new FindWindow Window
		{
			get {
				return (FindWindow)base.Window;
			}
		}

		public bool MatchName
		{
			get { return _nameToggle.State == NSCellStateValue.On; }
		}
		
		public bool MatchValue
		{
			get { return _valueToggle.State == NSCellStateValue.On; }
		}
		
		public string NameToken
		{
			get { return _nameField.StringValue; }
		}
		
		public string ValueToken
		{
			get { return _valueField.StringValue; }
		}

		partial void ActionFind (NSObject sender)
		{
			NSApplication.SharedApplication.StopModalWithCode((int)ModalResult.OK);
		}

		partial void ActionCancel (NSObject sender)
		{
			NSApplication.SharedApplication.StopModalWithCode((int)ModalResult.Cancel);
		}
	}
}

