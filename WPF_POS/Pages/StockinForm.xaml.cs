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

        public void loadOrders()
        {
            string sql = "SELECT purchase_order_id, supplier_name, product_name, order_date, purchase_order_quantity, purchase_order_total  FROM tblpurchaseorder INNER JOIN tblsupplier ON tblsupplier.supplier_id=tblpurchaseorder.supplier_id INNER JOIN tblproduct ON tblproduct.product_id=tblpurchaseorder.product_id WHERE tblpurchaseorder.status = 'Ordered'";
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
            string sql = "SELECT supplier_name, product_name, order_date, received_date, purchase_order_quantity, purchase_order_total  FROM tblpurchaseorder INNER JOIN tblsupplier ON tblsupplier.supplier_id=tblpurchaseorder.supplier_id INNER JOIN tblproduct ON tblproduct.product_id=tblpurchaseorder.product_id WHERE tblpurchaseorder.status = 'Received'";
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

                string sql = @"UPDATE tblpurchaseorder SET status=@status, received_date=@received_date WHERE purchase_order_id=@purchase_id";
                SqlCommand cmd = new SqlCommand(sql, con);

                cmd.Parameters.AddWithValue("@status", "Received");
                cmd.Parameters.AddWithValue("@received_date", DateTime.Now);
                cmd.Parameters.AddWithValue("@purchase_id", purchase_id);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                MessageBox.Show("Order received.");
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
