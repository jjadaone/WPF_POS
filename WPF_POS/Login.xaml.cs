using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data.SqlClient;
using System.Data;


namespace WPF_POS
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        SqlConnection con = new SqlConnection(@"Data Source=(localdb)\ProjectModels;Initial Catalog=pos;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
        CustomerOrderWindow sales = new CustomerOrderWindow();
        private void btnLoginClick(object sender, RoutedEventArgs e)
        {
            try
            {
                string sql = @"SELECT * FROM tbluser WHERE username=@username AND password=@password";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@username", username.Text);
                cmd.Parameters.AddWithValue("@password", password.Text);
                con.Open();
                //MessageBox.Show(username.Text, password.Text);

                //cmd.ExecuteNonQuery();
                int user_id = Convert.ToInt32(cmd.ExecuteScalar());


                if (user_id != 0)
                {
                    this.Close();
                    sales.Show();
                }
                else
                {
                    MessageBox.Show("Invalid username and password");
                }


            }
            catch
            {
                MessageBox.Show("Error login");
            }
            finally
            {
                con.Close();
            }
        }
    }
}
