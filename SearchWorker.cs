using System;
using System.Collections.Generic;
using NBTExplorer.Model;

namespace NBTExplorer
{
    internal interface ISearchState
    {
        DataNode RootNode { get; set; }
        string SearchName { get; set; }
        string SearchValue { get; set; }

        IEnumerator<DataNode> State { get; set; }

        void InvokeDiscoverCallback (DataNode node);
        void InvokeProgressCallback (DataNode node);
        void InvokeCollapseCallback (DataNode node);
        void InvokeEndCallback (DataNode node);
    }

    internal class SearchWorker
    {
        private ISearchState _state;
        private bool _cancel;
        private object _lock;

        public SearchWorker (ISearchState state)
        {
            _state = state;
            _lock = new object();
        }

        public void Cancel ()
        {
            lock (_lock) {
                _cancel = true;
            }
        }

        public void Run ()
        {
            if (_state.State == null)
                _state.State = FindNode(_state.RootNode).GetEnumerator();

            if (!_state.State.MoveNext())
                InvokeEndCallback();
        }

        private IEnumerable<DataNode> FindNode (DataNode node)
        {
            lock (_lock) {
                if (_cancel)
                    yield break;
            }

            if (node == null)
                yield break;

            bool searchExpanded = false;
            if (!node.IsExpanded) {
                node.Expand();
                searchExpanded = true;
            }

            TagDataNode tagNode = node as TagDataNode;
            if (tagNode != null) {
                bool mName = _state.SearchName == null;
                bool mValue = _state.SearchValue == null;

                if (_state.SearchName != null) {
                    string tagName = node.NodeName;
                    if (tagName != null)
                        mName = tagName.Contains(_state.SearchName);
                }
                if (_state.SearchValue != null) {
                    string tagValue = node.NodeDisplay;
                    if (tagValue != null)
                        mValue = tagValue.Contains(_state.SearchValue);
                }

                if (mName && mValue) {
                    InvokeDiscoverCallback(node);
                    yield return node;
                }
            }

            foreach (DataNode sub in node.Nodes) {
                foreach (DataNode s in FindNode(sub))
                    yield return s;
            }

            if (searchExpanded) {
                if (!node.IsModified) {
                    node.Collapse();
                    InvokeCollapseCallback(node);
                }
            }
        }

        private void InvokeDiscoverCallback (DataNode node)
        {
            _state.InvokeDiscoverCallback(node);
        }

        private void InvokeCollapseCallback (DataNode node)
        {
            _state.InvokeCollapseCallback(node);
        }

        private void InvokeEndCallback ()
        {
            _state.InvokeEndCallback(null);
        }
    }
}
