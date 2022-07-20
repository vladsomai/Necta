using System.Windows.Forms;

namespace Necta
{
    partial class Necta
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

            Program.logServiceThread.Abort();
            Program.service.Abort();
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Necta));
            this.Print = new System.Windows.Forms.Button();
            this.ApiGetUri_lable = new System.Windows.Forms.Label();
            this.RequestInterval_label = new System.Windows.Forms.Label();
            this.RequestIntervalTime_label = new System.Windows.Forms.Label();
            this.ApiRequestInterval_value = new System.Windows.Forms.NumericUpDown();
            this.ApiUpdateUri_label = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.SaveButton = new MaterialSkin.Controls.MaterialFlatButton();
            this.ApiGetUri_textBox = new MaterialSkin.Controls.MaterialSingleLineTextField();
            this.ApiUpdateUri_textBox = new MaterialSkin.Controls.MaterialSingleLineTextField();
            this.NectaNotifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.ApiPrinterInfoUri_textBox = new MaterialSkin.Controls.MaterialSingleLineTextField();
            this.ApiPrinterInfoUri_label = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.ApiRequestInterval_value)).BeginInit();
            this.SuspendLayout();
            // 
            // Print
            // 
            this.Print.Location = new System.Drawing.Point(1180, 791);
            this.Print.Margin = new System.Windows.Forms.Padding(4);
            this.Print.Name = "Print";
            this.Print.Size = new System.Drawing.Size(124, 32);
            this.Print.TabIndex = 0;
            this.Print.Text = "Print";
            this.Print.UseVisualStyleBackColor = true;
            // 
            // ApiGetUri_lable
            // 
            this.ApiGetUri_lable.AutoSize = true;
            this.ApiGetUri_lable.Location = new System.Drawing.Point(140, 196);
            this.ApiGetUri_lable.Name = "ApiGetUri_lable";
            this.ApiGetUri_lable.Size = new System.Drawing.Size(110, 18);
            this.ApiGetUri_lable.TabIndex = 3;
            this.ApiGetUri_lable.Text = "API GET URI";
            // 
            // RequestInterval_label
            // 
            this.RequestInterval_label.AutoSize = true;
            this.RequestInterval_label.Location = new System.Drawing.Point(735, 196);
            this.RequestInterval_label.Name = "RequestInterval_label";
            this.RequestInterval_label.Size = new System.Drawing.Size(143, 18);
            this.RequestInterval_label.TabIndex = 4;
            this.RequestInterval_label.Text = "Request interval";
            // 
            // RequestIntervalTime_label
            // 
            this.RequestIntervalTime_label.AutoSize = true;
            this.RequestIntervalTime_label.Location = new System.Drawing.Point(878, 223);
            this.RequestIntervalTime_label.Name = "RequestIntervalTime_label";
            this.RequestIntervalTime_label.Size = new System.Drawing.Size(32, 18);
            this.RequestIntervalTime_label.TabIndex = 5;
            this.RequestIntervalTime_label.Text = "ms";
            // 
            // ApiRequestInterval_value
            // 
            this.ApiRequestInterval_value.Increment = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.ApiRequestInterval_value.Location = new System.Drawing.Point(738, 218);
            this.ApiRequestInterval_value.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.ApiRequestInterval_value.Name = "ApiRequestInterval_value";
            this.ApiRequestInterval_value.Size = new System.Drawing.Size(133, 27);
            this.ApiRequestInterval_value.TabIndex = 6;
            this.ApiRequestInterval_value.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.ApiRequestInterval_value.UpDownAlign = System.Windows.Forms.LeftRightAlignment.Left;
            this.ApiRequestInterval_value.Value = new decimal(new int[] {
            3000,
            0,
            0,
            0});
            // 
            // ApiUpdateUri_label
            // 
            this.ApiUpdateUri_label.AutoSize = true;
            this.ApiUpdateUri_label.Location = new System.Drawing.Point(140, 271);
            this.ApiUpdateUri_label.Name = "ApiUpdateUri_label";
            this.ApiUpdateUri_label.Size = new System.Drawing.Size(142, 18);
            this.ApiUpdateUri_label.TabIndex = 8;
            this.ApiUpdateUri_label.Text = "API UPDATE URI";
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Pacifico", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(457, 64);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(153, 93);
            this.label1.TabIndex = 10;
            this.label1.Text = "meals";
            // 
            // SaveButton
            // 
            this.SaveButton.AutoSize = true;
            this.SaveButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.SaveButton.Depth = 0;
            this.SaveButton.Font = new System.Drawing.Font("Verdana", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SaveButton.Location = new System.Drawing.Point(511, 424);
            this.SaveButton.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.SaveButton.MouseState = MaterialSkin.MouseState.HOVER;
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Primary = false;
            this.SaveButton.Size = new System.Drawing.Size(46, 36);
            this.SaveButton.TabIndex = 11;
            this.SaveButton.Text = "Save";
            this.SaveButton.UseVisualStyleBackColor = true;
            this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // ApiGetUri_textBox
            // 
            this.ApiGetUri_textBox.Depth = 0;
            this.ApiGetUri_textBox.Hint = "";
            this.ApiGetUri_textBox.Location = new System.Drawing.Point(143, 221);
            this.ApiGetUri_textBox.MouseState = MaterialSkin.MouseState.HOVER;
            this.ApiGetUri_textBox.Name = "ApiGetUri_textBox";
            this.ApiGetUri_textBox.PasswordChar = '\0';
            this.ApiGetUri_textBox.SelectedText = "";
            this.ApiGetUri_textBox.SelectionLength = 0;
            this.ApiGetUri_textBox.SelectionStart = 0;
            this.ApiGetUri_textBox.Size = new System.Drawing.Size(538, 23);
            this.ApiGetUri_textBox.TabIndex = 12;
            this.ApiGetUri_textBox.UseSystemPasswordChar = false;
            // 
            // ApiUpdateUri_textBox
            // 
            this.ApiUpdateUri_textBox.Depth = 0;
            this.ApiUpdateUri_textBox.Hint = "";
            this.ApiUpdateUri_textBox.Location = new System.Drawing.Point(143, 292);
            this.ApiUpdateUri_textBox.MouseState = MaterialSkin.MouseState.HOVER;
            this.ApiUpdateUri_textBox.Name = "ApiUpdateUri_textBox";
            this.ApiUpdateUri_textBox.PasswordChar = '\0';
            this.ApiUpdateUri_textBox.SelectedText = "";
            this.ApiUpdateUri_textBox.SelectionLength = 0;
            this.ApiUpdateUri_textBox.SelectionStart = 0;
            this.ApiUpdateUri_textBox.Size = new System.Drawing.Size(538, 23);
            this.ApiUpdateUri_textBox.TabIndex = 13;
            this.ApiUpdateUri_textBox.UseSystemPasswordChar = false;
            // 
            // NectaNotifyIcon1
            // 
            this.NectaNotifyIcon1.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.NectaNotifyIcon1.BalloonTipText = "Necta";
            this.NectaNotifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("NectaNotifyIcon1.Icon")));
            this.NectaNotifyIcon1.Text = "Necta";
            this.NectaNotifyIcon1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.NectaNotifyIcon1_MouseDoubleClick);
            // 
            // ApiPrinterInfoUri_textBox
            // 
            this.ApiPrinterInfoUri_textBox.Depth = 0;
            this.ApiPrinterInfoUri_textBox.Hint = "";
            this.ApiPrinterInfoUri_textBox.Location = new System.Drawing.Point(143, 364);
            this.ApiPrinterInfoUri_textBox.MouseState = MaterialSkin.MouseState.HOVER;
            this.ApiPrinterInfoUri_textBox.Name = "ApiPrinterInfoUri_textBox";
            this.ApiPrinterInfoUri_textBox.PasswordChar = '\0';
            this.ApiPrinterInfoUri_textBox.SelectedText = "";
            this.ApiPrinterInfoUri_textBox.SelectionLength = 0;
            this.ApiPrinterInfoUri_textBox.SelectionStart = 0;
            this.ApiPrinterInfoUri_textBox.Size = new System.Drawing.Size(538, 23);
            this.ApiPrinterInfoUri_textBox.TabIndex = 15;
            this.ApiPrinterInfoUri_textBox.UseSystemPasswordChar = false;
            // 
            // ApiPrinterInfoUri_label
            // 
            this.ApiPrinterInfoUri_label.AutoSize = true;
            this.ApiPrinterInfoUri_label.Location = new System.Drawing.Point(140, 343);
            this.ApiPrinterInfoUri_label.Name = "ApiPrinterInfoUri_label";
            this.ApiPrinterInfoUri_label.Size = new System.Drawing.Size(196, 18);
            this.ApiPrinterInfoUri_label.TabIndex = 14;
            this.ApiPrinterInfoUri_label.Text = "API PRINTER INFO URI";
            // 
            // Necta
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1029, 514);
            this.Controls.Add(this.ApiPrinterInfoUri_textBox);
            this.Controls.Add(this.ApiPrinterInfoUri_label);
            this.Controls.Add(this.ApiUpdateUri_textBox);
            this.Controls.Add(this.ApiGetUri_textBox);
            this.Controls.Add(this.SaveButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ApiUpdateUri_label);
            this.Controls.Add(this.ApiRequestInterval_value);
            this.Controls.Add(this.RequestIntervalTime_label);
            this.Controls.Add(this.RequestInterval_label);
            this.Controls.Add(this.ApiGetUri_lable);
            this.Controls.Add(this.Print);
            this.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.Name = "Necta";
            this.Sizable = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Resize += new System.EventHandler(this.onResize);
            ((System.ComponentModel.ISupportInitialize)(this.ApiRequestInterval_value)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Print;
        private Label ApiGetUri_lable;
        private Label RequestInterval_label;
        private Label RequestIntervalTime_label;
        private NumericUpDown ApiRequestInterval_value;
        private Label ApiUpdateUri_label;
        private Label label1;
        private MaterialSkin.Controls.MaterialFlatButton SaveButton;
        private MaterialSkin.Controls.MaterialSingleLineTextField ApiGetUri_textBox;
        private MaterialSkin.Controls.MaterialSingleLineTextField ApiUpdateUri_textBox;
        private NotifyIcon NectaNotifyIcon1;
        private MaterialSkin.Controls.MaterialSingleLineTextField ApiPrinterInfoUri_textBox;
        private Label ApiPrinterInfoUri_label;
    }
}

