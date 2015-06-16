#region Notice
/*
 * Author: Yunoske
 * Create Date: May 27, 2015
 * Description :
 * 
 */
#endregion

#region Using Statement
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
    public partial class FormInputMasterPassword : Form
    {
        public FormInputMasterPassword()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Get hash of input master password.
        /// </summary>
        /// <remarks>Input password string will be sanitized immediately after hash is computed.</remarks>
        /// <returns></returns>
        public unsafe byte[] GetMasterPasswordHash()
        {
            string password = this.textBox_InputMasterPassword.Text;

            if (String.IsNullOrEmpty(password))
            {
                return Utility.GetHash(new byte[]{0});
            }

            byte[] retVal = Utility.GetHash(password.ToCharArray());

            fixed(char* p = password)
            {
                for (int i = 0; i < password.Length; i++)
                {
                    p[i] = '\0';
                }
            }

            this.textBox_InputMasterPassword.Text = String.Empty;

            return retVal;
        }

        private void SetupLanguage()
        {
            this.label_InputMasterPassword.Text = strings.Form_InputMasterPassword_Caption;
            this.button_InputMasterPassword_OK.Text = strings.Form_InputMasterPassword_OK;
            this.button_InputMasterPassword_Cancel.Text = strings.Form_InputMasterPassword_Cancel;
        }
    }
}
