
using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;

namespace NBTExplorer.Mac
{
	public partial class EditStringWindowController : NSWindowController
	{
		private string _string;

		#region Constructors
		
		// Called when created from unmanaged code
		public EditStringWindowController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public EditStringWindowController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		
		// Call to load from the XIB/NIB file
		public EditStringWindowController () : base ("EditStringWindow")
		{
			Initialize ();
		}
		
		// Shared initialization code
		void Initialize ()
		{
		}

		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();
			_textView.Value = _string;
		}
		
		#endregion
		
		//strongly typed window accessor
		public new EditStringWindow Window
		{
			get {
				return (EditStringWindow)base.Window;
			}
		}

		public string StringValue
		{
			get { return _string; }
			set {
				_string = value;
				if (_textView != null)
					_textView.Value = _string;
			}
		}

		private void Apply ()
		{
			if (ValidateInput()) {
				NSApplication.SharedApplication.StopModalWithCode((int)ModalResult.OK);
				return;
			}
		}
		
		private bool ValidateInput ()
		{
			return ValidateStringInput();
		}
		
		private bool ValidateStringInput ()
		{
			_string = _textView.Value.Trim();
			return true;
		}

		partial void ActionOK (NSObject sender)
		{
			Apply ();
		}

		partial void ActionCancel (NSObject sender)
		{
			NSApplication.SharedApplication.StopModalWithCode((int)ModalResult.Cancel);
		}
	}
}

