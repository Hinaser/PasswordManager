namespace PasswordManager
{
    partial class FormChangeMasterPassword
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
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
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
            this.textBox_ChangeMasterPassword_1 = new System.Windows.Forms.TextBox();
            this.textBox_ChangeMasterPassword_2 = new System.Windows.Forms.TextBox();
            this.button_ChangeMasterPassword_OK = new System.Windows.Forms.Button();
            this.button_ChangeMasterPassword_Cancel = new System.Windows.Forms.Button();
            this.label_ChangeMasterPassword = new System.Windows.Forms.Label();
            this.checkBox_ChangeMasterPassword = new System.Windows.Forms.CheckBox();
            this.groupBox_NewPassword_Test = new System.Windows.Forms.GroupBox();
            this.panel_NewPassword_Strength = new System.Windows.Forms.Panel();
            this.richTextBox_NewPassword_Strength = new System.Windows.Forms.RichTextBox();
            this.label_NewPassword_Secure = new System.Windows.Forms.Label();
            this.label_NewPassword_Normal = new System.Windows.Forms.Label();
            this.label_NewPassword_Weak = new System.Windows.Forms.Label();
            this.label_ChangeMasterPassword_RepeatPassword = new System.Windows.Forms.Label();
            this.groupBox_NewPassword_Test.SuspendLayout();
            this.panel_NewPassword_Strength.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBox_ChangeMasterPassword_1
            // 
            this.textBox_ChangeMasterPassword_1.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.textBox_ChangeMasterPassword_1.Location = new System.Drawing.Point(12, 24);
            this.textBox_ChangeMasterPassword_1.Name = "textBox_ChangeMasterPassword_1";
            this.textBox_ChangeMasterPassword_1.Size = new System.Drawing.Size(270, 19);
            this.textBox_ChangeMasterPassword_1.TabIndex = 0;
            this.textBox_ChangeMasterPassword_1.UseSystemPasswordChar = true;
            // 
            // textBox_ChangeMasterPassword_2
            // 
            this.textBox_ChangeMasterPassword_2.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.textBox_ChangeMasterPassword_2.Location = new System.Drawing.Point(12, 61);
            this.textBox_ChangeMasterPassword_2.Name = "textBox_ChangeMasterPassword_2";
            this.textBox_ChangeMasterPassword_2.Size = new System.Drawing.Size(270, 19);
            this.textBox_ChangeMasterPassword_2.TabIndex = 1;
            this.textBox_ChangeMasterPassword_2.UseSystemPasswordChar = true;
            // 
            // button_ChangeMasterPassword_OK
            // 
            this.button_ChangeMasterPassword_OK.Location = new System.Drawing.Point(12, 275);
            this.button_ChangeMasterPassword_OK.Name = "button_ChangeMasterPassword_OK";
            this.button_ChangeMasterPassword_OK.Size = new System.Drawing.Size(75, 23);
            this.button_ChangeMasterPassword_OK.TabIndex = 2;
            this.button_ChangeMasterPassword_OK.Text = "OK";
            this.button_ChangeMasterPassword_OK.UseVisualStyleBackColor = true;
            // 
            // button_ChangeMasterPassword_Cancel
            // 
            this.button_ChangeMasterPassword_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button_ChangeMasterPassword_Cancel.Location = new System.Drawing.Point(207, 275);
            this.button_ChangeMasterPassword_Cancel.Name = "button_ChangeMasterPassword_Cancel";
            this.button_ChangeMasterPassword_Cancel.Size = new System.Drawing.Size(75, 23);
            this.button_ChangeMasterPassword_Cancel.TabIndex = 3;
            this.button_ChangeMasterPassword_Cancel.Text = "Cancel";
            this.button_ChangeMasterPassword_Cancel.UseVisualStyleBackColor = true;
            // 
            // label_ChangeMasterPassword
            // 
            this.label_ChangeMasterPassword.AutoSize = true;
            this.label_ChangeMasterPassword.Location = new System.Drawing.Point(10, 9);
            this.label_ChangeMasterPassword.Name = "label_ChangeMasterPassword";
            this.label_ChangeMasterPassword.Size = new System.Drawing.Size(147, 12);
            this.label_ChangeMasterPassword.TabIndex = 4;
            this.label_ChangeMasterPassword.Text = "Enter new master password";
            // 
            // checkBox_ChangeMasterPassword
            // 
            this.checkBox_ChangeMasterPassword.AutoSize = true;
            this.checkBox_ChangeMasterPassword.Location = new System.Drawing.Point(12, 86);
            this.checkBox_ChangeMasterPassword.Name = "checkBox_ChangeMasterPassword";
            this.checkBox_ChangeMasterPassword.Size = new System.Drawing.Size(103, 16);
            this.checkBox_ChangeMasterPassword.TabIndex = 5;
            this.checkBox_ChangeMasterPassword.Text = "Show password";
            this.checkBox_ChangeMasterPassword.UseVisualStyleBackColor = true;
            // 
            // groupBox_NewPassword_Test
            // 
            this.groupBox_NewPassword_Test.Controls.Add(this.panel_NewPassword_Strength);
            this.groupBox_NewPassword_Test.Controls.Add(this.label_NewPassword_Secure);
            this.groupBox_NewPassword_Test.Controls.Add(this.label_NewPassword_Normal);
            this.groupBox_NewPassword_Test.Controls.Add(this.label_NewPassword_Weak);
            this.groupBox_NewPassword_Test.Location = new System.Drawing.Point(12, 108);
            this.groupBox_NewPassword_Test.Name = "groupBox_NewPassword_Test";
            this.groupBox_NewPassword_Test.Size = new System.Drawing.Size(270, 161);
            this.groupBox_NewPassword_Test.TabIndex = 8;
            this.groupBox_NewPassword_Test.TabStop = false;
            this.groupBox_NewPassword_Test.Text = "Password strength test";
            // 
            // panel_NewPassword_Strength
            // 
            this.panel_NewPassword_Strength.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel_NewPassword_Strength.Controls.Add(this.richTextBox_NewPassword_Strength);
            this.panel_NewPassword_Strength.Location = new System.Drawing.Point(14, 40);
            this.panel_NewPassword_Strength.Name = "panel_NewPassword_Strength";
            this.panel_NewPassword_Strength.Padding = new System.Windows.Forms.Padding(5);
            this.panel_NewPassword_Strength.Size = new System.Drawing.Size(241, 115);
            this.panel_NewPassword_Strength.TabIndex = 3;
            // 
            // richTextBox_NewPassword_Strength
            // 
            this.richTextBox_NewPassword_Strength.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox_NewPassword_Strength.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox_NewPassword_Strength.Location = new System.Drawing.Point(5, 5);
            this.richTextBox_NewPassword_Strength.Name = "richTextBox_NewPassword_Strength";
            this.richTextBox_NewPassword_Strength.ReadOnly = true;
            this.richTextBox_NewPassword_Strength.ShortcutsEnabled = false;
            this.richTextBox_NewPassword_Strength.Size = new System.Drawing.Size(229, 103);
            this.richTextBox_NewPassword_Strength.TabIndex = 3;
            this.richTextBox_NewPassword_Strength.TabStop = false;
            this.richTextBox_NewPassword_Strength.Text = "";
            this.richTextBox_NewPassword_Strength.WordWrap = false;
            // 
            // label_NewPassword_Secure
            // 
            this.label_NewPassword_Secure.BackColor = System.Drawing.Color.Transparent;
            this.label_NewPassword_Secure.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label_NewPassword_Secure.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_NewPassword_Secure.ForeColor = System.Drawing.Color.DarkGray;
            this.label_NewPassword_Secure.Location = new System.Drawing.Point(174, 21);
            this.label_NewPassword_Secure.Name = "label_NewPassword_Secure";
            this.label_NewPassword_Secure.Size = new System.Drawing.Size(81, 20);
            this.label_NewPassword_Secure.TabIndex = 2;
            this.label_NewPassword_Secure.Text = "Secure";
            this.label_NewPassword_Secure.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label_NewPassword_Normal
            // 
            this.label_NewPassword_Normal.BackColor = System.Drawing.Color.Transparent;
            this.label_NewPassword_Normal.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label_NewPassword_Normal.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_NewPassword_Normal.ForeColor = System.Drawing.Color.DarkGray;
            this.label_NewPassword_Normal.Location = new System.Drawing.Point(94, 21);
            this.label_NewPassword_Normal.Name = "label_NewPassword_Normal";
            this.label_NewPassword_Normal.Size = new System.Drawing.Size(81, 20);
            this.label_NewPassword_Normal.TabIndex = 1;
            this.label_NewPassword_Normal.Text = "Normal";
            this.label_NewPassword_Normal.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label_NewPassword_Weak
            // 
            this.label_NewPassword_Weak.BackColor = System.Drawing.Color.Transparent;
            this.label_NewPassword_Weak.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label_NewPassword_Weak.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_NewPassword_Weak.ForeColor = System.Drawing.Color.DarkGray;
            this.label_NewPassword_Weak.Location = new System.Drawing.Point(14, 21);
            this.label_NewPassword_Weak.Name = "label_NewPassword_Weak";
            this.label_NewPassword_Weak.Size = new System.Drawing.Size(81, 20);
            this.label_NewPassword_Weak.TabIndex = 0;
            this.label_NewPassword_Weak.Text = "Weak";
            this.label_NewPassword_Weak.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label_ChangeMasterPassword_RepeatPassword
            // 
            this.label_ChangeMasterPassword_RepeatPassword.AutoSize = true;
            this.label_ChangeMasterPassword_RepeatPassword.Location = new System.Drawing.Point(10, 46);
            this.label_ChangeMasterPassword_RepeatPassword.Name = "label_ChangeMasterPassword_RepeatPassword";
            this.label_ChangeMasterPassword_RepeatPassword.Size = new System.Drawing.Size(170, 12);
            this.label_ChangeMasterPassword_RepeatPassword.TabIndex = 9;
            this.label_ChangeMasterPassword_RepeatPassword.Text = "Repeat the same password here";
            // 
            // FormChangeMasterPassword
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(298, 307);
            this.Controls.Add(this.label_ChangeMasterPassword_RepeatPassword);
            this.Controls.Add(this.groupBox_NewPassword_Test);
            this.Controls.Add(this.checkBox_ChangeMasterPassword);
            this.Controls.Add(this.label_ChangeMasterPassword);
            this.Controls.Add(this.button_ChangeMasterPassword_Cancel);
            this.Controls.Add(this.button_ChangeMasterPassword_OK);
            this.Controls.Add(this.textBox_ChangeMasterPassword_2);
            this.Controls.Add(this.textBox_ChangeMasterPassword_1);
            this.Name = "FormChangeMasterPassword";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "FormChangeMasterPassword";
            this.groupBox_NewPassword_Test.ResumeLayout(false);
            this.panel_NewPassword_Strength.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox_ChangeMasterPassword_1;
        private System.Windows.Forms.TextBox textBox_ChangeMasterPassword_2;
        private System.Windows.Forms.Button button_ChangeMasterPassword_OK;
        private System.Windows.Forms.Button button_ChangeMasterPassword_Cancel;
        private System.Windows.Forms.Label label_ChangeMasterPassword;
        private System.Windows.Forms.CheckBox checkBox_ChangeMasterPassword;
        private System.Windows.Forms.GroupBox groupBox_NewPassword_Test;
        private System.Windows.Forms.Panel panel_NewPassword_Strength;
        private System.Windows.Forms.RichTextBox richTextBox_NewPassword_Strength;
        private System.Windows.Forms.Label label_NewPassword_Secure;
        private System.Windows.Forms.Label label_NewPassword_Normal;
        private System.Windows.Forms.Label label_NewPassword_Weak;
        private System.Windows.Forms.Label label_ChangeMasterPassword_RepeatPassword;
    }
}