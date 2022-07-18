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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SqlClient;
using System.Data;

namespace WPF_POS.Pages
{
    /// <summary>
    /// Interaction logic for UserForm.xaml
    /// </summary>
    public partial class UserForm : Page
    {
        public UserForm()
        {
            InitializeComponent();
            loadData();
            fill_combo();
        }

        SqlConnection con = new SqlConnection(@"Data Source=localhost;Initial Catalog=pos;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");

        public void loadData()
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM tbluser", con);
            DataTable dt = new DataTable();
            con.Open();
            SqlDataReader sdr = cmd.ExecuteReader();
            dt.Load(sdr);
            con.Close();
            dataGridBasket.ItemsSource = dt.DefaultView;

        }

        public void clearData()
        {
            CBUID.Items.Clear();
            fullname.Clear();
            username.Clear();
            password.Clear();
           
        }

        private void insertBtn(object sender, RoutedEventArgs e)
        {
            try
            {
                SqlCommand cmd = new SqlCommand(@"INSERT INTO tbluser(fullname, username, password, role) VALUES (@fullname, @username, @password, @role)", con);
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.AddWithValue("@fullname", fullname.Text);
                cmd.Parameters.AddWithValue("@username", username.Text);
                cmd.Parameters.AddWithValue("@password", password.Text);
                cmd.Parameters.AddWithValue("@role", role.SelectionBoxItem.ToString());

                con.Open();

                cmd.ExecuteNonQuery();
                con.Close();
                loadData();
                clearData();
                MessageBox.Show("Data inserted successfully!");
            }
            catch
            {
                MessageBox.Show("Error");
            }
        }

        private void updateBtn(object sender, RoutedEventArgs e)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("update tbluser set fullname = '" + fullname.Text + "', username = '" + username.Text + "', password = '" + password.Text + "', role = '" + role.Text + "' WHERE user_id = '" + CBUID.Text + "' ", con);

            try
            {
                cmd.ExecuteNonQuery();
                MessageBox.Show("User updated successfully", "Updated", MessageBoxButton.OK, MessageBoxImage.Information);

            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                con.Close();
                clearData();
                loadData();


            }
        }

        private void deleteBtn(object sender, RoutedEventArgs e)
        {
            //int id = dataGridBasket.SelectedIndex;
            //DataRowView row = dataGridBasket.SelectedItem as DataRowView;
            //MessageBox.Show(id.ToString(), row.Row.ItemArray[0].ToString());

            DataRowView row = dataGridBasket.SelectedItem as DataRowView;
            string id = row.Row.ItemArray[0].ToString();

            SqlCommand cmd = new SqlCommand("DELETE FROM tbluser WHERE user_id=" + id, con);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
            MessageBox.Show("Deleted successfully");
            loadData();
            clearData();

        }
        void fill_combo()
        {
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("select * FROM tbluser", con);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    string uid = dr.GetInt32(0).ToString();
                    CBUID.Items.Add(uid);
                }
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void CBUID_DropDownClosed(object sender, EventArgs e)
        {
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("select * FROM tbluser where user_id ='" + CBUID.Text + "' ", con);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {

                    string uname = dr.GetString(4);
                    fullname.Text = uname;
                    string usname = dr.GetString(1);
                    username.Text = usname;
                    string pass = dr.GetString(2);
                    password.Text = pass;
                    string r = dr.GetString(3);
                    role.Text = r;
                }
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }



        //private void clearBtn(object sender, RoutedEventArgs e)
        //{
        //    clearData();
        //}

    }
}
