using Substrate.Nbt;
using System.Collections.Generic;

namespace NBTExplorer.Model
{
    public class RootDataNode : TagCompoundDataNode
    {
        private string _display = "";
        private string _name = "Root";

        public RootDataNode()
            : base(new TagNodeCompound())
        {
        }

        public override string NodeName => _name;

        public override string NodeDisplay => _display;

        public void SetNodeName(string name)
        {
            _name = name;
        }

        public void SetDisplayName(string name)
        {
            _display = name;
        }

        /*public override bool CanCreateTag (TagType type)
        {
            return Enum.IsDefined(typeof(TagType), type) && type != TagType.TAG_END;
        }*/
    }

    // FilterDataNode
    // AndFilterDataNode
    // OrFilterDataNode

    public class DataNode
    {
        private bool _childModified;

        private bool _dataModified;

        public DataNode()
        {
            Nodes = new DataNodeCollection(this);
        }

        public DataNode Parent { get; internal set; }

        public DataNode Root => Parent == null ? this : Parent.Root;

        public DataNodeCollection Nodes { get; }

        public bool IsModified => _dataModified || _childModified;

        protected bool IsDataModified
        {
            get => _dataModified;
            set
            {
                _dataModified = value;
                CalculateChildModifiedState();
            }
        }

        protected bool IsChildModified
        {
            get => _childModified;
            set
            {
                _childModified = value;
                CalculateChildModifiedState();
            }
        }

        protected bool IsParentModified
        {
            get => Parent != null && Parent.IsModified;
            set
            {
                if (Parent != null)
                    Parent.IsDataModified = value;
            }
        }

        public bool IsExpanded { get; private set; }

        public virtual string NodeName => "";

        public string NodePath
        {
            get
            {
                var name = NodePathName;
                if (string.IsNullOrEmpty(name))
                    name = "*";

                return Parent != null ? Parent.NodePath + '/' + name : '/' + name;
            }
        }

        public virtual string NodePathName => NodeName;

        public virtual string NodeDisplay => "";

        public virtual bool IsContainerType => false;

        public virtual bool HasUnexpandedChildren => false;

        private void CalculateChildModifiedState()
        {
            _childModified = false;
            foreach (var child in Nodes)
                if (child.IsModified)
                    _childModified = true;

            if (Parent != null)
                Parent.CalculateChildModifiedState();
        }

        public void Expand()
        {
            if (!IsExpanded)
            {
                ExpandCore();
                IsExpanded = true;
            }
        }

        protected virtual void ExpandCore()
        {
        }

        public void Collapse()
        {
            if (IsExpanded && !IsModified)
            {
                Release();
                IsExpanded = false;
            }
        }

        public void Release()
        {
            foreach (var node in Nodes)
                node.Release();

            ReleaseCore();
            IsExpanded = false;
            IsDataModified = false;
        }

        protected virtual void ReleaseCore()
        {
            Nodes.Clear();
        }

        public void Save()
        {
            foreach (var node in Nodes)
                if (node.IsModified)
                    node.Save();

            SaveCore();
            IsDataModified = false;
        }

        protected virtual void SaveCore()
        {
        }

        protected Dictionary<string, object> BuildExpandSet(DataNode node)
        {
            if (node == null || !node.IsExpanded)
                return null;

            var dict = new Dictionary<string, object>();
            foreach (var child in node.Nodes)
            {
                var childDict = BuildExpandSet(child);
                if (childDict != null) dict[child.NodePathName] = childDict;
            }

            return dict;
        }

        protected void RestoreExpandSet(DataNode node, Dictionary<string, object> expandSet)
        {
            if (expandSet == null)
                return;

            node.Expand();

            foreach (var child in node.Nodes)
                if (expandSet.ContainsKey(child.NodePathName))
                {
                    var childDict = (Dictionary<string, object>)expandSet[child.NodePathName];
                    if (childDict != null)
                        RestoreExpandSet(child, childDict);
                }
        }

        #region Node Capabilities

        protected virtual NodeCapabilities Capabilities => NodeCapabilities.None;

        public virtual bool CanRenameNode => (Capabilities & NodeCapabilities.Rename) != NodeCapabilities.None;

        public virtual bool CanEditNode => (Capabilities & NodeCapabilities.Edit) != NodeCapabilities.None;

        public virtual bool CanDeleteNode => (Capabilities & NodeCapabilities.Delete) != NodeCapabilities.None;

        public virtual bool CanCopyNode => (Capabilities & NodeCapabilities.Copy) != NodeCapabilities.None;

        public virtual bool CanCutNode => (Capabilities & NodeCapabilities.Cut) != NodeCapabilities.None;

        public virtual bool CanPasteIntoNode => (Capabilities & NodeCapabilities.PasteInto) != NodeCapabilities.None;

        public virtual bool CanSearchNode => (Capabilities & NodeCapabilities.Search) != NodeCapabilities.None;

        public virtual bool CanReoderNode => (Capabilities & NodeCapabilities.Reorder) != NodeCapabilities.None;

        public virtual bool CanRefreshNode => (Capabilities & NodeCapabilities.Refresh) != NodeCapabilities.None;

        public virtual bool CanMoveNodeUp => false;

        public virtual bool CanMoveNodeDown => false;

        public virtual bool CanCreateTag(TagType type)
        {
            return false;
        }

        #endregion Node Capabilities

        #region Group Capabilities

        public virtual GroupCapabilities RenameNodeCapabilities => GroupCapabilities.Single;

        public virtual GroupCapabilities EditNodeCapabilities => GroupCapabilities.Single;

        public virtual GroupCapabilities DeleteNodeCapabilities =>
            GroupCapabilities.MultiMixedType | GroupCapabilities.ElideChildren;

        public virtual GroupCapabilities CutNodeCapabilities => GroupCapabilities.Single;

        public virtual GroupCapabilities CopyNodeCapabilities => GroupCapabilities.Single;

        public virtual GroupCapabilities PasteIntoNodeCapabilities => GroupCapabilities.Single;

        public virtual GroupCapabilities SearchNodeCapabilites => GroupCapabilities.Single;

        public virtual GroupCapabilities ReorderNodeCapabilities => GroupCapabilities.Single;

        public virtual GroupCapabilities RefreshNodeCapabilites => GroupCapabilities.Single;

        #endregion Group Capabilities

        #region Operations

        public virtual bool CreateNode(TagType type)
        {
            return false;
        }

        public virtual bool RenameNode()
        {
            return false;
        }

        public virtual bool EditNode()
        {
            return false;
        }

        public virtual bool DeleteNode()
        {
            return false;
        }

        public virtual bool CopyNode()
        {
            return false;
        }

        public virtual bool CutNode()
        {
            return false;
        }

        public virtual bool PasteNode()
        {
            return false;
        }

        public virtual bool ChangeRelativePosition(int offset)
        {
            return false;
        }

        public virtual bool RefreshNode()
        {
            return false;
        }

        #endregion Operations
    }
}