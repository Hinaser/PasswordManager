namespace PasswordManager
{
    partial class FormMovePassword
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
            this.panel_MovePassword_TreeView = new System.Windows.Forms.Panel();
            this.panel_MovePassword_Button = new System.Windows.Forms.Panel();
            this.button_MovePassword_OK = new System.Windows.Forms.Button();
            this.button_MovePassword_Cancel = new System.Windows.Forms.Button();
            this.treeView_MovePassword_Folders = new System.Windows.Forms.TreeView();
            this.panel_MovePassword_TreeView.SuspendLayout();
            this.panel_MovePassword_Button.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel_MovePassword_TreeView
            // 
            this.panel_MovePassword_TreeView.Controls.Add(this.treeView_MovePassword_Folders);
            this.panel_MovePassword_TreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_MovePassword_TreeView.Location = new System.Drawing.Point(0, 0);
            this.panel_MovePassword_TreeView.Name = "panel_MovePassword_TreeView";
            this.panel_MovePassword_TreeView.Size = new System.Drawing.Size(246, 257);
            this.panel_MovePassword_TreeView.TabIndex = 0;
            // 
            // panel_MovePassword_Button
            // 
            this.panel_MovePassword_Button.Controls.Add(this.button_MovePassword_Cancel);
            this.panel_MovePassword_Button.Controls.Add(this.button_MovePassword_OK);
            this.panel_MovePassword_Button.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel_MovePassword_Button.Location = new System.Drawing.Point(0, 257);
            this.panel_MovePassword_Button.Name = "panel_MovePassword_Button";
            this.panel_MovePassword_Button.Size = new System.Drawing.Size(246, 36);
            this.panel_MovePassword_Button.TabIndex = 1;
            // 
            // button_MovePassword_OK
            // 
            this.button_MovePassword_OK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button_MovePassword_OK.Location = new System.Drawing.Point(12, 6);
            this.button_MovePassword_OK.Name = "button_MovePassword_OK";
            this.button_MovePassword_OK.Size = new System.Drawing.Size(75, 23);
            this.button_MovePassword_OK.TabIndex = 0;
            this.button_MovePassword_OK.Text = "OK";
            this.button_MovePassword_OK.UseVisualStyleBackColor = true;
            // 
            // button_MovePassword_Cancel
            // 
            this.button_MovePassword_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button_MovePassword_Cancel.Location = new System.Drawing.Point(93, 6);
            this.button_MovePassword_Cancel.Name = "button_MovePassword_Cancel";
            this.button_MovePassword_Cancel.Size = new System.Drawing.Size(75, 23);
            this.button_MovePassword_Cancel.TabIndex = 1;
            this.button_MovePassword_Cancel.Text = "Cancel";
            this.button_MovePassword_Cancel.UseVisualStyleBackColor = true;
            // 
            // treeView_MovePassword_Folders
            // 
            this.treeView_MovePassword_Folders.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView_MovePassword_Folders.Location = new System.Drawing.Point(0, 0);
            this.treeView_MovePassword_Folders.Name = "treeView_MovePassword_Folders";
            this.treeView_MovePassword_Folders.Size = new System.Drawing.Size(246, 257);
            this.treeView_MovePassword_Folders.TabIndex = 0;
            // 
            // FormMovePassword
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(246, 293);
            this.Controls.Add(this.panel_MovePassword_TreeView);
            this.Controls.Add(this.panel_MovePassword_Button);
            this.MinimumSize = new System.Drawing.Size(230, 270);
            this.Name = "FormMovePassword";
            this.Text = "Select folder";
            this.panel_MovePassword_TreeView.ResumeLayout(false);
            this.panel_MovePassword_Button.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel_MovePassword_TreeView;
        private System.Windows.Forms.Panel panel_MovePassword_Button;
        private System.Windows.Forms.Button button_MovePassword_Cancel;
        private System.Windows.Forms.Button button_MovePassword_OK;
        private System.Windows.Forms.TreeView treeView_MovePassword_Folders;
    }
}