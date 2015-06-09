namespace PasswordManager
{
    partial class MainForm_PasswordManager
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm_PasswordManager));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.treeView_Folders = new System.Windows.Forms.TreeView();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.listView_PasswordItems = new System.Windows.Forms.ListView();
            this.columnHeader_caption = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader_ID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader_password = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.textBox_ItemDescription = new System.Windows.Forms.TextBox();
            this.contextMenuStrip_TreeViewNode = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ToolStripMenuItem_AddSubFolder = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_RenameFolder = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_DeleteFolder = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Seperator = new System.Windows.Forms.ToolStripSeparator();
            this.ToolStripMenuItem_AddPassword = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip_Main = new System.Windows.Forms.MenuStrip();
            this.ToolStripMenuItem_File = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_File_Open = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_File_Save = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Edit = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Edit_AddSubFolder = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Edit_RenameFolder = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Edit_DeleteFolder = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem_Edit_Seperator = new System.Windows.Forms.ToolStripSeparator();
            this.ToolStripMenuItem_Edit_AddPassword = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_option = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Language = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Language_English = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Language_Japanese = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_about = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip_Main = new System.Windows.Forms.ToolStrip();
            this.toolStripButton_Open = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton_Save = new System.Windows.Forms.ToolStripButton();
            this.statusStrip_Main = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.contextMenuStrip_TreeViewNode.SuspendLayout();
            this.menuStrip_Main.SuspendLayout();
            this.toolStrip_Main.SuspendLayout();
            this.statusStrip_Main.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(12, 52);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.treeView_Folders);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(595, 321);
            this.splitContainer1.SplitterDistance = 202;
            this.splitContainer1.TabIndex = 1;
            // 
            // treeView_Folders
            // 
            this.treeView_Folders.AllowDrop = true;
            this.treeView_Folders.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView_Folders.LabelEdit = true;
            this.treeView_Folders.Location = new System.Drawing.Point(0, 0);
            this.treeView_Folders.Name = "treeView_Folders";
            this.treeView_Folders.Size = new System.Drawing.Size(202, 321);
            this.treeView_Folders.TabIndex = 0;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.listView_PasswordItems);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.textBox_ItemDescription);
            this.splitContainer2.Size = new System.Drawing.Size(389, 321);
            this.splitContainer2.SplitterDistance = 200;
            this.splitContainer2.TabIndex = 0;
            // 
            // listView_PasswordItems
            // 
            this.listView_PasswordItems.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader_caption,
            this.columnHeader_ID,
            this.columnHeader_password});
            this.listView_PasswordItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView_PasswordItems.FullRowSelect = true;
            this.listView_PasswordItems.Location = new System.Drawing.Point(0, 0);
            this.listView_PasswordItems.Name = "listView_PasswordItems";
            this.listView_PasswordItems.Size = new System.Drawing.Size(389, 200);
            this.listView_PasswordItems.TabIndex = 0;
            this.listView_PasswordItems.UseCompatibleStateImageBehavior = false;
            this.listView_PasswordItems.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader_caption
            // 
            this.columnHeader_caption.Text = "Caption";
            this.columnHeader_caption.Width = 138;
            // 
            // columnHeader_ID
            // 
            this.columnHeader_ID.Text = "ID";
            this.columnHeader_ID.Width = 121;
            // 
            // columnHeader_password
            // 
            this.columnHeader_password.Text = "Password";
            this.columnHeader_password.Width = 106;
            // 
            // textBox_ItemDescription
            // 
            this.textBox_ItemDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox_ItemDescription.Location = new System.Drawing.Point(0, 0);
            this.textBox_ItemDescription.Multiline = true;
            this.textBox_ItemDescription.Name = "textBox_ItemDescription";
            this.textBox_ItemDescription.ReadOnly = true;
            this.textBox_ItemDescription.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox_ItemDescription.Size = new System.Drawing.Size(389, 117);
            this.textBox_ItemDescription.TabIndex = 0;
            // 
            // contextMenuStrip_TreeViewNode
            // 
            this.contextMenuStrip_TreeViewNode.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItem_AddSubFolder,
            this.ToolStripMenuItem_RenameFolder,
            this.ToolStripMenuItem_DeleteFolder,
            this.ToolStripMenuItem_Seperator,
            this.ToolStripMenuItem_AddPassword});
            this.contextMenuStrip_TreeViewNode.Name = "contextMenuStrip_TreeView";
            this.contextMenuStrip_TreeViewNode.Size = new System.Drawing.Size(160, 98);
            // 
            // ToolStripMenuItem_AddSubFolder
            // 
            this.ToolStripMenuItem_AddSubFolder.Name = "ToolStripMenuItem_AddSubFolder";
            this.ToolStripMenuItem_AddSubFolder.Size = new System.Drawing.Size(159, 22);
            this.ToolStripMenuItem_AddSubFolder.Text = "Add sub folder";
            // 
            // ToolStripMenuItem_RenameFolder
            // 
            this.ToolStripMenuItem_RenameFolder.Name = "ToolStripMenuItem_RenameFolder";
            this.ToolStripMenuItem_RenameFolder.Size = new System.Drawing.Size(159, 22);
            this.ToolStripMenuItem_RenameFolder.Text = "Rename folder";
            // 
            // ToolStripMenuItem_DeleteFolder
            // 
            this.ToolStripMenuItem_DeleteFolder.Name = "ToolStripMenuItem_DeleteFolder";
            this.ToolStripMenuItem_DeleteFolder.Size = new System.Drawing.Size(159, 22);
            this.ToolStripMenuItem_DeleteFolder.Text = "Delete folder";
            // 
            // ToolStripMenuItem_Seperator
            // 
            this.ToolStripMenuItem_Seperator.Name = "ToolStripMenuItem_Seperator";
            this.ToolStripMenuItem_Seperator.Size = new System.Drawing.Size(156, 6);
            // 
            // ToolStripMenuItem_AddPassword
            // 
            this.ToolStripMenuItem_AddPassword.Name = "ToolStripMenuItem_AddPassword";
            this.ToolStripMenuItem_AddPassword.Size = new System.Drawing.Size(159, 22);
            this.ToolStripMenuItem_AddPassword.Text = "Add password";
            // 
            // menuStrip_Main
            // 
            this.menuStrip_Main.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.menuStrip_Main.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItem_File,
            this.ToolStripMenuItem_Edit,
            this.ToolStripMenuItem_option,
            this.ToolStripMenuItem_about});
            this.menuStrip_Main.Location = new System.Drawing.Point(0, 0);
            this.menuStrip_Main.Name = "menuStrip_Main";
            this.menuStrip_Main.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.menuStrip_Main.Size = new System.Drawing.Size(619, 24);
            this.menuStrip_Main.TabIndex = 2;
            this.menuStrip_Main.Text = "menuStrip1";
            // 
            // ToolStripMenuItem_File
            // 
            this.ToolStripMenuItem_File.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItem_File_Open,
            this.ToolStripMenuItem_File_Save});
            this.ToolStripMenuItem_File.Name = "ToolStripMenuItem_File";
            this.ToolStripMenuItem_File.Size = new System.Drawing.Size(39, 20);
            this.ToolStripMenuItem_File.Text = "File";
            // 
            // ToolStripMenuItem_File_Open
            // 
            this.ToolStripMenuItem_File_Open.Name = "ToolStripMenuItem_File_Open";
            this.ToolStripMenuItem_File_Open.Size = new System.Drawing.Size(152, 22);
            this.ToolStripMenuItem_File_Open.Text = "Open file";
            // 
            // ToolStripMenuItem_File_Save
            // 
            this.ToolStripMenuItem_File_Save.Name = "ToolStripMenuItem_File_Save";
            this.ToolStripMenuItem_File_Save.Size = new System.Drawing.Size(152, 22);
            this.ToolStripMenuItem_File_Save.Text = "Save file";
            // 
            // ToolStripMenuItem_Edit
            // 
            this.ToolStripMenuItem_Edit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItem_Edit_AddSubFolder,
            this.ToolStripMenuItem_Edit_RenameFolder,
            this.ToolStripMenuItem_Edit_DeleteFolder,
            this.toolStripMenuItem_Edit_Seperator,
            this.ToolStripMenuItem_Edit_AddPassword});
            this.ToolStripMenuItem_Edit.Name = "ToolStripMenuItem_Edit";
            this.ToolStripMenuItem_Edit.Size = new System.Drawing.Size(41, 20);
            this.ToolStripMenuItem_Edit.Text = "Edit";
            // 
            // ToolStripMenuItem_Edit_AddSubFolder
            // 
            this.ToolStripMenuItem_Edit_AddSubFolder.Name = "ToolStripMenuItem_Edit_AddSubFolder";
            this.ToolStripMenuItem_Edit_AddSubFolder.Size = new System.Drawing.Size(159, 22);
            this.ToolStripMenuItem_Edit_AddSubFolder.Text = "Add sub folder";
            // 
            // ToolStripMenuItem_Edit_RenameFolder
            // 
            this.ToolStripMenuItem_Edit_RenameFolder.Name = "ToolStripMenuItem_Edit_RenameFolder";
            this.ToolStripMenuItem_Edit_RenameFolder.Size = new System.Drawing.Size(159, 22);
            this.ToolStripMenuItem_Edit_RenameFolder.Text = "Rename folder";
            // 
            // ToolStripMenuItem_Edit_DeleteFolder
            // 
            this.ToolStripMenuItem_Edit_DeleteFolder.Name = "ToolStripMenuItem_Edit_DeleteFolder";
            this.ToolStripMenuItem_Edit_DeleteFolder.Size = new System.Drawing.Size(159, 22);
            this.ToolStripMenuItem_Edit_DeleteFolder.Text = "Delete folder";
            // 
            // toolStripMenuItem_Edit_Seperator
            // 
            this.toolStripMenuItem_Edit_Seperator.Name = "toolStripMenuItem_Edit_Seperator";
            this.toolStripMenuItem_Edit_Seperator.Size = new System.Drawing.Size(156, 6);
            // 
            // ToolStripMenuItem_Edit_AddPassword
            // 
            this.ToolStripMenuItem_Edit_AddPassword.Name = "ToolStripMenuItem_Edit_AddPassword";
            this.ToolStripMenuItem_Edit_AddPassword.Size = new System.Drawing.Size(159, 22);
            this.ToolStripMenuItem_Edit_AddPassword.Text = "New password";
            // 
            // ToolStripMenuItem_option
            // 
            this.ToolStripMenuItem_option.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItem_Language});
            this.ToolStripMenuItem_option.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.ToolStripMenuItem_option.Name = "ToolStripMenuItem_option";
            this.ToolStripMenuItem_option.Size = new System.Drawing.Size(57, 20);
            this.ToolStripMenuItem_option.Text = "Option";
            // 
            // ToolStripMenuItem_Language
            // 
            this.ToolStripMenuItem_Language.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItem_Language_English,
            this.ToolStripMenuItem_Language_Japanese});
            this.ToolStripMenuItem_Language.Name = "ToolStripMenuItem_Language";
            this.ToolStripMenuItem_Language.Size = new System.Drawing.Size(152, 22);
            this.ToolStripMenuItem_Language.Text = "Language";
            // 
            // ToolStripMenuItem_Language_English
            // 
            this.ToolStripMenuItem_Language_English.Name = "ToolStripMenuItem_Language_English";
            this.ToolStripMenuItem_Language_English.Size = new System.Drawing.Size(127, 22);
            this.ToolStripMenuItem_Language_English.Text = "English";
            // 
            // ToolStripMenuItem_Language_Japanese
            // 
            this.ToolStripMenuItem_Language_Japanese.Name = "ToolStripMenuItem_Language_Japanese";
            this.ToolStripMenuItem_Language_Japanese.Size = new System.Drawing.Size(127, 22);
            this.ToolStripMenuItem_Language_Japanese.Text = "Japanese";
            // 
            // ToolStripMenuItem_about
            // 
            this.ToolStripMenuItem_about.Name = "ToolStripMenuItem_about";
            this.ToolStripMenuItem_about.Size = new System.Drawing.Size(53, 20);
            this.ToolStripMenuItem_about.Text = "About";
            // 
            // toolStrip_Main
            // 
            this.toolStrip_Main.BackColor = System.Drawing.SystemColors.Control;
            this.toolStrip_Main.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip_Main.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton_Open,
            this.toolStripButton_Save});
            this.toolStrip_Main.Location = new System.Drawing.Point(0, 24);
            this.toolStrip_Main.Name = "toolStrip_Main";
            this.toolStrip_Main.Size = new System.Drawing.Size(619, 25);
            this.toolStrip_Main.TabIndex = 3;
            // 
            // toolStripButton_Open
            // 
            this.toolStripButton_Open.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton_Open.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_Open.Image")));
            this.toolStripButton_Open.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_Open.Margin = new System.Windows.Forms.Padding(10, 1, 10, 2);
            this.toolStripButton_Open.Name = "toolStripButton_Open";
            this.toolStripButton_Open.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton_Open.Text = "toolStripButton1";
            // 
            // toolStripButton_Save
            // 
            this.toolStripButton_Save.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton_Save.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_Save.Image")));
            this.toolStripButton_Save.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_Save.Name = "toolStripButton_Save";
            this.toolStripButton_Save.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton_Save.Text = "toolStripButton2";
            // 
            // statusStrip_Main
            // 
            this.statusStrip_Main.BackColor = System.Drawing.Color.Transparent;
            this.statusStrip_Main.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.statusStrip_Main.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip_Main.Location = new System.Drawing.Point(0, 376);
            this.statusStrip_Main.Name = "statusStrip_Main";
            this.statusStrip_Main.Size = new System.Drawing.Size(619, 22);
            this.statusStrip_Main.TabIndex = 4;
            this.statusStrip_Main.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(43, 17);
            this.toolStripStatusLabel1.Text = "Ready";
            // 
            // MainForm_PasswordManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(619, 398);
            this.Controls.Add(this.statusStrip_Main);
            this.Controls.Add(this.toolStrip_Main);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.menuStrip_Main);
            this.MainMenuStrip = this.menuStrip_Main;
            this.Name = "MainForm_PasswordManager";
            this.Text = "PasswordManager";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            this.splitContainer2.ResumeLayout(false);
            this.contextMenuStrip_TreeViewNode.ResumeLayout(false);
            this.menuStrip_Main.ResumeLayout(false);
            this.menuStrip_Main.PerformLayout();
            this.toolStrip_Main.ResumeLayout(false);
            this.toolStrip_Main.PerformLayout();
            this.statusStrip_Main.ResumeLayout(false);
            this.statusStrip_Main.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TreeView treeView_Folders;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.TextBox textBox_ItemDescription;
        private System.Windows.Forms.MenuStrip menuStrip_Main;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_option;
        private System.Windows.Forms.ListView listView_PasswordItems;
        private System.Windows.Forms.ToolStrip toolStrip_Main;
        private System.Windows.Forms.ToolStripButton toolStripButton_Open;
        private System.Windows.Forms.StatusStrip statusStrip_Main;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_about;
        private System.Windows.Forms.ColumnHeader columnHeader_caption;
        private System.Windows.Forms.ColumnHeader columnHeader_ID;
        private System.Windows.Forms.ColumnHeader columnHeader_password;
        private System.Windows.Forms.ToolStripButton toolStripButton_Save;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Language;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Language_English;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Language_Japanese;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip_TreeViewNode;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_AddSubFolder;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_RenameFolder;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_DeleteFolder;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_AddPassword;
        private System.Windows.Forms.ToolStripSeparator ToolStripMenuItem_Seperator;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_File;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_File_Open;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_File_Save;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Edit;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Edit_AddSubFolder;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Edit_RenameFolder;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Edit_DeleteFolder;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_Edit_Seperator;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Edit_AddPassword;





    }
}

