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
            this.SuspendLayout();
            // 
            // textBox_InputMasterPassword
            // 
            this.textBox_InputMasterPassword.Location = new System.Drawing.Point(12, 39);
            this.textBox_InputMasterPassword.Name = "textBox_InputMasterPassword";
            this.textBox_InputMasterPassword.Size = new System.Drawing.Size(157, 19);
            this.textBox_InputMasterPassword.TabIndex = 0;
            // 
            // button_InputMasterPassword_OK
            // 
            this.button_InputMasterPassword_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button_InputMasterPassword_OK.Location = new System.Drawing.Point(12, 73);
            this.button_InputMasterPassword_OK.Name = "button_InputMasterPassword_OK";
            this.button_InputMasterPassword_OK.Size = new System.Drawing.Size(75, 23);
            this.button_InputMasterPassword_OK.TabIndex = 1;
            this.button_InputMasterPassword_OK.Text = "OK";
            this.button_InputMasterPassword_OK.UseVisualStyleBackColor = true;
            // 
            // button_InputMasterPassword_Cancel
            // 
            this.button_InputMasterPassword_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button_InputMasterPassword_Cancel.Location = new System.Drawing.Point(94, 73);
            this.button_InputMasterPassword_Cancel.Name = "button_InputMasterPassword_Cancel";
            this.button_InputMasterPassword_Cancel.Size = new System.Drawing.Size(75, 23);
            this.button_InputMasterPassword_Cancel.TabIndex = 2;
            this.button_InputMasterPassword_Cancel.Text = "Cancel";
            this.button_InputMasterPassword_Cancel.UseVisualStyleBackColor = true;
            // 
            // label_InputMasterPassword
            // 
            this.label_InputMasterPassword.AutoSize = true;
            this.label_InputMasterPassword.Location = new System.Drawing.Point(12, 13);
            this.label_InputMasterPassword.Name = "label_InputMasterPassword";
            this.label_InputMasterPassword.Size = new System.Drawing.Size(123, 12);
            this.label_InputMasterPassword.TabIndex = 3;
            this.label_InputMasterPassword.Text = "Enter master password";
            // 
            // FormInputMasterPassword
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(182, 108);
            this.Controls.Add(this.label_InputMasterPassword);
            this.Controls.Add(this.button_InputMasterPassword_Cancel);
            this.Controls.Add(this.button_InputMasterPassword_OK);
            this.Controls.Add(this.textBox_InputMasterPassword);
            this.Name = "FormInputMasterPassword";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox_InputMasterPassword;
        private System.Windows.Forms.Button button_InputMasterPassword_OK;
        private System.Windows.Forms.Button button_InputMasterPassword_Cancel;
        private System.Windows.Forms.Label label_InputMasterPassword;
    }
}