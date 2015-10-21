#region Notice
/*
 * Copyright 2015 Hinaser
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PasswordManager
{
    public partial class FormMovePassword : Form
    {
        #region Fields
        private int DestinationContainerID = ContainerIDUnSet;
        private static readonly int ContainerIDUnSet = -1;
        #endregion

        #region Constructor
        private FormMovePassword() { } // Disable default constructor
        public FormMovePassword(ICollection<PasswordContainer> containers, PasswordIndexerBase indexer)
        {
            InitializeComponent();

            // Events
            this.treeView_MovePassword_Folders.AfterSelect += treeView_MovePassword_Folders_AfterSelect;
            this.button_MovePassword_OK.Click += button_MovePassword_OK_Click;

            // Construct treeview control
            this.InitializeTreeStructure(containers, indexer);
            this.treeView_MovePassword_Folders.ExpandAll();
        }
        #endregion

        #region Getter method
        /// <summary>
        /// Get destination container id
        /// </summary>
        /// <returns></returns>
        public int GetTargetContainerID()
        {
            if (this.DestinationContainerID == ContainerIDUnSet)
            {
                throw new InvalidOperationException();
            }

            return this.DestinationContainerID;
        }
        #endregion

        #region Events
        /// <summary>
        /// Enable OK button when one of treeview item is selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void treeView_MovePassword_Folders_AfterSelect(object sender, TreeViewEventArgs e)
        {
            this.button_MovePassword_OK.Enabled = true;
        }

        /// <summary>
        /// Get back to caller with selected folder information
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void button_MovePassword_OK_Click(object sender, EventArgs e)
        {
            // When any nodes are not selected, show warning message and exit
            if (this.treeView_MovePassword_Folders.SelectedNode == null)
            {
                MessageBox.Show(strings.General_MovePassword_NotSelected_Text, strings.General_MovePassword_NotSelected_Caption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // This code should not be executed.
            if (this.treeView_MovePassword_Folders.SelectedNode.Tag == null)
            {
                throw new InvalidOperationException();
            }

            this.DestinationContainerID = (int)this.treeView_MovePassword_Folders.SelectedNode.Tag;

            this.DialogResult = DialogResult.OK;
            this.Close();
            return;
        }
        #endregion

        #region Utility
        /// <summary>
        /// Set up password container/record tree structure by PasswordIndexer object
        /// </summary>
        /// <param name="indexer"></param>
        public void InitializeTreeStructure(ICollection<PasswordContainer> containers, PasswordIndexerBase indexer)
        {
            // Remove all children from folder
            this.treeView_MovePassword_Folders.Nodes.Clear();

            // Construct container tree
            this.treeView_MovePassword_Folders.Nodes.Add(FormMain.GetTreeViewNodeBuilt(containers, indexer, null));
        }
        #endregion
    }
}
