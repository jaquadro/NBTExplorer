using System;
using System.Collections.Generic;
using Substrate.Core;

namespace Substrate.Nbt
{
    /// <summary>
    /// Indicates how an <see cref="NbtVerifier"/> event processor should respond to returning event handler.
    /// </summary>
    public enum TagEventCode
    {
        /// <summary>
        /// The event processor should process the next event in the chian.
        /// </summary>
        NEXT,

        /// <summary>
        /// The event processor should ignore the verification failure and stop processing any remaining events.
        /// </summary>
        PASS,

        /// <summary>
        /// The event processor should fail and stop processing any remaining events.
        /// </summary>
        FAIL,
    }

    /// <summary>
    /// Event arguments for <see cref="NbtVerifier"/> failure events.
    /// </summary>
    public class TagEventArgs : EventArgs
    {
        private string _tagName;
        private TagNode _parent;
        private TagNode _tag;
        private SchemaNode _schema;

        /// <summary>
        /// Gets the expected name of the <see cref="TagNode"/> referenced by this event.
        /// </summary>
        public string TagName
        {
            get { return _tagName; }
        }

        /// <summary>
        /// Gets the parent  <see cref="TagNode"/> of the <see cref="TagNode"/> referenced by this event, if it exists.
        /// </summary>
        public TagNode Parent
        {
            get { return _parent; }
        }

        /// <summary>
        /// Gets the <see cref="TagNode"/> referenced by this event.
        /// </summary>
        public TagNode Tag
        {
            get { return _tag; }
        }

        /// <summary>
        /// Gets the <see cref="SchemaNode"/> corresponding to the <see cref="TagNode"/> referenced by this event.
        /// </summary>
        public SchemaNode Schema
        {
            get { return _schema; }
        }

        /// <summary>
        /// Constructs a new event argument set.
        /// </summary>
        /// <param name="tagName">The expected name of a <see cref="TagNode"/>.</param>
        public TagEventArgs (string tagName)
            : base()
        {
            _tagName = tagName;
        }

        /// <summary>
        /// Constructs a new event argument set.
        /// </summary>
        /// <param name="tagName">The expected name of a <see cref="TagNode"/>.</param>
        /// <param name="tag">The <see cref="TagNode"/> involved in this event.</param>
        public TagEventArgs (string tagName, TagNode tag)
            : base()
        {
            _tag = tag;
            _tagName = tagName;
        }

        /// <summary>
        /// Constructs a new event argument set.
        /// </summary>
        /// <param name="schema">The <see cref="SchemaNode"/> corresponding to the <see cref="TagNode"/> involved in this event.</param>
        /// <param name="tag">The <see cref="TagNode"/> involved in this event.</param>
        public TagEventArgs (SchemaNode schema, TagNode tag)
            : base()
        {
            _tag = tag;
            _schema = schema;
        }
    }

    /// <summary>
    /// An event handler for intercepting and responding to verification failures of NBT trees.
    /// </summary>
    /// <param name="eventArgs">Information relating to a verification event.</param>
    /// <returns>A <see cref="TagEventCode"/> determining how the event processor should respond.</returns>
    public delegate TagEventCode VerifierEventHandler (TagEventArgs eventArgs);

    /// <summary>
    /// Verifies the integrity of an NBT tree against a schema definition.
    /// </summary>
    public class NbtVerifier
    {
        private TagNode _root;
        private SchemaNode _schema;

        /// <summary>
        /// An event that gets fired whenever an expected <see cref="TagNode"/> is not found.
        /// </summary>
        public static event VerifierEventHandler MissingTag;

        /// <summary>
        /// An event that gets fired whenever an expected <see cref="TagNode"/> is of the wrong type and cannot be cast.
        /// </summary>
        public static event VerifierEventHandler InvalidTagType;

        /// <summary>
        /// An event that gets fired whenever an expected <see cref="TagNode"/> has a value that violates the schema.
        /// </summary>
        public static event VerifierEventHandler InvalidTagValue;

        private Dictionary<string, TagNode> _scratch = new Dictionary<string,TagNode>();

        /// <summary>
        /// Constructs a new <see cref="NbtVerifier"/> object for a given NBT tree and schema.
        /// </summary>
        /// <param name="root">A <see cref="TagNode"/> representing the root of an NBT tree.</param>
        /// <param name="schema">A <see cref="SchemaNode"/> representing the root of a schema definition for the NBT tree.</param>
        public NbtVerifier (TagNode root, SchemaNode schema)
        {
            _root = root;
            _schema = schema;
        }

        /// <summary>
        /// Invokes the verifier.
        /// </summary>
        /// <returns>Status indicating whether the NBT tree is valid for the given schema.</returns>
        public virtual bool Verify ()
        {
            return Verify(null, _root, _schema);
        }

