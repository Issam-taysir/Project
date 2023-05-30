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
    public partial class Form3 : Form
    {
        string connectionString = "Data Source=localhost;" +
    "Initial Catalog=Project;" +
    "Integrated Security=True";

        SqlConnection connection;
        private SqlDataReader myreader;
        public Form3()
        {
            InitializeComponent();
        }
        private void Form3_Load(object sender, EventArgs e)
        {
            this.itemsTableAdapter.Fill(this.projectDataSet.Items);
            numericUpDown1.ValueChanged += numericUpDown1_ValueChanged;


        }
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (int.TryParse(txtprice.Text, out int price) && int.TryParse(numericUpDown1.Value.ToString(), out int quantity))
            {
                int total = price * quantity;
                totalp.Text = total.ToString();
            }
        }
        private void fillListbox()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string commandString = $"Select * from Items";
                SqlCommand command = new SqlCommand(commandString, connection);
                SqlDataAdapter da = new SqlDataAdapter(command);
                DataTable dt = new DataTable();

                da.Fill(dt);

                listBox1.DataSource = dt;
            }
        }
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string commandString = $"SELECT * FROM Items WHERE product_name = @ProductName;";
                SqlCommand command = new SqlCommand(commandString, connection);
                command.Parameters.AddWithValue("@ProductName", listBox1.Text);

                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    int id = (int)reader["Id_product"];
                    string name = (string)reader["product_Name"];
                    int price = (int)reader["price"];

                    txtname.Text = name;
                    txtprice.Text = price.ToString();
                }
                else
                {
                    txtname.Text = string.Empty;
                    txtprice.Text = string.Empty;
                }

                reader.Close();
            }
        }
        private void confirm_Click(object sender, EventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string commandString = "INSERT INTO Sales (SaleDate, item_Name, price, Quantity, Total) VALUES (@SaleDate, @ProductName, @price, @Quantity, @Total)";
                SqlCommand command = new SqlCommand(commandString, connection);
                DateTime saleDate;
                if (DateTime.TryParse(dateTime.Text, out saleDate))
                {
                    string formattedDate = saleDate.ToString("yyyy-MM-dd HH:mm:ss");

                    command.Parameters.AddWithValue("@SaleDate", formattedDate);
                }
                else
                {
                    MessageBox.Show("Invalid date format.");
                    return;
                }
                command.Parameters.AddWithValue("@ProductName", txtname.Text);
                command.Parameters.AddWithValue("@price", txtprice.Text);
                command.Parameters.AddWithValue("@Quantity", numericUpDown1.Text);
                command.Parameters.AddWithValue("@Total", totalp.Text);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    MessageBox.Show("The product has been added successfully");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred while adding the product: " + ex.Message);
                }
                finally
                {
                    connection.Close();
                }
            }

            fillListbox();
            txtname.Text = "";
            txtprice.Text = "";
            totalp.Text = "";
        }
        private void cancel_Click(object sender, EventArgs e)
        {
            txtname.Clear();
            txtprice.Clear();
            totalp.Clear();
        }
        private void fillBy1ToolStripButton_Click(object sender, EventArgs e)
        {
            try
            {
                this.itemsTableAdapter.
                    FillBy1(this.projectDataSet.Items);
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }

        }

        private void listBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string commandString = $"SELECT * FROM Items WHERE product_name = @ProductName;";
                SqlCommand command = new SqlCommand(commandString, connection);
                command.Parameters.AddWithValue("@ProductName", listBox1.Text);

                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    int id = (int)reader["Id_product"];
                    string name = (string)reader["product_Name"];
                    int price = (int)reader["price"];

                    txtname.Text = name;
                    txtprice.Text = price.ToString();
                }
                else
                {
                    txtname.Text = string.Empty;
                    txtprice.Text = string.Empty;
                }

                reader.Close();
            }
        }
    }
}
