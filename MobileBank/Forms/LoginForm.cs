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
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MobileBank.Forms
{
    public partial class LoginForm : Form
    {
        DataBaseConnection database = new DataBaseConnection();

        public LoginForm()
        {
            InitializeComponent();
        }

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        private void LoginForm_Load(object sender, EventArgs e)
        {
            PhoneNumberTextBox.Select();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            RegistrationForm registrationForm = new RegistrationForm();
            registrationForm.ShowDialog();
        }

        private void ShowPasswordCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (ShowPasswordCheckBox.Checked == true)
            {
                PasswordTextBox.UseSystemPasswordChar = false;
            }
            else
            {
                PasswordTextBox.UseSystemPasswordChar = true;
            }
        }
        
        private void LoginForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
        
        private void CloseButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            if(!string.IsNullOrEmpty(PhoneNumberTextBox.Text) && !string.IsNullOrEmpty(PasswordTextBox.Text)) 
            {
                var querySelectClient = $"SELECT * FROM client WHERE client_phone_number = '{PhoneNumberTextBox.Text}' AND client_password = '{PasswordTextBox.Text}'";
                var queryGetId = $"SELECT id_client FROM client WHERE client_phone_number = '{PhoneNumberTextBox.Text}'";
                var commandGetId = new SqlCommand(queryGetId, database.getConnection());
                database.openConnection();
                SqlDataReader reader = commandGetId.ExecuteReader();    
                while(reader.Read())
                {
                    DataStorage.idClient = reader[0].ToString();
                }
                reader.Close();

                SqlDataAdapter adapter = new SqlDataAdapter();
                DataTable table = new DataTable();  

                SqlCommand command = new SqlCommand(querySelectClient, database.getConnection());
                adapter.SelectCommand = command;
                adapter.Fill(table);
                if(table.Rows.Count > 0)
                {
                    var user = new checkUser(table.Rows[0].ItemArray[1].ToString(), Convert.ToBoolean(table.Rows[0].ItemArray[8]));
                    MessageBox.Show(user.Status);
                    if (user.Status == "User")
                    {
                        PhoneNumberTextBox.Clear();
                        PasswordTextBox.Clear();
                        ShowPasswordCheckBox.Checked = true;
                        Hide();
                        MainForm mainForm = new MainForm();
                        mainForm.ShowDialog();
                        mainForm = null;
                        Show();
                        PhoneNumberTextBox.Select();
                    }
                    else
                    {
                        PhoneNumberTextBox.Clear();
                        PasswordTextBox.Clear();
                        ShowPasswordCheckBox.Checked = true;
                        Hide();
                        AdminForm adminForm = new AdminForm();
                        adminForm.ShowDialog();
                        adminForm = null;
                        Show();
                        PhoneNumberTextBox.Select();
                    }
                }
                else
                {
                    MessageBox.Show("Имя пользователя или пароль неверны. Попробуйте еще раз!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Stop );
                    PhoneNumberTextBox.Focus();
                    PhoneNumberTextBox.SelectAll();
                }
            }
            else
            {
                MessageBox.Show("Введите имя пользователя или пароль", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                PhoneNumberTextBox.Select();
            }
        }
    }
}
