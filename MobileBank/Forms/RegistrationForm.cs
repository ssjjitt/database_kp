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
    public partial class RegistrationForm : Form
    {
        DataBaseConnection database = new DataBaseConnection();
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        public RegistrationForm()
        {
            InitializeComponent();
        }

        private void RegistrationForm_Load(object sender, EventArgs e)
        {
            LastNameTextBox.Select();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            MessageBoxButtons btn = MessageBoxButtons.OK;
            MessageBoxIcon ico = MessageBoxIcon.Information;
            string caption = "Дата сохранения";
            if (!Regex.IsMatch(LastNameTextBox.Text, "[А-Яа-я]+$"))
            {
                MessageBox.Show("Введите повторно фамилию", caption, btn, ico);
                LastNameTextBox.Select();
                return;
            }
            if (!Regex.IsMatch(FirstNameTextBox.Text, "[А-Яа-я]+$"))
            {
                MessageBox.Show("Введите повторно имя", caption, btn, ico);
                FirstNameTextBox.Select();
                return;
            }
            if (!Regex.IsMatch(MiddleNameTextBox.Text, "[А-Яа-я]+$"))
            {
                MessageBox.Show("Введите повторно отчество", caption, btn, ico);
                MiddleNameTextBox.Select();
                return;
            }
            if (string.IsNullOrEmpty(GenderComboBox.SelectedItem.ToString())) {
                MessageBox.Show("Выберите пол", caption, btn, ico);
                GenderComboBox.Select();
                return;
            }
            if (!Regex.IsMatch(PasswordTextBox.Text, "^(?=.*?[A-Z)(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$"))
            {
                MessageBox.Show("Введите пароль", caption, btn, ico);
                PasswordTextBox.Select();
                return;
            }
            if (!Regex.IsMatch(ConfirmPasswordTextBox.Text, "^(?=.*?[A-Z)(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$"))
            {
                MessageBox.Show("Введите повторно пароль", caption, btn, ico);
                ConfirmPasswordTextBox.Select();
                return;
            }
            if (PasswordTextBox.Text != ConfirmPasswordTextBox.Text)
            {
                MessageBox.Show("Введенные пароли не совпадают", caption, btn, ico);
                ConfirmPasswordTextBox.Select();
                return;
            }
            if (!Regex.IsMatch(EmailTextBox.Text, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$"))
            {
                MessageBox.Show("Введите почту", caption, btn, ico);
                EmailTextBox.Select();
                return;
            }
            if (!Regex.IsMatch(PhoneNumberTextBox.Text, @"^(\+375|80)(29|25|44|33)(\d{7})$"))
            {
                MessageBox.Show("Введите номер телефона", caption, btn, ico);
                PhoneNumberTextBox.Select();
                return;
            }

            string yourSQL = "SELECT client_phone_number FROM client WHERE client_phone_number = '" + PhoneNumberTextBox.Text + "'";

            SqlDataAdapter adapter = new SqlDataAdapter();
            DataTable table = new DataTable();
            SqlCommand command = new SqlCommand(yourSQL, database.getConnection());
            adapter.SelectCommand = command;
            adapter.Fill(table);
            if(table.Rows.Count > 0)
            {
                MessageBox.Show("Номер телефона уже зарегистрирован", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Question);
                PhoneNumberTextBox.SelectAll();
                return;
            }
            DialogResult result;
            result = MessageBox.Show("Вы хотите сохранить запись?", "Сохранение данных", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                string mySQL = string.Empty;
                mySQL += "INSERT INTO client (client_last_name, client_first_name, client_middle_name, client_gender, client_password, client_email, client_phone_number, is_admin)";
                mySQL += "VALUES ('" + LastNameTextBox.Text + "','" + FirstNameTextBox.Text + "','" + MiddleNameTextBox.Text + "','" + GenderComboBox.SelectedItem.ToString() + "','" + PasswordTextBox.Text + "','" + EmailTextBox.Text + "','" + PhoneNumberTextBox.Text + "', 0)";
                database.openConnection();
                SqlCommand commandAddNewUser = new SqlCommand(mySQL, database.getConnection());
                commandAddNewUser.ExecuteNonQuery();
                MessageBox.Show("Запись успешно сохранена", "Данные сохранены", MessageBoxButtons.OK, MessageBoxIcon.Information);
                clearControls();
                database.closeConnection();
                Close();
            }
        }
        private void clearControls()
        {
            foreach(TextBox textBox in Controls.OfType<TextBox>())
            {
                textBox.Text = string.Empty;
            }
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            LastNameTextBox.Select();
            clearControls();
        }

        private void ShowPasswordCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (ShowPasswordCheckBox.Checked == true)
            {
                PasswordTextBox.UseSystemPasswordChar = false;
                ConfirmPasswordTextBox.UseSystemPasswordChar = false;
            }
            else
            {
                PasswordTextBox.UseSystemPasswordChar = true;
                ConfirmPasswordTextBox.UseSystemPasswordChar = true;
            }
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void RegistrationForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
    }
}
