using Substrate.Nbt;

namespace NBTExplorer.Model
{
    public class DataNode
    {
        private DataNode _parent;
        private DataNodeCollection _children;

        private bool _expanded;
        private bool _modified;

        public DataNode ()
        {
            _children = new DataNodeCollection(this);
        }

        public DataNode Parent
        {
            get { return _parent; }
            internal set { _parent = value; }
        }

        public DataNodeCollection Nodes
        {
            get { return _children; }
        }

        public bool IsModified
        {
            get { return _modified; }
            set
            {
                if (value && Parent != null)
                    Parent.IsModified = value;
                _modified = value;
            }
        }

        public bool IsExpanded
        {
            get { return _expanded; }
            private set { _expanded = value; }
        }

        public void Expand ()
        {
            if (!IsExpanded) {
                ExpandCore();
                IsExpanded = true;
            }
        }

        protected virtual void ExpandCore () { }

        public void Collapse ()
        {
            if (IsExpanded && !IsModified) {
                Release();
                IsExpanded = false;
            }
        }

        public void Release ()
        {
            foreach (DataNode node in Nodes)
                node.Release();

            ReleaseCore();
            IsExpanded = false;
        }

        protected virtual void ReleaseCore ()
        {
            Nodes.Clear();
        }

        public void Save ()
        {
            foreach (DataNode node in Nodes)
                if (node.IsModified)
                    node.Save();

            SaveCore();
            IsModified = false;
        }

        protected virtual void SaveCore ()
        {
        }

        public virtual string NodeName
        {
            get { return ""; }
        }

        public virtual string NodeDisplay
        {
            get { return ""; }
        }

        public virtual bool HasUnexpandedChildren
        {
            get { return false; }
        }

        #region Capabilities

        protected virtual NodeCapabilities Capabilities
        {
            get { return NodeCapabilities.None; }
        }

        public virtual bool CanRenameNode
        {
            get { return (Capabilities & NodeCapabilities.Rename) != NodeCapabilities.None; }
        }

        public virtual bool CanEditNode
        {
            get { return (Capabilities & NodeCapabilities.Edit) != NodeCapabilities.None; }
        }

        public virtual bool CanDeleteNode
        {
            get { return (Capabilities & NodeCapabilities.Delete) != NodeCapabilities.None; }
        }

        public virtual bool CanCopyNode
        {
            get { return (Capabilities & NodeCapabilities.Copy) != NodeCapabilities.None; }
        }

        public virtual bool CanCutNode
        {
            get { return (Capabilities & NodeCapabilities.Cut) != NodeCapabilities.None; }
        }

        public virtual bool CanPasteIntoNode
        {
            get { return (Capabilities & NodeCapabilities.PasteInto) != NodeCapabilities.None; }
        }

        public virtual bool CanSearchNode
        {
            get { return (Capabilities & NodeCapabilities.Search) != NodeCapabilities.None; }
        }

        public virtual bool CanCreateTag (TagType type)
        {
            return false;
        }

        #endregion

        #region Operations

        public virtual bool CreateNode (TagType type)
        {
            return false;
        }

        public virtual bool RenameNode ()
        {
            return false;
        }

        public virtual bool EditNode ()
        {
            return false;
        }

        public virtual bool DeleteNode ()
        {
            return false;
        }

        public virtual bool CopyNode ()
        {
            return false;
        }

        public virtual bool CutNode ()
        {
            return false;
        }

        public virtual bool PasteNode ()
        {
            return false;
        }

        #endregion
    }
}
