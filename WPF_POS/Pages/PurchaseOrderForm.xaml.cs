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
    /// Interaction logic for PurchaseOrderForm.xaml
    /// </summary>
    public partial class PurchaseOrderForm : Page
    {
        public PurchaseOrderForm()
        {
            InitializeComponent();
            loadData();
            CBPName_combo();
            CBSName_combo();
            CBPOID_combo();
        }
        SqlConnection con = new SqlConnection(@"Data Source=localhost;Initial Catalog=pos;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");

        public void loadData()
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM tblpurchaseorder", con);
            DataTable dt = new DataTable();
            con.Open();
            SqlDataReader sdr = cmd.ExecuteReader();
            dt.Load(sdr);
            con.Close();
            DataGrid.ItemsSource = dt.DefaultView;
        }
        public void clearData()
        {
            CBPName.Items.Clear();
            CBSname.Items.Clear();
            txtOrderQuantity.Clear();
            txtOrderStatus.Clear();
            txtOrderTotal.Clear();
            txtProductID.Clear();
            txtSuppID.Clear();
          
        }
        public bool isValid()
        {
            if (CBPName.Text == String.Empty)
            {
                MessageBox.Show("Product name is required", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (CBSname.Text == String.Empty)
            {
                MessageBox.Show("Supplier name is required", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (txtOrderQuantity.Text == String.Empty)
            {
                MessageBox.Show("Order Quantity is required", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (txtOrderStatus.Text == String.Empty)
            {
                MessageBox.Show("Order Status is required", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (txtOrderTotal.Text == String.Empty)
            {
                MessageBox.Show("Order Total is required", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (OrderDate.Text == String.Empty)
            {
                MessageBox.Show("Order Total is required", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
           
            return true;
        }
        private void txtOrderQuantity_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);
        }

        private void AddOrderBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (isValid())
                {
                    SqlCommand cmd = new SqlCommand("INSERT INTO tblpurchaseorder VALUES (@product_id, @supplier_id, @purchase_order_quantity, @purchase_order_total, @status, @order_date, @received_date)", con);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@product_id", txtProductID.Text);
                    cmd.Parameters.AddWithValue("@supplier_id", txtSuppID.Text);
                    cmd.Parameters.AddWithValue("@purchase_order_quantity", txtOrderQuantity.Text);
                    cmd.Parameters.AddWithValue("@purchase_order_total", txtOrderTotal.Text);
                    cmd.Parameters.AddWithValue("@status", txtOrderStatus.Text);
                    cmd.Parameters.AddWithValue("@order_date", OrderDate.Text);
                    cmd.Parameters.AddWithValue("@received_date", ReceivedDate.Text);
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
        void CBPName_combo()
        {
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("select * FROM tblproduct", con);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    string pname = dr.GetString(2);
                    CBPName.Items.Add(pname);
                }
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        void CBSName_combo()
        {
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("select * FROM tblsupplier", con);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    string sname = dr.GetString(1);
                    CBSname.Items.Add(sname);
                }
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void CBPName_DropDownClosed(object sender, EventArgs e)
        {
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("select * FROM tblproduct where product_name ='" + CBPName.Text + "' ", con);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {

                    string pid = dr.GetInt32(0).ToString();
                    txtProductID.Text = pid;
                }
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void CBSname_DropDownClosed(object sender, EventArgs e)
        {
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("select * FROM tblsupplier where supplier_name ='" + CBSname.Text + "' ", con);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {

                    string sid = dr.GetInt32(0).ToString();
                    txtSuppID.Text = sid;
                }
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void DeleteOrderBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataRowView row = DataGrid.SelectedItem as DataRowView;
                string id = row.Row.ItemArray[0].ToString();

                SqlCommand cmd = new SqlCommand("DELETE FROM tblpurchaseorder WHERE purchase_order_id=" + id, con);
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
        void CBPOID_combo()
        {
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("select * FROM tblpurchaseorder", con);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    string poid = dr.GetInt32(0).ToString();
                    CBPOID.Items.Add(poid);
                }
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void CBPOID_DropDownClosed(object sender, EventArgs e)
        {
           
        }

        private void CBPOID_DropDownClosed_1(object sender, EventArgs e)
        {
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("select * FROM tblpurchaseorder where purchase_order_id ='" + CBPOID.Text + "' ", con);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {

                    string pid = dr.GetInt32(1).ToString();
                    txtProductID.Text = pid;

                    string sid = dr.GetInt32(2).ToString();
                    txtSuppID.Text = sid;

                    string oq = dr.GetInt32(3).ToString();
                    txtOrderQuantity.Text = oq;

                    string ot = dr.GetInt32(4).ToString();
                    txtOrderTotal.Text = ot;

                    string s = dr.GetString(5);
                    txtOrderStatus.Text = s;

                    string od = dr.GetDateTime(6).ToString();
                    OrderDate.Text = od;

                    string rd = dr.GetDateTime(7).ToString();
                    ReceivedDate.Text = rd;
                }
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }

        private void UpdateOrderBtn_Click(object sender, RoutedEventArgs e)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("update tblpurchaseorder set product_id = '" + txtProductID.Text + "', supplier_id = '" + txtSuppID.Text + "', purchase_order_total = '" + txtOrderTotal.Text + "', status = '" + txtOrderStatus.Text + "', order_date = '" + OrderDate.Text + "', received_date = '" + ReceivedDate.Text + "' WHERE purchase_order_id = '" + CBPOID.Text + "' ", con);

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
