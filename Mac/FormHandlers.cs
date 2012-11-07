using System;
using MonoMac.AppKit;

namespace NBTExplorer.Mac
{
	public static class FormHandlers
	{
		public static void Register ()
		{
			//FormRegistry.EditByteArray = EditByteArrayHandler;
			FormRegistry.EditString = EditStringHandler;
			FormRegistry.EditTagScalar = EditTagScalarValueHandler;
			FormRegistry.RenameTag = RenameTagHandler;
			//FormRegistry.CreateNode = CreateNodeHandler;
			
			FormRegistry.MessageBox = MessageBoxHandler;
		}

		private static ModalResult RunWindow (NSWindowController controller)
		{
			NSApplication.SharedApplication.BeginSheet (controller.Window, NSApplication.SharedApplication.MainWindow);
			int response = NSApplication.SharedApplication.RunModalForWindow (controller.Window);
			
			NSApplication.SharedApplication.EndSheet(controller.Window);
			controller.Window.Close();
			controller.Window.OrderOut(null);

			if (!Enum.IsDefined(typeof(ModalResult), response))
				response = 0;

			return (ModalResult)response;
		}
		
		public static void MessageBoxHandler (string message)
		{
			NSAlert.WithMessage(message, "OK", null, null, null).RunModal();
		}
		
		public static bool EditStringHandler (StringFormData data)
		{
			EditStringWindowController form = new EditStringWindowController ();
			form.StringValue = data.Value;

			if (RunWindow (form) == ModalResult.OK) {
				data.Value = form.StringValue;
				return true;
			}
			else
				return false;
		}
		
		public static bool RenameTagHandler (RestrictedStringFormData data)
		{
			EditNameWindowController form = new EditNameWindowController ();
			form.OriginalName = data.Value;
			form.InvalidNames.AddRange (data.RestrictedValues);

			if (RunWindow (form) == ModalResult.OK) {
				data.Value = form.TagName;
				return true;
			}
			else
				return false;
		}
		
		public static bool EditTagScalarValueHandler (TagScalarFormData data)
		{
			EditValue form = new EditValue(data.Tag);

			NSApplication.SharedApplication.BeginSheet(form, NSApplication.SharedApplication.MainWindow);
			NSApplication.SharedApplication.RunModalForWindow(form);
			NSApplication.SharedApplication.EndSheet(form);

			form.Close ();
			form.OrderOut(null);

			//if (form.ShowDialog() == DialogResult.OK)
			//	return true;
			//else
				return false;
		}
		
		/*public static bool EditByteArrayHandler (ByteArrayFormData data)
		{
			HexEditor form = new HexEditor(data.NodeName, data.Data, data.BytesPerElement);
			if (form.ShowDialog() == DialogResult.OK && form.Modified) {
				Array.Copy(form.Data, data.Data, data.Data.Length);
				return true;
			}
			else
				return false;
		}
		
		public static bool CreateNodeHandler (CreateTagFormData data)
		{
			CreateNodeForm form = new CreateNodeForm(data.TagType, data.HasName);
			form.InvalidNames.AddRange(data.RestrictedNames);
			
			if (form.ShowDialog() == DialogResult.OK) {
				data.TagNode = form.TagNode;
				data.TagName = form.TagName;
				return true;
			}
			else
				return false;
		}*/
	}
}

