namespace NBTExplorer.Windows
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose (bool disposing)
        {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent ()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._menuItemOpen = new System.Windows.Forms.ToolStripMenuItem();
            this._menuItemOpenFolder = new System.Windows.Forms.ToolStripMenuItem();
            this._menuItemOpenMinecraftSaveFolder = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator12 = new System.Windows.Forms.ToolStripSeparator();
            this._menuItemOpenInExplorer = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this._menuItemSave = new System.Windows.Forms.ToolStripMenuItem();
            this._menuItemRefresh = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this._menuItemRecentFiles = new System.Windows.Forms.ToolStripMenuItem();
            this._menuItemRecentFolders = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this._menuItemExit = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._menuItemCut = new System.Windows.Forms.ToolStripMenuItem();
            this._menuItemCopy = new System.Windows.Forms.ToolStripMenuItem();
            this._menuItemPaste = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this._menuItemRename = new System.Windows.Forms.ToolStripMenuItem();
            this._menuItemEditValue = new System.Windows.Forms.ToolStripMenuItem();
            this._menuItemDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
            this._menuItemMoveUp = new System.Windows.Forms.ToolStripMenuItem();
            this._menuItemMoveDown = new System.Windows.Forms.ToolStripMenuItem();
            this.searchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._menuItemFind = new System.Windows.Forms.ToolStripMenuItem();
            this._menuItemFindNext = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.replaceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
            this.findBlockToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._menuItemAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this._buttonOpen = new System.Windows.Forms.ToolStripButton();
            this._buttonOpenFolder = new System.Windows.Forms.ToolStripButton();
            this._buttonSave = new System.Windows.Forms.ToolStripButton();
            this._buttonRefresh = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this._buttonCut = new System.Windows.Forms.ToolStripButton();
            this._buttonCopy = new System.Windows.Forms.ToolStripButton();
            this._buttonPaste = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this._buttonRename = new System.Windows.Forms.ToolStripButton();
            this._buttonEdit = new System.Windows.Forms.ToolStripButton();
            this._buttonDelete = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this._buttonAddTagByte = new System.Windows.Forms.ToolStripButton();
            this._buttonAddTagShort = new System.Windows.Forms.ToolStripButton();
            this._buttonAddTagInt = new System.Windows.Forms.ToolStripButton();
            this._buttonAddTagLong = new System.Windows.Forms.ToolStripButton();
            this._buttonAddTagFloat = new System.Windows.Forms.ToolStripButton();
            this._buttonAddTagDouble = new System.Windows.Forms.ToolStripButton();
            this._buttonAddTagByteArray = new System.Windows.Forms.ToolStripButton();
            this._buttonAddTagIntArray = new System.Windows.Forms.ToolStripButton();
            this._buttonAddTagString = new System.Windows.Forms.ToolStripButton();
            this._buttonAddTagList = new System.Windows.Forms.ToolStripButton();
            this._buttonAddTagCompound = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this._buttonFindNext = new System.Windows.Forms.ToolStripButton();
            this.BottomToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.TopToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.RightToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.LeftToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.ContentPanel = new System.Windows.Forms.ToolStripContentPanel();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.testToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._nodeTree = new NBTExplorer.Vendor.MultiSelectTreeView.MultiSelectTreeView();
            this._buttonAddTagLongArray = new System.Windows.Forms.ToolStripButton();
            this.menuStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.searchToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(619, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._menuItemOpen,
            this._menuItemOpenFolder,
            this._menuItemOpenMinecraftSaveFolder,
            this.toolStripSeparator12,
            this._menuItemOpenInExplorer,
            this.toolStripSeparator3,
            this._menuItemSave,
            this._menuItemRefresh,
            this.toolStripSeparator4,
            this._menuItemRecentFiles,
            this._menuItemRecentFolders,
            this.toolStripSeparator8,
            this._menuItemExit});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // _menuItemOpen
            // 
            this._menuItemOpen.Image = ((System.Drawing.Image)(resources.GetObject("_menuItemOpen.Image")));
            this._menuItemOpen.Name = "_menuItemOpen";
            this._menuItemOpen.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this._menuItemOpen.Size = new System.Drawing.Size(233, 22);
            this._menuItemOpen.Text = "&Open...";
            // 
            // _menuItemOpenFolder
            // 
            this._menuItemOpenFolder.Image = ((System.Drawing.Image)(resources.GetObject("_menuItemOpenFolder.Image")));
            this._menuItemOpenFolder.Name = "_menuItemOpenFolder";
            this._menuItemOpenFolder.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.O)));
            this._menuItemOpenFolder.Size = new System.Drawing.Size(233, 22);
            this._menuItemOpenFolder.Text = "Open &Folder...";
            // 
            // _menuItemOpenMinecraftSaveFolder
            // 
            this._menuItemOpenMinecraftSaveFolder.Name = "_menuItemOpenMinecraftSaveFolder";
            this._menuItemOpenMinecraftSaveFolder.Size = new System.Drawing.Size(233, 22);
            this._menuItemOpenMinecraftSaveFolder.Text = "Open &Minecraft Save Folder";
            // 
            // toolStripSeparator12
            // 
            this.toolStripSeparator12.Name = "toolStripSeparator12";
            this.toolStripSeparator12.Size = new System.Drawing.Size(230, 6);
            // 
            // _menuItemOpenInExplorer
            // 
            this._menuItemOpenInExplorer.Enabled = false;
            this._menuItemOpenInExplorer.Name = "_menuItemOpenInExplorer";
            this._menuItemOpenInExplorer.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.E)));
            this._menuItemOpenInExplorer.Size = new System.Drawing.Size(233, 22);
            this._menuItemOpenInExplorer.Text = "Open in &Explorer";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(230, 6);
            // 
            // _menuItemSave
            // 
            this._menuItemSave.Image = ((System.Drawing.Image)(resources.GetObject("_menuItemSave.Image")));
            this._menuItemSave.Name = "_menuItemSave";
            this._menuItemSave.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this._menuItemSave.Size = new System.Drawing.Size(233, 22);
            this._menuItemSave.Text = "&Save";
            // 
            // _menuItemRefresh
            // 
            this._menuItemRefresh.Image = ((System.Drawing.Image)(resources.GetObject("_menuItemRefresh.Image")));
            this._menuItemRefresh.Name = "_menuItemRefresh";
            this._menuItemRefresh.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this._menuItemRefresh.Size = new System.Drawing.Size(233, 22);
            this._menuItemRefresh.Text = "Refresh";
            this._menuItemRefresh.Click += new System.EventHandler(this.refreshToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(230, 6);
            // 
            // _menuItemRecentFiles
            // 
            this._menuItemRecentFiles.Name = "_menuItemRecentFiles";
            this._menuItemRecentFiles.Size = new System.Drawing.Size(233, 22);
            this._menuItemRecentFiles.Text = "Recent Files";
            // 
            // _menuItemRecentFolders
            // 
            this._menuItemRecentFolders.Name = "_menuItemRecentFolders";
            this._menuItemRecentFolders.Size = new System.Drawing.Size(233, 22);
            this._menuItemRecentFolders.Text = "Recent Folders";
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            this.toolStripSeparator8.Size = new System.Drawing.Size(230, 6);
            // 
            // _menuItemExit
            // 
            this._menuItemExit.Image = ((System.Drawing.Image)(resources.GetObject("_menuItemExit.Image")));
            this._menuItemExit.Name = "_menuItemExit";
            this._menuItemExit.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this._menuItemExit.Size = new System.Drawing.Size(233, 22);
            this._menuItemExit.Text = "E&xit";
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._menuItemCut,
            this._menuItemCopy,
            this._menuItemPaste,
            this.toolStripSeparator7,
            this._menuItemRename,
            this._menuItemEditValue,
            this._menuItemDelete,
            this.toolStripSeparator10,
            this._menuItemMoveUp,
            this._menuItemMoveDown});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Up)));
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "&Edit";
            // 
            // _menuItemCut
            // 
            this._menuItemCut.Image = ((System.Drawing.Image)(resources.GetObject("_menuItemCut.Image")));
            this._menuItemCut.Name = "_menuItemCut";
            this._menuItemCut.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this._menuItemCut.Size = new System.Drawing.Size(203, 22);
            this._menuItemCut.Text = "Cu&t";
            // 
            // _menuItemCopy
            // 
            this._menuItemCopy.Image = ((System.Drawing.Image)(resources.GetObject("_menuItemCopy.Image")));
            this._menuItemCopy.Name = "_menuItemCopy";
            this._menuItemCopy.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this._menuItemCopy.Size = new System.Drawing.Size(203, 22);
            this._menuItemCopy.Text = "&Copy";
            // 
            // _menuItemPaste
            // 
            this._menuItemPaste.Image = ((System.Drawing.Image)(resources.GetObject("_menuItemPaste.Image")));
            this._menuItemPaste.Name = "_menuItemPaste";
            this._menuItemPaste.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this._menuItemPaste.Size = new System.Drawing.Size(203, 22);
            this._menuItemPaste.Text = "&Paste";
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(200, 6);
            // 
            // _menuItemRename
            // 
            this._menuItemRename.Image = ((System.Drawing.Image)(resources.GetObject("_menuItemRename.Image")));
            this._menuItemRename.Name = "_menuItemRename";
            this._menuItemRename.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
            this._menuItemRename.Size = new System.Drawing.Size(203, 22);
            this._menuItemRename.Text = "&Rename";
            // 
            // _menuItemEditValue
            // 
            this._menuItemEditValue.Image = ((System.Drawing.Image)(resources.GetObject("_menuItemEditValue.Image")));
            this._menuItemEditValue.Name = "_menuItemEditValue";
            this._menuItemEditValue.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.E)));
            this._menuItemEditValue.Size = new System.Drawing.Size(203, 22);
            this._menuItemEditValue.Text = "&Edit Value";
            // 
            // _menuItemDelete
            // 
            this._menuItemDelete.Image = ((System.Drawing.Image)(resources.GetObject("_menuItemDelete.Image")));
            this._menuItemDelete.Name = "_menuItemDelete";
            this._menuItemDelete.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this._menuItemDelete.Size = new System.Drawing.Size(203, 22);
            this._menuItemDelete.Text = "&Delete";
            // 
            // toolStripSeparator10
            // 
            this.toolStripSeparator10.Name = "toolStripSeparator10";
            this.toolStripSeparator10.Size = new System.Drawing.Size(200, 6);
            // 
            // _menuItemMoveUp
            // 
            this._menuItemMoveUp.Image = ((System.Drawing.Image)(resources.GetObject("_menuItemMoveUp.Image")));
            this._menuItemMoveUp.Name = "_menuItemMoveUp";
            this._menuItemMoveUp.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Up)));
            this._menuItemMoveUp.Size = new System.Drawing.Size(203, 22);
            this._menuItemMoveUp.Text = "Move &Up";
            this._menuItemMoveUp.Click += new System.EventHandler(this._menuItemMoveUp_Click);
            // 
            // _menuItemMoveDown
            // 
            this._menuItemMoveDown.Image = ((System.Drawing.Image)(resources.GetObject("_menuItemMoveDown.Image")));
            this._menuItemMoveDown.Name = "_menuItemMoveDown";
            this._menuItemMoveDown.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Down)));
            this._menuItemMoveDown.Size = new System.Drawing.Size(203, 22);
            this._menuItemMoveDown.Text = "Move Do&wn";
            this._menuItemMoveDown.Click += new System.EventHandler(this._menuItemMoveDown_Click);
            // 
            // searchToolStripMenuItem
            // 
            this.searchToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._menuItemFind,
            this._menuItemFindNext,
            this.toolStripSeparator9,
            this.replaceToolStripMenuItem,
            this.toolStripSeparator11,
            this.findBlockToolStripMenuItem});
            this.searchToolStripMenuItem.Name = "searchToolStripMenuItem";
            this.searchToolStripMenuItem.Size = new System.Drawing.Size(54, 20);
            this.searchToolStripMenuItem.Text = "&Search";
            // 
            // _menuItemFind
            // 
            this._menuItemFind.Image = ((System.Drawing.Image)(resources.GetObject("_menuItemFind.Image")));
            this._menuItemFind.Name = "_menuItemFind";
            this._menuItemFind.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
            this._menuItemFind.Size = new System.Drawing.Size(167, 22);
            this._menuItemFind.Text = "&Find...";
            // 
            // _menuItemFindNext
            // 
            this._menuItemFindNext.Image = ((System.Drawing.Image)(resources.GetObject("_menuItemFindNext.Image")));
            this._menuItemFindNext.Name = "_menuItemFindNext";
            this._menuItemFindNext.ShortcutKeys = System.Windows.Forms.Keys.F3;
            this._menuItemFindNext.Size = new System.Drawing.Size(167, 22);
            this._menuItemFindNext.Text = "Find &Next";
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            this.toolStripSeparator9.Size = new System.Drawing.Size(164, 6);
            // 
            // replaceToolStripMenuItem
            // 
            this.replaceToolStripMenuItem.Name = "replaceToolStripMenuItem";
            this.replaceToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.H)));
            this.replaceToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.replaceToolStripMenuItem.Text = "&Replace...";
            this.replaceToolStripMenuItem.Click += new System.EventHandler(this.replaceToolStripMenuItem_Click);
            // 
            // toolStripSeparator11
            // 
            this.toolStripSeparator11.Name = "toolStripSeparator11";
            this.toolStripSeparator11.Size = new System.Drawing.Size(164, 6);
            // 
            // findBlockToolStripMenuItem
            // 
            this.findBlockToolStripMenuItem.Name = "findBlockToolStripMenuItem";
            this.findBlockToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.findBlockToolStripMenuItem.Text = "&Chunk Finder...";
            this.findBlockToolStripMenuItem.Click += new System.EventHandler(this.findBlockToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._menuItemAbout});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // _menuItemAbout
            // 
            this._menuItemAbout.Image = ((System.Drawing.Image)(resources.GetObject("_menuItemAbout.Image")));
            this._menuItemAbout.Name = "_menuItemAbout";
            this._menuItemAbout.ShortcutKeys = System.Windows.Forms.Keys.F1;
            this._menuItemAbout.Size = new System.Drawing.Size(126, 22);
            this._menuItemAbout.Text = "&About";
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "document-attribute-b.png");
            this.imageList1.Images.SetKeyName(1, "document-attribute-s.png");
            this.imageList1.Images.SetKeyName(2, "document-attribute-i.png");
            this.imageList1.Images.SetKeyName(3, "document-attribute-l.png");
            this.imageList1.Images.SetKeyName(4, "document-attribute-f.png");
            this.imageList1.Images.SetKeyName(5, "document-attribute-d.png");
            this.imageList1.Images.SetKeyName(6, "edit-code.png");
            this.imageList1.Images.SetKeyName(7, "edit-small-caps.png");
            this.imageList1.Images.SetKeyName(8, "edit-list.png");
            this.imageList1.Images.SetKeyName(9, "box.png");
            this.imageList1.Images.SetKeyName(10, "folder.png");
            this.imageList1.Images.SetKeyName(11, "block.png");
            this.imageList1.Images.SetKeyName(12, "wooden-box.png");
            this.imageList1.Images.SetKeyName(13, "map.png");
            this.imageList1.Images.SetKeyName(14, "edit-code-i.png");
            this.imageList1.Images.SetKeyName(15, "question-white.png");
            this.imageList1.Images.SetKeyName(16, "edit-code-s.png");
            this.imageList1.Images.SetKeyName(17, "edit-code-l.png");
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._buttonOpen,
            this._buttonOpenFolder,
            this._buttonSave,
            this._buttonRefresh,
            this.toolStripSeparator1,
            this._buttonCut,
            this._buttonCopy,
            this._buttonPaste,
            this.toolStripSeparator6,
            this._buttonRename,
            this._buttonEdit,
            this._buttonDelete,
            this.toolStripSeparator2,
            this._buttonAddTagByte,
            this._buttonAddTagShort,
            this._buttonAddTagInt,
            this._buttonAddTagLong,
            this._buttonAddTagFloat,
            this._buttonAddTagDouble,
            this._buttonAddTagByteArray,
            this._buttonAddTagIntArray,
            this._buttonAddTagLongArray,
            this._buttonAddTagString,
            this._buttonAddTagList,
            this._buttonAddTagCompound,
            this.toolStripSeparator5,
            this._buttonFindNext});
            this.toolStrip1.Location = new System.Drawing.Point(0, 24);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(619, 25);
            this.toolStrip1.Stretch = true;
            this.toolStrip1.TabIndex = 0;
            // 
            // _buttonOpen
            // 
            this._buttonOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._buttonOpen.Image = ((System.Drawing.Image)(resources.GetObject("_buttonOpen.Image")));
            this._buttonOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._buttonOpen.Name = "_buttonOpen";
            this._buttonOpen.Size = new System.Drawing.Size(23, 22);
            this._buttonOpen.Text = "Open NBT Data Source";
            // 
            // _buttonOpenFolder
            // 
            this._buttonOpenFolder.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._buttonOpenFolder.Image = ((System.Drawing.Image)(resources.GetObject("_buttonOpenFolder.Image")));
            this._buttonOpenFolder.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._buttonOpenFolder.Name = "_buttonOpenFolder";
            this._buttonOpenFolder.Size = new System.Drawing.Size(23, 22);
            this._buttonOpenFolder.Text = "Open Folder";
            // 
            // _buttonSave
            // 
            this._buttonSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._buttonSave.Image = ((System.Drawing.Image)(resources.GetObject("_buttonSave.Image")));
            this._buttonSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._buttonSave.Name = "_buttonSave";
            this._buttonSave.Size = new System.Drawing.Size(23, 22);
            this._buttonSave.Text = "Save All Modified Tags";
            // 
            // _buttonRefresh
            // 
            this._buttonRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._buttonRefresh.Image = ((System.Drawing.Image)(resources.GetObject("_buttonRefresh.Image")));
            this._buttonRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._buttonRefresh.Name = "_buttonRefresh";
            this._buttonRefresh.Size = new System.Drawing.Size(23, 22);
            this._buttonRefresh.Text = "Refresh Content From Disk";
            this._buttonRefresh.Click += new System.EventHandler(this._buttonRefresh_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // _buttonCut
            // 
            this._buttonCut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._buttonCut.Image = ((System.Drawing.Image)(resources.GetObject("_buttonCut.Image")));
            this._buttonCut.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._buttonCut.Name = "_buttonCut";
            this._buttonCut.Size = new System.Drawing.Size(23, 22);
            this._buttonCut.Text = "Cut";
            // 
            // _buttonCopy
            // 
            this._buttonCopy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._buttonCopy.Image = ((System.Drawing.Image)(resources.GetObject("_buttonCopy.Image")));
            this._buttonCopy.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._buttonCopy.Name = "_buttonCopy";
            this._buttonCopy.Size = new System.Drawing.Size(23, 22);
            this._buttonCopy.Text = "Copy";
            // 
            // _buttonPaste
            // 
            this._buttonPaste.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._buttonPaste.Image = ((System.Drawing.Image)(resources.GetObject("_buttonPaste.Image")));
            this._buttonPaste.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._buttonPaste.Name = "_buttonPaste";
            this._buttonPaste.Size = new System.Drawing.Size(23, 22);
            this._buttonPaste.Text = "Paste";
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(6, 25);
            // 
            // _buttonRename
            // 
            this._buttonRename.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._buttonRename.Image = ((System.Drawing.Image)(resources.GetObject("_buttonRename.Image")));
            this._buttonRename.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._buttonRename.Name = "_buttonRename";
            this._buttonRename.Size = new System.Drawing.Size(23, 22);
            this._buttonRename.Text = "Rename Tag";
            // 
            // _buttonEdit
            // 
            this._buttonEdit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._buttonEdit.Image = ((System.Drawing.Image)(resources.GetObject("_buttonEdit.Image")));
            this._buttonEdit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._buttonEdit.Name = "_buttonEdit";
            this._buttonEdit.Size = new System.Drawing.Size(23, 22);
            this._buttonEdit.Text = "Edit Tag Value";
            // 
            // _buttonDelete
            // 
            this._buttonDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._buttonDelete.Image = ((System.Drawing.Image)(resources.GetObject("_buttonDelete.Image")));
            this._buttonDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._buttonDelete.Name = "_buttonDelete";
            this._buttonDelete.Size = new System.Drawing.Size(23, 22);
            this._buttonDelete.Text = "Delete Tag";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // _buttonAddTagByte
            // 
            this._buttonAddTagByte.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._buttonAddTagByte.Image = ((System.Drawing.Image)(resources.GetObject("_buttonAddTagByte.Image")));
            this._buttonAddTagByte.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._buttonAddTagByte.Name = "_buttonAddTagByte";
            this._buttonAddTagByte.Size = new System.Drawing.Size(23, 22);
            this._buttonAddTagByte.Text = "Add Byte Tag";
            // 
            // _buttonAddTagShort
            // 
            this._buttonAddTagShort.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._buttonAddTagShort.Image = ((System.Drawing.Image)(resources.GetObject("_buttonAddTagShort.Image")));
            this._buttonAddTagShort.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._buttonAddTagShort.Name = "_buttonAddTagShort";
            this._buttonAddTagShort.Size = new System.Drawing.Size(23, 22);
            this._buttonAddTagShort.Text = "Add Short Tag";
            // 
            // _buttonAddTagInt
            // 
            this._buttonAddTagInt.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._buttonAddTagInt.Image = ((System.Drawing.Image)(resources.GetObject("_buttonAddTagInt.Image")));
            this._buttonAddTagInt.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._buttonAddTagInt.Name = "_buttonAddTagInt";
            this._buttonAddTagInt.Size = new System.Drawing.Size(23, 22);
            this._buttonAddTagInt.Text = "Add Int Tag";
            // 
            // _buttonAddTagLong
            // 
            this._buttonAddTagLong.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._buttonAddTagLong.Image = ((System.Drawing.Image)(resources.GetObject("_buttonAddTagLong.Image")));
            this._buttonAddTagLong.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._buttonAddTagLong.Name = "_buttonAddTagLong";
            this._buttonAddTagLong.Size = new System.Drawing.Size(23, 22);
            this._buttonAddTagLong.Text = "Add Long Tag";
            // 
            // _buttonAddTagFloat
            // 
            this._buttonAddTagFloat.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._buttonAddTagFloat.Image = ((System.Drawing.Image)(resources.GetObject("_buttonAddTagFloat.Image")));
            this._buttonAddTagFloat.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._buttonAddTagFloat.Name = "_buttonAddTagFloat";
            this._buttonAddTagFloat.Size = new System.Drawing.Size(23, 22);
            this._buttonAddTagFloat.Text = "Add Float Tag";
            // 
            // _buttonAddTagDouble
            // 
            this._buttonAddTagDouble.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._buttonAddTagDouble.Image = ((System.Drawing.Image)(resources.GetObject("_buttonAddTagDouble.Image")));
            this._buttonAddTagDouble.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._buttonAddTagDouble.Name = "_buttonAddTagDouble";
            this._buttonAddTagDouble.Size = new System.Drawing.Size(23, 22);
            this._buttonAddTagDouble.Text = "Add Double Tag";
            // 
            // _buttonAddTagByteArray
            // 
            this._buttonAddTagByteArray.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._buttonAddTagByteArray.Image = ((System.Drawing.Image)(resources.GetObject("_buttonAddTagByteArray.Image")));
            this._buttonAddTagByteArray.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._buttonAddTagByteArray.Name = "_buttonAddTagByteArray";
            this._buttonAddTagByteArray.Size = new System.Drawing.Size(23, 22);
            this._buttonAddTagByteArray.Text = "Add Byte Array Tag";
            // 
            // _buttonAddTagIntArray
            // 
            this._buttonAddTagIntArray.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._buttonAddTagIntArray.Image = ((System.Drawing.Image)(resources.GetObject("_buttonAddTagIntArray.Image")));
            this._buttonAddTagIntArray.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this._buttonAddTagIntArray.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._buttonAddTagIntArray.Name = "_buttonAddTagIntArray";
            this._buttonAddTagIntArray.Size = new System.Drawing.Size(23, 22);
            this._buttonAddTagIntArray.Text = "Add Int Array Tag";
            // 
            // _buttonAddTagString
            // 
            this._buttonAddTagString.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._buttonAddTagString.Image = ((System.Drawing.Image)(resources.GetObject("_buttonAddTagString.Image")));
            this._buttonAddTagString.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._buttonAddTagString.Name = "_buttonAddTagString";
            this._buttonAddTagString.Size = new System.Drawing.Size(23, 22);
            this._buttonAddTagString.Text = "Add String Tag";
            // 
            // _buttonAddTagList
            // 
            this._buttonAddTagList.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._buttonAddTagList.Image = ((System.Drawing.Image)(resources.GetObject("_buttonAddTagList.Image")));
            this._buttonAddTagList.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._buttonAddTagList.Name = "_buttonAddTagList";
            this._buttonAddTagList.Size = new System.Drawing.Size(23, 22);
            this._buttonAddTagList.Text = "Add List Tag";
            // 
            // _buttonAddTagCompound
            // 
            this._buttonAddTagCompound.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._buttonAddTagCompound.Image = ((System.Drawing.Image)(resources.GetObject("_buttonAddTagCompound.Image")));
            this._buttonAddTagCompound.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._buttonAddTagCompound.Name = "_buttonAddTagCompound";
            this._buttonAddTagCompound.Size = new System.Drawing.Size(23, 22);
            this._buttonAddTagCompound.Text = "Add Compound Tag";
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(6, 25);
            // 
            // _buttonFindNext
            // 
            this._buttonFindNext.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._buttonFindNext.Image = ((System.Drawing.Image)(resources.GetObject("_buttonFindNext.Image")));
            this._buttonFindNext.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._buttonFindNext.Name = "_buttonFindNext";
            this._buttonFindNext.Size = new System.Drawing.Size(23, 22);
            this._buttonFindNext.Text = "Find / Find Next";
            // 
            // BottomToolStripPanel
            // 
            this.BottomToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.BottomToolStripPanel.Name = "BottomToolStripPanel";
            this.BottomToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.BottomToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.BottomToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // TopToolStripPanel
            // 
            this.TopToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.TopToolStripPanel.Name = "TopToolStripPanel";
            this.TopToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.TopToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.TopToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // RightToolStripPanel
            // 
            this.RightToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.RightToolStripPanel.Name = "RightToolStripPanel";
            this.RightToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.RightToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.RightToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // LeftToolStripPanel
            // 
            this.LeftToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.LeftToolStripPanel.Name = "LeftToolStripPanel";
            this.LeftToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.LeftToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.LeftToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // ContentPanel
            // 
            this.ContentPanel.Size = new System.Drawing.Size(562, 376);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.testToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(96, 26);
            // 
            // testToolStripMenuItem
            // 
            this.testToolStripMenuItem.Name = "testToolStripMenuItem";
            this.testToolStripMenuItem.Size = new System.Drawing.Size(95, 22);
            this.testToolStripMenuItem.Text = "Test";
            // 
            // _nodeTree
            // 
            this._nodeTree.AllowDrop = true;
            this._nodeTree.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._nodeTree.ImageIndex = 0;
            this._nodeTree.ImageList = this.imageList1;
            this._nodeTree.ItemHeight = 18;
            this._nodeTree.Location = new System.Drawing.Point(0, 49);
            this._nodeTree.Margin = new System.Windows.Forms.Padding(0);
            this._nodeTree.Name = "_nodeTree";
            this._nodeTree.SelectedImageIndex = 0;
            this._nodeTree.SelectedNodes = ((System.Collections.Generic.List<System.Windows.Forms.TreeNode>)(resources.GetObject("_nodeTree.SelectedNodes")));
            this._nodeTree.Size = new System.Drawing.Size(619, 374);
            this._nodeTree.TabIndex = 0;
            // 
            // _buttonAddTagLongArray
            // 
            this._buttonAddTagLongArray.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._buttonAddTagLongArray.Image = ((System.Drawing.Image)(resources.GetObject("_buttonAddTagLongArray.Image")));
            this._buttonAddTagLongArray.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this._buttonAddTagLongArray.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._buttonAddTagLongArray.Name = "_buttonAddTagLongArray";
            this._buttonAddTagLongArray.Size = new System.Drawing.Size(23, 22);
            this._buttonAddTagLongArray.Text = "Add Long Array Tag";
            // 
            // MainForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(619, 423);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this._nodeTree);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "NBTExplorer";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem searchToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private NBTExplorer.Vendor.MultiSelectTreeView.MultiSelectTreeView _nodeTree;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton _buttonOpen;
        private System.Windows.Forms.ToolStripButton _buttonSave;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton _buttonRename;
        private System.Windows.Forms.ToolStripButton _buttonEdit;
        private System.Windows.Forms.ToolStripButton _buttonDelete;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton _buttonAddTagByte;
        private System.Windows.Forms.ToolStripButton _buttonAddTagShort;
        private System.Windows.Forms.ToolStripButton _buttonAddTagInt;
        private System.Windows.Forms.ToolStripButton _buttonAddTagLong;
        private System.Windows.Forms.ToolStripButton _buttonAddTagFloat;
        private System.Windows.Forms.ToolStripButton _buttonAddTagDouble;
        private System.Windows.Forms.ToolStripButton _buttonAddTagByteArray;
        private System.Windows.Forms.ToolStripButton _buttonAddTagList;
        private System.Windows.Forms.ToolStripButton _buttonAddTagCompound;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ToolStripButton _buttonAddTagString;
        private System.Windows.Forms.ToolStripMenuItem _menuItemAbout;
        private System.Windows.Forms.ToolStripMenuItem _menuItemOpen;
        private System.Windows.Forms.ToolStripMenuItem _menuItemOpenFolder;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem _menuItemSave;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem _menuItemExit;
        private System.Windows.Forms.ToolStripMenuItem _menuItemFind;
        private System.Windows.Forms.ToolStripMenuItem _menuItemFindNext;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripButton _buttonFindNext;
        private System.Windows.Forms.ToolStripButton _buttonOpenFolder;
        private System.Windows.Forms.ToolStripMenuItem _menuItemOpenMinecraftSaveFolder;
        private System.Windows.Forms.ToolStripPanel BottomToolStripPanel;
        private System.Windows.Forms.ToolStripPanel TopToolStripPanel;
        private System.Windows.Forms.ToolStripPanel RightToolStripPanel;
        private System.Windows.Forms.ToolStripPanel LeftToolStripPanel;
        private System.Windows.Forms.ToolStripContentPanel ContentPanel;
        private System.Windows.Forms.ToolStripButton _buttonAddTagIntArray;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _menuItemCut;
        private System.Windows.Forms.ToolStripMenuItem _menuItemCopy;
        private System.Windows.Forms.ToolStripMenuItem _menuItemPaste;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripMenuItem _menuItemRename;
        private System.Windows.Forms.ToolStripMenuItem _menuItemEditValue;
        private System.Windows.Forms.ToolStripMenuItem _menuItemDelete;
        private System.Windows.Forms.ToolStripButton _buttonCut;
        private System.Windows.Forms.ToolStripButton _buttonCopy;
        private System.Windows.Forms.ToolStripButton _buttonPaste;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem testToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _menuItemRecentFiles;
        private System.Windows.Forms.ToolStripMenuItem _menuItemRecentFolders;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
        private System.Windows.Forms.ToolStripButton _buttonRefresh;
        private System.Windows.Forms.ToolStripMenuItem _menuItemRefresh;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
        private System.Windows.Forms.ToolStripMenuItem replaceToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator10;
        private System.Windows.Forms.ToolStripMenuItem _menuItemMoveUp;
        private System.Windows.Forms.ToolStripMenuItem _menuItemMoveDown;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator11;
        private System.Windows.Forms.ToolStripMenuItem findBlockToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator12;
        private System.Windows.Forms.ToolStripMenuItem _menuItemOpenInExplorer;
        private System.Windows.Forms.ToolStripButton _buttonAddTagLongArray;
    }
}

