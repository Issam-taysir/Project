using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;


namespace Project
{
    public partial class Form1 : Form
    {
        string connectionString = "Data Source=localhost;" +
    "Initial Catalog=Project;" +
    "Integrated Security=True";

        SqlConnection connection;
        public Form1()
        {
            InitializeComponent();
        }
        public enum UserType
        {
            None,
            Administrator,
            Cashier
        }
        private void btnlogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Text;

            UserType userType = AuthenticateUser(username, password);

            if (userType == UserType.Administrator)
            {
                OpenForm2Page();
            }
            else if (userType == UserType.Cashier)
            {
                OpenForm3Page();
            }
            else
            {
                MessageBox.Show("Invalid username or password. Please try again.");
            }
        }
        private UserType AuthenticateUser(string username, string password)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand("SELECT userType FROM Users WHERE Username = @Username AND Password = @Password", connection);
                command.Parameters.AddWithValue("@Username", username);
                command.Parameters.AddWithValue("@Password", password);

                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    UserType userType;
                    Enum.TryParse(reader["userType"].ToString(), out userType);

                    return userType;
                }

                return UserType.None;
            }
        }
        private void OpenForm2Page()
        {
            Form2 adminForm = new Form2();
            adminForm.Show();
            Hide();
        }
        private void OpenForm3Page()
        {
            Form3 cashierForm = new Form3();
            cashierForm.Show();
            Hide();
        }
    }
}
