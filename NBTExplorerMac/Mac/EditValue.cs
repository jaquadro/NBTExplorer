
using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;
using Substrate.Nbt;

namespace NBTExplorer.Mac
{
	public partial class EditValue : MonoMac.AppKit.NSWindow
	{
		public enum DialogResult
		{
			OK,
			Cancel,
		}

		private TagNode _tag;
		private DialogResult _result;

		#region Constructors
		
		// Called when created from unmanaged code
		public EditValue (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public EditValue (NSCoder coder) : base (coder)
		{
			Initialize ();
		}

		public EditValue (TagNode tag)
		{
			Initialize ();

			_tag = tag;

			if (tag == null) {
				_result = DialogResult.Cancel;
				Close();
				return;
			}

			_valueField.StringValue = _tag.ToString();
		}
		
		// Shared initialization code
		void Initialize ()
		{
		}
		
		#endregion

		public TagNode NodeTag
		{
			get { return _tag; }
		}

		public DialogResult Result
		{
			get { return _result; }
		}

		/*private TagNode _tag;
		
		public EditValue (TagNode tag)
		{
			InitializeComponent();
			
			_tag = tag;
			
			if (tag == null) {
				DialogResult = DialogResult.Abort;
				Close();
				return;
			}
			
			textBox1.Text = _tag.ToString();
		}
		
		public TagNode NodeTag
		{
			get { return _tag; }
		}
		
		private void Apply ()
		{
			if (ValidateInput()) {
				DialogResult = DialogResult.OK;
				Close();
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
					_tag.ToTagByte().Data = unchecked((byte)sbyte.Parse(textBox1.Text));
					break;
					
				case TagType.TAG_SHORT:
					_tag.ToTagShort().Data = short.Parse(textBox1.Text);
					break;
					
				case TagType.TAG_INT:
					_tag.ToTagInt().Data = int.Parse(textBox1.Text);
					break;
					
				case TagType.TAG_LONG:
					_tag.ToTagLong().Data = long.Parse(textBox1.Text);
					break;
					
				case TagType.TAG_FLOAT:
					_tag.ToTagFloat().Data = float.Parse(textBox1.Text);
					break;
					
				case TagType.TAG_DOUBLE:
					_tag.ToTagDouble().Data = double.Parse(textBox1.Text);
					break;
					
				case TagType.TAG_STRING:
					_tag.ToTagString().Data = textBox1.Text;
					break;
				}
			}
			catch (FormatException) {
				MessageBox.Show("The value is formatted incorrectly for the given type.");
				return false;
			}
			catch (OverflowException) {
				MessageBox.Show("The value is outside the acceptable range for the given type.");
				return false;
			}
			catch {
				return false;
			}
			
			return true;
		}
		
		private void _buttonOK_Click (object sender, EventArgs e)
		{
			Apply();
		}*/
	}
}

