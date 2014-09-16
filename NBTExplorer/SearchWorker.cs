using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using NBTExplorer.Model;

namespace NBTExplorer
{
    internal interface ISearchState
    {
        DataNode RootNode { get; set; }

        IEnumerator<DataNode> State { get; set; }
        bool TerminateOnDiscover { get; set; }
        bool IsTerminated { get; set; }

        float ProgressRate { get; set; }

        void InvokeDiscoverCallback (DataNode node);
        void InvokeProgressCallback (DataNode node);
        void InvokeCollapseCallback (DataNode node);
        void InvokeEndCallback (DataNode node);

        bool TestNode (DataNode node);
    }

    internal abstract class NameValueSearchState : ISearchState
    {
        public virtual string SearchName { get; set; }
        public virtual string SearchValue { get; set; }

        public DataNode RootNode { get; set; }
        public IEnumerator<DataNode> State { get; set; }
        public bool TerminateOnDiscover { get; set; }
        public bool IsTerminated { get; set; }
        public float ProgressRate { get; set; }

        public abstract void InvokeDiscoverCallback (DataNode node);
        public abstract void InvokeProgressCallback (DataNode node);
        public abstract void InvokeCollapseCallback (DataNode node);
        public abstract void InvokeEndCallback (DataNode node);

        protected NameValueSearchState ()
        {
            TerminateOnDiscover = true;
            ProgressRate = .5f;
        }

        public bool TestNode (DataNode node)
        {
            bool mName = SearchName == null;
            bool mValue = SearchValue == null;

            if (SearchName != null) {
                string tagName = node.NodeName;
                if (tagName != null)
                    mName = CultureInfo.InvariantCulture.CompareInfo.IndexOf(tagName, SearchName, CompareOptions.IgnoreCase) >= 0;
            }
            if (SearchValue != null) {
                string tagValue = node.NodeDisplay;
                if (tagValue != null)
                    mValue = CultureInfo.InvariantCulture.CompareInfo.IndexOf(tagValue, SearchValue, CompareOptions.IgnoreCase) >= 0;
            }

            if (mName && mValue) {
                return true;
            }

            return false;
        }
    }

    internal class SearchWorker
    {
        private ISearchState _state;
        private bool _cancel;
        private object _lock;

        private Stopwatch _progressWatch;
        private float _progressTime;
        private float _lastSampleTime;

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
            _progressWatch = new Stopwatch();
            _progressWatch.Start();

            if (_state.State == null)
                _state.State = FindNode(_state.RootNode).GetEnumerator();

            if (!_state.State.MoveNext())
                InvokeEndCallback();

            _progressWatch.Stop();
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
                float currentSampleTime = (float)_progressWatch.Elapsed.TotalSeconds;
                _progressTime += (currentSampleTime - _lastSampleTime);
                _lastSampleTime = currentSampleTime;

                if (_progressTime > _state.ProgressRate) {
                    InvokeProgressCallback(node);
                    _progressTime -= _state.ProgressRate;
                }

                if (_state.TestNode(tagNode)) {
                    InvokeDiscoverCallback(node);
                    if (_state.TerminateOnDiscover)
                        yield return node;
                }

                /*bool mName = _state.SearchName == null;
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
                }*/
            }

            using (node.Nodes.Snapshot()) {
                foreach (DataNode sub in node.Nodes) {
                    foreach (DataNode s in FindNode(sub))
                        yield return s;
                }
            }

            /*IList<DataNode> nodeList = node.Nodes;
            for (int i = 0; i < nodeList.Count; i++) {
                int changeset = node.Nodes.ChangeCount;
                foreach (DataNode s in FindNode(nodeList[i])) {

                }
            }

                foreach (DataNode sub in node.Nodes) {
                    foreach (DataNode s in FindNode(sub))
                        yield return s;
                }*/

            if (searchExpanded) {
                if (!node.IsModified) {
                    node.Collapse();
                    InvokeCollapseCallback(node);
                }
            }
        }

        private void InvokeProgressCallback (DataNode node)
        {
            _state.InvokeProgressCallback(node);
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
            _state.IsTerminated = true;
            _state.InvokeEndCallback(null);
        }
    }
}
