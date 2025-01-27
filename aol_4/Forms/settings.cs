﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using aol.Classes;

namespace aol.Forms
{
    public partial class settings : Form
    {
        #region DLLImports
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, uint wParam, uint lParam);
        #endregion

        #region win95_theme
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        const int _ = 2;

        new Rectangle Top { get { return new Rectangle(0, 0, this.ClientSize.Width, _); } }
        new Rectangle Left { get { return new Rectangle(0, 0, _, this.ClientSize.Height); } }
        new Rectangle Bottom { get { return new Rectangle(0, this.ClientSize.Height - _, this.ClientSize.Width, _); } }
        new Rectangle Right { get { return new Rectangle(this.ClientSize.Width - _, 0, _, this.ClientSize.Height); } }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return;
            e.Graphics.FillRectangle(Brushes.Gray, Top);
            e.Graphics.FillRectangle(Brushes.Gray, Left);
            e.Graphics.FillRectangle(Brushes.Gray, Right);
            e.Graphics.FillRectangle(Brushes.Gray, Bottom);
        }
        #endregion

        #region winform_functions
        public settings()
        {
            InitializeComponent();
        }

        private void fullscreenCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.fullScreen = fullscreenCheckbox.Checked;
            Properties.Settings.Default.Save();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void settings_Load(object sender, EventArgs e)
        {

        }

        private void settings_Shown(object sender, EventArgs e)
        {
            location.PositionWindow(this);
            homePageBox.Text = Properties.Settings.Default.homeSite;
            cityBox.Text = Properties.Settings.Default.city;
            countryBox.Text = Properties.Settings.Default.country;
            //saveWndP.Checked = Properties.Settings.Default.windowSize;
            fullscreenCheckbox.Checked = Properties.Settings.Default.fullScreen;
            fullnameBox.Text = sqlite_accounts.getFullName();

            searchProvider.Text = Properties.Settings.Default.searchProvider;

            reloadBrowseHistory();

            /*if (accForm.tmpUsername != "Guest" && accForm.tmpUsername != "") {
                // email info
                string[] accInfo = sqlite_accounts.getEmailInfo();
                bool checkSSL = Convert.ToInt32(accInfo[6]) != 0;
                emailAddress.Text = accInfo[0];
                emailPassword.Text = accInfo[1];
                imapServer.Text = accInfo[2];
                imapPort.Text = accInfo[3];
                smtpServer.Text = accInfo[4];
                smtpPort.Text = accInfo[5];
                useSSL.Checked = checkSSL;
            }*/
        }

        private void reloadBrowseHistory()
        {
            browseHistoryList.Items.Clear();
            List<string> tmpHistory = sqlite_accounts.getHistory();
            tmpHistory.Sort();
            foreach (string l in tmpHistory)
            {
                browseHistoryList.Items.Add(l);
            }
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void updateFNBtn_Click(object sender, EventArgs e)
        {
            RestAPI.updateFullName(fullnameBox.Text);
        }

        private void closeBtn_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void settings_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (accForm.tmpUsername != "Guest" && accForm.tmpUsername != "")
            {
                if (homePageBox.Text.Length > 4) // make sure it's not blank
                    Properties.Settings.Default.homeSite = homePageBox.Text;
                if (cityBox.Text.Length > 0)
                    Properties.Settings.Default.city = cityBox.Text;
                if (countryBox.Text.Length > 0)
                    Properties.Settings.Default.country = countryBox.Text;

                Properties.Settings.Default.searchProvider = searchProvider.Text;

                Properties.Settings.Default.Save();

                //int ssl = useSSL.Checked ? 1 : 0;
                //sqlite_accounts.emailAcc(emailAddress.Text, emailPassword.Text, imapServer.Text, Convert.ToInt32(imapPort.Text), smtpServer.Text, Convert.ToInt32(smtpPort.Text), ssl);
            }
        }
        #endregion

        private void MiniBtn_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void DeleteBrowserHistoryBtn_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem i in browseHistoryList.SelectedItems)
            {
                sqlite_accounts.deleteHistory(i.Text);
            }
            reloadBrowseHistory();
        }

        private void DeleteAllBrowsingHistory_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem i in browseHistoryList.Items)
            {
                sqlite_accounts.deleteHistory(i.Text);
            }
            reloadBrowseHistory();
        }
    }
}
