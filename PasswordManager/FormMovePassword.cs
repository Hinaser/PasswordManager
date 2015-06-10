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
        private FormMovePassword() { } // Disable default constructor
        public FormMovePassword(ICollection<PasswordContainer> containers, PasswordIndexerBase indexer)
        {
            InitializeComponent();
            this.InitializeTreeStructure(containers, indexer);
            this.treeView_MovePassword_Folders.ExpandAll();
        }

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
    }
}
