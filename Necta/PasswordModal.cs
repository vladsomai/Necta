﻿using MaterialSkin;
using MaterialSkin.Controls;
using Necta.NectaServices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Necta
{
    public partial class PasswordModal : MaterialForm
    {
        public static PasswordModal mInstance = null;
        private NectaApp MainInstance = null;

        public static void CreatePasswordModal(NectaApp instance)
        {
            if (mInstance == null)
                mInstance = new PasswordModal(instance);
        }
        private PasswordModal(NectaApp instance)
        {
            MainInstance = instance;

            InitializeComponent();
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.Red300, Primary.Red300, Primary.BlueGrey500, Accent.LightBlue200, TextShade.BLACK);
        }

        private void PasswordButton_Click(object sender, EventArgs e)
        {

            var currentPassword = ConfigContent<PasswordType>.ReadConfig(NectaConfigService.nectaPasswordFile);
            if (currentPassword.Password == Hasher.GetSha256Hash(PasswordTextbox.Text))
            {
                hidePasswordForm();
                MainInstance.SetTextboxesToVisible(true);
                MainInstance.ShowMainFrom();
            }
            else
                MessageBox.Show(this, "Invalid password, please try again.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign);
        }

        private void PasswordForm_Closing(object e, FormClosingEventArgs ev)
        {
            hidePasswordForm();
            MainInstance.ShowNotifyIcon();
        }

        private void hidePasswordForm()
        {
            Hide();
            this.WindowState = FormWindowState.Minimized;
        }

        public void showPasswordFrom()
        {
            MainInstance.HideMainForm();
            PasswordTextbox.Text = "";
            Show();
            this.WindowState = FormWindowState.Normal;
        }
    }
}
