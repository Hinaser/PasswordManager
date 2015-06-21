#region Notice
/*
 * Author: Yunoske
 * Create Date: May 27, 2015
 * Description :
 * 
 */
#endregion

#region Using statements
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Globalization;
using System.IO;
#endregion

namespace PasswordManager
{
    public partial class FormMain : Form
    {
        #region Fields
        private PasswordFileBody PasswordData = new PasswordFileBody();
        private TreeNode CurrentTreeNode = null;
        private List<string> FilterOrder = new List<string>();
        private byte[] MasterPasswordHash;
        private string CurrentPasswordFilePath;
        #endregion

        #region Constructor
        public FormMain()
        {
            InitializeComponent();

            // Initialize form element
            this.Initialize();

            // ListView event
            this.listView_PasswordItems.SizeChanged += listView_PasswordItems_SizeChanged;
            this.listView_PasswordItems.ColumnWidthChanged += listView_PasswordItems_ColumnWidthChanged;
            this.listView_PasswordItems.SelectedIndexChanged += listView_PasswordItems_SelectedIndexChanged;
            this.listView_PasswordItems.MouseUp += listView_PasswordItems_MouseUp;
            this.listView_PasswordItems.KeyUp += listView_PasswordItems_KeyUp;
            this.listView_PasswordItems.ItemDrag += listView_PasswordItems_ItemDrag;
            this.listView_PasswordItems.MouseDoubleClick += listView_PasswordItems_MouseDoubleClick;
            // TreeView event
            this.treeView_Folders.AfterSelect += treeView_Folders_AfterSelect;
            this.treeView_Folders.NodeMouseClick += treeView_Folders_NodeMouseClick;
            this.treeView_Folders.KeyUp += treeView_Folders_KeyUp;
            this.treeView_Folders.AfterLabelEdit += treeView_Folders_AfterLabelEdit;
            this.treeView_Folders.ItemDrag += treeView_Folders_ItemDrag;
            this.treeView_Folders.DragEnter += treeView_Folders_DragEnter;
            this.treeView_Folders.DragDrop += treeView_Folders_DragDrop;
            this.treeView_Folders.DragOver += treeView_Folders_DragOver;
            // Tooltip menu botton event
            this.toolStripButton_Open.Click += toolStripButton_Open_Click;
            this.toolStripButton_Save.Click += toolStripButton_Save_Click;
            this.toolStripButton_ExpandTree.Click += toolStripButton_ExpandTree_Click;
            this.toolStripButton_CollapseTree.Click += toolStripButton_CollapseTree_Click;
            // Context menu event
            this.ToolStripMenuItem_AddSubFolder.Click += ToolStripMenuItem_AddSubFolder_Click;
            this.ToolStripMenuItem_RenameFolder.Click += ToolStripMenuItem_RenameFolder_Click;
            this.ToolStripMenuItem_DeleteFolder.Click += ToolStripMenuItem_DeleteFolder_Click;
            this.ToolStripMenuItem_AddPassword.Click += ToolStripMenuItem_AddPassword_Click;
            this.ToolStripMenuItem_ListView_New.Click += ToolStripMenuItem_AddPassword_Click;
            this.ToolStripMenuItem_ListViewItem_New.Click += ToolStripMenuItem_AddPassword_Click;
            this.ToolStripMenuItem_ListViewItem_Edit.Click += ToolStripMenuItem_ListViewItem_Edit_Click;
            this.ToolStripMenuItem_ListViewItem_Move.Click += ToolStripMenuItem_ListViewItem_Move_Click;
            this.ToolStripMenuItem_ListViewItem_Delete.Click += ToolStripMenuItem_ListViewItem_Delete_Click;
            // Menu item click event
            this.ToolStripMenuItem_Language_English.Click += ToolStripMenuItem_Language_English_Click;
            this.ToolStripMenuItem_Language_Japanese.Click += ToolStripMenuItem_Language_Japanese_Click;
            this.ToolStripMenuItem_ChangeMasterPassword.Click += ToolStripMenuItem_ChangeMasterPassword_Click;
            this.ToolStripMenuItem_File_Open.Click += ToolStripMenuItem_File_Open_Click;
            this.ToolStripMenuItem_File_SaveAs.Click += ToolStripMenuItem_File_SaveAs_Click;
            this.ToolStripMenuItem_File_Save.Click += ToolStripMenuItem_File_Save_Click;

            // To change edit menu showing items, judging by last focused control(TreeView or ListView)
            this.ToolStripMenuItem_Edit.MouseDown += ToolStripMenuItem_Edit_MouseDown;

            // Add common menu tool script for editing to "Edit" menu
            this.ToolStripMenuItem_Edit_AddSubFolder.Click += ToolStripMenuItem_AddSubFolder_Click;
            this.ToolStripMenuItem_Edit_RenameFolder.Click += ToolStripMenuItem_RenameFolder_Click;
            this.ToolStripMenuItem_Edit_DeleteFolder.Click += ToolStripMenuItem_DeleteFolder_Click;
            this.ToolStripMenuItem_Edit_AddPassword.Click += ToolStripMenuItem_AddPassword_Click;

            // General keyboard shortcut events
            this.KeyDown += FormMain_KeyDown;
        }
        #endregion

