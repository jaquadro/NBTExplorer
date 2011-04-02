using System;
using System.Collections.Generic;
using System.Text;

namespace NBToolkit.Map.NBT
{
    public delegate void MissingTagHandler (Object o, TagEventArgs e);
    public delegate void InvalidTagTypeHandler (Object o, TagEventArgs e);
    public delegate void InvalidTagValueHandler (Object o, TagEventArgs e);

    public interface INBTVerifier
    {
        event MissingTagHandler MissingTag;
        event InvalidTagTypeHandler InvalidTagType;
        event InvalidTagValueHandler InvalidTagValue;

        bool Verify ();
    }

    public class TagEventArgs : EventArgs
    {
        protected string _tagName;
        protected NBT_Value _tag;
        protected NBTSchemaNode _schema;

        public string TagName
        {
            get { return _tagName; }
        }

        public TagEventArgs (string tagName)
            : base()
        {
            _tagName = tagName;
        }

        public TagEventArgs (string tagName, NBT_Value tag)
            : base()
        {
            _tag = tag;
            _tagName = tagName;
        }

        public TagEventArgs (NBTSchemaNode schema, NBT_Value tag)
            : base()
        {
            _tag = tag;
            _schema = schema;
        }
    }

    public class NBTVerifier : INBTVerifier
    {
        private NBT_Value _root;
        private NBTSchemaNode _schema;

        public event MissingTagHandler MissingTag;
        public event InvalidTagTypeHandler InvalidTagType;
        public event InvalidTagValueHandler InvalidTagValue;

        public NBTVerifier () { }

        public NBTVerifier (NBT_Value root, NBTSchemaNode schema)
        {
            _root = root;
            _schema = schema;
        }

        public bool Verify ()
        {
            return Verify(_root, _schema);
        }

        static NBTCompoundNode inventorySchema = new NBTCompoundNode("")
        {
            new NBTScalerNode("id", NBT_Type.TAG_SHORT),
            new NBTScalerNode("Damage", NBT_Type.TAG_SHORT),
            new NBTScalerNode("Count", NBT_Type.TAG_BYTE),
            new NBTScalerNode("Slot", NBT_Type.TAG_BYTE),
        };

        private bool Verify (NBT_Value tag, NBTSchemaNode schema)
        {
            if (tag == null) {
                OnMissingTag(new TagEventArgs(schema.Name));
                return false;
            }

            NBTScalerNode scaler = schema as NBTScalerNode;
            if (scaler != null) {
                return VerifyScaler(tag, scaler);
            }

            NBTStringNode str = schema as NBTStringNode;
            if (str != null) {
                return VerifyString(tag, str);
            }

            NBTArrayNode array = schema as NBTArrayNode;
            if (array != null) {
                return VerifyArray(tag, array);
            }

            NBTListNode list = schema as NBTListNode;
            if (list != null) {
                return VerifyList(tag, list);
            }

            NBTCompoundNode compound = schema as NBTCompoundNode;
            if (compound != null) {
                return VerifyCompound(tag, compound);
            }

            return false;
        }

        private bool VerifyScaler (NBT_Value tag, NBTScalerNode schema)
        {
            if (tag.GetNBTType() != schema.Type) {
                OnInvalidTagType(new TagEventArgs(schema.Name, tag));
                return false;
            }

            return true;
        }

        private bool VerifyString (NBT_Value tag, NBTStringNode schema)
        {
            NBT_String stag = tag as NBT_String;
            if (stag == null) {
                OnInvalidTagType(new TagEventArgs(schema, tag));
                return false;
            }
            if (schema.Length > 0 && stag.Length > schema.Length) {
                OnInvalidTagValue(new TagEventArgs(schema, tag));
                return false;
            }
            if (schema.Name != null && stag.Data != schema.Value) {
                OnInvalidTagValue(new TagEventArgs(schema, tag));
                return false;
            }

            return true;
        }


        private bool VerifyArray (NBT_Value tag, NBTArrayNode schema)
        {
            NBT_ByteArray atag = tag as NBT_ByteArray;
            if (atag == null) {
                OnInvalidTagType(new TagEventArgs(schema, tag));
                return false;
            }
            if (schema.Length > 0 && atag.Length != schema.Length) {
                OnInvalidTagValue(new TagEventArgs(schema, tag));
                return false;
            }

            return true;
        }

        private bool VerifyList (NBT_Value tag, NBTListNode schema)
        {
            NBT_List ltag = tag as NBT_List;
            if (ltag == null) {
                OnInvalidTagType(new TagEventArgs(schema, tag));
                return false;
            }
            if (ltag.Count > 0 && ltag.ValueType != schema.Type) {
                OnInvalidTagValue(new TagEventArgs(schema, tag));
                return false;
            }
            if (schema.Length > 0 && ltag.Count != schema.Length) {
                OnInvalidTagValue(new TagEventArgs(schema, tag));
                return false;
            }

            bool pass = true;

            // If a subschema is set, test all items in list against it

            if (schema.SubSchema != null) {
                foreach (NBT_Value v in ltag) {
                    pass = Verify(v, schema.SubSchema) && pass;
                }
            }

            return pass;
        }

        private bool VerifyCompound (NBT_Value tag, NBTCompoundNode schema)
        {
            NBT_Compound ctag = tag as NBT_Compound;
            if (ctag == null) {
                OnInvalidTagType(new TagEventArgs(schema, tag));
                return false;
            }

            bool pass = true;

            foreach (NBTSchemaNode node in schema) {
                NBT_Value value;
                ctag.TryGetValue(node.Name, out value);

                pass = Verify(value, node) && pass;
            }

            return pass;
        }

        #region Event Handlers

        protected void OnMissingTag (TagEventArgs e)
        {
            if (MissingTag != null) {
                MissingTag(this, e);
            }
        }

        protected void OnInvalidTagType (TagEventArgs e)
        {
            if (InvalidTagType != null) {
                InvalidTagType(this, e);
            }
        }

        protected void OnInvalidTagValue (TagEventArgs e)
        {
            if (InvalidTagValue != null) {
                InvalidTagValue(this, e);
            }
        }

        #endregion
    }
}
