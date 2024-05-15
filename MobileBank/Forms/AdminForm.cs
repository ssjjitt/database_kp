using MobileBank.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MobileBank.Forms
{
    enum RowState
    {
        Existed,
        New,
        Modified,
        ModifiedNew,
        Deleted
    }

    public partial class AdminForm : Form
    {
        DataBaseConnection dataBase = new DataBaseConnection();
        int selectedRow;
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        public AdminForm()
        {
            InitializeComponent();
        }

        private void CreateColumns()
        {
            dataGridView1.Columns.Clear();
            dataGridView1.Columns.Add("id_bank_card", "id");
            dataGridView1.Columns.Add("bank_card_type", "Тип карты");
            dataGridView1.Columns.Add("bank_card_number", "Номер карты");
            dataGridView1.Columns.Add("bank_card_cvv_code", "CVV-код");
            dataGridView1.Columns.Add("bank_card_balance", "Баланс");
            dataGridView1.Columns.Add("bank_card_currency", "Тип валюты");
            dataGridView1.Columns.Add("bank_card_payment", "Платежная система");
            dataGridView1.Columns.Add("bank_card_date", "Дата выдачи карты");
            dataGridView1.Columns.Add("bank_card_pin", "PIN-код");
            dataGridView1.Columns.Add("id_client", "id клиента");
            dataGridView1.Columns.Add("IsNew", String.Empty);
        }

        private void CreateColumnsClient()
        {
            dataGridView2.Columns.Clear();
            dataGridView2.Columns.Add("id_client", "id");
            dataGridView2.Columns.Add("client_last_name", "Фамилия");
            dataGridView2.Columns.Add("client_first_name", "Имя");
            dataGridView2.Columns.Add("client_middle_name", "Отчество");
            dataGridView2.Columns.Add("client_gender", "Пол");
            dataGridView2.Columns.Add("client_password", "Пароль");
            dataGridView2.Columns.Add("client_email", "Почта");
            dataGridView2.Columns.Add("client_phone_number", "Номер телефона");
            dataGridView2.Columns.Add("is_admin", "Админ/юзер");

        }

        private void ReadSingleRow(DataGridView dgw, IDataRecord record)
        {
            dgw.Rows.Add(record.GetInt32(0), // айди
                record.GetString(1), // тип
                record.GetString(2), // номер
                record.GetString(3), // cvv
                record.GetDecimal(4), // баланс
                record.GetString(5), // тип валюты
                record.GetString(6),  // плтежная система
                record.GetDateTime(7), // дата выдачи
                record.GetInt32(8), // pin
                record.GetInt32(9),  // айди клиента
                RowState.ModifiedNew); // состояние
        }

        private void ReadSingleRowClient(DataGridView dgw, IDataRecord record)
        {
            dgw.Rows.Add(record.GetInt32(0), // айди
                record.GetString(1), // фамилия
                record.GetString(2), // имя
                record.GetString(3), // отчество
                record.GetString(4), // пол
                record.GetString(5), // пароль
                record.GetString(6),  // почта
                record.GetString(7), // телефон
                record.GetBoolean(8)); // админ/юзер
        }


        private void RefreshDataGrid(DataGridView dgw)
        {
            dgw.DataSource = null;

            string queryString = "select id_bank_card, bank_card_type, bank_card_number, bank_card_cvv_code, bank_card_balance, bank_card_currency, bank_card_payment, bank_card_date, bank_card_pin, id_client from bank_card";
            SqlCommand sqlCommand = new SqlCommand(queryString, dataBase.getConnection());
            dataBase.openConnection();
            SqlDataReader reader = sqlCommand.ExecuteReader();

            dgw.Rows.Clear();

            while (reader.Read())
            {
                ReadSingleRow(dgw, reader);
            }
            reader.Close();
        }

        private void RefreshDataGridClient(DataGridView dgw)
        {
            dgw.DataSource = null;

            string queryString = "select * from client";
            SqlCommand sqlCommand = new SqlCommand(queryString, dataBase.getConnection());
            dataBase.openConnection();
            SqlDataReader reader = sqlCommand.ExecuteReader();

            dgw.Rows.Clear();

            while (reader.Read())
            {
                ReadSingleRowClient(dgw, reader);
            }
            reader.Close();

        }

        private void AdminForm_Load(object sender, EventArgs e)
        {
            CreateColumns();
            RefreshDataGrid(dataGridView1);
            CreateColumnsClient();
            RefreshDataGridClient(dataGridView2);
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;
            button5.Enabled = false;
            button11.Enabled = false;
            button12.Enabled = false;
            button16.Enabled = false;
            button17.Enabled = false;
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        // блокировка кнопок снимается
        private void idTextBox1_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(idTextBox1.Text))
            {
                button5.Enabled = false;
            }
            else
            {
                button5.Enabled = true;
            }
        }

        private void idTextBox_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(idTextBox.Text))
            {
                button4.Enabled = false;
            }
            else
            {
                button4.Enabled = true;
            }
        }

        private void sumTextBox_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(sumTextBox.Text))
            {
                button3.Enabled = false;
            }
            else
            {
                button3.Enabled = true;
            }
        }

        private void cardTextBox_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cardTextBox.Text))
            {
                button2.Enabled = false;
            }
            else
            {
                button2.Enabled = true;
            }
        }

        private void nameTextBox_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(nameTextBox.Text))
            {
                button1.Enabled = false;
            }
            else
            {
                button1.Enabled = true;
            }
        }

        private void genderTextBox_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(genderTextBox.Text))
            {
                button11.Enabled = false;
            }
            else
            {
                button11.Enabled = true;
            }
        }

        private void idTextBoxN_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(idTextBoxN.Text) && string.IsNullOrEmpty(nTextBox.Text))
            {
                button12.Enabled = false;
            }
            else
            {
                button12.Enabled = true;
            }
        }

        private void nTextBox_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(idTextBoxN.Text) && string.IsNullOrEmpty(nTextBox.Text))
            {
                button12.Enabled = false;
            }
            else
            {
                button12.Enabled = true;
            }
        }

        private void eachCardTextBox_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(eachCardTextBox.Text))
            {
                button16.Enabled = false;
            }
            else
            {
                button16.Enabled = true;
            }
        }

        private void idForTransTextBox_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(idForTransTextBox.Text))
            {
                button17.Enabled = false;
            }
            else
            {
                button17.Enabled = true;
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e) // обнова стр
        {
            CreateColumns();
            RefreshDataGrid(dataGridView1);
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            CreateColumnsClient();
            RefreshDataGridClient(dataGridView2);
        }

        private void pictureBox1_Click(object sender, EventArgs e) // главный поиск
        {
            string searchStr = searchTextBox.Text;

            SqlCommand command = new SqlCommand("SELECT * FROM dbo.SearchClients(@SearchString)", dataBase.getConnection());
            command.Parameters.Add("@SearchString", SqlDbType.NVarChar).Value = searchStr;

            SqlDataAdapter adapter = new SqlDataAdapter(command);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);

            if (dataTable.Rows.Count > 0)
            {
                dataGridView1.Columns.Clear();
                dataGridView1.DataSource = dataTable;
            }
            else
            {
                dataGridView1.DataSource = null;
            }
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            string searchStr = textBox1.Text;

            SqlCommand command = new SqlCommand("SELECT * FROM dbo.SearchClients(@SearchString)", dataBase.getConnection());
            command.Parameters.Add("@SearchString", SqlDbType.NVarChar).Value = searchStr;

            SqlDataAdapter adapter = new SqlDataAdapter(command);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);

            if (dataTable.Rows.Count > 0)
            {
                dataGridView2.Columns.Clear();
                dataGridView2.DataSource = dataTable;
            }
            else
            {
                dataGridView2.DataSource = null;
            }
        }

        private void button1_Click(object sender, EventArgs e) // поиск по фамилии(полное совпадение)
        {
            string name = nameTextBox.Text;
            SqlCommand command = new SqlCommand("SELECT * FROM dbo.SearchClientByLastName(@lastName)", dataBase.getConnection());
            command.Parameters.Add("@lastName", SqlDbType.NVarChar).Value = name;
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);
            if (dataTable.Rows.Count > 0)
            {
                dataGridView1.Columns.Clear();
                dataGridView1.DataSource = dataTable;
            }
            else
            {
                dataGridView1.DataSource = null;
            }
        }

        private void button2_Click(object sender, EventArgs e) // транзакции по номеру 
        {
            string str = cardTextBox.Text;
            SqlCommand command = new SqlCommand("SELECT * FROM dbo.GetTransByNumber(@bankCardNumber)", dataBase.getConnection());
            command.Parameters.Add("@bankCardNumber", SqlDbType.NVarChar).Value = str;
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);
            if (dataTable.Rows.Count > 0)
            {
                dataGridView1.Columns.Clear();
                dataGridView1.DataSource = dataTable;
            }
            else
            {
                dataGridView1.DataSource = null;
            }
        }

        private void button3_Click(object sender, EventArgs e) // общий баланс на картах
        {
            string str = sumTextBox.Text;
            SqlCommand command = new SqlCommand("SELECT dbo.CalculateTotalBalance(@clientId) AS TotalBalance", dataBase.getConnection());
            command.Parameters.Add("@clientId", SqlDbType.Int).Value = Convert.ToInt32(str);
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);
            if (dataTable.Rows.Count > 0)
            {
                dataGridView1.Columns.Clear();
                dataGridView1.DataSource = dataTable;
            }
            else
            {
                dataGridView1.DataSource = null;
            }
        }

        private void button4_Click(object sender, EventArgs e) // транзакция по айди
        {
            string str = idTextBox.Text;
            SqlCommand command = new SqlCommand("SELECT * FROM dbo.GetTransById(@bankCardId)", dataBase.getConnection());
            command.Parameters.Add("@bankCardId", SqlDbType.NVarChar).Value = str;
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);
            if (dataTable.Rows.Count > 0)
            {
                dataGridView1.Columns.Clear();
                dataGridView1.DataSource = dataTable;
            }
            else
            {
                dataGridView1.DataSource = null;
            }
        }

        private void button5_Click(object sender, EventArgs e) // активные кредиты
        {
            string str = idTextBox1.Text;
            SqlCommand command = new SqlCommand("SELECT dbo.HasActiveCredit(@clientId) AS HasActiveCredit", dataBase.getConnection());
            command.Parameters.Add("@clientId", SqlDbType.Int).Value = int.Parse(str);
            dataBase.openConnection();
            bool has = Convert.ToBoolean(command.ExecuteScalar());
            dataBase.closeConnection();
            if (has)
            {
                MessageBox.Show("Есть активные кредиты");
            }
            else
            {
                MessageBox.Show("Нет активных кредитов");
            }
        }

        private void button11_Click(object sender, EventArgs e) // поиск по гендеру
        {
            string str = genderTextBox.Text;
            SqlCommand command = new SqlCommand("SELECT * FROM dbo.GetClientsByGender(@Gender)", dataBase.getConnection());
            command.Parameters.Add("@Gender", SqlDbType.NVarChar).Value = str;
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);
            if (dataTable.Rows.Count > 0)
            {
                dataGridView1.Columns.Clear();
                dataGridView1.DataSource = dataTable;
            }
            else
            {
                dataGridView1.DataSource = null;
            }
        }

        private void button12_Click(object sender, EventArgs e) // получить ласт транзакции
        {
            int str = Convert.ToInt32(idTextBoxN.Text);
            int str2 = Convert.ToInt32(nTextBox.Text);
            SqlCommand command = new SqlCommand("SELECT * FROM dbo.GetRecentTransactions(@BankCardID, @NumTransactions)", dataBase.getConnection());
            command.Parameters.Add("@BankCardID", SqlDbType.Int).Value = str;
            command.Parameters.Add("@NumTransactions", SqlDbType.Int).Value = str2;
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);
            if (dataTable.Rows.Count > 0)
            {
                dataGridView1.Columns.Clear();
                dataGridView1.DataSource = dataTable;
            }
            else
            {
                dataGridView1.DataSource = null;
            }
        }

        private void AdminForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void button15_Click_1(object sender, EventArgs e)
        {
            SqlCommand command = new SqlCommand("SELECT * FROM dbo.GetClientsWithCreditDebt()", dataBase.getConnection());
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);

            if (dataTable.Rows.Count > 0)
            {
                dataGridView1.Columns.Clear();
                dataGridView1.DataSource = dataTable;
            }
            else
            {
                MessageBox.Show("Нет данных о клиентах с задолженностью по кредиту.", "Отсутствие данных");
                dataGridView1.DataSource = null;
            }
        }

        private void button16_Click(object sender, EventArgs e)
        {
            int str = Convert.ToInt32(eachCardTextBox.Text);

            SqlCommand command = new SqlCommand("SELECT * FROM dbo.GetLastTransactionForEachCard(@ClientID)", dataBase.getConnection());
            command.Parameters.Add("@ClientID", SqlDbType.Int).Value = str;
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);

            if (dataTable.Rows.Count > 0)
            {
                dataGridView1.Columns.Clear();
                dataGridView1.DataSource = dataTable;
            }
            else
            {
                dataGridView1.DataSource = null;
            }
        }

        private void button17_Click(object sender, EventArgs e)
        {
            int idBankCard = Convert.ToInt32(idForTransTextBox.Text);

            SqlCommand command = new SqlCommand("SELECT dbo.GetTransactionTotalAmount(@id_bank_card)", dataBase.getConnection());
            command.Parameters.Add("@id_bank_card", SqlDbType.Int).Value = idBankCard;

            decimal totalAmount = 0;
            object result = command.ExecuteScalar();
            if (result != DBNull.Value)
            {
                totalAmount = (decimal)result;
                MessageBox.Show("Общая сумма транзакций: " + totalAmount.ToString("C"));
            }
            else
            {
                MessageBox.Show("Ничего не найдено!");
            }
        }

        private void button18_Click(object sender, EventArgs e)
        {
            SqlCommand command = new SqlCommand("SELECT dbo.GetClientCount()", dataBase.getConnection());
            int count = (int)command.ExecuteScalar();

            if (count > 0)
            {
                MessageBox.Show("Количество клиентов: " + count, "Результат");
            }
            else
            {
                MessageBox.Show("Нет клиентов.", "Результат");
            }
        }

        private void button20_Click(object sender, EventArgs e)
        {
            SqlCommand command = new SqlCommand("SELECT dbo.GetBankCardCount()", dataBase.getConnection());
            int count = (int)command.ExecuteScalar();
            if (count > 0)
            {
                MessageBox.Show("Количество банковских карт: " + count, "Результат");
            }
            else
            {
                MessageBox.Show("Нет банковских карт.", "Результат");
            }
        }

        private void button21_Click(object sender, EventArgs e)
        {
            SqlCommand command = new SqlCommand("SELECT dbo.GetTransactionCount()", dataBase.getConnection());
            int count = (int)command.ExecuteScalar();
            if (count > 0)
            {
                MessageBox.Show("Количество транзакций: " + count, "Результат");
            }
            else
            {
                MessageBox.Show("Нет транзакций.", "Результат");
            }
        }

        private void button22_Click(object sender, EventArgs e)
        {
            SqlCommand command = new SqlCommand("SELECT dbo.GetCreditCount()", dataBase.getConnection());
            int count = (int)command.ExecuteScalar();
            if (count > 0)
            {
                MessageBox.Show("Количество кредитов: " + count, "Результат");
            }
            else
            {
                MessageBox.Show("Нет кредитов.", "Результат");
            }
        }

        private void button23_Click(object sender, EventArgs e)
        {
            SqlCommand command = new SqlCommand("SELECT dbo.GetActiveServiceCount()", dataBase.getConnection());
            int count = (int)command.ExecuteScalar();
            if (count > 0)
            {
                MessageBox.Show("Количество клиентов без активных банковских карт: " + count, "Результат");
            }
            else
            {
                MessageBox.Show("Нет клиентов без активных банковских карт.", "Результат");
            }
        }

        private void button19_Click(object sender, EventArgs e)
        {
            SqlCommand command = new SqlCommand("SELECT * FROM dbo.GetClientsWithoutActiveBankCards()", dataBase.getConnection());
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);
            if (dataTable.Rows.Count > 0)
            {
                dataGridView1.Columns.Clear();
                dataGridView1.DataSource = dataTable;
            }
            else
            {
                dataGridView1.DataSource = null;
            }
        }

        private void button6_Click_1(object sender, EventArgs e)
        {
            dataBase.openConnection();
            try
            {
                // Проверка на заполнение всех полей
                if (string.IsNullOrEmpty(textBox2.Text) ||
                    string.IsNullOrEmpty(textBox3.Text) ||
                    string.IsNullOrEmpty(textBox4.Text) ||
                    string.IsNullOrEmpty(textBox5.Text) ||
                    string.IsNullOrEmpty(textBox6.Text) ||
                    string.IsNullOrEmpty(textBox7.Text) ||
                    string.IsNullOrEmpty(textBox8.Text) ||
                    string.IsNullOrEmpty(textBox9.Text))
                {
                    MessageBox.Show("Необходимо заполнить все поля.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                using (SqlCommand command = new SqlCommand("dbo.CreateClient", dataBase.getConnection()))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@client_last_name", textBox2.Text);
                    command.Parameters.AddWithValue("@client_first_name", textBox3.Text);
                    command.Parameters.AddWithValue("@client_middle_name", textBox4.Text);
                    command.Parameters.AddWithValue("@client_gender", textBox5.Text);
                    command.Parameters.AddWithValue("@client_password", textBox6.Text);
                    command.Parameters.AddWithValue("@client_email", textBox7.Text);
                    command.Parameters.AddWithValue("@client_phone_number", textBox8.Text);

                    bool isAdmin;
                    if (bool.TryParse(textBox9.Text, out isAdmin))
                    {
                        command.Parameters.AddWithValue("@is_admin", isAdmin);

                        command.ExecuteNonQuery();

                        // Очистка полей после успешного выполнения запроса
                        textBox2.Text = "";
                        textBox3.Text = "";
                        textBox4.Text = "";
                        textBox5.Text = "";
                        textBox6.Text = "";
                        textBox7.Text = "";
                        textBox8.Text = "";
                        textBox9.Text = "";
                    }
                    else
                    {
                        MessageBox.Show("Неверное значение в поле 'isAdmin'. Введите 'true' или 'false'.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при выполнении запроса: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (dataGridView2.SelectedRows.Count > 0)
            {
                int clientId = Convert.ToInt32(dataGridView2.SelectedRows[0].Cells["id_client"].Value);

                try
                {
                    using (SqlCommand command = new SqlCommand("dbo.DeleteClient", dataBase.getConnection()))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@id_client", clientId);

                        command.ExecuteNonQuery();

                        MessageBox.Show("Клиент успешно удален.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (SqlException ex)
                {
                    string errorMessage = ex.Message;
                    int errorSeverity = ex.Class;
                    int errorState = ex.State;

                    MessageBox.Show(errorMessage, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Выберите клиента для удаления.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView2.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridView2.SelectedRows[0];
                int clientId = Convert.ToInt32(selectedRow.Cells["id_client"].Value);
                string lastName = Convert.ToString(selectedRow.Cells["client_last_name"].Value);
                string firstName = Convert.ToString(selectedRow.Cells["client_first_name"].Value);
                string middleName = Convert.ToString(selectedRow.Cells["client_middle_name"].Value);
                string gender = Convert.ToString(selectedRow.Cells["client_gender"].Value);
                string password = Convert.ToString(selectedRow.Cells["client_password"].Value);
                string email = Convert.ToString(selectedRow.Cells["client_email"].Value);
                string phoneNumber = Convert.ToString(selectedRow.Cells["client_phone_number"].Value);
                bool isAdmin = Convert.ToBoolean(selectedRow.Cells["is_admin"].Value);
                textBox2.Text = lastName;
                textBox3.Text = firstName;
                textBox4.Text = middleName;
                textBox5.Text = gender;
                textBox6.Text = password;
                textBox7.Text = email;
                textBox8.Text = phoneNumber;
                textBox9.Text = isAdmin.ToString();
            }
            else
            {
                MessageBox.Show("Выберите клиента для редактирования.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (dataGridView2.SelectedRows.Count > 0)
            {
                int clientId = Convert.ToInt32(dataGridView2.SelectedRows[0].Cells["id_client"].Value);

                try
                {
                    using (SqlCommand command = new SqlCommand("dbo.UpdateClient", dataBase.getConnection()))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@id_client", clientId);
                        command.Parameters.AddWithValue("@client_last_name", textBox2.Text);
                        command.Parameters.AddWithValue("@client_first_name", textBox3.Text);
                        command.Parameters.AddWithValue("@client_middle_name", textBox4.Text);
                        command.Parameters.AddWithValue("@client_gender", textBox5.Text);
                        command.Parameters.AddWithValue("@client_password", textBox6.Text);
                        command.Parameters.AddWithValue("@client_email", textBox7.Text);
                        command.Parameters.AddWithValue("@client_phone_number", textBox8.Text);

                        bool isAdmin;
                        if (bool.TryParse(textBox9.Text, out isAdmin))
                        {
                            command.Parameters.AddWithValue("@is_admin", isAdmin);

                            command.ExecuteNonQuery();

                            MessageBox.Show("Клиент успешно обновлен.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Неверное значение в поле 'isAdmin'. Введите 'true' или 'false'.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (SqlException ex)
                {
                    string errorMessage = ex.Message;
                    int errorSeverity = ex.Class;
                    int errorState = ex.State;
                    MessageBox.Show(errorMessage, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Выберите клиента для обновления.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}