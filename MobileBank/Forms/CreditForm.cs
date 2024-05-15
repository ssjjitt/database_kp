using MobileBank.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MobileBank.Forms
{
    public partial class CreditForm : Form
    {
        DataBaseConnection database = new DataBaseConnection();
        Random rand = new Random();
        SqlDataAdapter adapter = new SqlDataAdapter();
        DataTable table = new DataTable();
        Validation validation = new Validation();

        public const int WM_NCLBUTTONDOWN = 0xA1;

        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImport("user32.dll")]

        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [System.Runtime.InteropServices.DllImport("user32.dll")]

        public static extern bool ReleaseCapture();
        public CreditForm()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";
            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ButtonPay_Click(object sender, EventArgs e)
        {
            DateTime toPayDate = Convert.ToDateTime(labelDateToPay.Text);
            toPayDate = toPayDate.AddMonths(1);
            var sumToPay = labelToPay.Text;
            double toPaySum = 0;
            DateTime dateRepay = new DateTime();
            bool error = false;

            database.openConnection();

            double cardBalanceCheck = 0;
            var queryCheckCard = $"select bank_card_balance from bank_card where bank_card_number = '{DataStorage.cardNumber}'";
            SqlCommand commandCheckCard = new SqlCommand(queryCheckCard, database.getConnection());
            database.openConnection();
            SqlDataReader reader = commandCheckCard.ExecuteReader();
            while (reader.Read())
            {
                cardBalanceCheck = Convert.ToDouble(reader[0].ToString());
            }
            reader.Close();
            database.closeConnection();

            double checkSum = Convert.ToDouble(labelSum.Text);
            double checkTotalSum = Convert.ToDouble(labelTotalSum.Text);
            bool checkStatus = false;

            if (checkSum >= checkTotalSum)
            {
                MessageBox.Show("Кредит погашен!", "Сообщение", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Close();
                checkStatus = true;
            }

            if (checkStatus == false)
            {
                double paymentSum = Convert.ToDouble(labelToPay.Text);

                if (paymentSum > cardBalanceCheck)
                {
                    MessageBox.Show("Ошибка. Недостаточно средств для совершения операции", "Отмена", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    error = true;
                }

                if (error == false)
                {
                    database.openConnection();
                    string formattedTransactionDate = toPayDate.ToString("yyyy-MM-dd HH:mm:ss");
                    var queryPayCredit = $"update credits set repayment_date = '{formattedTransactionDate}', credit_sum = credit_sum + repayment_sum where id_bank_card = (select id_bank_card from bank_card where bank_card_number = '{DataStorage.cardNumber}')";
                    var queryPay = $"update bank_card set bank_card_balance = bank_card_balance - '{sumToPay}' where bank_card_number = '{DataStorage.cardNumber}'";

                    DateTime transactionDate = DateTime.Now;
                    var transactionNumber = "P";
                    for (int i = 0; i < 10; i++)
                    {
                        transactionNumber += Convert.ToString(rand.Next(0, 10));
                    }
                    string formattedTransactionDate2 = transactionDate.ToString("yyyy-MM-dd HH:mm:ss");
                    var queryTransaction = $"insert into transactions(transaction_type, transaction_destination, transaction_date, transaction_number, transaction_value, id_bank_card) values ('Кредит', 'Погашение кредита', '{formattedTransactionDate2}', '{transactionNumber}', '{sumToPay}', (select id_bank_card from bank_card where bank_card_number = '{DataStorage.cardNumber}'))";

                    var command = new SqlCommand(queryPayCredit, database.getConnection());
                    var command1 = new SqlCommand(queryPay, database.getConnection());
                    var command2 = new SqlCommand(queryTransaction, database.getConnection());

                    command.ExecuteNonQuery();
                    command1.ExecuteNonQuery();
                    command2.ExecuteNonQuery();

                    var querySelectRepayment = $"select repayment_date, credit_sum from credits where id_bank_card = (select id_bank_card from bank_card where bank_card_number = '{DataStorage.cardNumber}')";
                    SqlCommand commandSelectRepayment = new SqlCommand(querySelectRepayment, database.getConnection());
                    SqlDataReader reader1 = commandSelectRepayment.ExecuteReader();
                    while (reader1.Read())
                    {
                        dateRepay = Convert.ToDateTime(reader1[0].ToString());
                        toPaySum = Convert.ToDouble(reader1[1].ToString());
                    }
                    reader1.Close();
                    database.closeConnection();

                    labelSum.Text = Math.Round(toPaySum, 2).ToString();
                    labelDateToPay.Text = dateRepay.ToShortDateString();
                }
            }
        }

        private void CreditForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
        private void calculateCredit()
        {
            double monthlyRate = 0.01;
            double sum = Convert.ToDouble(TextBoxSum.Text);
            int numberOfMonths = Convert.ToInt32(TextBoxMonth.Text);
            double rezult = sum * (monthlyRate + (monthlyRate / (Math.Pow(1 + monthlyRate, numberOfMonths) - 1)));
            labelMonthlyPayment.Text = Math.Round(rezult, 2).ToString();
        }
        private void CreditForm_Load(object sender, EventArgs e)
        {
            TextBoxSum.Text = trackBar1.Value.ToString();
            TextBoxMonth.Text = trackBar2.Value.ToString();
            panel2.Visible = false;
            ButtonPay.Visible = false;

            var totalSum = "";
            var sum = "";
            DateTime date1 = new DateTime();
            var idCredit = "";

            double creditTotalSumToCheck = 0;
            double creditSumToCheck = 0;

            string date = date1.ToString("yyyy-MM-dd HH:mm:ss");

            var queryCheckCreditStatus = $"select credit_total_sum, credit_sum from credits where id_bank_card = (select id_bank_card from bank_card where bank_card_number = '{DataStorage.cardNumber}')";
            SqlCommand commandCheckCreditStatus = new SqlCommand(queryCheckCreditStatus, database.getConnection());
            database.openConnection();
            SqlDataReader reader3 = commandCheckCreditStatus.ExecuteReader();
            while (reader3.Read())
            {
                creditTotalSumToCheck = Convert.ToDouble(reader3[0]);
                creditSumToCheck = Convert.ToDouble(reader3[1]);
            }
            reader3.Close();

            if (creditSumToCheck >= creditTotalSumToCheck)
            {
                var queryDeleteCredit = $"delete from credits where id_bank_card = (select id_bank_card from bank_card where bank_card_number = '{DataStorage.cardNumber}')";
                SqlCommand commandDeleteCredit = new SqlCommand(queryDeleteCredit, database.getConnection());
                commandDeleteCredit.ExecuteNonQuery();
            }

            var querySelectIdCard = $"select credits.id_bank_card, credits.credit_total_sum, credits.credit_sum, credits.credit_date, credits.id_credit from credits inner join bank_card on credits.id_bank_card = bank_card.id_bank_card where bank_card.bank_card_number = '{DataStorage.cardNumber}'";
            SqlCommand commandSelectCredit = new SqlCommand(querySelectIdCard, database.getConnection());
            SqlDataReader reader = commandSelectCredit.ExecuteReader();
            while (reader.Read())
            {
                totalSum = reader[1].ToString();
                sum = reader[2].ToString();
                date1 = Convert.ToDateTime(reader[3].ToString());
                idCredit = reader[4].ToString();
            }
            reader.Close();

            SqlCommand commandSelectIdCard = new SqlCommand(querySelectIdCard, database.getConnection());
            adapter.SelectCommand = commandSelectIdCard;
            adapter.Fill(table);
            if (table.Rows.Count > 0)
            {
                panel2.Visible = true;
                ButtonPay.Visible = true;

                labelSum.Text = Math.Round(Convert.ToDouble(sum), 2).ToString();
                labelTotalSum.Text = Math.Round(Convert.ToDouble(totalSum), 2).ToString();
                labelDate.Text = date1.ToShortDateString();

                double toPaySum = 0;
                DateTime dateRepay = new DateTime();

                var querySelectRepayment = $"select repayment_date, repayment_sum from credits where id_credit = '{idCredit}'";
                SqlCommand commandSelectRepayment = new SqlCommand(querySelectRepayment, database.getConnection());
                SqlDataReader reader1 = commandSelectRepayment.ExecuteReader();
                while (reader1.Read())
                {
                    string dateString = reader1[0].ToString();
                    if (DateTime.TryParse(dateString, out DateTime dateRepayValue))
                    {
                        dateRepay = dateRepayValue;
                    }
                    toPaySum = Convert.ToDouble(reader1[1].ToString());
                }
                reader1.Close();
                database.closeConnection();

                labelToPay.Text = Math.Round(toPaySum, 2).ToString();
                labelDateToPay.Text = dateRepay.ToShortDateString();
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            TextBoxSum.Text = trackBar1.Value.ToString();
        }

        private void TextBoxSum_Click(object sender, EventArgs e)
        {
            trackBar1.Value = Convert.ToInt32(TextBoxSum.Text);
            calculateCredit();
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            TextBoxMonth.Text = trackBar2.Value.ToString();
        }

        private void TextBoxMonth_TextChanged(object sender, EventArgs e)
        {
            trackBar2.Value = Convert.ToInt32(TextBoxMonth.Text);
            calculateCredit();
        }

        private void buttonCredit__Click(object sender, EventArgs e)
        {
            trackBar1.Value = Convert.ToInt32(TextBoxSum.Text);
            trackBar2.Value = Convert.ToInt32(TextBoxMonth.Text);
            calculateCredit();

            DataStorage.bankCard = DataStorage.cardNumber;
            validation.ShowDialog();

            if (DataStorage.attemps > 0)
            {
                var totalSum = Convert.ToDouble(labelMonthlyPayment.Text) * Convert.ToDouble(TextBoxMonth.Text);
                DateTime creditDate = DateTime.Now;
                var repaymentDate = creditDate.AddMonths(1);
                var payment = labelMonthlyPayment.Text;

                database.openConnection();
                string formattedTransactionDate = creditDate.ToString("yyyy-MM-dd HH:mm:ss");
                var queryCredit = $"insert into credits(credit_total_sum, credit_sum, credit_date, id_bank_card) values ('{totalSum}', 0, '{formattedTransactionDate}', (select id_bank_card from bank_card where bank_card_number = '{DataStorage.cardNumber}'))";
                var command1 = new SqlCommand(queryCredit, database.getConnection());
                command1.ExecuteNonQuery();

                var idCredit = "";
                var querySelectId = $"select id_credit from credits where id_bank_card = (select id_bank_card from bank_card where bank_card_number = '{DataStorage.cardNumber}')";
                SqlCommand command3 = new SqlCommand(querySelectId, database.getConnection());
                SqlDataReader reader = command3.ExecuteReader();
                while (reader.Read())
                {
                    idCredit = reader[0].ToString();
                }
                reader.Close();

                var sum1 = TextBoxSum.Text;
                string formattedTransactionDate2 = repaymentDate.ToString("yyyy-MM-dd HH:mm:ss");
                var queryRepayment = $"update credits set repayment_date = '{formattedTransactionDate2}', repayment_sum = '{payment}' where id_credit = '{idCredit}'";
                var queryCardUpdate = $"update bank_card set bank_card_balance = bank_card_balance + '{sum1}' where bank_card_number = '{DataStorage.cardNumber}'";

                var command4 = new SqlCommand(queryCardUpdate, database.getConnection());
                var command2 = new SqlCommand(queryRepayment, database.getConnection());

                command4.ExecuteNonQuery();
                command2.ExecuteNonQuery();

                MessageBox.Show("Кредит оформлен!", "Успешно", MessageBoxButtons.OK, MessageBoxIcon.Information);

                DateTime toPatDate = new DateTime();
                DateTime creditTakeDate = new DateTime();
                double creditSum = 0;
                double creditTotalSum = 0;
                double creditToPaySum = 0;

                var querySelectRepayment = $"select credit_date, credit_sum, credit_total_sum, repayment_date, repayment_sum from credits where id_bank_card = (select id_bank_card from bank_card where bank_card_number = '{DataStorage.cardNumber}')";
                SqlCommand commandSelectRepayment = new SqlCommand(querySelectRepayment, database.getConnection());
                SqlDataReader readerUpdate = commandSelectRepayment.ExecuteReader();
                while (readerUpdate.Read())
                {
                    creditTakeDate = Convert.ToDateTime(readerUpdate[0].ToString());
                    creditSum = Convert.ToDouble(readerUpdate[1].ToString());
                    creditTotalSum = Convert.ToDouble(readerUpdate[2].ToString());
                    toPatDate = Convert.ToDateTime(readerUpdate[3].ToString());
                    creditToPaySum = Convert.ToDouble(readerUpdate[4].ToString());
                }
                readerUpdate.Close();
                database.closeConnection();

                labelDate.Text = creditTakeDate.ToShortDateString();
                labelSum.Text = Math.Round(creditSum, 2).ToString();
                labelTotalSum.Text = Math.Round(creditTotalSum, 2).ToString();
                labelDateToPay.Text = toPatDate.ToShortDateString();
                labelToPay.Text = Math.Round(creditToPaySum, 2).ToString();

                ButtonPay.Visible = true;
                panel2.Visible = true;
            }
        }
    }
}
