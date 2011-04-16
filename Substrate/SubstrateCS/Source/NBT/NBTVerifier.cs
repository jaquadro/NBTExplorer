using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.NBT
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
        protected TagValue _tag;
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

        public TagEventArgs (string tagName, TagValue tag)
            : base()
        {
            _tag = tag;
            _tagName = tagName;
        }

        public TagEventArgs (NBTSchemaNode schema, TagValue tag)
            : base()
        {
            _tag = tag;
            _schema = schema;
        }
    }

    public class NBTVerifier : INBTVerifier
    {
        private TagValue _root;
        private NBTSchemaNode _schema;

        public event MissingTagHandler MissingTag;
        public event InvalidTagTypeHandler InvalidTagType;
        public event InvalidTagValueHandler InvalidTagValue;

        private Dictionary<string, TagValue> _scratch = new Dictionary<string,TagValue>();

        public NBTVerifier () { }

        public NBTVerifier (TagValue root, NBTSchemaNode schema)
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
            new NBTScalerNode("id", TagType.TAG_SHORT),
            new NBTScalerNode("Damage", TagType.TAG_SHORT),
            new NBTScalerNode("Count", TagType.TAG_BYTE),
            new NBTScalerNode("Slot", TagType.TAG_BYTE),
        };

        private bool Verify (TagValue tag, NBTSchemaNode schema)
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

        private bool VerifyScaler (TagValue tag, NBTScalerNode schema)
        {
            if (!tag.IsCastableTo(schema.Type)) {
                OnInvalidTagType(new TagEventArgs(schema.Name, tag));
                return false;
            }

            return true;
        }

        private bool VerifyString (TagValue tag, NBTStringNode schema)
        {
            TagString stag = tag as TagString;
            if (stag == null) {
                OnInvalidTagType(new TagEventArgs(schema, tag));
                return false;
            }
            if (schema.Length > 0 && stag.Length > schema.Length) {
                OnInvalidTagValue(new TagEventArgs(schema, tag));
                return false;
            }
            if (schema.Value != null && stag.Data != schema.Value) {
                OnInvalidTagValue(new TagEventArgs(schema, tag));
                return false;
            }

            return true;
        }


        private bool VerifyArray (TagValue tag, NBTArrayNode schema)
        {
            TagByteArray atag = tag as TagByteArray;
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

        private bool VerifyList (TagValue tag, NBTListNode schema)
        {
            TagList ltag = tag as TagList;
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

            // Patch up empty lists
            //if (schema.Length == 0) {
            //    tag = new NBT_List(schema.Type);
            //}

            bool pass = true;

            // If a subschema is set, test all items in list against it

            if (schema.SubSchema != null) {
                foreach (TagValue v in ltag) {
                    pass = Verify(v, schema.SubSchema) && pass;
                }
            }

            return pass;
        }

        private bool VerifyCompound (TagValue tag, NBTCompoundNode schema)
        {
            TagCompound ctag = tag as TagCompound;
            if (ctag == null) {
                OnInvalidTagType(new TagEventArgs(schema, tag));
                return false;
            }

            bool pass = true;

            foreach (NBTSchemaNode node in schema) {
                TagValue value;
                ctag.TryGetValue(node.Name, out value);

                if (value == null) {
                    if ((node.Options & NBTOptions.CREATE_ON_MISSING) == NBTOptions.CREATE_ON_MISSING) {
                        _scratch[node.Name] = node.BuildDefaultTree();
                        continue;
                    }
                    else if ((node.Options & NBTOptions.OPTIONAL) == NBTOptions.OPTIONAL) {
                        continue;
                    }
                }

                pass = Verify(value, node) && pass;
            }

            foreach (KeyValuePair<string, TagValue> item in _scratch) {
                ctag[item.Key] = item.Value;
            }

            _scratch.Clear();

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
