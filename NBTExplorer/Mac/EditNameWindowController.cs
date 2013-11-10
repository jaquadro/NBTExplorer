
using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;

namespace NBTExplorer.Mac
{
	public partial class EditNameWindowController : NSWindowController
	{
		private string _originalName;
		private string _name;

		private List<string> _invalidNames = new List<string>();

		#region Constructors
		
		// Called when created from unmanaged code
		public EditNameWindowController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public EditNameWindowController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		
		// Call to load from the XIB/NIB file
		public EditNameWindowController () : base ("EditNameWindow")
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
			_textField.StringValue = _name;
		}
		
		#endregion

		public string OriginalName
		{
			get { return _originalName; }
			set {
				_originalName = value;
				TagName = value;
			}
		}

		public string TagName
		{
			get { return _name; }
			set {
				_name = value;
				if (_textField != null)
					_textField.StringValue = _name;
			}
		}

		public List<string> InvalidNames
		{
			get { return _invalidNames; }
		}
		
		public bool IsModified
		{
			get { return _name != _originalName; }
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
			return ValidateNameInput();
		}
		
		private bool ValidateNameInput ()
		{
			string text = _textField.StringValue.Trim();
			
			if (text != _originalName && _invalidNames.Contains(text)) {
				NSAlert.WithMessage("Duplicate name provided.", "OK", null, null, "Duplicate name provided.").RunModal();
				return false;
			}

			_name = _textField.StringValue.Trim();
			return true;
		}

		partial void OkayAction (MonoMac.Foundation.NSObject sender)
		{
			Apply ();
		}

		partial void CancelAction (MonoMac.Foundation.NSObject sender)
		{
			NSApplication.SharedApplication.StopModalWithCode((int)ModalResult.Cancel);
		}
		
		//strongly typed window accessor
		public new EditNameWindow Window
		{
			get {
				return (EditNameWindow)base.Window;
			}
		}
	}
}

