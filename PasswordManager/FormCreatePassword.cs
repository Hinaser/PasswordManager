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
        private PasswordRecord Password = null;
        #endregion

        #region Constructor
        /// <summary>
        /// After this class is instantiated, parentContainerID is always with the instance
        /// </summary>
        /// <param name="parentContainerID"></param>
        public FormCreatePassword()
        {
            InitializeComponent();

            // Checkbox event
            this.checkBox_NewPassword_UseSymbols.CheckedChanged += checkBox_NewPassword_UseSymbols_CheckedChanged;
            // Button event
            this.button_NewPassword_OK.Click += button_NewPassword_OK_Click;
            this.button_NewPassword_GeneratePassword.Click += button_NewPassword_GeneratePassword_Click;
            // Textbox event
            this.textBox_NewPassword_Caption.TextChanged += textBox_NewPassword_Caption_TextChanged;

            // Do initializing process
            this.InitializeFormStatus();
        }
        #endregion

        #region Initialize
        /// <summary>
        /// Initialize form control status
        /// </summary>
        public void InitializeFormStatus()
        {
            this.checkBox_NewPassword_UseNumerics.Checked = true;
            this.checkBox_NewPassword_UseSymbols.Checked = true;
            this.SetupLanguage();
        }
        #endregion

        #region Getter method
        /// <summary>
        /// Get password object which user entered
        /// </summary>
        /// <returns></returns>
        public PasswordRecord GetPassword()
        {
            return this.Password;
        }
        #endregion

        #region Event

        #region Checkbox event
        /// <summary>
        /// When UseSysmbols checkbox is checked/unchecked, associated symbol checkboxes are checked/unchecked as well.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void checkBox_NewPassword_UseSymbols_CheckedChanged(object sender, EventArgs e)
        {
            foreach (CheckBox c in this.groupBox_NewPassword_AllowedSymbols.Controls)
            {
                c.Checked = this.checkBox_NewPassword_UseSymbols.Checked;
            }
        }
        #endregion

        #region Button event
        /// <summary>
        /// If OK button is clicked, try to make 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void button_NewPassword_OK_Click(object sender, EventArgs e)
        {
            // Set dialog input values to PasswordRecord object
            this.Password = new PasswordRecord();
            this.Password.SetHeaderData(this.textBox_NewPassword_Caption.Text, DateTime.UtcNow, DateTime.UtcNow);
            this.Password.SetPrivateData(this.textBox_NewPassword_ID.Text, this.textBox_NewPassword_Password.Text, this.textBox_NewPassword_Memo.Text);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        /// <summary>
        /// Generate password text stream
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void button_NewPassword_GeneratePassword_Click(object sender, EventArgs e)
        {
            var aaa = this.GeneratePasswordCharacterPool();
            int a = 0;
        }
        #endregion

        #region Textbox event
        /// <summary>
        /// When user enters some chars to Caption textbox and the textbox is not empty, enable OK button on the dialog.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void textBox_NewPassword_Caption_TextChanged(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(this.textBox_NewPassword_Caption.Text) || this.textBox_NewPassword_Caption.Text.Length < 1)
            {
                if (this.button_NewPassword_OK.Enabled)
                {
                    this.button_NewPassword_OK.Enabled = false;
                }

                return;
            }

            this.button_NewPassword_OK.Enabled = true;
        }
        #endregion

        #endregion

        #region Utility
        /// <summary>
        /// Return character pool which can be used to construct actual password string
        /// </summary>
        /// <returns></returns>
        public char[] GeneratePasswordCharacterPool()
        {
            List<char> pool = new List<char>();

            // Alphabet characters are always used as password
            pool.AddRange(Utility.Alphabet);
            pool.AddRange(Utility.ALPHABET);

            if (this.checkBox_NewPassword_UseNumerics.Checked)
            {
                pool.AddRange(Utility.Numeric);
            }

            if (this.checkBox_NewPassword_UseSymbols.Checked)
            {
                foreach (CheckBox c in this.groupBox_NewPassword_AllowedSymbols.Controls)
                {
                    if (c.Checked)
                    {
                        char symbol = c.Text.ToCharArray(0, 1)[0];
                        if(c.Text == this.checkBox_NewPassword_Space.Text)
                        {
                            symbol = ' ';
                        }
                        pool.Add(symbol);
                    }
                }
            }

            return pool.ToArray();
        }
        #endregion

        #region Setup language
        /// <summary>
        /// Setup language-variable text to this dialog
        /// </summary>
        public void SetupLanguage()
        {
            this.label_NewPassword_Caption.Text = strings.Form_NewPassword_Caption;
            this.label_NewPassword_ID.Text = strings.Form_NewPassword_ID;
            this.label_NewPassword_Password.Text = strings.Form_NewPassword_Password;
            this.groupBox_NewPassword_Test.Text = strings.Form_NewPassword_StrengthTest;
            this.groupBox_NewPassword_Randomize.Text = strings.Form_NewPassword_Randomize;
            this.checkBox_NewPassword_UseNumerics.Text = strings.Form_NewPassword_UseNumeric;
            this.checkBox_NewPassword_UseSymbols.Text = strings.Form_NewPassword_UseSymbolic;
            this.checkBox_NewPassword_UseLowerCase.Text = strings.Form_NewPassword_UseLowercase;
            this.checkBox_NewPassword_UseUpperCase.Text = strings.Form_NewPassword_UseUppercase;
            this.button_NewPassword_GeneratePassword.Text = strings.Form_NewPassword_Generate;
            this.groupBox_NewPassword_AllowedSymbols.Text = strings.Form_NewPassword_AllowedSymbols;
            this.label_NewPassword_MinChars.Text = strings.Form_NewPassword_MinChars;
            this.label_NewPassword_MaxChars.Text = strings.Form_NewPassword_MaxChars;
            this.label_NewPassword_Week.Text = strings.Form_NewPassword_PasswdWeak;
            this.label_NewPassword_Normal.Text = strings.Form_NewPassword_PasswdNormal;
            this.label_NewPassword_Secure.Text = strings.Form_NewPassword_PasswdSecure;
            this.label_NewPassword_Memo.Text = strings.Form_NewPassword_Memo;
            this.button_NewPassword_OK.Text = strings.Form_NewPassword_OK;
            this.button_NewPassword_Cancel.Text = strings.Form_NewPassword_Cancel;
            this.Text = strings.Form_NewPassword_WndTitle;
        }
        #endregion
    }
}
