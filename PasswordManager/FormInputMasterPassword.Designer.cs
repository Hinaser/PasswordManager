namespace PasswordManager
{
    partial class FormInputMasterPassword
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
                this.SanitizeInputPassword();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.textBox_InputMasterPassword = new System.Windows.Forms.TextBox();
            this.button_InputMasterPassword_OK = new System.Windows.Forms.Button();
            this.button_InputMasterPassword_Cancel = new System.Windows.Forms.Button();
            this.label_InputMasterPassword = new System.Windows.Forms.Label();
            this.comboBox_InputMasterPassword_Language = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // textBox_InputMasterPassword
            // 
            this.textBox_InputMasterPassword.AcceptsReturn = true;
            this.textBox_InputMasterPassword.Location = new System.Drawing.Point(12, 79);
            this.textBox_InputMasterPassword.Name = "textBox_InputMasterPassword";
            this.textBox_InputMasterPassword.PasswordChar = '*';
            this.textBox_InputMasterPassword.Size = new System.Drawing.Size(283, 19);
            this.textBox_InputMasterPassword.TabIndex = 0;
            // 
            // button_InputMasterPassword_OK
            // 
            this.button_InputMasterPassword_OK.Location = new System.Drawing.Point(12, 113);
            this.button_InputMasterPassword_OK.Name = "button_InputMasterPassword_OK";
            this.button_InputMasterPassword_OK.Size = new System.Drawing.Size(132, 23);
            this.button_InputMasterPassword_OK.TabIndex = 1;
            this.button_InputMasterPassword_OK.Text = "OK";
            this.button_InputMasterPassword_OK.UseVisualStyleBackColor = true;
            // 
            // button_InputMasterPassword_Cancel
            // 
            this.button_InputMasterPassword_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button_InputMasterPassword_Cancel.Location = new System.Drawing.Point(163, 113);
            this.button_InputMasterPassword_Cancel.Name = "button_InputMasterPassword_Cancel";
            this.button_InputMasterPassword_Cancel.Size = new System.Drawing.Size(132, 23);
            this.button_InputMasterPassword_Cancel.TabIndex = 2;
            this.button_InputMasterPassword_Cancel.Text = "Cancel";
            this.button_InputMasterPassword_Cancel.UseVisualStyleBackColor = true;
            // 
            // label_InputMasterPassword
            // 
            this.label_InputMasterPassword.AutoSize = true;
            this.label_InputMasterPassword.Location = new System.Drawing.Point(10, 50);
            this.label_InputMasterPassword.Name = "label_InputMasterPassword";
            this.label_InputMasterPassword.Size = new System.Drawing.Size(123, 12);
            this.label_InputMasterPassword.TabIndex = 3;
            this.label_InputMasterPassword.Text = "Enter master password";
            // 
            // comboBox_InputMasterPassword_Language
            // 
            this.comboBox_InputMasterPassword_Language.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_InputMasterPassword_Language.FormattingEnabled = true;
            this.comboBox_InputMasterPassword_Language.Location = new System.Drawing.Point(12, 12);
            this.comboBox_InputMasterPassword_Language.Name = "comboBox_InputMasterPassword_Language";
            this.comboBox_InputMasterPassword_Language.Size = new System.Drawing.Size(132, 20);
            this.comboBox_InputMasterPassword_Language.TabIndex = 4;
            // 
            // FormInputMasterPassword
            // 
            this.AcceptButton = this.button_InputMasterPassword_OK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(307, 148);
            this.Controls.Add(this.comboBox_InputMasterPassword_Language);
            this.Controls.Add(this.label_InputMasterPassword);
            this.Controls.Add(this.button_InputMasterPassword_Cancel);
            this.Controls.Add(this.button_InputMasterPassword_OK);
            this.Controls.Add(this.textBox_InputMasterPassword);
            this.MaximumSize = new System.Drawing.Size(323, 187);
            this.MinimumSize = new System.Drawing.Size(323, 187);
            this.Name = "FormInputMasterPassword";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Master Password";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox_InputMasterPassword;
        private System.Windows.Forms.Button button_InputMasterPassword_OK;
        private System.Windows.Forms.Button button_InputMasterPassword_Cancel;
        private System.Windows.Forms.Label label_InputMasterPassword;
        private System.Windows.Forms.ComboBox comboBox_InputMasterPassword_Language;
    }
}