using Substrate.Nbt;
using System;
using System.Collections.Generic;

namespace NBTModel.Interop
{
    public static class FormRegistry
    {
        public delegate bool CreateNodeAction(CreateTagFormData data);

        public delegate bool EditByteArrayAction(ByteArrayFormData data);

        public delegate bool EditRestrictedStringAction(RestrictedStringFormData data);

        public delegate bool EditStringAction(StringFormData data);

        public delegate bool EditTagScalarAction(TagScalarFormData data);

        public static EditStringAction EditString { get; set; }
        public static EditRestrictedStringAction RenameTag { get; set; }
        public static EditTagScalarAction EditTagScalar { get; set; }
        public static EditByteArrayAction EditByteArray { get; set; }
        public static CreateNodeAction CreateNode { get; set; }

        public static Action<string> MessageBox { get; set; }
    }

    public class TagScalarFormData
    {
        public TagScalarFormData(TagNode tag)
        {
            Tag = tag;
        }

        public TagNode Tag { get; }
    }

    public class StringFormData
    {
        public StringFormData(string value)
        {
            Value = value;
        }

        public string Value { get; set; }
        public bool AllowEmpty { get; set; }
    }

    public class RestrictedStringFormData : StringFormData
    {
        public RestrictedStringFormData(string value)
            : base(value)
        {
        }

        public List<string> RestrictedValues { get; } = new List<string>();
    }

    public class CreateTagFormData
    {
        public CreateTagFormData()
        {
            RestrictedNames = new List<string>();
        }

        public TagType TagType { get; set; }
        public bool HasName { get; set; }
        public List<string> RestrictedNames { get; }

        public TagNode TagNode { get; set; }
        public string TagName { get; set; }
    }

    public class ByteArrayFormData
    {
        public string NodeName { get; set; }
        public int BytesPerElement { get; set; }
        public byte[] Data { get; set; }
    }
}