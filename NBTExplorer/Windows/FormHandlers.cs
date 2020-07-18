using NBTModel.Interop;
using System;
using System.Windows.Forms;

namespace NBTExplorer.Windows
{
    public static class FormHandlers
    {
        public static void Register()
        {
            FormRegistry.EditByteArray = EditByteArrayHandler;
            FormRegistry.EditString = EditStringHandler;
            FormRegistry.EditTagScalar = EditTagScalarValueHandler;
            FormRegistry.RenameTag = RenameTagHandler;
            FormRegistry.CreateNode = CreateNodeHandler;

            FormRegistry.MessageBox = MessageBoxHandler;
        }

        public static void MessageBoxHandler(string message)
        {
            MessageBox.Show(message);
        }

        public static bool EditStringHandler(StringFormData data)
        {
            var form = new EditString(data.Value);
            if (form.ShowDialog() == DialogResult.OK)
            {
                data.Value = form.StringValue;
                return true;
            }
            return false;
        }

        public static bool RenameTagHandler(RestrictedStringFormData data)
        {
            var form = new EditName(data.Value)
            {
                AllowEmpty = data.AllowEmpty
            };
            form.InvalidNames.AddRange(data.RestrictedValues);

            if (form.ShowDialog() == DialogResult.OK && form.IsModified)
            {
                data.Value = form.TagName;
                return true;
            }
            return false;
        }

        public static bool EditTagScalarValueHandler(TagScalarFormData data)
        {
            var form = new EditValue(data.Tag);
            if (form.ShowDialog() == DialogResult.OK)
                return true;
            return false;
        }

        public static bool EditByteArrayHandler(ByteArrayFormData data)
        {
            var form = new HexEditor(data.NodeName, data.Data, data.BytesPerElement);
            if (form.ShowDialog() == DialogResult.OK && form.Modified)
            {
                data.Data = new byte[form.Data.Length];
                Array.Copy(form.Data, data.Data, data.Data.Length);
                return true;
            }
            return false;
        }

        public static bool CreateNodeHandler(CreateTagFormData data)
        {
            var form = new CreateNodeForm(data.TagType, data.HasName);
            form.InvalidNames.AddRange(data.RestrictedNames);

            if (form.ShowDialog() == DialogResult.OK)
            {
                data.TagNode = form.TagNode;
                data.TagName = form.TagName;
                return true;
            }
            return false;
        }
    }
}