
using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;
using Substrate.Nbt;

namespace NBTExplorer.Mac
{
	public partial class EditValueWindowController : MonoMac.AppKit.NSWindowController
	{
		private TagNode _tag;

		#region Constructors
		
		// Called when created from unmanaged code
		public EditValueWindowController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public EditValueWindowController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		
		// Call to load from the XIB/NIB file
		public EditValueWindowController () : base ("EditValueWindow")
		{
			Initialize ();
		}
		
		// Shared initialization code
		void Initialize ()
		{
		}
		
		#endregion
		
		//strongly typed window accessor
		public new EditValueWindow Window
		{
			get {
				return (EditValueWindow)base.Window;
			}
		}

		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();

			if (_tag != null)
				_valueField.StringValue = _tag.ToString();
		}
		
		public TagNode NodeTag
		{
			get { return _tag; }
			set {
				_tag = value;
				if (_valueField != null)
					_valueField.StringValue = _tag.ToString();
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
			return ValidateValueInput();
		}
		
		private bool ValidateValueInput ()
		{
			try {
				switch (_tag.GetTagType()) {
				case TagType.TAG_BYTE:
					_tag.ToTagByte().Data = unchecked((byte)sbyte.Parse(_valueField.StringValue));
					break;
					
				case TagType.TAG_SHORT:
					_tag.ToTagShort().Data = short.Parse(_valueField.StringValue);
					break;
					
				case TagType.TAG_INT:
					_tag.ToTagInt().Data = int.Parse(_valueField.StringValue);
					break;
					
				case TagType.TAG_LONG:
					_tag.ToTagLong().Data = long.Parse(_valueField.StringValue);
					break;
					
				case TagType.TAG_FLOAT:
					_tag.ToTagFloat().Data = float.Parse(_valueField.StringValue);
					break;
					
				case TagType.TAG_DOUBLE:
					_tag.ToTagDouble().Data = double.Parse(_valueField.StringValue);
					break;
					
				case TagType.TAG_STRING:
					_tag.ToTagString().Data = _valueField.StringValue;
					break;
				}
			}
			catch (FormatException) {
				NSAlert.WithMessage("Invalid Format", "OK", null, null, "The value is formatted incorrectly for the given type.").RunModal();
				return false;
			}
			catch (OverflowException) {
				NSAlert.WithMessage("Invalid Range", "OK", null, null, "The value is outside the acceptable range for the given type.").RunModal();
				return false;
			}
			catch {
				return false;
			}
			
			return true;
		}

		partial void ActionOK (MonoMac.Foundation.NSObject sender)
		{
			Apply ();
		}

		partial void ActionCancel (MonoMac.Foundation.NSObject sender)
		{
			NSApplication.SharedApplication.StopModalWithCode((int)ModalResult.Cancel);
		}
	}
}

