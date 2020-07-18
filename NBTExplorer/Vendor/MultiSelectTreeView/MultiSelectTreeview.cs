// Copyright 2007 Andrew D. Weiss
// Licensed under CPOL
// http://www.codeproject.com/Articles/20581/Multiselect-Treeview-Implementation

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace NBTExplorer.Vendor.MultiSelectTreeView
{
    public class MultiSelectTreeView : TreeView
    {
        private const int WM_ERASEBKGND = 0x14;

        public MultiSelectTreeView()
        {
            DoubleBuffered = true;

            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);

            m_SelectedNodes = new List<TreeNode>();
            base.SelectedNode = null;
        }

        private void UpdateExtendedStyles()
        {
            if (Interop.WinInteropAvailable)
            {
                var style = 0;

                if (DoubleBuffered)
                    style |= NativeInterop.TVS_EX_DOUBLEBUFFER;

                if (style != 0)
                    Interop.SendMessage(Handle, NativeInterop.TVM_SETEXTENDEDSTYLE,
                        (IntPtr)NativeInterop.TVS_EX_DOUBLEBUFFER, (IntPtr)style);
            }
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_ERASEBKGND) //if message is is erase background
                return;

            base.WndProc(ref m);
        }

        #region Selected Node(s) Properties

        private readonly List<TreeNode> m_SelectedNodes;

        public List<TreeNode> SelectedNodes
        {
            get => m_SelectedNodes;
            set
            {
                BeginUpdate();

                ClearSelectedNodes();
                if (value != null)
                    foreach (var node in value)
                        ToggleNode(node, true);

                EndUpdate();
            }
        }

        // Note we use the new keyword to Hide the native treeview's SelectedNode property.
        private TreeNode m_SelectedNode;

        public new TreeNode SelectedNode
        {
            get => m_SelectedNode;
            set
            {
                ClearSelectedNodes();
                if (value != null)
                    SelectNode(value);
            }
        }

        #endregion Selected Node(s) Properties

        #region Overridden Events

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            if (Interop.WinInteropAvailable)
            {
                UpdateExtendedStyles();
                if (!Interop.IsWinXP)
                    Interop.SendMessage(Handle, NativeInterop.TVM_SETBKCOLOR, IntPtr.Zero,
                        (IntPtr)ColorTranslator.ToWin32(BackColor));
            }
        }

        protected override void OnGotFocus(EventArgs e)
        {
            // Make sure at least one node has a selection
            // this way we can tab to the ctrl and use the
            // keyboard to select nodes
            try
            {
                if (m_SelectedNode == null && TopNode != null)
                    ToggleNode(TopNode, true);

                base.OnGotFocus(e);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            // If the user clicks on a node that was not
            // previously selected, select it now.

            try
            {
                base.SelectedNode = null;

                var node = GetNodeAt(e.Location);
                if (node != null)
                {
                    var leftBound = node.Bounds.X; // - 20; // Allow user to click on image
                    if (node.TreeView.ImageList != null)
                        leftBound -= 20;
                    var rightBound = node.Bounds.Right + 10; // Give a little extra room
                    if (e.Location.X > leftBound && e.Location.X < rightBound)
                        if (ModifierKeys == Keys.None && m_SelectedNodes.Contains(node))
                        {
                            // Potential Drag Operation
                            // Let Mouse Up do select
                        }
                        else
                        {
                            SelectNode(node);
                        }
                }

                base.OnMouseDown(e);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            // If the clicked on a node that WAS previously
            // selected then, reselect it now. This will clear
            // any other selected nodes. e.g. A B C D are selected
            // the user clicks on B, now A C & D are no longer selected.
            try
            {
                // Check to see if a node was clicked on
                var node = GetNodeAt(e.Location);
                if (node != null)
                    if (ModifierKeys == Keys.None && m_SelectedNodes.Contains(node))
                    {
                        var leftBound = node.Bounds.X; // -20; // Allow user to click on image
                        var rightBound = node.Bounds.Right + 10; // Give a little extra room
                        if (e.Location.X > leftBound && e.Location.X < rightBound)
                            SelectNode(node);
                    }

                base.OnMouseUp(e);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        protected override void OnItemDrag(ItemDragEventArgs e)
        {
            // If the user drags a node and the node being dragged is NOT
            // selected, then clear the active selection, select the
            // node being dragged and drag it. Otherwise if the node being
            // dragged is selected, drag the entire selection.
            try
            {
                var node = e.Item as TreeNode;

                if (node != null)
                    if (!m_SelectedNodes.Contains(node))
                    {
                        SelectSingleNode(node);
                        ToggleNode(node, true);
                    }

                base.OnItemDrag(e);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        protected override void OnBeforeSelect(TreeViewCancelEventArgs e)
        {
            // Never allow base.SelectedNode to be set!
            try
            {
                base.SelectedNode = null;
                e.Cancel = true;

                base.OnBeforeSelect(e);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        protected override void OnAfterSelect(TreeViewEventArgs e)
        {
            // Never allow base.SelectedNode to be set!
            try
            {
                base.OnAfterSelect(e);
                base.SelectedNode = null;
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            // Handle all possible key strokes for the control.
            // including navigation, selection, etc.

            base.OnKeyDown(e);

            if (e.KeyCode == Keys.ShiftKey) return;

            //this.BeginUpdate();
            var bShift = ModifierKeys == Keys.Shift;

            try
            {
                // Nothing is selected in the tree, this isn't a good state
                // select the top node
                if (m_SelectedNode == null && TopNode != null)
                    ToggleNode(TopNode, true);

                // Nothing is still selected in the tree, this isn't a good state, leave.
                if (m_SelectedNode == null) return;

                if (e.KeyCode == Keys.Left)
                {
                    if (m_SelectedNode.IsExpanded && m_SelectedNode.Nodes.Count > 0)
                        m_SelectedNode.Collapse();
                    else if (m_SelectedNode.Parent != null)
                        SelectSingleNode(m_SelectedNode.Parent);
                }
                else if (e.KeyCode == Keys.Right)
                {
                    if (!m_SelectedNode.IsExpanded)
                        m_SelectedNode.Expand();
                    else
                        SelectSingleNode(m_SelectedNode.FirstNode);
                }
                else if (e.KeyCode == Keys.Up)
                {
                    // Select the previous node
                    if (m_SelectedNode.PrevVisibleNode != null)
                        SelectNode(m_SelectedNode.PrevVisibleNode);
                }
                else if (e.KeyCode == Keys.Down)
                {
                    // Select the next node
                    if (m_SelectedNode.NextVisibleNode != null)
                        SelectNode(m_SelectedNode.NextVisibleNode);
                }
                else if (e.KeyCode == Keys.Home)
                {
                    if (bShift)
                    {
                        if (m_SelectedNode.Parent == null)
                        {
                            // Select all of the root nodes up to this point
                            if (Nodes.Count > 0)
                                SelectNode(Nodes[0]);
                        }
                        else
                        {
                            // Select all of the nodes up to this point under this nodes parent
                            SelectNode(m_SelectedNode.Parent.FirstNode);
                        }
                    }
                    else
                    {
                        // Select this first node in the tree
                        if (Nodes.Count > 0)
                            SelectSingleNode(Nodes[0]);
                    }
                }
                else if (e.KeyCode == Keys.End)
                {
                    if (bShift)
                    {
                        if (m_SelectedNode.Parent == null)
                        {
                            // Select the last ROOT node in the tree
                            if (Nodes.Count > 0)
                                SelectNode(Nodes[Nodes.Count - 1]);
                        }
                        else
                        {
                            // Select the last node in this branch
                            SelectNode(m_SelectedNode.Parent.LastNode);
                        }
                    }
                    else
                    {
                        if (Nodes.Count > 0)
                        {
                            // Select the last node visible node in the tree.
                            // Don't expand branches incase the tree is virtual
                            var ndLast = Nodes[0].LastNode;
                            while (ndLast.IsExpanded && ndLast.LastNode != null)
                                ndLast = ndLast.LastNode;
                            SelectSingleNode(ndLast);
                        }
                    }
                }
                else if (e.KeyCode == Keys.PageUp)
                {
                    // Select the highest node in the display
                    var nCount = VisibleCount;
                    var ndCurrent = m_SelectedNode;
                    while (nCount > 0 && ndCurrent.PrevVisibleNode != null)
                    {
                        ndCurrent = ndCurrent.PrevVisibleNode;
                        nCount--;
                    }
                    SelectSingleNode(ndCurrent);
                }
                else if (e.KeyCode == Keys.PageDown)
                {
                    // Select the lowest node in the display
                    var nCount = VisibleCount;
                    var ndCurrent = m_SelectedNode;
                    while (nCount > 0 && ndCurrent.NextVisibleNode != null)
                    {
                        ndCurrent = ndCurrent.NextVisibleNode;
                        nCount--;
                    }
                    SelectSingleNode(ndCurrent);
                }
                else
                {
                    // Assume this is a search character a-z, A-Z, 0-9, etc.
                    // Select the first node after the current node that
                    // starts with this character
                    var sSearch = ((char)e.KeyValue).ToString();

                    var ndCurrent = m_SelectedNode;
                    while (ndCurrent.NextVisibleNode != null)
                    {
                        ndCurrent = ndCurrent.NextVisibleNode;
                        if (ndCurrent.Text.StartsWith(sSearch))
                        {
                            SelectSingleNode(ndCurrent);
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
            finally
            {
                EndUpdate();
            }
        }

        #endregion Overridden Events

        #region Helper Methods

        private void SelectNode(TreeNode node)
        {
            try
            {
                BeginUpdate();

                if (m_SelectedNode == null || ModifierKeys == Keys.Control)
                {
                    // Ctrl+Click selects an unselected node, or unselects a selected node.
                    var bIsSelected = m_SelectedNodes.Contains(node);
                    ToggleNode(node, !bIsSelected);
                }
                else if (ModifierKeys == Keys.Shift)
                {
                    // Shift+Click selects nodes between the selected node and here.
                    var ndStart = m_SelectedNode;
                    var ndEnd = node;

                    if (ndStart.Parent == ndEnd.Parent)
                    {
                        // Selected node and clicked node have same parent, easy case.
                        if (ndStart.Index < ndEnd.Index)
                        {
                            // If the selected node is beneath the clicked node walk down
                            // selecting each Visible node until we reach the end.
                            while (ndStart != ndEnd)
                            {
                                ndStart = ndStart.NextVisibleNode;
                                if (ndStart == null) break;
                                ToggleNode(ndStart, true);
                            }
                        }
                        else if (ndStart.Index == ndEnd.Index)
                        {
                            // Clicked same node, do nothing
                        }
                        else
                        {
                            // If the selected node is above the clicked node walk up
                            // selecting each Visible node until we reach the end.
                            while (ndStart != ndEnd)
                            {
                                ndStart = ndStart.PrevVisibleNode;
                                if (ndStart == null) break;
                                ToggleNode(ndStart, true);
                            }
                        }
                    }
                    else
                    {
                        // Selected node and clicked node have same parent, hard case.
                        // We need to find a common parent to determine if we need
                        // to walk down selecting, or walk up selecting.

                        var ndStartP = ndStart;
                        var ndEndP = ndEnd;
                        var startDepth = Math.Min(ndStartP.Level, ndEndP.Level);

                        // Bring lower node up to common depth
                        while (ndStartP.Level > startDepth)
                            ndStartP = ndStartP.Parent;

                        // Bring lower node up to common depth
                        while (ndEndP.Level > startDepth)
                            ndEndP = ndEndP.Parent;

                        // Walk up the tree until we find the common parent
                        while (ndStartP.Parent != ndEndP.Parent)
                        {
                            ndStartP = ndStartP.Parent;
                            ndEndP = ndEndP.Parent;
                        }

                        // Select the node
                        if (ndStartP.Index < ndEndP.Index)
                            while (ndStart != ndEnd)
                            {
                                ndStart = ndStart.NextVisibleNode;
                                if (ndStart == null) break;
                                ToggleNode(ndStart, true);
                            }
                        else if (ndStartP.Index == ndEndP.Index)
                            if (ndStart.Level < ndEnd.Level)
                                while (ndStart != ndEnd)
                                {
                                    ndStart = ndStart.NextVisibleNode;
                                    if (ndStart == null) break;
                                    ToggleNode(ndStart, true);
                                }
                            else
                                while (ndStart != ndEnd)
                                {
                                    ndStart = ndStart.PrevVisibleNode;
                                    if (ndStart == null) break;
                                    ToggleNode(ndStart, true);
                                }
                        else
                            while (ndStart != ndEnd)
                            {
                                ndStart = ndStart.PrevVisibleNode;
                                if (ndStart == null) break;
                                ToggleNode(ndStart, true);
                            }
                    }
                }
                else
                {
                    // Just clicked a node, select it
                    SelectSingleNode(node);
                }

                OnAfterSelect(new TreeViewEventArgs(m_SelectedNode));
            }
            finally
            {
                EndUpdate();
            }
        }

        private void ClearSelectedNodes()
        {
            try
            {
                foreach (var node in m_SelectedNodes)
                {
                    node.BackColor = BackColor;
                    node.ForeColor = ForeColor;
                }
            }
            finally
            {
                m_SelectedNodes.Clear();
                m_SelectedNode = null;
            }
        }

        private void SelectSingleNode(TreeNode node)
        {
            if (node == null)
                return;

            ClearSelectedNodes();
            ToggleNode(node, true);
            node.EnsureVisible();
        }

        private void ToggleNode(TreeNode node, bool bSelectNode)
        {
            if (bSelectNode)
            {
                m_SelectedNode = node;
                if (!m_SelectedNodes.Contains(node))
                    m_SelectedNodes.Add(node);
                node.BackColor = SystemColors.Highlight;
                node.ForeColor = SystemColors.HighlightText;
            }
            else
            {
                m_SelectedNodes.Remove(node);
                node.BackColor = BackColor;
                node.ForeColor = ForeColor;
            }
        }

        private void HandleException(Exception ex)
        {
            // Perform some error handling here.
            // We don't want to bubble errors to the CLR.
            MessageBox.Show(ex.Message);
        }

        #endregion Helper Methods
    }
}