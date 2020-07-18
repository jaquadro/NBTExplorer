using NBTExplorer.Model;
using NBTExplorer.Windows;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace NBTExplorer.Controllers
{
    internal class ExplorerBarController
    {
        private readonly ToolStrip _explorerStrip;
        private readonly ImageList _iconList;
        private readonly IconRegistry _registry;
        private DataNode _rootNode;

        public ExplorerBarController(ToolStrip toolStrip, IconRegistry registry, ImageList iconList, DataNode rootNode)
        {
            _explorerStrip = toolStrip;
            _registry = registry;
            _iconList = iconList;
            _rootNode = rootNode;

            Initialize();
        }

        public DataNode SearchRoot
        {
            get => _rootNode;
            set
            {
                if (_rootNode == value)
                    return;

                _rootNode = value;
                Initialize();

                OnSearchRootChanged();
            }
        }

        private void Initialize()
        {
            _explorerStrip.Items.Clear();

            var ancestry = new List<DataNode>();
            var node = _rootNode;

            while (node != null)
            {
                ancestry.Add(node);
                node = node.Parent;
            }

            ancestry.Reverse();

            foreach (var item in ancestry)
            {
                var itemButton = new ToolStripSplitButton(item.NodePathName)
                {
                    Tag = item
                };
                itemButton.ButtonClick += (s, e) =>
                {
                    var button = s as ToolStripSplitButton;
                    if (button != null)
                        SearchRoot = button.Tag as DataNode;
                };
                itemButton.DropDown.ImageList = _iconList;

                if (_explorerStrip.Items.Count == 0)
                    itemButton.ImageIndex = _registry.Lookup(item.GetType());

                if (!item.IsExpanded)
                    item.Expand();

                foreach (var subItem in item.Nodes)
                {
                    if (!subItem.IsContainerType)
                        continue;

                    var menuItem = new ToolStripMenuItem(subItem.NodePathName)
                    {
                        ImageIndex = _registry.Lookup(subItem.GetType()),
                        Tag = subItem
                    };
                    menuItem.Click += (s, e) =>
                    {
                        var mItem = s as ToolStripMenuItem;
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

        public event EventHandler SearchRootChanged;

        protected virtual void OnSearchRootChanged()
        {
            var ev = SearchRootChanged;
            if (ev != null)
                ev(this, EventArgs.Empty);
        }
    }
}