using System;
using System.Collections.Generic;
using System.Windows.Forms;
using NBTExplorer.Model;

namespace NBTExplorer
{
    internal class SearchState
    {
        public DataNode RootNode { get; set; }
        public string SearchName { get; set; }
        public string SearchValue { get; set; }

        public IEnumerator<DataNode> State { get; set; }

        public Action<DataNode> DiscoverCallback { get; set; }
        public Action<DataNode> ProgressCallback { get; set; }
        public Action<DataNode> CollapseCallback { get; set; }
        public Action EndCallback { get; set; }
    }

    internal class SearchWorker
    {
        private ContainerControl _sender;
        private SearchState _state;
        private bool _cancel;
        private object _lock;

        public SearchWorker (SearchState state, ContainerControl sender)
        {
            _state = state;
            _sender = sender;
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
            if (_sender != null && _state.DiscoverCallback != null)
                _sender.BeginInvoke(_state.DiscoverCallback, new object[] { node });
        }

        private void InvokeCollapseCallback (DataNode node)
        {
            if (_sender != null && _state.CollapseCallback != null)
                _sender.BeginInvoke(_state.CollapseCallback, new object[] { node });
        }

        private void InvokeEndCallback ()
        {
            if (_sender != null && _state.EndCallback != null)
                _sender.BeginInvoke(_state.EndCallback);
        }
    }
}
