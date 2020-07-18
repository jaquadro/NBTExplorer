using NBTExplorer.Model;
using System;
using System.Windows.Forms;

namespace NBTExplorer.Windows
{
    internal class SearchStateWin : NameValueSearchState
    {
        private readonly ContainerControl _sender;

        public SearchStateWin(ContainerControl sender)
        {
            _sender = sender;
        }

        public Action<DataNode> DiscoverCallback { get; set; }
        public Action<DataNode> ProgressCallback { get; set; }
        public Action<DataNode> CollapseCallback { get; set; }
        public Action<DataNode> EndCallback { get; set; }

        public override void InvokeDiscoverCallback(DataNode node)
        {
            if (_sender != null && DiscoverCallback != null)
                _sender.Invoke(DiscoverCallback, node);
        }

        public override void InvokeProgressCallback(DataNode node)
        {
            if (_sender != null && ProgressCallback != null)
                _sender.Invoke(ProgressCallback, node);
        }

        public override void InvokeCollapseCallback(DataNode node)
        {
            if (_sender != null && CollapseCallback != null)
                _sender.Invoke(CollapseCallback, node);
        }

        public override void InvokeEndCallback(DataNode node)
        {
            if (_sender != null && EndCallback != null)
                _sender.Invoke(EndCallback, node);
        }
    }
}