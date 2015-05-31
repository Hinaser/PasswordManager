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
        PasswordFileBody PasswordData = new PasswordFileBody();
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
            // Tooltip menu botton event
            this.toolStripButton_Open.Click += toolStripButton_Open_Click;
            this.toolStripButton_Save.Click += toolStripButton_Save_Click;

            // Apply language setting
            SetupLanguage(InternalApplicationConfig.DefaultLocale);
        }
        #endregion

        #region Event
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

            this.PasswordData = new PasswordFileBody(new PasswordIndexer(), new List<PasswordContainer>(), new List<PasswordRecord>());
            this.PasswordData.Containers.Add(new PasswordContainer(1, "test1"));
            this.PasswordData.Containers.Add(new PasswordContainer(2, "test2"));
            this.PasswordData.Containers.Add(new PasswordContainer(3, "test3"));
            this.PasswordData.Containers.Add(new PasswordContainer(4, "test4"));
            this.PasswordData.Containers.Add(new PasswordContainer(5, "test5"));
            this.PasswordData.Containers.Add(new PasswordContainer(6, "test6"));
            this.PasswordData.Containers.Add(new PasswordContainer(7, "test7"));
            this.PasswordData.Records.Add(new PasswordRecord(1));
            this.PasswordData.Records.Add(new PasswordRecord(2));
            this.PasswordData.Records.Add(new PasswordRecord(3));
            this.PasswordData.Records.Add(new PasswordRecord(4));
            this.PasswordData.Indexer.AppendContainer(1, 0);
            this.PasswordData.Indexer.AppendContainer(2, 0);
            this.PasswordData.Indexer.AppendContainer(3, 1);
            this.PasswordData.Indexer.AppendContainer(4, 3);
            this.PasswordData.Indexer.AppendContainer(5, 7);
            this.PasswordData.Indexer.AppendContainer(6, 2);
            this.PasswordData.Indexer.AppendContainer(7, 6);
            this.PasswordData.Indexer.AppendRecord(1, 0);
            this.PasswordData.Indexer.AppendRecord(2, 1);
            this.PasswordData.Indexer.AppendRecord(3, 1);
            this.PasswordData.Indexer.AppendRecord(4, 2);
            f.WritePasswordToFile(Utility.GetHash(new byte[] { 0xff, 0xfe, 0x00, 0x01, 0x02 }), this.PasswordData);
        }

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

            // Initialize listview/textbox control
            this.listView_PasswordItems.Items.Clear();
            this.textBox_ItemDescription.Text = String.Empty;

            foreach (int recordID in this.PasswordData.Indexer.GetChildRecords(containerID))
            {
                PasswordRecord record = this.PasswordData.Indexer.GetRecordByID(this.PasswordData.Records, recordID);
                ListViewItem lvi = new ListViewItem();
                lvi.Text = record.GetCaption();
                lvi.SubItems.Add(record.GetID().ToString());
                lvi.SubItems.Add(record.GetPassword());
            }
        }
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

                parentNode.Nodes.Add(node); // Add the current node to parnet node

                this.AddContainerToTreeView(containers, indexer, childContainerID, node);
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
        }
        #endregion
    }
}
