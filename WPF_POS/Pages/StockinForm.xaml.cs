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
    /// Interaction logic for StockinForm.xaml
    /// </summary>
    public partial class StockinForm : Page
    {
        public StockinForm()
        {
            InitializeComponent();
            loadOrders();
            loadReceived();
        }

        SqlConnection con = new SqlConnection(@"Data Source=localhost;Initial Catalog=pos;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
        //SqlConnection con = new SqlConnection(@"Data Source=(localdb)\ProjectModels;Initial Catalog=pos;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");

        public void loadOrders()
        {
            string sql = "SELECT purchase_order_id AS 'PURCHASE ID', supplier_name AS SUPPLIER, tblproduct.product_id AS 'PRODUCT', product_name AS 'NAME', order_date AS 'ORDER DATE', purchase_order_quantity AS QUANTITY, purchase_order_total AS TOTAL  FROM tblpurchaseorder INNER JOIN tblsupplier ON tblsupplier.supplier_id=tblpurchaseorder.supplier_id INNER JOIN tblproduct ON tblproduct.product_id=tblpurchaseorder.product_id WHERE tblpurchaseorder.status = 'Ordered'";
            SqlCommand cmd = new SqlCommand(sql, con);
            DataTable dt = new DataTable();
            con.Open();
            SqlDataReader sdr = cmd.ExecuteReader();
            dt.Load(sdr);
            con.Close();
            orders.ItemsSource = dt.DefaultView;
        }

        public void loadReceived()
        {
            string sql = "SELECT supplier_name AS SUPPLIER, product_name AS NAME, order_date AS 'ORDER DATE', received_date AS 'RECEIVED DATE', purchase_order_quantity AS QUANTITY, purchase_order_total AS TOTAL  FROM tblpurchaseorder INNER JOIN tblsupplier ON tblsupplier.supplier_id=tblpurchaseorder.supplier_id INNER JOIN tblproduct ON tblproduct.product_id=tblpurchaseorder.product_id WHERE tblpurchaseorder.status = 'Received'";
            SqlCommand cmd = new SqlCommand(sql, con);
            DataTable dt = new DataTable();
            con.Open();
            SqlDataReader sdr = cmd.ExecuteReader();
            dt.Load(sdr);
            con.Close();
            received.ItemsSource = dt.DefaultView;
        }

        private void btnSaveClick(object sender, RoutedEventArgs e)
        {
            try
            {
                DataRowView row = orders.SelectedItem as DataRowView;
                string purchase_id = row.Row.ItemArray[0].ToString();
                string product_id = row.Row.ItemArray[2].ToString();
                string quantity = row.Row.ItemArray[5].ToString();
                MessageBox.Show(product_id, quantity);

                string sql = @"UPDATE tblpurchaseorder SET status=@status, received_date=@received_date WHERE purchase_order_id=@purchase_id";
                SqlCommand cmd = new SqlCommand(sql, con);

                cmd.Parameters.AddWithValue("@status", "Received");
                cmd.Parameters.AddWithValue("@received_date", DateTime.Now);
                cmd.Parameters.AddWithValue("@purchase_id", purchase_id);
                con.Open();
                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();

                cmd.CommandText = @"UPDATE tblproduct SET product_quantity=product_quantity+@received_quantity WHERE product_id=@product_id";
                cmd.Parameters.AddWithValue("@received_quantity", quantity);
                cmd.Parameters.AddWithValue("@product_id", product_id);
                cmd.ExecuteNonQuery();
                con.Close();
                MessageBox.Show("Order received.");
                // needs refreshing the datagrids
                loadOrders();
                loadReceived();

            }
            catch
            {
                MessageBox.Show("Error receiving order.");
            }
        }

        private void btnClearClick(object sender, RoutedEventArgs e)
        {

        }
    }
}
