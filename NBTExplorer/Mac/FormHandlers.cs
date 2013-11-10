using System;
using MonoMac.AppKit;

namespace NBTExplorer.Mac
{
	public static class FormHandlers
	{
		public static void Register ()
		{
			FormRegistry.EditByteArray = EditByteArrayHandler;
			FormRegistry.EditString = EditStringHandler;
			FormRegistry.EditTagScalar = EditTagScalarValueHandler;
			FormRegistry.RenameTag = RenameTagHandler;
			FormRegistry.CreateNode = CreateNodeHandler;
			
			FormRegistry.MessageBox = MessageBoxHandler;
		}

		private static ModalResult RunWindow (NSWindowController controller)
		{
			//NSApplication.SharedApplication.BeginSheet (controller.Window, NSApplication.SharedApplication.MainWindow);
			int response = NSApplication.SharedApplication.RunModalForWindow (controller.Window);
			
			//NSApplication.SharedApplication.EndSheet(controller.Window);
			controller.Window.Close();
			controller.Window.OrderOut(null);

			if (!Enum.IsDefined(typeof(ModalResult), response))
				response = 0;

			return (ModalResult)response;
		}
		
		public static void MessageBoxHandler (string message)
		{
			NSAlert.WithMessage(message, "OK", null, null, "").RunModal();
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
			EditValueWindowController form = new EditValueWindowController () {
				NodeTag = data.Tag,
			};

			if (RunWindow (form) == ModalResult.OK)
				return true;
			else
				return false;
		}
		
		public static bool EditByteArrayHandler (ByteArrayFormData data)
		{
			NSAlert.WithMessage("Not supported.", "OK", null, null, "Array editing is currently not supported in the Mac version of NBTExplorer.").RunModal();
			return false;

			/*HexEditor form = new HexEditor(data.NodeName, data.Data, data.BytesPerElement);
			if (form.ShowDialog() == DialogResult.OK && form.Modified) {
				Array.Copy(form.Data, data.Data, data.Data.Length);
				return true;
			}
			else
				return false;*/
		}
		
		public static bool CreateNodeHandler (CreateTagFormData data)
		{
			CreateNodeWindowController form = new CreateNodeWindowController () {
				TagType = data.TagType,
				HasName = data.HasName,
			};
			form.InvalidNames.AddRange (data.RestrictedNames);

			if (RunWindow (form) == ModalResult.OK) {
				data.TagNode = form.TagNode;
				data.TagName = form.TagName;
				return true;
			}
			else
				return false;
		}
	}
}

