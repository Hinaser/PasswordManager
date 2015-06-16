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
using System.Globalization;
using System.IO;
using System.Threading;
#endregion

namespace PasswordManager
{
    public partial class FormInputMasterPassword : Form
    {
        private PasswordHashChecker checker = null;
        private string PasswordFilePath;
        private int SleepMiliSecond = InternalApplicationConfig.InitialWaitMiliSecondsWhenPasswordIsInvalid;
        private int ElapsedMiliSecond = 0;
        private System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();

        private FormInputMasterPassword() { } // Disable default constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Func"></param>
        /// <param name="filePath"></param>
        public FormInputMasterPassword(PasswordHashChecker Func, string filePath)
        {
            if (Func == null || String.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                throw new ArgumentException();
            }

            if (SleepMiliSecond < 1)
            {
                throw new ApplicationException();
            }

            InitializeComponent();
            this.Initialize();

            this.checker = Func;
            this.PasswordFilePath = filePath;
            this.timer.Interval = InternalApplicationConfig.RetryTickIntervalMiliSec;
            this.timer.Tick += timer_Tick;
        }

        /// <summary>
        /// Initialize dialog custom settings
        /// </summary>
        public void Initialize()
        {
            // Setup langauge combbox
            this.comboBox_InputMasterPassword_Language.Items.Add(strings.Form_MenuItem_Language_English);
            this.comboBox_InputMasterPassword_Language.Items.Add(strings.Form_MenuItem_Language_Japanese);
            this.comboBox_InputMasterPassword_Language.Text = strings.Form_MenuItem_Language_English;

            // Setup control events
            this.comboBox_InputMasterPassword_Language.SelectedIndexChanged += comboBox_InputMasterPassword_Language_SelectedValueChanged;
            this.button_InputMasterPassword_OK.Click += button_InputMasterPassword_OK_Click;
        }

        /// <summary>
        /// Change language internal value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void comboBox_InputMasterPassword_Language_SelectedValueChanged(object sender, EventArgs e)
        {
            if (this.comboBox_InputMasterPassword_Language.Text == strings.Form_MenuItem_Language_English)
            {
                this.SetupLanguage(InternalApplicationConfig.LocaleEnUS);
                return;
            }

            if (this.comboBox_InputMasterPassword_Language.Text == strings.Form_MenuItem_Language_Japanese)
            {
                this.SetupLanguage(InternalApplicationConfig.LocaleJaJP);
                return;
            }
        }

        /// <summary>
        /// Try user input password
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void button_InputMasterPassword_OK_Click(object sender, EventArgs e)
        {
            // Get input password hash
            byte[] hash = this.GetMasterPasswordHash();

            // Challenge password
            bool isPasswordHashValid = this.checker(this.PasswordFilePath, hash);

            // If password is OK, close form.
            if (isPasswordHashValid == true)
            {
                this.Close();
                this.DialogResult = DialogResult.OK;
                return;
            }

            // If password is not OK, sleep a bit time and re-enable buttons
            this.label_InputMasterPassword.Text = strings.Form_InputMasterPassword_InvalidPassword;
            this.button_InputMasterPassword_OK.Text = String.Format(strings.Form_InputMasterPassword_Waiting, this.SleepMiliSecond/1000);
            this.button_InputMasterPassword_OK.Enabled = false;
            this.timer.Start();
        }

        /// <summary>
        /// Activate OK button or update label text of OK button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void timer_Tick(object sender, EventArgs e)
        {
            this.ElapsedMiliSecond += InternalApplicationConfig.RetryTickIntervalMiliSec;

            // When time has not been elapsed enough, update button text and continue;
            if (this.ElapsedMiliSecond < this.SleepMiliSecond)
            {
                double remainDouble = (this.SleepMiliSecond - this.ElapsedMiliSecond) / 1000.0;
                int remainInt = (int)Math.Ceiling(remainDouble);
                this.button_InputMasterPassword_OK.Text = String.Format(strings.Form_InputMasterPassword_Waiting, remainInt);
                return;
            }

            // When time has expired, re-enable button and stop timer
            this.button_InputMasterPassword_OK.Text = strings.Form_InputMasterPassword_OK;
            this.button_InputMasterPassword_OK.Enabled = true;
            this.label_InputMasterPassword.Text = strings.Form_InputMasterPassword_Caption;
            this.ElapsedMiliSecond = 0;
            this.SleepMiliSecond = this.SleepMiliSecond * 2;
            this.timer.Stop();
        }

        /// <summary>
        /// Get hash of input master password.
        /// </summary>
        /// <remarks>Input password string will be sanitized immediately after hash is computed.</remarks>
        /// <returns></returns>
        public byte[] GetMasterPasswordHash()
        {
            string password = this.textBox_InputMasterPassword.Text;

            if (String.IsNullOrEmpty(password))
            {
                return Utility.GetHash(new byte[]{0});
            }

            byte[] retVal = Utility.GetHash(password.ToCharArray());

            return retVal;
        }

        /// <summary>
        /// Delete password string completely from password textbox
        /// </summary>
        public unsafe void SanitizeInputPassword()
        {
            string password = this.textBox_InputMasterPassword.Text;

            fixed (char* p = password)
            {
                for (int i = 0; i < password.Length; i++)
                {
                    p[i] = '\0';
                }
            }

            this.textBox_InputMasterPassword.Text = String.Empty;
        }

        /// <summary>
        /// Change UI language appearance
        /// </summary>
        /// <param name="locale"></param>
        private void SetupLanguage(string locale)
        {
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(locale);

            this.label_InputMasterPassword.Text = strings.Form_InputMasterPassword_Caption;
            this.button_InputMasterPassword_OK.Text = strings.Form_InputMasterPassword_OK;
            this.button_InputMasterPassword_Cancel.Text = strings.Form_InputMasterPassword_Cancel;
            this.Text = strings.Form_InputMasterPassword_Title;
        }
    }
}
