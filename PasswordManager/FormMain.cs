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
#endregion

namespace PasswordManager
{
    public partial class MainForm_PasswordManager : Form
    {
        #region Fields
        private PasswordFileBody PasswordData = new PasswordFileBody();
        private TreeNode CurrentTreeNode = null;
        #endregion

        #region Constructor
        public MainForm_PasswordManager()
        {
            InitializeComponent();
            
            // ListView event
            this.listView_PasswordItems.SizeChanged += listView_PasswordItems_SizeChanged;
            this.listView_PasswordItems.ColumnWidthChanged += listView_PasswordItems_ColumnWidthChanged;
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
            // Context menu event
            this.ToolStripMenuItem_AddSubFolder.Click += ToolStripMenuItem_AddSubFolder_Click;
            this.ToolStripMenuItem_RenameFolder.Click += ToolStripMenuItem_RenameFolder_Click;
            this.ToolStripMenuItem_DeleteFolder.Click += ToolStripMenuItem_DeleteFolder_Click;
            this.ToolStripMenuItem_AddPassword.Click += ToolStripMenuItem_AddPassword_Click;
            // Menu item click event
            this.ToolStripMenuItem_Language_English.Click += ToolStripMenuItem_Language_English_Click;
            this.ToolStripMenuItem_Language_Japanese.Click += ToolStripMenuItem_Language_Japanese_Click;

            // Apply language setting
            this.SetupLanguage(InternalApplicationConfig.DefaultLocale);

            // Initialize form element
            this.Initialize();
        }
        #endregion

