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
    /// Interaction logic for CategoryForm.xaml
    /// </summary>
    public partial class CategoryForm : Page
    {
        public CategoryForm()
        {
            InitializeComponent();
            loadData();
            CID_Combo();
        }
        SqlConnection con = new SqlConnection(@"Data Source=localhost;Initial Catalog=pos;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");

        public void loadData()
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM tblcategory", con);
            DataTable dt = new DataTable();
            con.Open();
            SqlDataReader sdr = cmd.ExecuteReader();
            dt.Load(sdr);
            con.Close();
            DataGrid.ItemsSource = dt.DefaultView;

        }
        public void clearData()
        {
            txtCategory.Clear();
            txtDesc.Clear();
           

        }

        public bool isValid()
        {
            if (txtCategory.Text == String.Empty)
            {
                MessageBox.Show("Category Name is required", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (txtDesc.Text == String.Empty)
            {
                MessageBox.Show("Category Description is required", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }

        private void AddDescBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (isValid())
                {
                    SqlCommand cmd = new SqlCommand("INSERT INTO tblcategory VALUES (@category_name, @category_desc)", con);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@category_name", txtCategory.Text);
                    cmd.Parameters.AddWithValue("@category_desc", txtDesc.Text);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                    loadData();
                    MessageBox.Show("Successfully Entered", "Saved", MessageBoxButton.OK, MessageBoxImage.Information);
                    clearData();
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        

        private void deleteBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataRowView row = DataGrid.SelectedItem as DataRowView;
                string id = row.Row.ItemArray[0].ToString();

                SqlCommand cmd = new SqlCommand("DELETE FROM tblcategory WHERE category_id=" + id, con);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                MessageBox.Show("Deleted successfully");
                loadData();
                clearData();
                con.Close();

            }
            catch (SqlException ex)
            {
                MessageBox.Show("Not Deleted" + ex.Message);
            }
            finally
            {
                con.Close();
            }
            //int id = dataGridBasket.SelectedIndex;
            //DataRowView row = dataGridBasket.SelectedItem as DataRowView;
            //MessageBox.Show(id.ToString(), row.Row.ItemArray[0].ToString());


        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("update tblcategory set category_name = '" + txtCategory.Text + "', category_desc = '" + txtDesc.Text + "' WHERE category_id = '" + CBCatID.Text + "' ", con);

            try
            {
                cmd.ExecuteNonQuery();
                MessageBox.Show("Category updated successfully", "Updated", MessageBoxButton.OK, MessageBoxImage.Information);

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

        /* private void Button_Click(object sender, RoutedEventArgs e)
         {

             con.Open();
             SqlCommand cmd = new SqlCommand("update tblcategory set category_name = '" + txtCategory.Text + "', category_desc = '" + txtDesc.Text + "' WHERE category_id = '" + txtCategoryid.Text + "' ", con);

             try
             {
                 cmd.ExecuteNonQuery();
                 MessageBox.Show("Category updated successfully", "Updated", MessageBoxButton.OK, MessageBoxImage.Information);

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
        */
        private void CBCatID_DropDownClosed(object sender, EventArgs e)
        {
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("select * FROM tblcategory where category_id ='" + CBCatID.Text + "' ", con);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    string cname = dr.GetString(1);
                    txtCategory.Text = cname;

                    string catdesc = dr.GetString(2);
                    txtDesc.Text = catdesc;
      
                }
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void CID_Combo()
        {
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("select * FROM tblcategory", con);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    string pid = dr.GetInt32(0).ToString();
                    CBCatID.Items.Add(pid);



                }
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
    }
}
