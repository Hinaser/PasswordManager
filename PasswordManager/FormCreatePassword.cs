#region Notice
/*
 * Author: Yunoske
 * Create Date: June 4, 2015
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
#endregion

namespace PasswordManager
{
    /// <summary>
    /// This form is used to create new password record
    /// </summary>
    public partial class FormCreatePassword : Form
    {
        #region Field
        private int ParentContainerID;
        #endregion

        #region Constructor
        private FormCreatePassword() { } // Disable default constructor

        /// <summary>
        /// After this class is instantiated, parentContainerID is always with the instance
        /// </summary>
        /// <param name="parentContainerID"></param>
        public FormCreatePassword(int parentContainerID)
        {
            this.ParentContainerID = parentContainerID;
            InitializeComponent();
        }
        #endregion
    }
}