        #region Initialize
        /// <summary>
        /// Initialize form element
        /// </summary>
        void Initialize()
        {
            this.InitializeTreeStructure(this.PasswordData.Containers, this.PasswordData.Indexer);
            this.listView_PasswordItems.Invalidate();
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
        #endregion

        #region toolstrip event
        /// <summary>
        /// Open and read password file and construct associated windows form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void toolStripButton_Open_Click(object sender, EventArgs e)
        {
            PasswordFile f = new PasswordFile("test.txt");
            DebugFilter df = new DebugFilter();
            f.AddIOFilter(df);
            f.AddFilterOrder(df.ToString());

            this.PasswordData = f.ReadPasswordFromFile(Utility.GetHash(new byte[] { 0xff, 0xfe, 0x00, 0x01, 0x02 }));
            this.InitializeTreeStructure(this.PasswordData.Containers, this.PasswordData.Indexer);
            this.listView_PasswordItems.Invalidate();
        }

        /// <summary>
        /// Save current password data into a file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void toolStripButton_Save_Click(object sender, EventArgs e)
        {
            PasswordFile f = new PasswordFile("test.txt");
            DebugFilter df = new DebugFilter();
            f.AddIOFilter(df);
            f.AddFilterOrder(df.ToString());

            f.WritePasswordToFile(Utility.GetHash(new byte[] { 0xff, 0xfe, 0x00, 0x01, 0x02 }), this.PasswordData);
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
        #endregion

        #region context menu event
        /// <summary>
        /// Add subfolder to current node
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ToolStripMenuItem_AddSubFolder_Click(object sender, EventArgs e)
        {
            // Throw an exception when CurrentTreeNode value is null. This should not be happend.
            if (this.CurrentTreeNode == null || this.CurrentTreeNode.Tag == null)
            {
                //return;
                throw new InvalidOperationException();
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
            if (this.CurrentTreeNode == null)
            {
                //return;
                throw new InvalidOperationException();
            }

            this.CurrentTreeNode.BeginEdit();
        }

        void ToolStripMenuItem_DeleteFolder_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Add new password record to specified parent container
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ToolStripMenuItem_AddPassword_Click(object sender, EventArgs e)
        {
            if (this.CurrentTreeNode.Tag == null)
            {
                throw new InvalidOperationException();
            }

            FormCreatePassword form = new FormCreatePassword();
            form.StartPosition = FormStartPosition.Manual;
            form.Location = this.Location;
            if (form.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            PasswordRecord record = form.GetPassword();
            form.Dispose();

            // If caption is empty, do nothing
            if (record == null || String.IsNullOrEmpty(record.GetCaption()))
            {
                return;
            }

            // Get selected container
            // Throw an exception when CurrentTreeNode value is null. This should not be happend.
            if (this.CurrentTreeNode == null || this.CurrentTreeNode.Tag == null)
            {
                //return;
                throw new InvalidOperationException();
            }

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
            if (e.Node.Tag == null)
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
                case Keys.F2:
                    this.CurrentTreeNode.BeginEdit();
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
            if (e.Label == null || e.Node.Tag == null)
            {
                return;
            }

            PasswordContainer container = this.PasswordData.Indexer.GetContainerByID(this.PasswordData.Containers, (int)e.Node.Tag);

            if (container == null)
            {
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
            // Retrieve the client coordinates of the mouse position.
            Point targetPoint = this.treeView_Folders.PointToClient(new Point(e.X, e.Y));

            // Select the node at the mouse position.
            this.treeView_Folders.SelectedNode = this.treeView_Folders.GetNodeAt(targetPoint);
            this.CurrentTreeNode = this.treeView_Folders.SelectedNode;
        }

        /// <summary>
        /// This is called when dragged node is dropped
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void treeView_Folders_DragDrop(object sender, DragEventArgs e)
        {
            Point targetPoint = treeView_Folders.PointToClient(new Point(e.X, e.Y));
            TreeNode targetNode = treeView_Folders.GetNodeAt(targetPoint);
            TreeNode draggedNode = (TreeNode)e.Data.GetData(typeof(TreeNode));

            if(targetNode.Tag == null || draggedNode.Tag == null)
            {
                return;
            }

            if (!draggedNode.Equals(targetNode) && !ContainsNode(draggedNode, targetNode))
            {
                if (e.Effect == DragDropEffects.Move)
                {
                    draggedNode.Remove();
                    this.PasswordData.Indexer.RemoveContainer((int)draggedNode.Tag);

                    targetNode.Nodes.Add(draggedNode);
                    this.PasswordData.Indexer.AppendContainer((int)draggedNode.Tag, (int)targetNode.Tag);
                }

                targetNode.Expand();
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
            this.treeView_Folders.Nodes.Add(this.GetTreeViewNodeBuilt(containers, indexer));
        }

        /// <summary>
        /// Generate completed tree object by provided containers and indexer object
        /// </summary>
        /// <param name="containers"></param>
        /// <param name="indexer"></param>
        /// <returns></returns>
        private TreeNode GetTreeViewNodeBuilt(ICollection<PasswordContainer> containers, PasswordIndexerBase indexer)
        {
            // Setup root parent container
            int rootContainerID = InternalApplicationConfig.RootContainerID;
            TreeNode rootNode = new TreeNode(InternalApplicationConfig.RootContainerLabel);
            rootNode.Tag = rootContainerID;

            // Execute recursive tree method
            this.AddContainerToTreeView(containers, indexer, rootContainerID, rootNode);

            // Attach context menu strip
            rootNode.ContextMenuStrip = this.contextMenuStrip_TreeViewNode;

            return rootNode;
        }

        /// <summary>
        /// Setup container tree in a recursive manner.
        /// </summary>
        /// <param name="parentContainerID"></param>
        /// <param name="containers"></param>
        /// <param name="indexer"></param>
        private void AddContainerToTreeView(ICollection<PasswordContainer> containers, PasswordIndexerBase indexer, int parentContainerID, TreeNode parentNode)
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
                node.ContextMenuStrip = this.contextMenuStrip_TreeViewNode; // Attach context menu strip

                parentNode.Nodes.Add(node); // Add the current node to parent node

                this.AddContainerToTreeView(containers, indexer, childContainerID, node);
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
        /// Determine whether one node is a parent or ancestor of a second node.
        /// </summary>
        /// <param name="node1"></param>
        /// <param name="node2"></param>
        /// <returns></returns>
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
            this.toolStripStatusLabel1.Text = strings.Status_Ready;
            this.toolStripButton_Open.ToolTipText = strings.Form_Tooltip_OpenFile;
            this.toolStripButton_Save.ToolTipText = strings.Form_Tooltip_SaveFile;
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
        }
        #endregion
    }
}
