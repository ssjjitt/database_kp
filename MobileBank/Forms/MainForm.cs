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
    public partial class MainForm : Form
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        DataBaseConnection database = new DataBaseConnection();
        Random rand = new Random();
        SqlDataAdapter adapter = new SqlDataAdapter();
        DataTable table = new DataTable();

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            cardsComboBox.SelectedIndexChanged += cardsComboBox_SelectedIndexChanged;
            label3.BringToFront();
            label3.Text = "";
            label4.BringToFront();
            label5.BringToFront();
            label6.BringToFront();
            label7.BringToFront();
            currencyLabel.BringToFront();
            label9.BringToFront();
            pictureBox4.Visible = false;
            pictureBox5.Visible = false;
            var queryMyCards = $"select id_bank_card, bank_card_number from bank_card where id_client = '{DataStorage.idClient}'";
            SqlDataAdapter commandMyCards = new SqlDataAdapter(queryMyCards, database.getConnection());
            database.openConnection();
            DataTable cards = new DataTable();
            commandMyCards.Fill(cards);
            cardsComboBox.DataSource = cards;
            cardsComboBox.ValueMember = "id_bank_card";
            cardsComboBox.DisplayMember = "bank_card_number";
            database.closeConnection();
            selectBankCard();
        }
        private void cardsComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectBankCard();
        }
        private void selectBankCard()
        {
            label3.Text = "";
            string paymentSystem = "";
            string querySelectCard = $"select bank_card_number, bank_card_cvv_code, CONCAT(FORMAT(bank_card_date, '%M'), '/', FORMAT(bank_card_date, '%y')), bank_card_payment, bank_card_balance, bank_card_currency from bank_card where bank_card_number = '{cardsComboBox.GetItemText(cardsComboBox.SelectedItem)}'";
            SqlCommand command = new SqlCommand(querySelectCard, database.getConnection());
            database.openConnection();
            SqlDataReader reader = command.ExecuteReader();
            while(reader.Read())
            {
                var cardNumber = reader[0].ToString();
                int tmp = 0;
                int tmp1 = 4;
                for(int m = 0; m < 4;  m++)
                {
                    for(int n = tmp; n < tmp1; n++)
                    {
                        label3.Text += cardNumber[n].ToString();
                    }
                    label3.Text += " ";
                    tmp += 4;
                    tmp1 += 4;
                }
                label7.Text = reader[1].ToString(); // cvv
                label5.Text = reader[2].ToString(); // карта до
                paymentSystem = reader[3].ToString(); // система
                label9.Text = Math.Round(Convert.ToDouble(reader[4]), 2).ToString();// баланс
                currencyLabel.Text = reader[5].ToString();
                DataStorage.cardCVV = label7.Text;
                label7.Text = "***";
            }
            reader.Close();
            if(paymentSystem == "VISA")
            {
                pictureBox4.Visible = true;
                pictureBox5.Visible = false;
            }
            else
            {
                pictureBox4.Visible = false;
                pictureBox5.Visible = true;
            }
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            AddBankCard addBankCard = new AddBankCard();
            addBankCard.ShowDialog();
        }

        private void MainForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
        private void label6_Click(object sender, EventArgs e)
        {
            if(label7.Text == "***")
            {
                label7.Text = DataStorage.cardCVV;
            }
            else
            {
                label7.Text = "***";
            }
        }

        private void label7_Click(object sender, EventArgs e)
        {
            if (label7.Text == "***")
            {
                label7.Text = DataStorage.cardCVV;
            }
            else
            {
                label7.Text = "***";
            }
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            var queryMyCards = $"select id_bank_card, bank_card_number from bank_card where id_client = '{DataStorage.idClient}'";
            SqlDataAdapter commandMyCards = new SqlDataAdapter(queryMyCards, database.getConnection());
            database.openConnection();
            DataTable cards = new DataTable();
            commandMyCards.Fill(cards);
            cardsComboBox.DataSource = cards;
            cardsComboBox.ValueMember = "id_bank_card";
            cardsComboBox.DisplayMember = "bank_card_number";
            database.closeConnection();
            selectBankCard();
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SendToForm sendToForm = new SendToForm();
            DataStorage.bankCard = CardTextBox.Text;
            DataStorage.cardNumber = cardsComboBox.GetItemText(cardsComboBox.SelectedItem);
            cardsComboBox.Text = "";
            sendToForm.ShowDialog();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            UserForm userForm = new UserForm();
            userForm.Show();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            History history = new History();
            history.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            PhoneForm phoneForm = new PhoneForm();
            DataStorage.cardNumber = cardsComboBox.GetItemText(cardsComboBox.SelectedItem);
            DataStorage.phoneNumber = textBox1.Text;
            textBox1.Text = "";
            phoneForm.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            CommunalPayments communalPayments = new CommunalPayments();
            DataStorage.cardNumber = cardsComboBox.GetItemText(cardsComboBox.SelectedItem);
            communalPayments.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            InternetAndTvPayments internetAndTvPayments = new InternetAndTvPayments();
            DataStorage.cardNumber = cardsComboBox.GetItemText(cardsComboBox.SelectedItem);
            internetAndTvPayments.Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            CharityForm charityForm = new CharityForm();
            DataStorage.cardNumber = cardsComboBox.GetItemText(cardsComboBox.SelectedItem);
            charityForm.Show();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            DataStorage.cardNumber = cardsComboBox.GetItemText(cardsComboBox.SelectedItem);
            var cardCurrency = "";
            var queryCheckCurrency = $"select bank_card_currency from bank_card where bank_card_number = '{DataStorage.cardNumber}'";
            SqlCommand commandCheckCurrency = new SqlCommand(queryCheckCurrency, database.getConnection());
            SqlDataReader reader = commandCheckCurrency.ExecuteReader();
            while (reader.Read())
            {
                cardCurrency = reader[0].ToString();
            }
            reader.Close();

            if (cardCurrency == "BYN")
            {
                CreditForm credit = new CreditForm();
                credit.Show();
            }
            else
                MessageBox.Show("Операции с кредитом могут производиться только в BYN", "Отказ", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }
    }
}
