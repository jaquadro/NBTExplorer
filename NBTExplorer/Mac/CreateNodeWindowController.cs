
using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;
using Substrate.Nbt;

namespace NBTExplorer.Mac
{
	public partial class CreateNodeWindowController : MonoMac.AppKit.NSWindowController
	{
		private string _name;
		private int _size;
		private TagType _type;
		private TagNode _tag;
		
		private bool _hasName;
		
		private List<string> _invalidNames = new List<string>();

		#region Constructors
		
		// Called when created from unmanaged code
		public CreateNodeWindowController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public CreateNodeWindowController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		
		// Call to load from the XIB/NIB file
		public CreateNodeWindowController () : base ("CreateNodeWindow")
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
			SetNameBoxState();
			SetSizeBoxState();
		}
		
		#endregion
		
		//strongly typed window accessor
		public new CreateNodeWindow Window
		{
			get {
				return (CreateNodeWindow)base.Window;
			}
		}

		private void SetNameBoxState ()
		{
			if (_nameField == null || _nameFieldLabel == null)
				return;

			if (HasName) {
				_nameFieldLabel.Enabled = true;
				_nameFieldLabel.TextColor = NSColor.ControlText;
				_nameField.Enabled = true;
			}
			else {
				_nameFieldLabel.Enabled = false;
				_nameFieldLabel.TextColor = NSColor.DisabledControlText;
				_nameField.Enabled = false;
			}
		}
		
		private void SetSizeBoxState ()
		{
			if (_sizeField == null || _sizeFieldLabel == null)
				return;

			if (IsTagSizedType) {
				_sizeFieldLabel.Enabled = true;
				_sizeFieldLabel.TextColor = NSColor.ControlText;
				_sizeField.Enabled = true;
			}
			else {
				_sizeFieldLabel.Enabled = false;
				_sizeFieldLabel.TextColor = NSColor.DisabledControlText;
				_sizeField.Enabled = false;
			}
		}

		public TagType TagType
		{
			get { return _type; }
			set {
				_type = value;
				SetSizeBoxState();
			}
		}
		
		public string TagName
		{
			get { return _name; }
		}
		
		public TagNode TagNode
		{
			get { return _tag; }
		}
		
		public List<string> InvalidNames
		{
			get { return _invalidNames; }
		}
		
		public bool HasName
		{
			get { return _hasName; }
			set 
			{ 
				_hasName = value;
				SetNameBoxState();
			}
		}
		
		private void Apply ()
		{
			if (ValidateInput()) {
				_tag = CreateTag();
				NSApplication.SharedApplication.StopModalWithCode((int)ModalResult.OK);
				return;
			}
		}
		
		private TagNode CreateTag ()
		{
			switch (_type) {
			case TagType.TAG_BYTE:
				return new TagNodeByte();
			case TagType.TAG_BYTE_ARRAY:
				return new TagNodeByteArray(new byte[_size]);
			case TagType.TAG_COMPOUND:
				return new TagNodeCompound();
			case TagType.TAG_DOUBLE:
				return new TagNodeDouble();
			case TagType.TAG_FLOAT:
				return new TagNodeFloat();
			case TagType.TAG_INT:
				return new TagNodeInt();
			case TagType.TAG_INT_ARRAY:
				return new TagNodeIntArray(new int[_size]);
			case TagType.TAG_LIST:
				return new TagNodeList(TagType.TAG_BYTE);
			case TagType.TAG_LONG:
				return new TagNodeLong();
			case TagType.TAG_SHORT:
				return new TagNodeShort();
			case TagType.TAG_STRING:
				return new TagNodeString();
			default:
				return new TagNodeByte();
			}
		}
		
		private bool ValidateInput ()
		{
			return ValidateNameInput()
				&& ValidateSizeInput();
		}
		
		private bool ValidateNameInput ()
		{
			if (!HasName)
				return true;
			
			string text = _nameField.StringValue.Trim();

			if (_invalidNames.Contains(text)) {
				NSAlert.WithMessage("Duplicate name.", "OK", null, null, "You cannot specify a name already in use by another tag within the same container.").RunModal();
				return false;
			}
			
			_name = _nameField.StringValue.Trim();
			return true;
		}
		
		private bool ValidateSizeInput ()
		{
			if (!IsTagSizedType)
				return true;
			
			if (!Int32.TryParse(_sizeField.StringValue.Trim(), out _size)) {
				NSAlert.WithMessage("Invalid size.", "OK", null, null, "The size field must be a valid integer value.").RunModal();
				return false;
			}
			
			_size = Math.Max(0, _size);
			return true;
		}
		
		private bool IsTagSizedType
		{
			get
			{
				switch (_type) {
				case TagType.TAG_BYTE_ARRAY:
				case TagType.TAG_INT_ARRAY:
					return true;
				default:
					return false;
				}
			}
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

