﻿#region Notice
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
using System.Text.RegularExpressions;
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
            this.textBox_NewPassword_Password.TextChanged += textBox_NewPassword_Password_TextChanged;

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
            this.checkBox_NewPassword_UseLowerCase.Checked = true;
            this.checkBox_NewPassword_UseUpperCase.Checked = true;
            this.checkBox_NewPassword_UseNumerics.Checked = true;
            this.checkBox_NewPassword_UseSymbols.Checked = false;
            this.numericUpDown_NewPassword_Minchars.Minimum = InternalApplicationConfig.PasswordMinLength;
            this.numericUpDown_NewPassword_Minchars.Maximum = InternalApplicationConfig.passwordMaxLength;
            this.numericUpDown_NewPassword_Maxchars.Minimum = InternalApplicationConfig.PasswordMinLength;
            this.numericUpDown_NewPassword_Maxchars.Maximum = InternalApplicationConfig.passwordMaxLength;
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
            // When even one of checkbox status is different from UseSymbols checkbox, do nothing
            foreach (CheckBox c in this.groupBox_NewPassword_AllowedSymbols.Controls)
            {
                if (c.Checked != !this.checkBox_NewPassword_UseSymbols.Checked)
                {
                    return;
                }
            }

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
            // Disable Generate button for a moment
            this.button_NewPassword_GeneratePassword.Enabled = false;

            // Check parameters
            // Check min-max characters value
            if (this.numericUpDown_NewPassword_Minchars.Value < InternalApplicationConfig.PasswordMinLength
                || this.numericUpDown_NewPassword_Minchars.Value < Int32.MinValue)
            {
                MessageBox.Show(strings.Form_NewPassword_ErrorMinTooSmall, strings.Form_NewPassword_ErrorDialogCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.button_NewPassword_GeneratePassword.Enabled = true;
                this.button_NewPassword_GeneratePassword.Focus();
                return;
            }
            if (this.numericUpDown_NewPassword_Maxchars.Value > InternalApplicationConfig.passwordMaxLength
                || this.numericUpDown_NewPassword_Maxchars.Value > Int32.MaxValue)
            {
                MessageBox.Show(strings.Form_NewPassword_ErrorMaxTooLarge, strings.Form_NewPassword_ErrorDialogCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.button_NewPassword_GeneratePassword.Enabled = true;
                this.button_NewPassword_GeneratePassword.Focus();
                return;
            }
            if (this.numericUpDown_NewPassword_Minchars.Value > this.numericUpDown_NewPassword_Maxchars.Value)
            {
                MessageBox.Show(strings.Form_NewPassword_ErrorMinLTMax, strings.Form_NewPassword_ErrorDialogCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.button_NewPassword_GeneratePassword.Enabled = true;
                this.button_NewPassword_GeneratePassword.Focus();
                return;
            }
            int minChars = (int)this.numericUpDown_NewPassword_Minchars.Value;
            int maxChars = (int)this.numericUpDown_NewPassword_Maxchars.Value;

            // Decide password length between min and max
            int passwordLength = GetRandomInt(minChars, maxChars, 0);
            char[] password = new char[passwordLength];
            char[] passwordPool = this.GeneratePasswordCharacterPool();

            // Randome seed
            int seed = this.Location.X * this.Location.Y;

            // Pick up random characters from password pool
            for (int i = 0; i < password.Length; i++)
            {
                password[i] = passwordPool[GetRandomInt(passwordPool.Length - 1, seed)];
            }

            this.textBox_NewPassword_Password.Text = new String(password);
            // Clear password data from temporary variable
            for (int i = 0; i < password.Length; i++) password[i] = (char)0;

            // Enable button again
            this.button_NewPassword_GeneratePassword.Enabled = true;
            this.button_NewPassword_GeneratePassword.Focus();

            return;
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

        /// <summary>
        /// At the moment password text is updated, validate the password text strength
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void textBox_NewPassword_Password_TextChanged(object sender, EventArgs e)
        {
            TextBox t = sender as TextBox;
            double adjustedPasswordLength = this.AdjustPasswordStrength(t.Text);

            double strength = FormCreatePassword.CalculatePasswordStrength(t.Text, adjustedPasswordLength);

            // Reset label color
            this.label_NewPassword_Weak.BackColor = Color.Transparent;
            this.label_NewPassword_Weak.ForeColor = Color.DarkGray;
            this.label_NewPassword_Normal.BackColor = Color.Transparent;
            this.label_NewPassword_Normal.ForeColor = Color.DarkGray;
            this.label_NewPassword_Secure.BackColor = Color.Transparent;
            this.label_NewPassword_Secure.ForeColor = Color.DarkGray;

            // When judged as a weak password
            if (strength <= InternalApplicationConfig.MaxWeakPasswordStrength)
            {
                this.label_NewPassword_Weak.BackColor = Color.Red;
                this.label_NewPassword_Weak.ForeColor = Color.Black;
            }
            // When judged as a normal password
            else if (InternalApplicationConfig.MaxWeakPasswordStrength < strength && strength <= InternalApplicationConfig.MaxNormalPasswordStrength)
            {
                this.label_NewPassword_Normal.BackColor = Color.Yellow;
                this.label_NewPassword_Normal.ForeColor = Color.Black;
            }
            // When judged as a strong password
            else if (InternalApplicationConfig.MaxNormalPasswordStrength < strength)
            {
                this.label_NewPassword_Secure.BackColor = Color.Green;
                this.label_NewPassword_Secure.ForeColor = Color.Black;
            }
        }
        #endregion

        #endregion

        #region Utility
        /// <summary>
        /// Return character pool which can be used to construct actual password string
        /// </summary>
        /// <returns></returns>
        private char[] GeneratePasswordCharacterPool()
        {
            List<char> pool = new List<char>();

            // Check alphabet characters are indicated to be used
            if (this.checkBox_NewPassword_UseLowerCase.Checked)
            {
                pool.AddRange(Utility.Alphabet);
            }
            if (this.checkBox_NewPassword_UseUpperCase.Checked)
            {
                pool.AddRange(Utility.ALPHABET);
            }

            // Check numerics are indicated to be used
            if (this.checkBox_NewPassword_UseNumerics.Checked)
            {
                pool.AddRange(Utility.Numeric);
            }

            // Check symbol characters are indicated to be used
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

        /// <summary>
        /// Get randomized integer between 0 and specified max value
        /// </summary>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int GetRandomInt(int max, int seed)
        {
            if (max <= 0)
            {
                return 0;
            }

            System.Threading.Thread.Sleep(1 + Math.Abs(seed) % 10);
            Random rand = new Random((int)(DateTime.Now.Ticks & 0x0000FFFF));
            return rand.Next() % (max + 1);
        }

        /// <summary>
        /// Get randomized integer between specified min value and specified max value
        /// </summary>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int GetRandomInt(int min, int max, int seed)
        {
            if (max <= 0)
            {
                return 0;
            }

            if (min < 0)
            {
                min = 0;
            }

            if (min > max)
            {
                return max;
            }

            System.Threading.Thread.Sleep(1 + Math.Abs(seed) % 10);
            Random rand = new Random((int)(DateTime.Now.Ticks & 0x0000FFFF));
            return rand.Next() % (max - min + 1) + min;
        }

        /// <summary>
        /// Calculate password strength value for specified password string.
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static double CalculatePasswordStrength(string password, double adjustedPasswordLength)
        {
            if (String.IsNullOrEmpty(password))
            {
                return 0;
            }

            // Get password class
            PasswordTextClass passClass = FormCreatePassword.GetPasswordClass(password);

            // Get pattern number for the password class
            int nPattern = FormCreatePassword.GetNumberOfPasswordPattern(passClass);

            // Calculate password strength
            double strength = adjustedPasswordLength * Math.Log((double)nPattern, 2);

            return strength;
        }

        /// <summary>
        /// Inspect and return password class for input password text
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static PasswordTextClass GetPasswordClass(string password)
        {
            string lower = "a-z";
            string upper = "A-Z";
            string number = "0-9";
            string symbol = Regex.Escape("#$()^\\|[{+*.? ") + "!\"%&'-=~@`;:\\]},<>/_";
            if ((new Regex(String.Format("^[{0}]+$", lower))).IsMatch(password)) return PasswordTextClass.UseLowercaseOnly;
            if ((new Regex(String.Format("^[{0}]+$", upper))).IsMatch(password)) return PasswordTextClass.UseUppercaseOnly;
            if ((new Regex(String.Format("^[{0}]+$", number))).IsMatch(password)) return PasswordTextClass.UseNumberOnly;
            if ((new Regex(String.Format("^[{0}]+$", symbol))).IsMatch(password)) return PasswordTextClass.UseSymbolOnly;
            if ((new Regex(String.Format("^[{0}{1}]+$", lower, upper))).IsMatch(password)) return PasswordTextClass.UseLowerUpper;
            if ((new Regex(String.Format("^[{0}{1}]+$", lower, number))).IsMatch(password)) return PasswordTextClass.UseLowerNumber;
            if ((new Regex(String.Format("^[{0}{1}]+$", lower, symbol))).IsMatch(password)) return PasswordTextClass.UseLowerSymbol;
            if ((new Regex(String.Format("^[{0}{1}]+$", upper, number))).IsMatch(password)) return PasswordTextClass.UseUpperNumber;
            if ((new Regex(String.Format("^[{0}{1}]+$", upper, symbol))).IsMatch(password)) return PasswordTextClass.UseUpperSymbol;
            if ((new Regex(String.Format("^[{0}{1}]+$", number, symbol))).IsMatch(password)) return PasswordTextClass.UseNumberSymbol;
            if ((new Regex(String.Format("^[{0}{1}{2}]+$", lower, upper, number))).IsMatch(password)) return PasswordTextClass.UseLowerUpperNumber;
            if ((new Regex(String.Format("^[{0}{1}{2}]+$", lower, upper, symbol))).IsMatch(password)) return PasswordTextClass.UseLowerUpperSymbol;
            if ((new Regex(String.Format("^[{0}{1}{2}]+$", lower, number, symbol))).IsMatch(password)) return PasswordTextClass.UseLowerNumberSymbol;
            if ((new Regex(String.Format("^[{0}{1}{2}]+$", upper, number, symbol))).IsMatch(password)) return PasswordTextClass.UseUpperNumberSymbol;
            if ((new Regex(String.Format("^[{0}{1}{2}{3}]+$", lower, upper, number, symbol))).IsMatch(password)) return PasswordTextClass.UseLowerUpperNumberSymbol;

            return PasswordTextClass.Unknown;
        }

        /// <summary>
        /// Get pattern number for specified PasswordTextClass
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        private static int GetNumberOfPasswordPattern(PasswordTextClass c)
        {
            int lowers = Utility.Alphabet.Length;
            int uppers = Utility.ALPHABET.Length;
            int numerics = Utility.Numeric.Length;
            int symbols = Utility.Symbol.Length;

            switch (c)
            {
                case PasswordTextClass.UseLowercaseOnly:
                    return lowers;
                case PasswordTextClass.UseUppercaseOnly:
                    return uppers;
                case PasswordTextClass.UseNumberOnly:
                    return numerics;
                case PasswordTextClass.UseSymbolOnly:
                    return symbols;
                case PasswordTextClass.UseLowerUpper:
                    return lowers + uppers;
                case PasswordTextClass.UseLowerNumber:
                    return lowers + numerics;
                case PasswordTextClass.UseLowerSymbol:
                    return lowers + symbols;
                case PasswordTextClass.UseUpperNumber:
                    return uppers + numerics;
                case PasswordTextClass.UseUpperSymbol:
                    return uppers + symbols;
                case PasswordTextClass.UseNumberSymbol:
                    return numerics + symbols;
                case PasswordTextClass.UseLowerUpperNumber:
                    return lowers + uppers + numerics;
                case PasswordTextClass.UseLowerUpperSymbol:
                    return lowers + uppers + symbols;
                case PasswordTextClass.UseLowerNumberSymbol:
                    return lowers + numerics + symbols;
                case PasswordTextClass.UseUpperNumberSymbol:
                    return uppers + numerics + symbols;
                case PasswordTextClass.UseLowerUpperNumberSymbol:
                    return lowers + uppers + numerics + symbols;
                default:
                    break;
            }

            return 0;
        }

        /// <summary>
        /// Adjust password following some adjustment rules
        /// </summary>
        /// <param name="password"></param>
        /// <returns>Adjusted password character length</returns>
        private double AdjustPasswordStrength(string password)
        {
            this.textBox_NewPassword_strength.Clear();

            if (String.IsNullOrEmpty(password))
            {
                return 0;
            }

            char[] passwordChars = password.ToCharArray();
            double adjusted = password.Length;

            //All characters are different from each other: Length*1.2
            HashSet<char> hash = new HashSet<char>();
            bool isAllDifferent = true;
            foreach (char c in passwordChars)
            {
                if (!hash.Add(c))
                {
                    isAllDifferent = false;
                    break;
                }
            }
            if (isAllDifferent)
            {
                this.textBox_NewPassword_strength.AppendText("All characters are different from each other: Length*1.2" + Environment.NewLine);
                adjusted *= 1.2;
            }

            //Use the same character in a row: Length-0.5/count
            for (int i = 1; i < passwordChars.Length; i++)
            {
                if (passwordChars[i - 1] == password[i])
                {
                    this.textBox_NewPassword_strength.AppendText("Use the same character in a row: Length-0.5/count" + Environment.NewLine);
                    adjusted -= 0.5;
                }
            }

            //Use the same kind of 5 characters in a row: Length-0.2
            if (passwordChars.Length >= 5)
            {
                for (int i = 0; i < passwordChars.Length - 4; i++)
                {
                    PasswordTextClass[] pswdClass = new PasswordTextClass[5]
                    {
                        FormCreatePassword.GetPasswordClass(new String(passwordChars, i+0, 1)),
                        FormCreatePassword.GetPasswordClass(new String(passwordChars, i+1, 1)),
                        FormCreatePassword.GetPasswordClass(new String(passwordChars, i+2, 1)),
                        FormCreatePassword.GetPasswordClass(new String(passwordChars, i+3, 1)),
                        FormCreatePassword.GetPasswordClass(new String(passwordChars, i+4, 1))
                    };

                    bool isTheSameClassInARow = true;
                    for (int j = 1; j < pswdClass.Length; j++)
                    {
                        isTheSameClassInARow = isTheSameClassInARow && (pswdClass[j] == pswdClass[0]);
                        if (!isTheSameClassInARow) break;
                    }
                    if (isTheSameClassInARow)
                    {
                        this.textBox_NewPassword_strength.AppendText("Use the same kind of 5 characters in a row: Length-0.2" + Environment.NewLine);
                        adjusted -= 0.2;
                    }
                }
            }

            //Not Use the same kind of 4 characters in a row: Length*1.15
            if (passwordChars.Length >= 4)
            {
                bool areAllCharFrameNeverUsingTheSameClass = true;
                for (int i = 0; i < passwordChars.Length - 3; i++)
                {
                    PasswordTextClass[] pswdClass = new PasswordTextClass[4]
                    {
                        FormCreatePassword.GetPasswordClass(new String(passwordChars, i+0, 1)),
                        FormCreatePassword.GetPasswordClass(new String(passwordChars, i+1, 1)),
                        FormCreatePassword.GetPasswordClass(new String(passwordChars, i+2, 1)),
                        FormCreatePassword.GetPasswordClass(new String(passwordChars, i+3, 1))
                    };

                    bool isTheSameClassInARow = true;
                    for (int j = 1; j < pswdClass.Length; j++)
                    {
                        isTheSameClassInARow = isTheSameClassInARow && (pswdClass[j] == pswdClass[0]);
                        if (!isTheSameClassInARow) break;
                    }
                    areAllCharFrameNeverUsingTheSameClass = areAllCharFrameNeverUsingTheSameClass && !isTheSameClassInARow;
                }

                if (areAllCharFrameNeverUsingTheSameClass)
                {
                    this.textBox_NewPassword_strength.AppendText("Not Use the same kind of 4 characters in a row: Length*1.15" + Environment.NewLine);
                    adjusted *= 1.15;
                }
            }

            return adjusted;
        }

        /// <summary>
        /// Classification for validating password strength
        /// </summary>
        public enum PasswordTextClass
        {
            Unknown,
            UseLowercaseOnly,
            UseUppercaseOnly,
            UseNumberOnly,
            UseSymbolOnly,
            UseLowerUpper,
            UseLowerNumber,
            UseLowerSymbol,
            UseUpperNumber,
            UseUpperSymbol,
            UseNumberSymbol,
            UseLowerUpperNumber,
            UseLowerUpperSymbol,
            UseLowerNumberSymbol,
            UseUpperNumberSymbol,
            UseLowerUpperNumberSymbol
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
            this.label_NewPassword_Weak.Text = strings.Form_NewPassword_PasswdWeak;
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