        #region Initialize
        /// <summary>
        /// Initialize form element
        /// </summary>
        void Initialize()
        {
            // Apply language setting
            this.SetupLanguage(InternalApplicationConfig.DefaultLocale);

            // Set default filter order
            this.FilterOrder.Add(typeof(DebugFilter).ToString());

            // Set default master password hash
            this.MasterPasswordHash = Utility.GetHash(InternalApplicationConfig.DefaultMasterPassword.ToCharArray());

            // Try to open password file in the default location
            if (File.Exists(InternalApplicationConfig.DefaultPasswordFilePath))
            {
                bool validPasswordEntered = false;

                // When the password file can be opened by default password, go without prompting masterpassword to user.
                // Expected possible cases are as follows:
                //   - Default master password is OK
                //       -> Keep using default master password  as a master password.
                //   - User input master password is OK
                //       -> Stored master password hash will be replaced by user input password.
                //   - User gives up to enter master password
                //       -> Stored master password hash is kept as default master password.
                //            In this case, this app do not try to open password file but open new password manager instance on memory.
                //            Then user can save the instance to new password file. (Existing password file shouldn't be overwritten.)
                if (PasswordFile.ChallengeHashedMasterPassword(InternalApplicationConfig.DefaultPasswordFilePath, this.MasterPasswordHash))
                {
                    validPasswordEntered = true;
                }
                else
                {
                    // Ask master password hash
                    // Input password will be sanitised after exiting this using statement by user-defined Dispose() method on FormInputMasterPassword class.
                    using (FormInputMasterPassword form = new FormInputMasterPassword(PasswordFile.ChallengeHashedMasterPassword, InternalApplicationConfig.DefaultPasswordFilePath))
                    {
                        form.StartPosition = FormStartPosition.CenterScreen;
                        DialogResult result = form.ShowDialog();

                        // Only when valid master password has been entered, master password of class member field will be updated.
                        if (result == DialogResult.OK)
                        {
                            this.MasterPasswordHash = form.GetMasterPasswordHash();
                            validPasswordEntered = true;
                        }
                        // Master password of class member field remains default password.
                        else
                        {
                            validPasswordEntered = false;
                        }
                    }
                }

                if (validPasswordEntered)
                {
                    try
                    {
                        // Try to parse specified password file
                        PasswordFile f = new PasswordFile(InternalApplicationConfig.DefaultPasswordFilePath);
                        this.PasswordData = f.ReadPasswordFromFile(this.MasterPasswordHash);

                        // Set password file information
                        this.CurrentPasswordFilePath = InternalApplicationConfig.DefaultPasswordFilePath;
                    }
                    catch (InvalidMasterPasswordException)
                    {
                        MessageBox.Show(strings.General_InvalidMasterPassword_Text, strings.General_InvalidMasterPassword_Caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    catch (NoCorrespondingFilterFoundException e)
                    {
                        string msgText = String.Format(strings.General_ParseFilterFailed_Text, e.Message);
                        MessageBox.Show(msgText, strings.General_ParseFilterFailed_Caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    catch
                    {
                        ;
                    }
                }
            }

            this.InitializeTreeStructure(this.PasswordData.Containers, this.PasswordData.Indexer);
            this.treeView_Folders.Invalidate();
            this.listView_PasswordItems.Invalidate();
            this.SetFileLoadStatusLabel();
        }
        #endregion

        #region Event

        #region listview event
        /// <summary>
        /// Adjust column size when parent listview size is changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void listView_PasswordItems_SizeChanged(object sender, EventArgs e)
        {
            this.AdjustColumnSize((ListView)sender, new ColumnWidthChangedEventArgs(0));
        }

        /// <summary>
        /// Adjust column size when it is changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void listView_PasswordItems_ColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e)
        {
            this.AdjustColumnSize((ListView)sender, e);
        }

        /// <summary>
        /// When selected item is changed(not meaning deselected), show corresponding comment for selected password record
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void listView_PasswordItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Get selected listview item collection
            ListView.SelectedListViewItemCollection selectedItem = this.listView_PasswordItems.SelectedItems;

            // When multiple items are selected or no items are selected, do nothing
            if (selectedItem.Count != 1)
            {
                return;
            }

            // Get selected listview item
            ListViewItem selectedListViewItem = selectedItem[0];
            if (selectedListViewItem.Tag == null)
            {
                throw new InvalidOperationException();
            }

            int recordID = (int)selectedListViewItem.Tag;

            string comment = this.PasswordData.Indexer.GetRecordByID(this.PasswordData.Records, recordID).GetDescription();
            if (comment == null) comment = String.Empty;

            this.textBox_ItemDescription.Text = comment;

            return;
        }

        /// <summary>
        /// For right click context menu for list view.
        /// This method is writter in order to separete menu content between clicked on empty area on listview or selected listview item.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void listView_PasswordItems_MouseUp(object sender, MouseEventArgs e)
        {
            // Show right-click menu
            if (e.Button == MouseButtons.Right)
            {
                // When any records are not focused
                if (this.listView_PasswordItems.FocusedItem == null)
                {
                    contextMenuStrip_ListView.Show(Cursor.Position);
                    return;
                }

                if (this.listView_PasswordItems.FocusedItem.Bounds.Contains(e.Location) == true)
                {
                    this.contextMenuStrip_ListViewItem.Items.Clear();
                    this.contextMenuStrip_ListViewItem.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                        this.ToolStripMenuItem_ListViewItem_New,
                        this.ToolStripMenuItem_ListViewItem_Edit,
                        this.ToolStripMenuItem_ListViewItem_Move,
                        this.ToolStripMenuItem_ListViewItem_Delete
                    });
                    contextMenuStrip_ListViewItem.Show(Cursor.Position);
                }
                else
                {
                    contextMenuStrip_ListView.Show(Cursor.Position);
                }

                return;
            }

            return;
        }

        /// <summary>
        /// Edit existing password record
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ToolStripMenuItem_ListViewItem_Edit_Click(object sender, EventArgs e)
        {
            // Do nothing if no tree nodes are selected
            if (this.CurrentTreeNode == null || this.CurrentTreeNode.Tag == null || this.CurrentTreeNode.Tag == null)
            {
                return;
            }

            // Do nothiing if no list item is selected or multiple items are selected
            if (this.listView_PasswordItems.SelectedItems.Count != 1)
            {
                return;
            }
            ListViewItem item = this.listView_PasswordItems.SelectedItems[0];
            if (this.listView_PasswordItems.SelectedItems[0].Tag == null)
            {
                return;
            }
            int recordID = (int)this.listView_PasswordItems.SelectedItems[0].Tag;

            PasswordRecord record = this.PasswordData.Indexer.GetRecordByID(this.PasswordData.Records, recordID);
            if (record == null)
            {
                throw new InvalidOperationException();
            }

            // Create form instance
            string caption;
            using (FormCreatePassword form = new FormCreatePassword())
            {
                form.StartPosition = FormStartPosition.CenterParent;
                form.Text = strings.Form_UpdatePassword_Title;

                // Pass current password record reference
                form.SetPasswordData(record);

                // Save caption string for it is cleared by unknown reason
                caption = String.Copy(record.GetCaption());

                // Activate dialog
                if (form.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                //record = form.GetPassword(); // For C#, this would be redundant because all object arguments are passed by reference
            }

            // If caption is empty, recover saved caption text and exit
            if (String.IsNullOrEmpty(record.GetCaption()))
            {
                record.SetCaption(caption);
                return;
            }

            // Refresh listview
            TreeViewEventArgs tvea = new TreeViewEventArgs(this.CurrentTreeNode);
            this.treeView_Folders_AfterSelect(null, tvea);
        }

        /// <summary>
        /// Move selected password(s) to specified folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ToolStripMenuItem_ListViewItem_Move_Click(object sender, EventArgs e)
        {
            // Do nothiing if no list item is selected
            if (this.listView_PasswordItems.SelectedItems.Count < 1)
            {
                return;
            }
            int[] recordIDs = new int[this.listView_PasswordItems.SelectedItems.Count];
            for(int i=0;i<recordIDs.Length;i++)
            {
                recordIDs[i] = (int)this.listView_PasswordItems.SelectedItems[i].Tag; // If tag is null, NullReferenceException will be thrown
            }

            FormMovePassword form = new FormMovePassword(this.PasswordData.Containers, this.PasswordData.Indexer);
            form.StartPosition = FormStartPosition.CenterParent;

            // Show dialog. When result is not OK, then exit
            if (form.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            // This may throw when targetContainerID is not set
            int destContainerID = form.GetTargetContainerID();

            // Move all records to designated folder
            List<string> failedRecordCaptions = new List<string>();
            foreach (int recordID in recordIDs)
            {
                // In case current selected containerID = destination containerID, skip it
                int currentContainerID = this.PasswordData.Indexer.GetParentContainerOfRecord(recordID);
                if (destContainerID == currentContainerID)
                {
                    continue;
                }

                // If it fails to move, save the record information
                if (!this.PasswordData.Indexer.MoveRecord(recordID, destContainerID))
                {
                    string caption = this.PasswordData.Indexer.GetRecordByID(this.PasswordData.Records, recordID).GetCaption();
                    if (String.IsNullOrEmpty(caption))
                    {
                        caption = recordID.ToString();
                    }
                    failedRecordCaptions.Add(caption);
                }
            }

            // If some of records were failed to move, show warning message
            if (failedRecordCaptions.Count > 1)
            {
                string recordList = String.Join(InternalApplicationConfig.Separater1, failedRecordCaptions.ToArray());
                string destContainerCaption = this.PasswordData.Indexer.GetContainerByID(this.PasswordData.Containers, destContainerID).GetLabel();
                string warnText = String.Format(strings.General_MovePassword_Fail_Text, recordList, destContainerCaption);
                MessageBox.Show(warnText, strings.General_MovePassword_Fail_Caption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            // Refresh listview
            TreeViewEventArgs tvea = new TreeViewEventArgs(this.CurrentTreeNode);
            this.treeView_Folders_AfterSelect(null, tvea);
        }

        /// <summary>
        /// Delete selected password record
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ToolStripMenuItem_ListViewItem_Delete_Click(object sender, EventArgs e)
        {
            // Check current folder
            if (this.CurrentTreeNode == null || this.CurrentTreeNode.Tag == null)
            {
                return;
            }
            int containerID = (int)this.CurrentTreeNode.Tag;
            PasswordIndexerBase indexer = this.PasswordData.Indexer;

            // Check selected record
            if (this.listView_PasswordItems.SelectedItems.Count < 1)
            {
                return;
            }

            // Check and get deleting pasword records
            List<PasswordRecord> deletingRecords = new List<PasswordRecord>();
            List<string> deletingRecordsCaptions = new List<string>();
            foreach (ListViewItem lvi in this.listView_PasswordItems.SelectedItems)
            {
                if (lvi.Tag == null)
                {
                    throw new NullReferenceException();
                }

                int recordID = (int)lvi.Tag;
                PasswordRecord deletingRecord = indexer.GetRecordByID(this.PasswordData.Records, recordID);
                if (deletingRecord == null)
                {
                    throw new InvalidOperationException();
                }

                deletingRecords.Add(deletingRecord);
                deletingRecordsCaptions.Add(deletingRecord.GetCaption());
            }

            if (deletingRecords.Count != deletingRecordsCaptions.Count || deletingRecords.Count != this.listView_PasswordItems.SelectedItems.Count)
            {
                throw new InvalidOperationException();
            }

            // Confirm really want to delete
            string concatenatedCaption = String.Join(Environment.NewLine, deletingRecordsCaptions.ToArray());
            DialogResult dresult = MessageBox.Show(String.Format(strings.General_DeletePassword_Text, concatenatedCaption), strings.General_DeletePassword_Caption, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            if (dresult != DialogResult.OK)
            {
                return;
            }

            // Delete all selected records
            foreach(PasswordRecord record in deletingRecords)
            {
                // Remove target password record from indexer.
                // If it fails to remove record from index, exit.
                if (!indexer.RemoveRecord(record.GetRecordID()))
                {
                    return;
                }

                // Remove target password record from the list in password data
                if (!this.PasswordData.Records.Remove(record))
                {
                    throw new Exception();
                }
            }

            // Refresh listview
            TreeViewEventArgs tvea = new TreeViewEventArgs(this.CurrentTreeNode);
            this.treeView_Folders_AfterSelect(null, tvea);

            return;
        }

        /// <summary>
        /// Keyboard event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void listView_PasswordItems_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                // Delete operation
                case Keys.Delete:
                    this.ToolStripMenuItem_ListViewItem_Delete_Click(null, null);
                    break;
                default:
                    break;
            }

            return;
        }

        /// <summary>
        /// Drag start event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void listView_PasswordItems_ItemDrag(object sender, ItemDragEventArgs e)
        {
            // Move the dragged node when the left mouse button is used.
            if (e.Button == MouseButtons.Left)
            {
                // Get selected listview items
                List<ListViewItem> items = new List<ListViewItem>();
                foreach (ListViewItem lvi in this.listView_PasswordItems.SelectedItems)
                {
                    items.Add(lvi);
                }

                // Construct data object to send
                DataObject dobj = new DataObject(items);

                // Transfer object data
                DoDragDrop(dobj, DragDropEffects.Move);
            }
        }

        /// <summary>
        /// Copy id or password into clipboard for selected password item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void listView_PasswordItems_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // Get which list column has been double-clicked
                Point mousePos = this.listView_PasswordItems.PointToClient(Control.MousePosition);
                ListViewHitTestInfo hitTest = this.listView_PasswordItems.HitTest(mousePos);
                if (hitTest.Location == ListViewHitTestLocations.None
                    || hitTest.Item == null
                    || hitTest.SubItem == null)
                {
                    return;
                }

                PasswordRecord record = this.GetSelectedPasswordRecordOnList();
                if (record == null)
                {
                    return;
                }

                // Copy caption
                if (hitTest.Item.SubItems.IndexOf(hitTest.SubItem) == this.columnHeader_caption.Index)
                {
                    // Copy id into clipboard
                    Clipboard.SetDataObject(record.GetCaption());

                    // Notify status bar
                    string shortText = Utility.GetShorterTextRight(record.GetCaption(), InternalApplicationConfig.StatusStripPasswordLabelMaxLength);
                    this.toolStripStatusLabel_ClipboardStatus.Text = String.Format(strings.General_Copy_Caption, shortText);

                    return;
                }
                // Copy ID
                if (hitTest.Item.SubItems.IndexOf(hitTest.SubItem) == this.columnHeader_ID.Index)
                {
                    // Copy id into clipboard
                    Clipboard.SetDataObject(record.GetID());

                    // Notify status bar
                    string shortText = Utility.GetShorterTextRight(record.GetCaption(), InternalApplicationConfig.StatusStripPasswordLabelMaxLength);
                    this.toolStripStatusLabel_ClipboardStatus.Text = String.Format(strings.General_Copy_ID, shortText);

                    return;
                }
                // Copy password
                if (hitTest.Item.SubItems.IndexOf(hitTest.SubItem) == this.columnHeader_password.Index)
                {
                    // Copy password into clipboard
                    Clipboard.SetDataObject(record.GetPassword());

                    // Notify status bar
                    string shortText = Utility.GetShorterTextRight(record.GetCaption(), InternalApplicationConfig.StatusStripPasswordLabelMaxLength);
                    this.toolStripStatusLabel_ClipboardStatus.Text = String.Format(strings.General_Copy_Password, shortText);

                    return;
                }
            }

            return;
        }
        #endregion

        #region toolstrip event
        /// <summary>
        /// Open and read password file and construct associated windows form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void toolStripButton_Open_Click(object sender, EventArgs e)
        {
            // Query target file path
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Environment.CurrentDirectory;
            openFileDialog.Filter = InternalApplicationConfig.OpeningPasswordFileFilter;
            openFileDialog.FilterIndex = 1;
            if (openFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            string targetFilePath = openFileDialog.FileName;

            // Check the file exits
            if (!File.Exists(targetFilePath))
            {
                MessageBox.Show(strings.General_FileNotFound, strings.General_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Instantiate password file object
            PasswordFile f = new PasswordFile(targetFilePath);

            // Initialize master password hash
            byte[] passwordHash = Utility.GetHash(InternalApplicationConfig.DefaultMasterPassword);

            // Try default master password once.
            // When default master password is used to target password file,
            // user do not need to enter master password by itself.
            if (!PasswordFile.ChallengeHashedMasterPassword(targetFilePath, passwordHash))
            {
                // Query master password for target password file
                using (FormInputMasterPassword form = new FormInputMasterPassword(PasswordFile.ChallengeHashedMasterPassword, targetFilePath))
                {
                    form.StartPosition = FormStartPosition.CenterParent;
                    form.SetupLanguage(Thread.CurrentThread.CurrentUICulture.Name);
                    if (form.ShowDialog() != DialogResult.OK)
                    {
                        return;
                    }
                    passwordHash = form.GetMasterPasswordHash();
                }
            }

            // Load password file
            try
            {
                // Load password file
                this.PasswordData = f.ReadPasswordFromFile(passwordHash);

                // Reconstruct treeview and listview
                this.InitializeTreeStructure(this.PasswordData.Containers, this.PasswordData.Indexer);
                this.listView_PasswordItems.Invalidate();

                // Remember the loaded file name and input password hash for saving
                this.CurrentPasswordFilePath = targetFilePath;
                this.MasterPasswordHash = passwordHash;

                MessageBox.Show(strings.General_OpenFile_Success_Text, strings.General_OpenFile_Success_Caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (NoCorrespondingFilterFoundException ex)
            {
                string msgText = String.Format(strings.General_ParseFilterFailed_Text, ex.Message);
                MessageBox.Show(msgText, strings.General_ParseFilterFailed_Caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            // Update file load status label
            this.SetFileLoadStatusLabel();
        }

        /// <summary>
        /// Save current password data into a file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void toolStripButton_Save_Click(object sender, EventArgs e)
        {
            this.SaveNewFile();
        }

        /// <summary>
        /// Expand all node on treeview
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void toolStripButton_ExpandTree_Click(object sender, EventArgs e)
        {
            this.treeView_Folders.BeginUpdate();
            this.treeView_Folders.ExpandAll();
            this.treeView_Folders.EndUpdate();
        }

        /// <summary>
        /// Collapse all node on treeview
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void toolStripButton_CollapseTree_Click(object sender, EventArgs e)
        {
            this.treeView_Folders.BeginUpdate();
            this.treeView_Folders.CollapseAll();
            this.treeView_Folders.EndUpdate();
        }
        #endregion

        #region menuitem event
        /// <summary>
        /// Change language setting to Japanese
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ToolStripMenuItem_Language_Japanese_Click(object sender, EventArgs e)
        {
            this.SetupLanguage(InternalApplicationConfig.LocaleJaJP);
        }

        /// <summary>
        /// Change language setting to English
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ToolStripMenuItem_Language_English_Click(object sender, EventArgs e)
        {
            this.SetupLanguage(InternalApplicationConfig.LocaleEnUS);
        }

        void ToolStripMenuItem_Edit_MouseDown(object sender, EventArgs e)
        {
            if (this.listView_PasswordItems.Focused)
            {
                this.ToolStripMenuItem_Edit.DropDownItems.Clear();
                this.ToolStripMenuItem_Edit.DropDownItems.AddRange(new ToolStripItem[]{
                    this.ToolStripMenuItem_ListViewItem_New,
                    this.ToolStripMenuItem_ListViewItem_Edit,
                    this.ToolStripMenuItem_ListViewItem_Move,
                    this.ToolStripMenuItem_ListViewItem_Delete
                });
            }
            else
            {
                this.ToolStripMenuItem_Edit.DropDownItems.Clear();
                this.ToolStripMenuItem_Edit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                    this.ToolStripMenuItem_Edit_AddSubFolder,
                    this.ToolStripMenuItem_Edit_RenameFolder,
                    this.ToolStripMenuItem_Edit_DeleteFolder,
                    this.toolStripMenuItem_Edit_Seperator,
                    this.ToolStripMenuItem_Edit_AddPassword
                });
            }
        }

        /// <summary>
        /// Change master password
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ToolStripMenuItem_ChangeMasterPassword_Click(object sender, EventArgs e)
        {
            using (FormChangeMasterPassword form = new FormChangeMasterPassword())
            {
                form.StartPosition = FormStartPosition.CenterParent;

                DialogResult result = form.ShowDialog();
                if (result != DialogResult.OK)
                {
                    MessageBox.Show(strings.Form_ChangeMasterPassword_NotChanged_Text, strings.Form_ChangeMasterPassword_NotChanged_Caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                this.MasterPasswordHash = form.GetMasterPasswordHash();
                MessageBox.Show(strings.Form_ChangeMasterPassword_Success_Text, strings.Form_ChangeMasterPassword_Success_Caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// Open and load password file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ToolStripMenuItem_File_Open_Click(object sender, EventArgs e)
        {
            this.toolStripButton_Open_Click(null, null);
        }

        /// <summary>
        /// Save password data as a new file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ToolStripMenuItem_File_SaveAs_Click(object sender, EventArgs e)
        {
            this.SaveNewFile();
        }

        /// <summary>
        /// Overwrite password information to the file currently loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ToolStripMenuItem_File_Save_Click(object sender, EventArgs e)
        {
            this.OverwriteFile();
        }
        #endregion

        #region context menu event
        /// <summary>
        /// Add subfolder to current node
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ToolStripMenuItem_AddSubFolder_Click(object sender, EventArgs e)
        {
            // Do nothing if no tree nodes are selected
            if (this.CurrentTreeNode == null || this.CurrentTreeNode.Tag == null)
            {
                return;
            }

            // Add a child container to current selected container
            PasswordContainer container = new PasswordContainer(this.PasswordData.Indexer.GetUniqueContainerID(), InternalApplicationConfig.NewUnnamedContainerLabel);
            this.PasswordData.Containers.Add(container);
            this.PasswordData.Indexer.AppendContainer(container.GetContainerID(), (int)this.CurrentTreeNode.Tag);

            // Add sub folder to current selected tree node
            TreeNode node = this.AddContainerToTreeView(container, this.CurrentTreeNode);

            // Invoke editlabel feature on the created folder
            this.treeView_Folders.SelectedNode = node;
            node.BeginEdit();
        }

        /// <summary>
        /// Rename current selected folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ToolStripMenuItem_RenameFolder_Click(object sender, EventArgs e)
        {
            // Do nothing if no tree nodes are selected
            if (this.CurrentTreeNode == null)
            {
                return;
            }

            this.CurrentTreeNode.BeginEdit();
        }

        /// <summary>
        /// Delete selected folder and its subfolders
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ToolStripMenuItem_DeleteFolder_Click(object sender, EventArgs e)
        {
            if (this.CurrentTreeNode == null)
            {
                return;
            }

            TreeNode targetNode = this.CurrentTreeNode;
            int containerID = (int)targetNode.Tag;

            // If root folder is the target, exit with error message
            if (containerID == InternalApplicationConfig.RootContainerID)
            {
                MessageBox.Show(strings.General_DeleteContainer_CannotDeleteRoot_Text, strings.General_DeleteContainer_CannotDeleteRoot_Caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Show warning message dialog
            string warningText = String.Format(strings.General_DeleteContainer_Text, this.PasswordData.Indexer.GetContainerByID(this.PasswordData.Containers, containerID).GetLabel());
            DialogResult res = MessageBox.Show(warningText, strings.General_DeleteContainer_Caption, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            if (res != DialogResult.OK)
            {
                return;
            }

            // Delete associated container/record index with target container id
            PasswordIndexerBase.RemovedObjects removedObj = this.PasswordData.Indexer.RemoveAllContainers(containerID);

            // Delete actual Password records
            foreach (int removedRecordID in removedObj.Removed[typeof(PasswordRecord)])
            {
                this.PasswordData.Records.Remove(this.PasswordData.Indexer.GetRecordByID(this.PasswordData.Records, removedRecordID));
            }

            // Delete actual Password containers
            foreach (int removedContainerID in removedObj.Removed[typeof(PasswordContainer)])
            {
                this.PasswordData.Containers.Remove(this.PasswordData.Indexer.GetContainerByID(this.PasswordData.Containers, removedContainerID));
            }

            // Delete treeview node
            this.treeView_Folders.BeginUpdate();
            targetNode.Remove();
            this.treeView_Folders.EndUpdate();
        }

        /// <summary>
        /// Add new password record to specified parent container
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ToolStripMenuItem_AddPassword_Click(object sender, EventArgs e)
        {
            // Do nothing if no tree nodes are selected
            if (this.CurrentTreeNode.Tag == null || this.CurrentTreeNode.Tag == null)
            {
                return;
            }

            PasswordRecord record = null;
            using (FormCreatePassword form = new FormCreatePassword())
            {
                form.StartPosition = FormStartPosition.CenterParent;
                if (form.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                record = form.GetPassword();
            }

            // If caption is empty, do nothing
            if (record == null || String.IsNullOrEmpty(record.GetCaption()))
            {
                return;
            }

            // Get selected container
            int containerID = (int)this.CurrentTreeNode.Tag;
            int recordID = this.PasswordData.Indexer.GetUniqueRecordID();

            // Set new unique recordID
            record.SetRecordID(recordID);

            // Update password index
            if (!this.PasswordData.Indexer.AppendRecord(record.GetRecordID(), containerID))
            {
                throw new ApplicationException();
            }

            // Add password record to PasswordFile object
            this.PasswordData.Records.Add(record);

            // Refresh listview
            TreeViewEventArgs tvea = new TreeViewEventArgs(this.CurrentTreeNode);
            this.treeView_Folders_AfterSelect(null, tvea);

            return;
        }
        #endregion

        #region treeview event
        /// <summary>
        /// Click event for Node on TreeView 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void treeView_Folders_AfterSelect(object sender, TreeViewEventArgs e)
        {
            // When tag contains no information about actual pointer to internal PasswordObjects, stop processing.
            if (e == null || e.Node == null || e.Node.Tag == null)
            {
                return;
            }
            int containerID = (int)e.Node.Tag;
            this.CurrentTreeNode = e.Node;

            // Initialize listview/textbox control
            this.listView_PasswordItems.Items.Clear();
            this.textBox_ItemDescription.Text = String.Empty;

            ICollection<int> ids = this.PasswordData.Indexer.GetChildRecords(containerID);
            if (ids == null || ids.Count < 1)
            {
                return;
            }

            // Only the top of records obtained is appeared in text box.
            this.textBox_ItemDescription.Text = this.PasswordData.Indexer.GetRecordByID(this.PasswordData.Records, ids.ElementAt(0)).GetDescription();

            foreach (int recordID in ids)
            {
                PasswordRecord record = this.PasswordData.Indexer.GetRecordByID(this.PasswordData.Records, recordID);
                ListViewItem lvi = new ListViewItem();
                lvi.Text = record.GetCaption();
                lvi.SubItems.Add(record.GetID().ToString());
                lvi.SubItems.Add(record.GetPassword());
                lvi.Tag = record.GetRecordID();

                this.listView_PasswordItems.Items.Add(lvi);
            }
        }

        /// <summary>
        /// When node is clicked, selectednode property on treeview is changed here
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void treeView_Folders_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            this.CurrentTreeNode = e.Node;
        }

        /// <summary>
        /// Handling keyboard events here
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void treeView_Folders_KeyUp(object sender, KeyEventArgs e)
        {
            if (this.CurrentTreeNode == null)
            {
                throw new InvalidOperationException();
            }

            switch (e.KeyCode)
            {
                // Rename folder
                case Keys.F2:
                    this.ToolStripMenuItem_RenameFolder_Click(null, null);
                    break;
                // Delete folder
                case Keys.Delete:
                    this.ToolStripMenuItem_DeleteFolder_Click(null, null);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Set edited label value to password container object
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void treeView_Folders_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            // When the user presses ESC to cancel edit or pressed ENTER without modifying label text, e.Label is null.
            if (e == null || e.Label == null || e.Node.Tag == null || e.CancelEdit)
            {
                return;
            }

            // Get container object
            PasswordContainer container = this.PasswordData.Indexer.GetContainerByID(this.PasswordData.Containers, (int)e.Node.Tag);
            if (container == null)
            {
                return;
            }

            // If label text is larger than max value, set back to previous label text and exit this method
            if (e.Label.Length > InternalApplicationConfig.ContainerNameMax)
            {
                string text = String.Format(strings.General_EditLabel_GoOverMaxLength_Text, InternalApplicationConfig.ContainerNameMax, e.Label.Length);
                MessageBox.Show(text, strings.General_EditLabel_GoOverMaxLength_Caption, MessageBoxButtons.OK, MessageBoxIcon.Error);

                e.CancelEdit = true;
                return;
            }

            container.SetLabel(e.Label);
        }

        /// <summary>
        /// Move operation is occured when some of nodes are began being dragged.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void treeView_Folders_ItemDrag(object sender, ItemDragEventArgs e)
        {
            // Move the dragged node when the left mouse button is used.
            if (e.Button == MouseButtons.Left)
            {
                DoDragDrop(e.Item, DragDropEffects.Move);
            }
        }

        /// <summary>
        /// This is called when dragged item is entered on another control item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void treeView_Folders_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = e.AllowedEffect;
        }

        /// <summary>
        /// Select the node under the mouse pointer to indicate the expected drop location.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void treeView_Folders_DragOver(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(TreeNode))
                && !e.Data.GetDataPresent(typeof(List<ListViewItem>)))
            {
                e.Effect = DragDropEffects.None;
                return;
            }

            // Retrieve the client coordinates of the mouse position.
            Point targetPoint = this.treeView_Folders.PointToClient(new Point(e.X, e.Y));

            // Select the node at the mouse position.
            this.treeView_Folders.SelectedNode = this.treeView_Folders.GetNodeAt(targetPoint);

            // Only if dragging cursor is on some treenode
            if (this.treeView_Folders.SelectedNode != null)
            {
                this.CurrentTreeNode = this.treeView_Folders.SelectedNode;

                // Expand the node
                this.CurrentTreeNode.Expand();
            }
        }

        /// <summary>
        /// This is called when dragged node is dropped
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void treeView_Folders_DragDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(TreeNode))
                && !e.Data.GetDataPresent(typeof(List<ListViewItem>)))
            {
                return;
            }

            // Get destination node
            Point targetPoint = treeView_Folders.PointToClient(new Point(e.X, e.Y));
            TreeNode targetNode = treeView_Folders.GetNodeAt(targetPoint);

            // When dragged item is tree node (Password container)
            if (e.Data.GetDataPresent(typeof(TreeNode)))
            {
                TreeNode draggedNode = (TreeNode)e.Data.GetData(typeof(TreeNode));
                this.TreeNodeDropped(draggedNode, targetNode, e);

                return;
            }

            // When dragged item is listview item (Password record)
            if (e.Data.GetDataPresent(typeof(List<ListViewItem>)))
            {
                List<ListViewItem> draggedItem = (List<ListViewItem>)e.Data.GetData(typeof(List<ListViewItem>));
                this.ListViewItemDropped(draggedItem, targetNode, e);

                return;
            }

            return;
        }

        /// <summary>
        /// Process treenode drop event
        /// </summary>
        /// <param name="srcNode"></param>
        /// <param name="dstNode"></param>
        /// <param name="e"></param>
        void TreeNodeDropped(TreeNode srcNode, TreeNode dstNode, DragEventArgs e)
        {
            if (dstNode == null || srcNode == null || dstNode.Tag == null || srcNode.Tag == null)
            {
                return;
            }

            if (!srcNode.Equals(dstNode) && !ContainsNode(srcNode, dstNode))
            {
                if (e.Effect == DragDropEffects.Move)
                {
                    srcNode.Remove();
                    this.PasswordData.Indexer.ReleaseContainer((int)srcNode.Tag);

                    dstNode.Nodes.Add(srcNode);
                    this.PasswordData.Indexer.AppendContainer((int)srcNode.Tag, (int)dstNode.Tag);
                }

                dstNode.Expand();
            }
        }

        /// <summary>
        /// Process listview item drop event
        /// </summary>
        /// <param name="srcItem"></param>
        /// <param name="dstNode"></param>
        /// <param name="e"></param>
        void ListViewItemDropped(List<ListViewItem> srcItem, TreeNode dstNode, DragEventArgs e)
        {
            // If arguments are null or insufficient, do nothing
            if (srcItem == null || srcItem.Count < 1 || dstNode == null || dstNode.Tag == null)
            {
                return;
            }
            foreach (ListViewItem lvi in srcItem)
            {
                if (lvi.Tag == null)
                {
                    return;
                }
            }

            int destContainerID = (int)dstNode.Tag;

            if (e.Effect == DragDropEffects.Move)
            {
                foreach (ListViewItem lvi in srcItem)
                {
                    int recordID = (int)lvi.Tag;
                    this.PasswordData.Indexer.MoveRecord(recordID, destContainerID);
                }
            }

            // Refresh listview
            TreeViewEventArgs tvea = new TreeViewEventArgs(this.CurrentTreeNode);
            this.treeView_Folders_AfterSelect(null, tvea);

            return;
        }
        #endregion

        #region General keyboard shortcut
        /// <summary>
        /// Handles keyboard shortcuts
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void FormMain_KeyDown(object sender, KeyEventArgs e)
        {
            // Ctrl+Shift+S
            if (e.KeyCode == Keys.S && ((Control.ModifierKeys & Keys.Control) != 0) && ((Control.ModifierKeys & Keys.Shift) != 0))
            {
                this.SaveNewFile();
                return;
            }

            // Ctrl+S
            if (e.KeyCode == Keys.S && ((Control.ModifierKeys & Keys.Control) != 0))
            {
                this.OverwriteFile();
                return;
            }
        }
        #endregion

        #endregion

        #region Utility
        /// <summary>
        /// Adjust the size of last column not to make any margin between listview and column
        /// </summary>
        /// <param name="lv"></param>
        private void AdjustColumnSize(ListView lv, ColumnWidthChangedEventArgs e)
        {
            // If the changed column is the last index column, do nothing
            if (e.ColumnIndex == (lv.Columns.Count - 1))
            {
                return;
            }

            // Resize the last column in the listview in order to trim margin
            int sizeListView = lv.Size.Width;
            int sizeColumnExceptForTheLastOne = 0;
            for (int i = 0; i < lv.Columns.Count - 1; i++)
            {
                sizeColumnExceptForTheLastOne += lv.Columns[i].Width;
            }

            if (sizeListView - sizeColumnExceptForTheLastOne <= 0)
            {
                return;
            }

            // When horizontal scroll bar is visible, decrease its size from listview
            if (IsVScrollbarVisibleOnListView(lv)) // If scroll bar is visible
            {
                lv.Columns[lv.Columns.Count - 1].Width = sizeListView - sizeColumnExceptForTheLastOne - (lv.Margin.Right + 1) - SystemInformation.VerticalScrollBarWidth;
            }
            else // If scroll bar is not visible
            {
                lv.Columns[lv.Columns.Count - 1].Width = sizeListView - sizeColumnExceptForTheLastOne - (lv.Margin.Right + 1);
            }
        }

        /// <summary>
        /// Check the ListView object has a vertical scroll bar visible
        /// </summary>
        /// <param name="lv"></param>
        /// <returns></returns>
        public bool IsVScrollbarVisibleOnListView(ListView lv)
        {
            long wndStyle = Utility.GetWindowLong(lv.Handle, Utility.GwlStyle);

            if ((wndStyle & Utility.WsVScroll) != 0)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Set up password container/record tree structure by PasswordIndexer object
        /// </summary>
        /// <param name="indexer"></param>
        public void InitializeTreeStructure(ICollection<PasswordContainer> containers, PasswordIndexerBase indexer)
        {
            // Remove all children from folder
            this.treeView_Folders.Nodes.Clear();

            // Construct container tree
            TreeNode node = GetTreeViewNodeBuilt(containers, indexer, this.contextMenuStrip_TreeViewNode);
            this.treeView_Folders.Nodes.Add(node);

            // Expand firt level node
            node.Expand();
        }

        /// <summary>
        /// Generate completed tree object by provided containers and indexer object
        /// </summary>
        /// <param name="containers"></param>
        /// <param name="indexer"></param>
        /// <returns></returns>
        public static TreeNode GetTreeViewNodeBuilt(ICollection<PasswordContainer> containers, PasswordIndexerBase indexer, ContextMenuStrip context)
        {
            // Setup root parent container
            int rootContainerID = InternalApplicationConfig.RootContainerID;
            TreeNode rootNode = new TreeNode(InternalApplicationConfig.RootContainerLabel);
            rootNode.Tag = rootContainerID;

            // Execute recursive tree method
            AddContainerToTreeView(containers, indexer, rootContainerID, rootNode, context);

            // Attach context menu strip
            rootNode.ContextMenuStrip = context;

            return rootNode;
        }

        /// <summary>
        /// Setup container tree in a recursive manner.
        /// </summary>
        /// <param name="parentContainerID"></param>
        /// <param name="containers"></param>
        /// <param name="indexer"></param>
        public static void AddContainerToTreeView(ICollection<PasswordContainer> containers, PasswordIndexerBase indexer, int parentContainerID, TreeNode parentNode, ContextMenuStrip context)
        {
            ICollection<int> childrenContainers = indexer.GetChildContainers(parentContainerID);

            // When there are no child containers, do nothing. This path should be walked into when executing end-leaf object.
            if (childrenContainers == null)
            {
                return;
            }

            // Loop over all associated child containers of specified parent container
            foreach (int childContainerID in childrenContainers)
            {
                PasswordContainer c = indexer.GetContainerByID(containers, childContainerID);
                TreeNode node = new TreeNode(c.GetLabel());
                node.Tag = childContainerID; // Store identification information
                node.ContextMenuStrip = context; // Attach context menu strip

                parentNode.Nodes.Add(node); // Add the current node to parent node

                AddContainerToTreeView(containers, indexer, childContainerID, node, context);
            }
        }

        /// <summary>
        /// Add container to specified parent tree node
        /// </summary>
        /// <param name="container"></param>
        /// <param name="parentNode"></param>
        private TreeNode AddContainerToTreeView(PasswordContainer container, TreeNode parentNode)
        {
            if (container == null || parentNode == null)
            {
                throw new ArgumentNullException();
            }

            TreeNode node = new TreeNode(!String.IsNullOrEmpty(container.GetLabel()) ? container.GetLabel() : InternalApplicationConfig.NewUnnamedContainerLabel);
            node.Tag = container.GetContainerID();
            node.ContextMenuStrip = this.contextMenuStrip_TreeViewNode;

            parentNode.Nodes.Add(node);

            return node;
        }

        /// <summary>
        /// Determine whether node1 is a parent or ancestor of a node2.
        /// </summary>
        /// <param name="node1"></param>
        /// <param name="node2"></param>
        /// <returns>Result whether node1 is an ancester of node2</returns>
        private bool ContainsNode(TreeNode node1, TreeNode node2)
        {
            // Check the parent node of the second node.
            if (node2.Parent == null) return false;
            if (node2.Parent.Equals(node1)) return true;

            // If the parent node is not null or equal to the first node, 
            // call the ContainsNode method recursively using the parent of 
            // the second node.
            return ContainsNode(node1, node2.Parent);
        }

        /// <summary>
        /// Get selected password item from listview. When multiple item on listview are selected, it returns null.
        /// </summary>
        /// <returns></returns>
        private PasswordRecord GetSelectedPasswordRecordOnList()
        {
            if (this.listView_PasswordItems.SelectedItems == null
                || this.listView_PasswordItems.SelectedItems.Count != 1)
            {
                return null;
            }

            ListViewItem lvi = this.listView_PasswordItems.SelectedItems[0];
            if (lvi.Tag == null)
            {
                throw new InvalidOperationException();
            }

            int recordID = (int)lvi.Tag;
            return this.PasswordData.Indexer.GetRecordByID(this.PasswordData.Records, recordID);
        }

        /// <summary>
        /// Overwrite password information to the file currently loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OverwriteFile()
        {
            string writingFilePath = this.CurrentPasswordFilePath;

            // Check specified filepath. If file is not loaded, then open save as dialog
            if (!File.Exists(writingFilePath))
            {
                this.SaveNewFile();
                return;
            }

            try
            {
                // Try to write password object content to specified file
                PasswordFile f = new PasswordFile(writingFilePath);
                f.SetFilterOrder(this.FilterOrder);
                f.WritePasswordToFile(this.MasterPasswordHash, this.PasswordData);

                // Update current path information
                this.CurrentPasswordFilePath = writingFilePath;
                this.SetFileSaveStatusLabel();
            }
            catch (Exception excp)
            {
                MessageBox.Show(excp.Message + Environment.NewLine + excp.StackTrace);
            }
        }

        /// <summary>
        /// Save current password data into a new file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveNewFile()
        {
            // Set default directory which will be shown on save file dialog
            string directory = Environment.CurrentDirectory;
            if (!String.IsNullOrEmpty(this.CurrentPasswordFilePath) && Directory.Exists(Path.GetDirectoryName(this.CurrentPasswordFilePath)))
            {
                directory = Path.GetDirectoryName(this.CurrentPasswordFilePath);
            }

            // Setup save file dialog
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.InitialDirectory = directory;
            saveDialog.DefaultExt = InternalApplicationConfig.DefaultFileExt;
            saveDialog.Filter = InternalApplicationConfig.OpeningPasswordFileFilter;
            saveDialog.FilterIndex = 1;
            saveDialog.OverwritePrompt = true;

            // Show save file dialog
            if (saveDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            string writingFilePath = saveDialog.FileName;

            // Check specified filepath
            if (!Directory.Exists(Path.GetDirectoryName(writingFilePath)))
            {
                MessageBox.Show(strings.General_DirNotFound, strings.General_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                // Try to write password object content to specified file
                PasswordFile f = new PasswordFile(writingFilePath);
                f.SetFilterOrder(this.FilterOrder);
                f.WritePasswordToFile(this.MasterPasswordHash, this.PasswordData);

                // Update current path information
                this.CurrentPasswordFilePath = writingFilePath;
                this.SetFileSaveStatusLabel();
            }
            catch (Exception excp)
            {
                MessageBox.Show(excp.Message + Environment.NewLine + excp.StackTrace);
            }
        }
        #endregion

        #region Language setup
        /// <summary>
        /// Set up specified language for this application
        /// </summary>
        private void SetupLanguage(string locale)
        {
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(locale);

            this.ToolStripMenuItem_about.Text = strings.Form_MenuItem_About;
            this.ToolStripMenuItem_option.Text = strings.Form_MenuItem_Option;
            this.toolStripStatusLabel_ClipboardStatus.Text = strings.Status_Ready;
            this.toolStripButton_Open.ToolTipText = strings.Form_Tooltip_OpenFile;
            this.toolStripButton_Save.ToolTipText = strings.Form_Tooltip_SaveFileAs;
            this.toolStripButton_ExpandTree.ToolTipText = strings.Form_Tooltip_ExpandTree;
            this.toolStripButton_CollapseTree.ToolTipText = strings.Form_Tooltip_CollapseTree;
            this.columnHeader_caption.Text = strings.Form_Listview_Caption;
            this.columnHeader_ID.Text = strings.Form_Listview_ID;
            this.columnHeader_password.Text = strings.Form_Listview_Password;
            this.ToolStripMenuItem_Language.Text = strings.Form_MenuItem_Language;
            this.ToolStripMenuItem_Language_English.Text = strings.Form_MenuItem_Language_English;
            this.ToolStripMenuItem_Language_Japanese.Text = strings.Form_MenuItem_Language_Japanese;
            this.ToolStripMenuItem_AddSubFolder.Text = strings.Form_ContextMenu_AddContainer;
            this.ToolStripMenuItem_RenameFolder.Text = strings.Form_ContextMenu_RenameContainer;
            this.ToolStripMenuItem_DeleteFolder.Text = strings.Form_ContextMenu_DeleteContainer;
            this.ToolStripMenuItem_AddPassword.Text = strings.Form_ContextMenu_AddPassword;
            this.ToolStripMenuItem_File.Text = strings.Form_MenuItem_File;
            this.ToolStripMenuItem_File_Open.Text = strings.Form_MenuItem_File_Open;
            this.ToolStripMenuItem_File_Save.Text = strings.Form_MenuItem_File_Save;
            this.ToolStripMenuItem_File_SaveAs.Text = strings.Form_MenuItem_File_SaveAs;
            this.ToolStripMenuItem_Edit.Text = strings.Form_MenuItem_Edit;
            this.ToolStripMenuItem_Edit_AddSubFolder.Text = strings.Form_ContextMenu_AddContainer;
            this.ToolStripMenuItem_Edit_RenameFolder.Text = strings.Form_ContextMenu_RenameContainer;
            this.ToolStripMenuItem_Edit_DeleteFolder.Text = strings.Form_ContextMenu_DeleteContainer;
            this.ToolStripMenuItem_Edit_AddPassword.Text = strings.Form_ContextMenu_AddPassword;
            this.ToolStripMenuItem_ListView_New.Text = strings.Form_ContextMenu_AddPassword;
            this.ToolStripMenuItem_ListViewItem_New.Text = strings.Form_ContextMenu_AddPassword;
            this.ToolStripMenuItem_ListViewItem_Edit.Text = strings.Form_ContextMenu_EditPassword;
            this.ToolStripMenuItem_ListViewItem_Delete.Text = strings.Form_ContextMenu_DeletePassword;
            this.ToolStripMenuItem_ListViewItem_Move.Text = strings.Form_ContextMenu_MovePassword;
            this.ToolStripMenuItem_ChangeMasterPassword.Text = strings.Form_MenuItem_ChangeMasterPassword;

            this.SetFileLoadStatusLabel();
        }

        /// <summary>
        /// Set file load status text to status strip
        /// </summary>
        public void SetFileLoadStatusLabel()
        {
            if (String.IsNullOrEmpty(this.CurrentPasswordFilePath))
            {
                this.toolStripStatusLabel_FileOpened.Text = strings.Form_StatusStrip_NoFileLoaded;
                this.toolStripStatusLabel_FileOpened.ToolTipText = String.Empty;
            }
            else
            {
                string shortPath = Utility.GetShorterTextMiddle(this.CurrentPasswordFilePath, InternalApplicationConfig.StatusStripFilePathMaxLength);
                this.toolStripStatusLabel_FileOpened.Text = String.Format(strings.Form_StatusStrip_FileLoaded, shortPath);
                this.toolStripStatusLabel_FileOpened.ToolTipText = this.CurrentPasswordFilePath;
            }
        }

        /// <summary>
        /// Set file save status text to status strip
        /// </summary>
        public void SetFileSaveStatusLabel()
        {
            string shortPath = Utility.GetShorterTextMiddle(this.CurrentPasswordFilePath, InternalApplicationConfig.StatusStripFilePathMaxLength);
            string text = String.Format(strings.Form_StatusStrip_FileSaved, shortPath);

            if (text == this.toolStripStatusLabel_FileOpened.Text)
            {
                // A space character is inserted before the original text in order for user to be notified that a data has been saved.
                text = ' ' + text;
            }

            this.toolStripStatusLabel_FileOpened.Text = text;
            this.toolStripStatusLabel_FileOpened.ToolTipText = this.CurrentPasswordFilePath;
        }
        #endregion
    }
}
