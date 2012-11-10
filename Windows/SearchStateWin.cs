using System;
using System.Collections.Generic;
using System.Text;
using NBTExplorer.Model;
using System.Windows.Forms;

namespace NBTExplorer.Windows
{
    internal class SearchStateWin : ISearchState
    {
        private ContainerControl _sender;

        public SearchStateWin (ContainerControl sender)
        {
            _sender = sender;
        }

        public Action<DataNode> DiscoverCallback { get; set; }
        public Action<DataNode> ProgressCallback { get; set; }
        public Action<DataNode> CollapseCallback { get; set; }
        public Action<DataNode> EndCallback { get; set; }

        #region ISearchState

        public DataNode RootNode { get; set; }
        public string SearchName { get; set; }
        public string SearchValue { get; set; }

        public IEnumerator<DataNode> State { get; set; }

        public void InvokeDiscoverCallback (DataNode node)
        {
            if (_sender != null && DiscoverCallback != null)
                _sender.BeginInvoke(DiscoverCallback, new object[] { node });
        }

        public void InvokeProgressCallback (DataNode node)
        {
            if (_sender != null && ProgressCallback != null)
                _sender.BeginInvoke(ProgressCallback, new object[] { node });
        }

        public void InvokeCollapseCallback (DataNode node)
        {
            if (_sender != null && CollapseCallback != null)
                _sender.BeginInvoke(CollapseCallback, new object[] { node });
        }

        public void InvokeEndCallback (DataNode node)
        {
            if (_sender != null && EndCallback != null)
                _sender.BeginInvoke(EndCallback, new object[] { node });
        }

        #endregion
    }
}
