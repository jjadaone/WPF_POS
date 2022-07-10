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
        }

        SqlConnection con = new SqlConnection(@"Data Source=(localdb)\ProjectModels;Initial Catalog=pos;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");

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
            user_id.Clear();
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
            try
            {
                string sql = "UPDATE tbluser SET user_id=@user_id, fullname=@fullname, username=@username, password=@password, role=@role WHERE user_id=@user_id";
                //string sql = "UPDATE employee SET emp_name='" + emp_name.Text + "', emp_age='" + emp_age.Text + "', emp_salary='"+emp_salary.Text+"', join_date='"+join_date.Text+"', phone='"+phone.Text+"' WHERE id='"+emp_id.Text+"' ";
                SqlCommand cmd = new SqlCommand(sql, con);

                cmd.Parameters.AddWithValue("@user_id", user_id.Text);
                cmd.Parameters.AddWithValue("@fullname", fullname.Text);
                cmd.Parameters.AddWithValue("@username", username.Text);
                cmd.Parameters.AddWithValue("@password", password.Text);
                cmd.Parameters.AddWithValue("@role", role.SelectionBoxItem.ToString());

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                MessageBox.Show("Data updated successfully!");
                loadData();
                clearData();
            }
            catch
            {
                MessageBox.Show("Error updating the data.");
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



        //private void clearBtn(object sender, RoutedEventArgs e)
        //{
        //    clearData();
        //}

    }
}
