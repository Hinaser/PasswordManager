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
        #region Constructor
        public MainForm_PasswordManager()
        {
            InitializeComponent();

            // ListView event
            this.listView_PasswordItems.SizeChanged += listView_PasswordItems_SizeChanged;
            this.listView_PasswordItems.ColumnWidthChanged += listView_PasswordItems_ColumnWidthChanged;
            // Tooltip menu botton event
            this.toolStripButton_Open.Click += toolStripButton_Open_Click;

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
        /// Run file open process
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void toolStripButton_Open_Click(object sender, EventArgs e)
        {
            PasswordFile f = new PasswordFile("test.txt");
            f.ReadPasswordFromFile(new byte[] { 1, 2, 3, 4, 5 });
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
            long wndStyle = PrivateUtility.GetWindowLong(lv.Handle, PrivateUtility.GwlStyle);

            if ((wndStyle & PrivateUtility.WsVScroll) != 0)
            {
                return true;
            }

            return false;
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