        private bool Verify (TagNode parent, TagNode tag, SchemaNode schema)
        {
            if (tag == null) {
                return OnMissingTag(new TagEventArgs(schema.Name));
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

            return OnInvalidTagType(new TagEventArgs(schema.Name, tag));
        }

        private bool VerifyScaler (TagNode tag, SchemaNodeScaler schema)
        {
            if (!tag.IsCastableTo(schema.Type)) {
                if (!OnInvalidTagType(new TagEventArgs(schema.Name, tag))) {
                    return false;
                }
            }

            return true;
        }

        private bool VerifyString (TagNode tag, SchemaNodeString schema)
        {
            TagNodeString stag = tag as TagNodeString;
            if (stag == null) {
                if (!OnInvalidTagType(new TagEventArgs(schema, tag))) {
                    return false;
                }
            }
            if (schema.Length > 0 && stag.Length > schema.Length) {
                if (!OnInvalidTagValue(new TagEventArgs(schema, tag))) {
                    return false;
                }
            }
            if (schema.Value != null && stag.Data != schema.Value) {
                if (!OnInvalidTagValue(new TagEventArgs(schema, tag))) {
                    return false;
                }
            }

            return true;
        }


        private bool VerifyArray (TagNode tag, SchemaNodeArray schema)
        {
            TagNodeByteArray atag = tag as TagNodeByteArray;
            if (atag == null) {
                if (!OnInvalidTagType(new TagEventArgs(schema, tag))) {
                    return false;
                }
            }
            if (schema.Length > 0 && atag.Length != schema.Length) {
                if (!OnInvalidTagValue(new TagEventArgs(schema, tag))) {
                    return false;
                }
            }

            return true;
        }

        private bool VerifyList (TagNode tag, SchemaNodeList schema)
        {
            TagNodeList ltag = tag as TagNodeList;
            if (ltag == null) {
                if (!OnInvalidTagType(new TagEventArgs(schema, tag))) {
                    return false;
                }
            }
            if (ltag.Count > 0 && ltag.ValueType != schema.Type) {
                if (!OnInvalidTagValue(new TagEventArgs(schema, tag))) {
                    return false;
                }
            }
            if (schema.Length > 0 && ltag.Count != schema.Length) {
                if (!OnInvalidTagValue(new TagEventArgs(schema, tag))) {
                    return false;
                }
            }

            // Patch up empty lists
            //if (schema.Length == 0) {
            //    tag = new NBT_List(schema.Type);
            //}

            bool pass = true;

            // If a subschema is set, test all items in list against it

            if (schema.SubSchema != null) {
                foreach (TagNode v in ltag) {
                    pass = Verify(tag, v, schema.SubSchema) && pass;
                }
            }

            return pass;
        }

        private bool VerifyCompound (TagNode tag, SchemaNodeCompound schema)
        {
            TagNodeCompound ctag = tag as TagNodeCompound;
            if (ctag == null) {
                if (!OnInvalidTagType(new TagEventArgs(schema, tag))) {
                    return false;
                }
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

                pass = Verify(tag, value, node) && pass;
            }

            foreach (KeyValuePair<string, TagNode> item in _scratch) {
                ctag[item.Key] = item.Value;
            }

            _scratch.Clear();

            return pass;
        }

        #region Event Handlers

        /// <summary>
        /// Processes registered events for <see cref="MissingTag"/> whenever an expected <see cref="TagNode"/> is not found.
        /// </summary>
        /// <param name="e">Arguments for this event.</param>
        /// <returns>Status indicating whether this event can be ignored.</returns>
        protected virtual bool OnMissingTag (TagEventArgs e)
        {
            if (MissingTag != null) {
                foreach (VerifierEventHandler func in MissingTag.GetInvocationList()) {
                    TagEventCode code = func(e);
                    switch (code) {
                        case TagEventCode.FAIL:
                            return false;
                        case TagEventCode.PASS:
                            return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Processes registered events for <see cref="InvalidTagType"/> whenever an expected <see cref="TagNode"/> is of the wrong type and cannot be cast.
        /// </summary>
        /// <param name="e">Arguments for this event.</param>
        /// <returns>Status indicating whether this event can be ignored.</returns>
        protected virtual bool OnInvalidTagType (TagEventArgs e)
        {
            if (InvalidTagType != null) {
                foreach (VerifierEventHandler func in InvalidTagType.GetInvocationList()) {
                    TagEventCode code = func(e);
                    switch (code) {
                        case TagEventCode.FAIL:
                            return false;
                        case TagEventCode.PASS:
                            return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Processes registered events for <see cref="InvalidTagValue"/> whenever an expected <see cref="TagNode"/> has a value that violates the schema.
        /// </summary>
        /// <param name="e">Arguments for this event.</param>
        /// <returns>Status indicating whether this event can be ignored.</returns>
        protected virtual bool OnInvalidTagValue (TagEventArgs e)
        {
            if (InvalidTagValue != null) {
                foreach (VerifierEventHandler func in InvalidTagValue.GetInvocationList()) {
                    TagEventCode code = func(e);
                    switch (code) {
                        case TagEventCode.FAIL:
                            return false;
                        case TagEventCode.PASS:
                            return true;
                    }
                }
            }

            return false;
        }

        #endregion
    }
}
