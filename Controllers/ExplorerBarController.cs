using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using NBTExplorer.Model;
using NBTExplorer.Windows;

namespace NBTExplorer.Controllers
{
    class ExplorerBarController
    {
        private ToolStrip _explorerStrip;
        private DataNode _rootNode;
        private IconRegistry _registry;
        private ImageList _iconList;

        public ExplorerBarController (ToolStrip toolStrip, IconRegistry registry, ImageList iconList, DataNode rootNode)
        {
            _explorerStrip = toolStrip;
            _registry = registry;
            _iconList = iconList;
            _rootNode = rootNode;

            Initialize();
        }

        private void Initialize ()
        {
            _explorerStrip.Items.Clear();

            List<DataNode> ancestry = new List<DataNode>();
            DataNode node = _rootNode;

            while (node != null) {
                ancestry.Add(node);
                node = node.Parent;
            }

            ancestry.Reverse();

            foreach (DataNode item in ancestry) {
                ToolStripSplitButton itemButton = new ToolStripSplitButton(item.NodePathName) {
                    Tag = item,
                };
                itemButton.ButtonClick += (s, e) => {
                    ToolStripSplitButton button = s as ToolStripSplitButton;
                    if (button != null)
                        SearchRoot = button.Tag as DataNode;
                };
                itemButton.DropDown.ImageList = _iconList;

                if (_explorerStrip.Items.Count == 0)
                    itemButton.ImageIndex = _registry.Lookup(item.GetType());

                if (!item.IsExpanded)
                    item.Expand();

                foreach (DataNode subItem in item.Nodes) {
                    if (!subItem.IsContainerType)
                        continue;

                    ToolStripMenuItem menuItem = new ToolStripMenuItem(subItem.NodePathName) {
                        ImageIndex = _registry.Lookup(subItem.GetType()),
                        Tag = subItem,
                    };
                    menuItem.Click += (s, e) => {
                        ToolStripMenuItem mItem = s as ToolStripMenuItem;
                        if (mItem != null)
                            SearchRoot = mItem.Tag as DataNode;
                    };

                    if (ancestry.Contains(subItem))
                        menuItem.Font = new Font(menuItem.Font, FontStyle.Bold);

                    itemButton.DropDownItems.Add(menuItem);
                }

                _explorerStrip.Items.Add(itemButton);
            }
        }

        public DataNode SearchRoot
        {
            get { return _rootNode; }
            set
            {
                if (_rootNode == value)
                    return;

                _rootNode = value;
                Initialize();

                OnSearchRootChanged();
            }
        }

        public event EventHandler SearchRootChanged;

        protected virtual void OnSearchRootChanged ()
        {
            var ev = SearchRootChanged;
            if (ev != null)
                ev(this, EventArgs.Empty);
        }
    }
}
