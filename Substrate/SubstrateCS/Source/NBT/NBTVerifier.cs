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
        protected TagNode _tag;
        protected SchemaNode _schema;

        public string TagName
        {
            get { return _tagName; }
        }

        public TagEventArgs (string tagName)
            : base()
        {
            _tagName = tagName;
        }

        public TagEventArgs (string tagName, TagNode tag)
            : base()
        {
            _tag = tag;
            _tagName = tagName;
        }

        public TagEventArgs (SchemaNode schema, TagNode tag)
            : base()
        {
            _tag = tag;
            _schema = schema;
        }
    }

    public class NBTVerifier : INBTVerifier
    {
        private TagNode _root;
        private SchemaNode _schema;

        public event MissingTagHandler MissingTag;
        public event InvalidTagTypeHandler InvalidTagType;
        public event InvalidTagValueHandler InvalidTagValue;

        private Dictionary<string, TagNode> _scratch = new Dictionary<string,TagNode>();

        public NBTVerifier () { }

        public NBTVerifier (TagNode root, SchemaNode schema)
        {
            _root = root;
            _schema = schema;
        }

        public bool Verify ()
        {
            return Verify(_root, _schema);
        }

        private bool Verify (TagNode tag, SchemaNode schema)
        {
            if (tag == null) {
                OnMissingTag(new TagEventArgs(schema.Name));
                return false;
            }

            SchemaNodeScaler scaler = schema as SchemaNodeScaler;
            if (scaler != null) {
                return VerifyScaler(tag, scaler);
            }

            SchemaNodeString str = schema as SchemaNodeString;
            if (str != null) {
                return VerifyString(tag, str);
            }

            SchemaNodeArray array = schema as SchemaNodeArray;
            if (array != null) {
                return VerifyArray(tag, array);
            }

            SchemaNodeList list = schema as SchemaNodeList;
            if (list != null) {
                return VerifyList(tag, list);
            }

            SchemaNodeCompound compound = schema as SchemaNodeCompound;
            if (compound != null) {
                return VerifyCompound(tag, compound);
            }

            return false;
        }

        private bool VerifyScaler (TagNode tag, SchemaNodeScaler schema)
        {
            if (!tag.IsCastableTo(schema.Type)) {
                OnInvalidTagType(new TagEventArgs(schema.Name, tag));
                return false;
            }

            return true;
        }

        private bool VerifyString (TagNode tag, SchemaNodeString schema)
        {
            TagNodeString stag = tag as TagNodeString;
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


        private bool VerifyArray (TagNode tag, SchemaNodeArray schema)
        {
            TagNodeByteArray atag = tag as TagNodeByteArray;
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

        private bool VerifyList (TagNode tag, SchemaNodeList schema)
        {
            TagNodeList ltag = tag as TagNodeList;
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
                foreach (TagNode v in ltag) {
                    pass = Verify(v, schema.SubSchema) && pass;
                }
            }

            return pass;
        }

        private bool VerifyCompound (TagNode tag, SchemaNodeCompound schema)
        {
            TagNodeCompound ctag = tag as TagNodeCompound;
            if (ctag == null) {
                OnInvalidTagType(new TagEventArgs(schema, tag));
                return false;
            }

            bool pass = true;

            foreach (SchemaNode node in schema) {
                TagNode value;
                ctag.TryGetValue(node.Name, out value);

                if (value == null) {
                    if ((node.Options & SchemaOptions.CREATE_ON_MISSING) == SchemaOptions.CREATE_ON_MISSING) {
                        _scratch[node.Name] = node.BuildDefaultTree();
                        continue;
                    }
                    else if ((node.Options & SchemaOptions.OPTIONAL) == SchemaOptions.OPTIONAL) {
                        continue;
                    }
                }

                pass = Verify(value, node) && pass;
            }

            foreach (KeyValuePair<string, TagNode> item in _scratch) {
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
