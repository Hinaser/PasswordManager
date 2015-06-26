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
            this.textBox_NewPassword_Caption.MaxLength = LocalConfig.CaptionMaxLength;
            this.textBox_NewPassword_ID.MaxLength = LocalConfig.IDMaxLength;
            this.numericUpDown_NewPassword_Minchars.Minimum = LocalConfig.PasswordMinLength;
            this.numericUpDown_NewPassword_Minchars.Maximum = LocalConfig.PasswordMaxLength;
            this.numericUpDown_NewPassword_Maxchars.Minimum = LocalConfig.PasswordMinLength;
            this.numericUpDown_NewPassword_Maxchars.Maximum = LocalConfig.PasswordMaxLength;
            this.textBox_NewPassword_Password.MaxLength = LocalConfig.PasswordMaxLength.GetHashCode();
            this.textBox_NewPassword_Memo.MaxLength = LocalConfig.DescriptionMaxLength;
            this.SetupLanguage();
        }
        #endregion

        #region Setter method
        // Set caption/id/password/description from outside caller
        public void SetCaption(string caption) { this.textBox_NewPassword_Caption.Text = caption; }
        public void SetID(string id) { this.textBox_NewPassword_ID.Text = id; }
        public void SetPassword(string password) { this.textBox_NewPassword_Password.Text = password; }
        public void SetDescription(string description) { this.textBox_NewPassword_Memo.Text = description; }

        /// <summary>
        /// Set password data for update
        /// </summary>
        /// <param name="record"></param>
        public void SetPasswordData(PasswordRecord record)
        {
            this.Password = record;
            this.SetCaption(record.GetCaption());
            this.SetID(record.GetID());
            this.SetPassword(record.GetPassword());
            this.SetDescription(record.GetDescription());
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
            // If password is being newly created
            if (this.Password == null)
            {
                this.Password = new PasswordRecord();
                this.Password.SetHeaderData(this.textBox_NewPassword_Caption.Text, DateTime.UtcNow, DateTime.UtcNow);
            }
            // If password is being updated
            else
            {
                this.Password.SetHeaderData(this.textBox_NewPassword_Caption.Text, this.Password.GetCreateDate(), DateTime.UtcNow);
            }
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
            if (this.numericUpDown_NewPassword_Minchars.Value < LocalConfig.PasswordMinLength
                || this.numericUpDown_NewPassword_Minchars.Value < Int32.MinValue)
            {
                MessageBox.Show(strings.Form_NewPassword_ErrorMinTooSmall, strings.Form_NewPassword_ErrorDialogCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.button_NewPassword_GeneratePassword.Enabled = true;
                this.button_NewPassword_GeneratePassword.Focus();
                return;
            }
            if (this.numericUpDown_NewPassword_Maxchars.Value > LocalConfig.PasswordMaxLength
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

            // Reset label color
            this.label_NewPassword_Weak.BackColor = Color.Transparent;
            this.label_NewPassword_Weak.ForeColor = Color.DarkGray;
            this.label_NewPassword_Normal.BackColor = Color.Transparent;
            this.label_NewPassword_Normal.ForeColor = Color.DarkGray;
            this.label_NewPassword_Secure.BackColor = Color.Transparent;
            this.label_NewPassword_Secure.ForeColor = Color.DarkGray;

            // Reset strength notification textarea
            this.richTextBox_NewPassword_Strength.Clear();

            // Do nothing if textbox is empty
            if (String.IsNullOrEmpty(t.Text))
            {
                return;
            }

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
            string symbol = @"\W|_";
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

            // If non-ASCII character exists
            if ((new Regex(@"[\u10000-\u10FFFF]")).IsMatch(password)) return PasswordTextClass.Use4bytesUTF8Char; // 4 bytes characters in UTF8
            if ((new Regex(@"[\u0800-\uFFFF]")).IsMatch(password)) return PasswordTextClass.Use3bytesUTF8Char; // 3 bytes characters in UTF8
            if ((new Regex(@"[\u0080-\u07FF]")).IsMatch(password)) return PasswordTextClass.Use2bytesUTF8Char; // 2 bytes characters in UTF8

            return PasswordTextClass.Unknown;
        }

        /// <summary>
        /// Convert class enum to its text
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static string GetPasswordClassText(PasswordTextClass c)
        {
            switch (c)
            {
                case PasswordTextClass.UseLowercaseOnly:
                    return strings.General_NewPassword_Strength_L;
                case PasswordTextClass.UseUppercaseOnly:
                    return strings.General_NewPassword_Strength_U;
                case PasswordTextClass.UseNumberOnly:
                    return strings.General_NewPassword_Strength_N;
                case PasswordTextClass.UseSymbolOnly:
                    return strings.General_NewPassword_Strength_S;
                case PasswordTextClass.UseLowerUpper:
                    return strings.General_NewPassword_Strength_LU;
                case PasswordTextClass.UseLowerNumber:
                    return strings.General_NewPassword_Strength_LN;
                case PasswordTextClass.UseLowerSymbol:
                    return strings.General_NewPassword_Strength_LS;
                case PasswordTextClass.UseUpperNumber:
                    return strings.General_NewPassword_Strength_UN;
                case PasswordTextClass.UseUpperSymbol:
                    return strings.General_NewPassword_Strength_US;
                case PasswordTextClass.UseNumberSymbol:
                    return strings.General_NewPassword_Strength_NS;
                case PasswordTextClass.UseLowerUpperNumber:
                    return strings.General_NewPassword_Strength_LUN;
                case PasswordTextClass.UseLowerUpperSymbol:
                    return strings.General_NewPassword_Strength_LUS;
                case PasswordTextClass.UseLowerNumberSymbol:
                    return strings.General_NewPassword_Strength_LNS;
                case PasswordTextClass.UseUpperNumberSymbol:
                    return strings.General_NewPassword_Strength_UNS;
                case PasswordTextClass.UseLowerUpperNumberSymbol:
                    return strings.General_NewPassword_Strength_LUNS;
                case PasswordTextClass.Use4bytesUTF8Char:
                    return strings.General_NewPassword_Strength_UTF8_4Bytes;
                case PasswordTextClass.Use3bytesUTF8Char:
                    return strings.General_NewPassword_Strength_UTF8_3Bytes;
                case PasswordTextClass.Use2bytesUTF8Char:
                    return strings.General_NewPassword_Strength_UTF8_2Bytes;
                default:
                    break;
            }

            return String.Empty;
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
            int utf8_2bytes = 1920; // \u0080-\u07FF
            int utf8_3bytes = 63488; // \u0800-\uFFFF
            int utf8_4bytes = 1048576; // \u10000-\u10FFFF

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
                case PasswordTextClass.Use4bytesUTF8Char:
                    return utf8_4bytes;
                case PasswordTextClass.Use3bytesUTF8Char:
                    return utf8_3bytes;
                case PasswordTextClass.Use2bytesUTF8Char:
                    return utf8_2bytes;
                default:
                    break;
            }

            return 0;
        }

        /// <summary>
        /// Add password strength check record to text area with color
        /// </summary>
        /// <param name="header"></param>
        /// <param name="text"></param>
        public static void AppendStrengthReport(RichTextBox rich, string header, string text, Color headerColor)
        {
            // Save current text length
            int selectionStart = rich.TextLength;
            rich.SelectionLength = 0;

            // Add text to form
            rich.AppendText(
                String.Format(
                LocalConfig.PasswordStrengthNoticeFormat,
                header,
                text));

            // Paint color
            rich.SelectionStart = selectionStart;
            rich.SelectionLength = LocalConfig.PasswordStrengthNoticeHeaderSize;
            rich.SelectionColor = headerColor;

            // Deselect in order not to remain color effect
            rich.DeselectAll();
        }

        /// <summary>
        /// Sanitize ID/Password text data from form instance.
        /// </summary>
        public unsafe void SanitizePasswordInfo()
        {
            string caption = this.textBox_NewPassword_Caption.Text;
            string id = this.textBox_NewPassword_ID.Text;
            string password = this.textBox_NewPassword_Password.Text;

            if (!String.IsNullOrEmpty(caption))
            {
                fixed (char* p = caption)
                {
                    for (int i = 0; i < caption.Length; i++)
                    {
                        p[i] = '\0';
                    }
                }
            }
            this.textBox_NewPassword_Caption.Text = String.Empty;

            if (!String.IsNullOrEmpty(id))
            {
                fixed (char* p = id)
                {
                    for (int i = 0; i < id.Length; i++)
                    {
                        p[i] = '\0';
                    }
                }
            }
            this.textBox_NewPassword_ID.Text = String.Empty;

            if (!String.IsNullOrEmpty(password))
            {
                fixed (char* p = password)
                {
                    for (int i = 0; i < password.Length; i++)
                    {
                        p[i] = '\0';
                    }
                }
            }
            this.textBox_NewPassword_Password.Text = String.Empty;
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
