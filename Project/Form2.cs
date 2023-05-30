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
using System.Windows.Forms.DataVisualization.Charting;

namespace Project
{
    public partial class Form2 : Form
    {
        string connectionString = "Data Source=localhost;" +
    "Initial Catalog=Project;" +
    "Integrated Security=True";

        SqlConnection connection;
        public Form2()
        {
            InitializeComponent();
            connection = new SqlConnection(connectionString);

        }

        private void Form2_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'projectDataSet.Sales' table. You can move, or remove it, as needed.
            this.salesTableAdapter.Fill(this.projectDataSet.Sales);
            // TODO: This line of code loads data into the 'projectDataSet.Items' table. You can move, or remove it, as needed.
            this.itemsTableAdapter.Fill(this.projectDataSet.Items);
            RefreshDataGridview();

        }
        private void RefreshDataGridview()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string commandString = $"Select * from Items";
                SqlCommand command = new SqlCommand(commandString, connection);

                SqlDataAdapter da = new SqlDataAdapter(command);

                DataTable dt = new DataTable();

                da.Fill(dt);

                dataitem.DataSource = dt;
            }
        }

        private void btnsignup_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text;

            bool isDuplicate = CheckDuplicateUsername(username);

            if (isDuplicate)
            {
                MessageBox.Show("Username already exists. Please choose a different username.");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtPassword.Text) || string.IsNullOrWhiteSpace(UserType.Text))
            {
                MessageBox.Show("Please enter all the required fields.");
                return;
            }

            string commandString = $"INSERT INTO Users (Username, Password, userType) VALUES (@Username, @Password, @UserType)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(commandString, connection);
                command.Parameters.AddWithValue("@Username", txtUsername.Text);
                command.Parameters.AddWithValue("@Password", txtPassword.Text);
                command.Parameters.AddWithValue("@UserType", UserType.Text);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    MessageBox.Show("User registration successful.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred while registering the user: " + ex.Message);
                }
                finally
                {
                    connection.Close();
                }
            }

            RefreshDataGridview();
            txtUsername.Text = "";
            txtPassword.Text = "";
            UserType.Text = "";
        }
        private bool CheckDuplicateUsername(string username)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM Users WHERE Username = @Username", connection);
                command.Parameters.AddWithValue("@Username", username);

                int count = (int)command.ExecuteScalar();

                return count > 0;
            }
        }
        private void btnAddItem_Click(object sender, EventArgs e)
        {
            string commandString = $"INSERT INTO Items (product_Name, price) VALUES ('{txtName.Text}', '{txtPrice.Text}')";
            SqlCommand command = new SqlCommand(commandString, connection);

            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();

            RefreshDataGridview();
            txtID.Text = "";
            txtName.Text = "";
            txtPrice.Text = "";
            MessageBox.Show("The product has been added successfully");
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            string commandString = $"Update Items Set product_Name = '{txtName.Text}', price = '{txtPrice.Text}' where id_product = {txtID.Text}";

            SqlCommand command = new SqlCommand(commandString, connection);

            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();

            RefreshDataGridview();
            txtID.Text = "";
            txtName.Text = "";
            txtPrice.Text = "";
            MessageBox.Show("The product has been update successfully");
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            string commandString = $"Delete from Items where id_product = {txtID.Text}";
            SqlCommand command = new SqlCommand(commandString, connection);

            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();

            RefreshDataGridview();
            txtID.Text = "";
            txtName.Text = "";
            txtPrice.Text = "";
            MessageBox.Show("The product has been delete successfully");
        }
        private void RefreshDataGridview1()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string commandString = $"SELECT * FROM Sales";
                SqlCommand command = new SqlCommand(commandString, connection);

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                dgvSales.DataSource = dataTable;
            }
        }

        private void btnViewAllSales_Click(object sender, EventArgs e)
        {
            string commandString = "SELECT * FROM Sales";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand(commandString, connection);

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                dgvSales.DataSource = dataTable;
            }
        }

        private void btnViewSales_Click(object sender, EventArgs e)
        {
            DateTime startDate = datePickerStartDate.Value;
            DateTime endDate = datePickerEndDate.Value;

            string commandString = $"SELECT * FROM Sales WHERE SaleDate BETWEEN @startDate AND @endDate";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand(commandString, connection);
                command.Parameters.AddWithValue("@startDate", startDate);
                command.Parameters.AddWithValue("@endDate", endDate);

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                dgvSales.DataSource = dataTable;
            }
        }

        private void btnAllSalesGraph_Click(object sender, EventArgs e)
        {
            string commandString = "SELECT SaleDate, Total FROM Sales";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand(commandString, connection);

                SqlDataReader reader = command.ExecuteReader();

                chartSales.Series.Clear();

                Series SalesSeries = new Series("Sales");
                SalesSeries.ChartType = SeriesChartType.Line;

                while (reader.Read())
                {
                    DateTime saleDate = (DateTime)reader["SaleDate"];
                    int Total = (int)reader["Total"];

                    SalesSeries.Points.AddXY(saleDate, Total);
                }
                chartSales.Series.Add(SalesSeries);

                reader.Close();
            }
        }

        private void btnSalesGraph_Click(object sender, EventArgs e)
        {
            DateTime startDate = datePickerStart.Value;
            DateTime endDate = datePickerEnd.Value;

            string commandString = $"SELECT SaleDate, Total FROM Sales WHERE SaleDate BETWEEN @StartDate AND @EndDate";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand(commandString, connection);
                command.Parameters.AddWithValue("@StartDate", startDate);
                command.Parameters.AddWithValue("@EndDate", endDate);

                SqlDataReader reader = command.ExecuteReader();

                chartSales.Series.Clear();

                Series SalesSeries = new Series("Sales");
                SalesSeries.ChartType = SeriesChartType.Line;

                while (reader.Read())
                {
                    DateTime saleDate = (DateTime)reader["SaleDate"];
                    int Total = (int)reader["Total"];

                    SalesSeries.Points.AddXY(saleDate, Total);
                }
                chartSales.Series.Add(SalesSeries);

                reader.Close();
            }
        }

        private void dataitem_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            txtID.Text = dataitem.Rows[e.RowIndex].Cells[0].Value.ToString();
            txtName.Text = dataitem.Rows[e.RowIndex].Cells[1].Value.ToString();
            txtPrice.Text = dataitem.Rows[e.RowIndex].Cells[2].Value.ToString();
        }
    }
}
