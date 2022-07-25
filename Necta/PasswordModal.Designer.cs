
namespace Necta
{
    partial class PasswordModal
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
       // private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            /*
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
             */
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PasswordModal));
            this.PasswordButton = new MaterialSkin.Controls.MaterialFlatButton();
            this.PasswordTextbox = new MaterialSkin.Controls.MaterialSingleLineTextField();
            this.SuspendLayout();
            // 
            // PasswordButton
            // 
            this.PasswordButton.AutoSize = true;
            this.PasswordButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.PasswordButton.Depth = 0;
            this.PasswordButton.Location = new System.Drawing.Point(118, 119);
            this.PasswordButton.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.PasswordButton.MouseState = MaterialSkin.MouseState.HOVER;
            this.PasswordButton.Name = "PasswordButton";
            this.PasswordButton.Primary = false;
            this.PasswordButton.Size = new System.Drawing.Size(57, 36);
            this.PasswordButton.TabIndex = 0;
            this.PasswordButton.Text = "Verify";
            this.PasswordButton.UseVisualStyleBackColor = true;
            this.PasswordButton.Click += new System.EventHandler(this.PasswordButton_Click);
            // 
            // PasswordTextbox
            // 
            this.PasswordTextbox.Depth = 0;
            this.PasswordTextbox.Hint = "";
            this.PasswordTextbox.Location = new System.Drawing.Point(31, 87);
            this.PasswordTextbox.MouseState = MaterialSkin.MouseState.HOVER;
            this.PasswordTextbox.Name = "PasswordTextbox";
            this.PasswordTextbox.PasswordChar = '\0';
            this.PasswordTextbox.SelectedText = "";
            this.PasswordTextbox.SelectionLength = 0;
            this.PasswordTextbox.SelectionStart = 0;
            this.PasswordTextbox.Size = new System.Drawing.Size(255, 23);
            this.PasswordTextbox.TabIndex = 0;
            this.PasswordTextbox.UseSystemPasswordChar = false;
            this.PasswordTextbox.UseSystemPasswordChar = true;
            // 
            // PasswordModal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(311, 170);
            this.Controls.Add(this.PasswordTextbox);
            this.Controls.Add(this.PasswordButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PasswordModal";
            this.Sizable = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Enter password";
            this.ResumeLayout(false);
            this.PerformLayout();

            this.FormClosing += PasswordForm_Closing;
        }

        #endregion

        private MaterialSkin.Controls.MaterialFlatButton PasswordButton;
        private MaterialSkin.Controls.MaterialSingleLineTextField PasswordTextbox;
    }
}