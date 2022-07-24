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
using System.Text.RegularExpressions;

namespace WPF_POS.Pages
{
    /// <summary>
    /// Interaction logic for SupplierForm.xaml
    /// </summary>
    public partial class SupplierForm : Page
    {
        public SupplierForm()
        {
            InitializeComponent();
            loadData();
            SID_combo();
        }
        //SqlConnection con = new SqlConnection(@"Data Source=localhost;Initial Catalog=pos;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
        SqlConnection con = new SqlConnection(@"Data Source=(localdb)\ProjectModels;Initial Catalog=pos;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");

        public void loadData()
        {
            SqlCommand cmd = new SqlCommand("SELECT supplier_id AS 'SUPPLIER ID', supplier_name AS 'SUPPLIER NAME', supplier_address AS 'ADDRESS' FROM tblsupplier", con);
            DataTable dt = new DataTable();
            con.Open();
            SqlDataReader sdr = cmd.ExecuteReader();
            dt.Load(sdr);
            con.Close();
            DataGrid.ItemsSource = dt.DefaultView;
        }
        public void clearData()
        {
            txtSname.Clear();
            txtScontact.Clear();
            txtSaddress.Clear();
        }
        public bool isValid()
        {
            if (txtSname.Text == String.Empty)
            {
                MessageBox.Show("Supplier name is required", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (txtScontact.Text == String.Empty)
            {
                MessageBox.Show("Supplier contact is required", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (txtSaddress.Text == String.Empty)
            {
                MessageBox.Show("Supplier address is required", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }
        private void txtScontact_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);
        }

        private void AddSuppBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (isValid())
                {
                    SqlCommand cmd = new SqlCommand("INSERT INTO tblsupplier VALUES (@supplier_name, @contact_number, @supplier_address)", con);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@supplier_name", txtSname.Text);
                    cmd.Parameters.AddWithValue("@contact_number", txtScontact.Text);
                    cmd.Parameters.AddWithValue("@supplier_address", txtSaddress.Text);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                    MessageBox.Show("Successfully Entered", "Saved", MessageBoxButton.OK, MessageBoxImage.Information);
                    loadData();
                    clearData();
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void DeleteSuppBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataRowView row = DataGrid.SelectedItem as DataRowView;
                string id = row.Row.ItemArray[0].ToString();

                SqlCommand cmd = new SqlCommand("DELETE FROM tblsupplier WHERE supplier_id=" + id, con);
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
                MessageBox.Show("Deletion Error" + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        private void ComboBox_DropDownClosed(object sender, EventArgs e)
        {
            

        }
        void SID_combo()
        {
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("select * FROM tblsupplier", con);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    string sid = dr.GetInt32(0).ToString();
                    CBSID.Items.Add(sid);




                }
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void CBSID_DropDownClosed(object sender, EventArgs e)
        {
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("select * FROM tblsupplier where supplier_id ='" + CBSID.Text + "' ", con);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {

                    string suppname = dr.GetString(1);
                    txtSname.Text = suppname;
                    string suppnum = dr.GetString(2);
                    txtScontact.Text = suppnum;
                    string suppadd = dr.GetString(3);
                    txtSaddress.Text = suppadd;
                }
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void UpdateSuppBtn_Click(object sender, RoutedEventArgs e)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("update tblsupplier set supplier_name = '" + txtSname.Text + "', contact_number = '" + txtScontact.Text + "', supplier_address = '" + txtSaddress.Text + "' WHERE supplier_id = '" + CBSID.Text + "' ", con);

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
    }
}
