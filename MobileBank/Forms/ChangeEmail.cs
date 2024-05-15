using MobileBank.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MobileBank.Forms
{
    public partial class ChangeEmail : Form
    {
        DataBaseConnection dataBase = new DataBaseConnection();

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        public ChangeEmail()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterParent; //////
        }

        private void ChangeBtn_Click(object sender, EventArgs e)
        {
            MessageBoxButtons btn = MessageBoxButtons.OK;
            MessageBoxIcon ico = MessageBoxIcon.Information;
            string caption = "Дата сохранения";

            if(!Regex.IsMatch(ChangeTextBox.Text, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$"))
            {
                MessageBox.Show("Перепроверьте ввод!", caption, btn, ico);
                ChangeTextBox.Select();
                return;
            }

            var text = ChangeTextBox.Text;
            var changeNumQuery = $"update client set client_email = '{text}' where id_client = '{DataStorage.idClient}'";
            var command = new SqlCommand(changeNumQuery, dataBase.getConnection());
            if(command.ExecuteNonQuery() == 1)
            {
                MessageBox.Show("Успешно изменено!");
                Close();
            }
            else
            {
                MessageBox.Show("Что-то пошло не так!");
            }
            dataBase.closeConnection();
        }

        private void ChangeEmail_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
