using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using NBTExplorer.Controllers;
using Substrate.Nbt;

namespace NBTExplorer.Windows
{
    public partial class FindReplace : Form
    {
        private NodeTreeController _findController;
        private NodeTreeController _replaceController;

        public FindReplace ()
        {
            InitializeComponent();

            _findController = new NodeTreeController(treeView1);
            _replaceController = new NodeTreeController(treeView2);
        }

        #region Find Toolbar Buttons

        private void _tbFindDelete_Click (object sender, EventArgs e)
        {

        }

        private void _tbFindAny_Click (object sender, EventArgs e)
        {

        }

        private void _tbFindByte_Click (object sender, EventArgs e)
        {
            _findController.CreateRootNode(TagType.TAG_BYTE);
        }

        private void _tbFindShort_Click (object sender, EventArgs e)
        {
            _findController.CreateRootNode(TagType.TAG_SHORT);
        }

        private void _tbFindInt_Click (object sender, EventArgs e)
        {
            _findController.CreateRootNode(TagType.TAG_INT);
        }

        private void _tbFindLong_Click (object sender, EventArgs e)
        {
            _findController.CreateRootNode(TagType.TAG_LONG);
        }

        private void _tbFindFloat_Click (object sender, EventArgs e)
        {
            _findController.CreateRootNode(TagType.TAG_FLOAT);
        }

        private void _tbFindDouble_Click (object sender, EventArgs e)
        {
            _findController.CreateRootNode(TagType.TAG_DOUBLE);
        }

        private void _tbFindString_Click (object sender, EventArgs e)
        {
            _findController.CreateRootNode(TagType.TAG_STRING);
        }

        #endregion

        #region Replace Toolbar Icons

        private void _tbReplaceDelete_Click (object sender, EventArgs e)
        {
            
        }

        private void _tbReplaceByte_Click (object sender, EventArgs e)
        {
            _replaceController.CreateRootNode(TagType.TAG_BYTE);
        }

        private void _tbReplaceShort_Click (object sender, EventArgs e)
        {
            _replaceController.CreateRootNode(TagType.TAG_SHORT);
        }

        private void _tbReplaceInt_Click (object sender, EventArgs e)
        {

        }

        private void _tbReplaceLong_Click (object sender, EventArgs e)
        {

        }

        private void _tbReplaceFloat_Click (object sender, EventArgs e)
        {

        }

        private void _tbReplaceDouble_Click (object sender, EventArgs e)
        {

        }

        private void _tbReplaceByteArray_Click (object sender, EventArgs e)
        {

        }

        private void _tbReplaceIntArray_Click (object sender, EventArgs e)
        {

        }

        private void _tbReplaceString_Click (object sender, EventArgs e)
        {

        }

        private void _tbReplaceList_Click (object sender, EventArgs e)
        {

        }

        private void _tbReplaceCompound_Click (object sender, EventArgs e)
        {

        }

        #endregion
    }
}
