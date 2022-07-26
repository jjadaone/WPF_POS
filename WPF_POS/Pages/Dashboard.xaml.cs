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
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class Dashboard : Page
    {
        public Dashboard()
        {
            InitializeComponent();
            lblSales();
            lblStocks();
            lblReturned();
            lblExchanged();
        }

        //SqlConnection con = new SqlConnection(@"Data Source=(localdb)\ProjectModels;Initial Catalog=pos;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
        SqlConnection con = new SqlConnection(@"Data Source=localhost;Initial Catalog=pos;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");

        public void lblSales()
        {
            string sql = "SELECT SUM(sales_order_detail_total) as total FROM tblsalesorderdetail";
            SqlCommand cmd = new SqlCommand(sql, con);
            con.Open();
            Object total = cmd.ExecuteScalar();
            lblsales.Content = total.ToString();
            con.Close();
        }

        public void lblStocks()
        {
            string sql = "SELECT SUM(product_quantity) as total FROM tblproduct";
            SqlCommand cmd = new SqlCommand(sql, con);
            con.Open();
            Object total = cmd.ExecuteScalar();
            lblstocks.Content = total.ToString();
            con.Close();
        }

        public void lblReturned()
        {
            string sql = "SELECT SUM(refund_quantity*p.product_price) as total FROM tblrefund as r INNER JOIN tblpurchaseorder as po ON po.purchase_order_id = r.purchase_order_id INNER JOIN tblproduct as p ON po.product_id = p.product_id WHERE r.status = 'Returned' ";
            SqlCommand cmd = new SqlCommand(sql, con);
            con.Open();
            Object total = cmd.ExecuteScalar();
            lblreturned.Content = total.ToString();
            con.Close();
        }

        public void lblExchanged()
        {
            string sql = "SELECT SUM(refund_quantity*p.product_price) as total FROM tblrefund as r INNER JOIN tblpurchaseorder as po ON po.purchase_order_id = r.purchase_order_id INNER JOIN tblproduct as p ON po.product_id = p.product_id WHERE r.status = 'Exchanged' ";
            SqlCommand cmd = new SqlCommand(sql, con);
            con.Open();
            Object total = cmd.ExecuteScalar();
            lblexchanged.Content = total.ToString();
            con.Close();
        }
    }
}
