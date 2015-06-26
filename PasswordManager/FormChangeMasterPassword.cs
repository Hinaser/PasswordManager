#region Notice
/*
 * Author: Yunoske
 * Create Date: June 18, 2015
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
    public partial class FormChangeMasterPassword : Form
    {
        public byte[] MasterPasswordHash;

        public FormChangeMasterPassword()
        {
            InitializeComponent();
            Initialize();
        }

        /// <summary>
        /// Initialize form
        /// </summary>
        public void Initialize()
        {
            // Setup language
            this.SetupLanguage();

            // Register events
            this.checkBox_ChangeMasterPassword.CheckedChanged += checkBox_ChangeMasterPassword_CheckedChanged;
            this.textBox_ChangeMasterPassword_1.TextChanged += textBox_ChangeMasterPassword_1_TextChanged;
            this.textBox_ChangeMasterPassword_2.TextChanged += textBox_ChangeMasterPassword_2_TextChanged;
            this.button_ChangeMasterPassword_OK.Click += button_ChangeMasterPassword_OK_Click;
        }

        /// <summary>
        /// Swithch password text box hidden status.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void checkBox_ChangeMasterPassword_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox_ChangeMasterPassword.Checked)
            {
                this.textBox_ChangeMasterPassword_1.UseSystemPasswordChar = false;
                this.textBox_ChangeMasterPassword_2.UseSystemPasswordChar = false;
            }
            else
            {
                this.textBox_ChangeMasterPassword_1.UseSystemPasswordChar = true;
                this.textBox_ChangeMasterPassword_2.UseSystemPasswordChar = true;
            }
        }

        /// <summary>
        /// Validate master password strength.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void textBox_ChangeMasterPassword_1_TextChanged(object sender, EventArgs e)
        {
            TextBox t = sender as TextBox;

            // Reset label color
            this.label_NewPassword_Weak.BackColor = Color.Transparent;
            this.label_NewPassword_Weak.ForeColor = Color.DarkGray;
            this.label_NewPassword_Normal.BackColor = Color.Transparent;
            this.label_NewPassword_Normal.ForeColor = Color.DarkGray;
            this.label_NewPassword_Secure.BackColor = Color.Transparent;
            this.label_NewPassword_Secure.ForeColor = Color.DarkGray;

            // Reset strength notification textarea
            this.richTextBox_NewPassword_Strength.Clear();

            if (!String.IsNullOrEmpty(t.Text))
            {
                // Analyze password class and show on strength report text area
                PasswordTextClass c = FormCreatePassword.GetPasswordClass(t.Text);
                this.richTextBox_NewPassword_Strength.AppendText(String.Format(LocalConfig.PasswordStrengthNoticeFormat, strings.General_NewPassword_Strength_TypePassClass, FormCreatePassword.GetPasswordClassText(c)));

                // Calculate password strength
                PasswordComplexityValidatorBase validator = new PasswordComplexityValidator();
                double adjustedPasswordLength = validator.GetAdjustedCharLength(this.richTextBox_NewPassword_Strength, t.Text, t.Text.Length, FormCreatePassword.AppendStrengthReport);
                double strength = FormCreatePassword.CalculatePasswordStrength(t.Text, adjustedPasswordLength);
                this.richTextBox_NewPassword_Strength.AppendText(String.Format(LocalConfig.PasswordStrengthNoticeFormat, strings.General_NewPassword_Strength_TypeResult, Math.Round(strength, 2).ToString()));

                // When judged as a weak password
                if (strength <= LocalConfig.MaxWeakPasswordStrength)
                {
                    this.label_NewPassword_Weak.BackColor = Color.Orange;
                    this.label_NewPassword_Weak.ForeColor = Color.Black;
                }
                // When judged as a normal password
                else if (LocalConfig.MaxWeakPasswordStrength < strength && strength <= LocalConfig.MaxNormalPasswordStrength)
                {
                    this.label_NewPassword_Normal.BackColor = Color.LightSeaGreen;
                    this.label_NewPassword_Normal.ForeColor = Color.Black;
                }
                // When judged as a strong password
                else if (LocalConfig.MaxNormalPasswordStrength < strength)
                {
                    this.label_NewPassword_Secure.BackColor = Color.GreenYellow;
                    this.label_NewPassword_Secure.ForeColor = Color.Black;
                }
            }

            this.ValidateBothPasswords();
        }

        /// <summary>
        /// Check password text matches
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void textBox_ChangeMasterPassword_2_TextChanged(object sender, EventArgs e)
        {
            this.ValidateBothPasswords();
        }

        /// <summary>
        /// Check 2 passwords match and close form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void button_ChangeMasterPassword_OK_Click(object sender, EventArgs e)
        {
            if (this.textBox_ChangeMasterPassword_1.Text != this.textBox_ChangeMasterPassword_2.Text)
            {
                return;
            }

            // If specified master password is empty, pop up an alert dialog.
            if (String.IsNullOrEmpty(this.textBox_ChangeMasterPassword_1.Text))
            {
                if (MessageBox.Show(strings.General_MasterPassword_Empty_Text, strings.General_MasterPassword_Empty_Caption, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) != DialogResult.OK)
                {
                    return;
                }
            }

            string password = this.textBox_ChangeMasterPassword_1.Text;

            this.MasterPasswordHash = Utility.Scramble(password, LocalConfig.DefaultSalt, LocalConfig.MasterPasswordHashedKeySize);
            this.SanitizeInputPassword();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        /// <summary>
        /// Delete password string completely from password textbox
        /// </summary>
        public unsafe void SanitizeInputPassword()
        {
            string password1 = this.textBox_ChangeMasterPassword_1.Text;
            string password2 = this.textBox_ChangeMasterPassword_2.Text;

            if (!String.IsNullOrEmpty(password1))
            {
                fixed (char* p = password1)
                {
                    for (int i = 0; i < password1.Length; i++)
                    {
                        p[i] = '\0';
                    }
                }
            }

            if (!String.IsNullOrEmpty(password2))
            {
                fixed (char* p = password2)
                {
                    for (int i = 0; i < password2.Length; i++)
                    {
                        p[i] = '\0';
                    }
                }
            }

            this.textBox_ChangeMasterPassword_1.Text = String.Empty;
            this.textBox_ChangeMasterPassword_2.Text = String.Empty;
        }

        /// <summary>
        /// Check whether values in both password textboxes match
        /// </summary>
        /// <returns></returns>
        private void ValidateBothPasswords()
        {
            if (this.textBox_ChangeMasterPassword_1.Text == this.textBox_ChangeMasterPassword_2.Text)
            {
                this.button_ChangeMasterPassword_OK.Enabled = true;
                return;
            }

            this.button_ChangeMasterPassword_OK.Enabled = false;
            return;
        }

        /// <summary>
        /// Get master password hash.
        /// </summary>
        /// <returns></returns>
        public byte[] GetMasterPasswordHash()
        {
            return this.MasterPasswordHash;
        }

        /// <summary>
        /// Set text to controls for current language setting.
        /// </summary>
        private void SetupLanguage()
        {
            this.Text = strings.Form_ChangeMasterPassword_Title;
            this.label_ChangeMasterPassword.Text = strings.Form_ChangeMasterPassword_Label;
            this.label_ChangeMasterPassword_RepeatPassword.Text = strings.Form_ChangeMasterPassword_RepeatPassword;
            this.checkBox_ChangeMasterPassword.Text = strings.Form_ChangeMasterPassword_Checkbox;
            this.button_ChangeMasterPassword_OK.Text = strings.Form_ChangeMasterPassword_OK;
            this.button_ChangeMasterPassword_Cancel.Text = strings.Form_ChangeMasterPassword_Cancel;
            this.groupBox_NewPassword_Test.Text = strings.Form_NewPassword_StrengthTest;
            this.label_NewPassword_Weak.Text = strings.Form_NewPassword_PasswdWeak;
            this.label_NewPassword_Normal.Text = strings.Form_NewPassword_PasswdNormal;
            this.label_NewPassword_Secure.Text = strings.Form_NewPassword_PasswdSecure;
        }
    }
}
