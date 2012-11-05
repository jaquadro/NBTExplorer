using System;
using System.Collections.Generic;
using Substrate.Nbt;

namespace NBTExplorer
{
    public static class FormRegistry
    {
        public delegate bool EditStringAction (StringFormData data);
        public delegate bool EditRestrictedStringAction (RestrictedStringFormData data);
        public delegate bool EditTagScalarAction (TagScalarFormData data);
        public delegate bool EditByteArrayAction (ByteArrayFormData data);

        public static EditStringAction EditString { get; set; }
        public static EditRestrictedStringAction RenameTag { get; set; }
        public static EditTagScalarAction EditTagScalar { get; set; }
        public static EditByteArrayAction EditByteArray { get; set; }
    }

    public class TagScalarFormData
    {
        public TagScalarFormData (TagNode tag)
        {
            Tag = tag;
        }
        
        public TagNode Tag { get; private set; }
    }

    public class StringFormData
    {
        public StringFormData (String value)
        {
            Value = value;
        }

        public String Value { get; set; }
    }

    public class RestrictedStringFormData : StringFormData
    {
        private List<String> _restricted = new List<string>();

        public RestrictedStringFormData (String value)
            : base(value)
        {
        }

        public List<String> RestrictedValues
        {
            get { return _restricted; }
        }
    }

    public class ByteArrayFormData
    {
        public string NodeName { get; set; }
        public int BytesPerElement { get; set; }
        public byte[] Data { get; set; }
    }
}