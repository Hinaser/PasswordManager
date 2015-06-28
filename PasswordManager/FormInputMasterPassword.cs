#region Notice
/*
 * Copyright 2015 Yunoske
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
        private int SleepMiliSecond = LocalConfig.InitialWaitMiliSecondsWhenPasswordIsInvalid;
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
            this.timer.Interval = LocalConfig.RetryTickIntervalMiliSec;
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

            // Setup language
            this.SetupLanguage(Thread.CurrentThread.CurrentUICulture.Name);
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
                this.SetupLanguage(LocalConfig.LocaleEnUS);
                return;
            }

            if (this.comboBox_InputMasterPassword_Language.Text == strings.Form_MenuItem_Language_Japanese)
            {
                this.SetupLanguage(LocalConfig.LocaleJaJP);
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
            this.label_InputMasterPassword.ForeColor = Color.Red;
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
            this.ElapsedMiliSecond += LocalConfig.RetryTickIntervalMiliSec;

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
            this.label_InputMasterPassword.ForeColor = Color.Black;
            this.ElapsedMiliSecond = 0;
            this.SleepMiliSecond = this.SleepMiliSecond * 2;
            this.timer.Stop();
        }

        /// <summary>
        /// Get hash of input master password.
        /// </summary>
        /// <remarks>Blank password is handled properly inside the method.</remarks>
        /// <returns></returns>
        public byte[] GetMasterPasswordHash()
        {
            string password = this.textBox_InputMasterPassword.Text;
            if (password == null)
            {
                password = String.Empty;
            }

            byte[] retVal = Utility.Scramble(password, LocalConfig.DefaultSalt, LocalConfig.MasterPasswordHashedKeySize);

            return retVal;
        }

        /// <summary>
        /// Delete password string completely from password textbox
        /// </summary>
        public unsafe void SanitizeInputPassword()
        {
            string password = this.textBox_InputMasterPassword.Text;

            if (String.IsNullOrEmpty(password))
            {
                return;
            }

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
        /// Change UI language appearance by specified locale string
        /// </summary>
        /// <param name="locale"></param>
        public void SetupLanguage(string locale)
        {
            if (!String.IsNullOrEmpty(locale))
            {
                Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(locale);
            }

            this.label_InputMasterPassword.Text = strings.Form_InputMasterPassword_Caption;
            this.button_InputMasterPassword_OK.Text = strings.Form_InputMasterPassword_OK;
            this.button_InputMasterPassword_Cancel.Text = strings.Form_InputMasterPassword_Cancel;
            this.Text = strings.Form_InputMasterPassword_Title;

            if (locale == LocalConfig.LocaleEnUS)
            {
                this.comboBox_InputMasterPassword_Language.Text = strings.Form_MenuItem_Language_English;
                return;
            }

            if (locale == LocalConfig.LocaleJaJP)
            {
                this.comboBox_InputMasterPassword_Language.Text = strings.Form_MenuItem_Language_Japanese;
                return;
            }
        }
    }
}
