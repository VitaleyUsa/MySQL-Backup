using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace EisBackup
{
    public partial class MainForm : Form
    {
        public MainForm(string[] args)
        {
            InitializeComponent();

            // Реализация передачи параметров при фоновом выполнении скрипта
            if (args.Length > 0)
                if (args[0] == "/s")
                {
                    this.WindowState = FormWindowState.Minimized;
                    this.ShowInTaskbar = false;
                    Program.SilentExport = true;
                    Program.SilentFilePath = args[1];
                    exportControl1.btExport_Click(null, null);
                }

            TextBoxWatermarkExtensionMethod.SetWatermark(settingsControl1.tbLogin, "Имя пользователя");
            TextBoxWatermarkExtensionMethod.SetWatermark(settingsControl1.tbPassword, "Пароль");
            TextBoxWatermarkExtensionMethod.SetWatermark(settingsControl1.tbDatabase, "База данных");
            TextBoxWatermarkExtensionMethod.SetWatermark(settingsControl1.tbServer, "Адрес сервера");

            exportControl1.ButtonClick += new EventHandler(MakeButtonsInactive);
            importControl1.ButtonClick += new EventHandler(MakeButtonsInactive);
            settingsControl1.ButtonClick += new EventHandler(CheckCred);

            if (Properties.Settings.Default.Login == "" ||
                Properties.Settings.Default.Password == "" ||
                Properties.Settings.Default.Database == "" ||
                Properties.Settings.Default.Server == "")
            {
                btExport.Enabled = false;
                btImport.Enabled = false;
                btTasker.Enabled = false;
            }
        }

        private void CheckCred(object sender, EventArgs e)
        {
            if (settingsControl1.tbLogin.Text == "" &&
                settingsControl1.tbPassword.Text == "" &&
                settingsControl1.tbDatabase.Text == " " &&
                settingsControl1.tbServer.Text == "")
            {
                btExport.Enabled = false;
                btImport.Enabled = false;
                btTasker.Enabled = false;
            }
            else
            {
                btExport.Enabled = true;
                btImport.Enabled = true;
                btTasker.Enabled = true;
            }
        }

        private void btExport_Click(object sender, EventArgs e)
        {
            settingsControl1.Hide();
            importControl1.Hide();
            exportControl1.Show();
            taskerControl1.Hide();
            aboutControl1.Hide();

            Program.TargetFile = "";
        }

        private void BtImport_Click(object sender, EventArgs e)
        {
            settingsControl1.Hide();
            importControl1.Show();
            exportControl1.Hide();
            taskerControl1.Hide();
            aboutControl1.Hide();

            importControl1.sourceObject.MyCurrentFile = "Не выбран";
        }

        private void BtSettings_Click(object sender, EventArgs e)
        {
            settingsControl1.Show();
            importControl1.Hide();
            exportControl1.Hide();
            taskerControl1.Hide();
            aboutControl1.Hide();
        }

        private void BtTasker_Click(object sender, EventArgs e)
        {
            settingsControl1.Hide();
            importControl1.Hide();
            exportControl1.Hide();
            taskerControl1.Show();
            aboutControl1.Hide();

            taskerControl1.GetTaskStatus();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            settingsControl1.tbLogin.Text = Properties.Settings.Default.Login;
            settingsControl1.tbPassword.Text = Properties.Settings.Default.Password.DecryptString();
            settingsControl1.tbDatabase.Text = Properties.Settings.Default.Database;
            settingsControl1.tbServer.Text = Properties.Settings.Default.Server;
        }

        private void MakeButtonsInactive(object sender, EventArgs e)
        {
            if (Program.Inactive)
            {
                btSettings.Enabled = false;
                btExport.Enabled = false;
                btImport.Enabled = false;
                btTasker.Enabled = false;
                btAbout.Enabled = false;
            }
            else
            {
                btSettings.Enabled = true;
                btExport.Enabled = true;
                btImport.Enabled = true;
                btTasker.Enabled = true;
                btAbout.Enabled = true;
            }
        }

        private void BtAbout_Click(object sender, EventArgs e)
        {
            settingsControl1.Hide();
            importControl1.Hide();
            exportControl1.Hide();
            taskerControl1.Hide();
            aboutControl1.Show();
        }
    }

    public static class TextBoxWatermarkExtensionMethod
    {
        private const uint ECM_FIRST = 0x1500;
        private const uint EM_SETCUEBANNER = ECM_FIRST + 1;

        public static void SetWatermark(this TextBox textBox, string watermarkText)
        {
            SendMessage(textBox.Handle, EM_SETCUEBANNER, 0, watermarkText);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, uint wParam, [MarshalAs(UnmanagedType.LPWStr)] string lParam);
    }
}