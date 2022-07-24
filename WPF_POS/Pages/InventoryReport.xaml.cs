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
    /// Interaction logic for InventoryReport.xaml
    /// </summary>
    public partial class InventoryReport : Page
    {
        public InventoryReport()
        {
            InitializeComponent();
            loadInventory();
        }

        SqlConnection con = new SqlConnection(@"Data Source=(localdb)\ProjectModels;Initial Catalog=pos;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
        //SqlConnection con = new SqlConnection(@"Data Source=localhost;Initial Catalog=pos;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");

        public void loadInventory()
        {
            //string sql = "SELECT p.product_name AS 'PRODUCT NAME', refund_quantity AS QUANTITY, (refund_quantity* p.product_price) AS TOTAL, r.status AS STATUS, [created_at] AS DATE FROM tblrefund as r INNER JOIN tblpurchaseorder as po ON po.purchase_order_id = r.purchase_order_id INNER JOIN tblproduct as p ON po.product_id = p.product_id ORDER BY status asc, r.refund_id desc";
            string sql = "SELECT p.product_name AS 'PRODUCT NAME', refund_quantity AS QUANTITY, (refund_quantity* p.product_price) AS TOTAL, r.status AS STATUS, [created_at] AS DATE FROM tblrefund as r INNER JOIN tblpurchaseorder as po ON po.purchase_order_id = r.purchase_order_id INNER JOIN tblproduct as p ON po.product_id = p.product_id";
            SqlCommand cmd = new SqlCommand(sql, con);
            //cmd.Parameters.AddWithValue("@to", toDate);
            DataTable dt = new DataTable();
            con.Open();
            SqlDataReader sdr = cmd.ExecuteReader();
            cmd.Parameters.Clear();
            dt.Load(sdr);
            con.Close();
            datagrid.ItemsSource = dt.DefaultView;
        }


    }
}
