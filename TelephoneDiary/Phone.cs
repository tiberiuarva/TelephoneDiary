using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;

namespace TelephoneDiary
{
    public partial class Phone : Form
    {
        SqlConnection phoneDiaryConnString = new SqlConnection(@"Data Source=.\sqlexpress;Initial Catalog=phone.diary;Integrated Security=True");

        public Phone()
        {
            InitializeComponent();
        }

        private void Phone_Load(object sender, EventArgs e)
        {
            Display();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            string query = String.Format(@"INSERT INTO [main.list] (FirstName, LastName, Mobile, EmailAddress, Category) 
                            VALUES ('{0}','{1}','{2}','{3}','{4}')",
                            textFirstName.Text, textLastName.Text, textMobile.Text, textEmail.Text, comboBoxCategory.Text);

            if (!AreInputFieldsEmpty())
            {
                RunSqlCmd(query);
                ClearInputFields();
                Display();
            }
            else
            {
                ShowMessageBoxInfo("All input fields must be filled in.");
            }
        }

        private void buttonDeleteAll_Click(object sender, EventArgs e)
        {
            string query = @"DELETE FROM [main.list]";
            RunSqlCmd(query);
            Display();
        }

        private void buttonNew_Click(object sender, EventArgs e)
        {
            ClearInputFields();
            Display();
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            int index = dataGridView1.CurrentCell.RowIndex;
            string Key = dataGridView1.Rows[index].Cells[2].Value.ToString();

            string query = String.Format(@"DELETE FROM [main.list] Where Mobile = '{0}'", Key);

            RunSqlCmd(query);
            Display();
        }

        private void dataGridView1_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            int index = dataGridView1.CurrentCell.RowIndex;
            textFirstName.Text = dataGridView1.Rows[index].Cells[0].Value.ToString();
            textLastName.Text = dataGridView1.Rows[index].Cells[1].Value.ToString();
            textMobile.Text = dataGridView1.Rows[index].Cells[2].Value.ToString();
            textEmail.Text = dataGridView1.Rows[index].Cells[3].Value.ToString();
            comboBoxCategory.Text = dataGridView1.Rows[index].Cells[4].Value.ToString();
        }

        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            string Key = textMobile.Text;
            bool KeyExists = false;

            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                if (Key == dataGridView1.Rows[i].Cells[2].Value.ToString())
                {
                    KeyExists = true;
                }
            }

            if (KeyExists)
            {
                if (AreInputFieldsEmpty())
                {
                    ShowMessageBoxInfo("All input fields must be filled in.");
                }
                else
                {
                    string query = String.Format(@"UPDATE [main.list] 
                                                    SET FirstName='{0}', LastName='{1}', Mobile='{2}', EmailAddress='{3}', Category='{4}'
                                                    Where Mobile = '{5}'", textFirstName.Text, textLastName.Text, textMobile.Text, textEmail.Text, comboBoxCategory.Text, Key);

                    RunSqlCmd(query);
                    Display();
                }
            }
            else
            {
                ShowMessageBoxInfo("Mobile phone not found.");
            }

            Display();
        }

        private void buttonSeedData_Click(object sender, EventArgs e)
        {
            RunSqlInsertCmd("John", "Tank", "010203050", "johntank@domain.com", "Home");
            RunSqlInsertCmd("Rick", "Morris", "010203060", "rickmorris@domain.com", "Office");
            RunSqlInsertCmd("Travis", "Ben", "010203070", "travisben@domain.com", "Business");
            RunSqlInsertCmd("Wiz", "Khalifa", "010203080", "wizkh@domain.com", "Personal");
            RunSqlInsertCmd("Neo", "Erst", "010203090", "neoernst@domain.com", "Family");
            RunSqlInsertCmd("Blue", "Lable", "010203010", "bluelabel@domain.com", "Family");

            Display();
        }

        private void RunSqlInsertCmd(string FirstName, string LastName, string Mobile, string Email, string Category)
        {
            string query = String.Format(@"INSERT INTO [main.list] (FirstName, LastName, Mobile, EmailAddress, Category) 
                            VALUES ('{0}','{1}','{2}','{3}','{4}')",
                FirstName, LastName, Mobile, Email, Category);
            RunSqlCmd(query);
        }

        private void ShowMessageBoxInfo(string message)
        {
            string title = "Info";
            MessageBoxButtons buttons = MessageBoxButtons.OK;
            MessageBox.Show(message, title, buttons, MessageBoxIcon.Warning);
        }

        private void Display()
        {
            string query = @"SELECT * FROM [main.list]";

            SqlDataAdapter sda = new SqlDataAdapter(query, phoneDiaryConnString);

            DataTable phoneList = new DataTable();

            sda.Fill(phoneList);

            dataGridView1.Rows.Clear();

            foreach (DataRow item in phoneList.Rows)
            {
                int n = dataGridView1.Rows.Add();

                dataGridView1.Rows[n].Cells[0].Value = item[0].ToString();
                dataGridView1.Rows[n].Cells[1].Value = item[1].ToString();
                dataGridView1.Rows[n].Cells[2].Value = item[2].ToString();
                dataGridView1.Rows[n].Cells[3].Value = item[3].ToString();
                dataGridView1.Rows[n].Cells[4].Value = item[4].ToString();
            }
        }
        private bool AreInputFieldsEmpty()
        {
            if (textFirstName.Text == "" ||
                textLastName.Text == "" ||
                textMobile.Text == "" ||
                textEmail.Text == "")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void ClearInputFields()
        {
            textFirstName.Clear();
            textLastName.Clear();
            textMobile.Clear();
            textEmail.Clear();
            comboBoxCategory.SelectedIndex = -1;
        }

        public void RunSqlCmd(string query)
        {
            // SqlCommand - insert update delete
            // SqlDataReader - select
            // SqlDataAdapter - insert update delete select
            try
            {
                phoneDiaryConnString.Open();
                SqlCommand runCommand = new SqlCommand(query, phoneDiaryConnString);
                runCommand.ExecuteNonQuery();
                phoneDiaryConnString.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                phoneDiaryConnString.Close();
            }
        }

    }
}
