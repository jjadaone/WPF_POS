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
    /// Interaction logic for SalesReport.xaml
    /// </summary>
    public partial class SalesReport : Page
    {
        public SalesReport()
        {
            InitializeComponent();
        
        }


        //SqlConnection con = new SqlConnection(@"Data Source=(localdb)\ProjectModels;Initial Catalog=pos;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
        SqlConnection con = new SqlConnection(@"Data Source=localhost;Initial Catalog=pos;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");

        //SELECT* FROM tblrefund as r INNER JOIN tblpurchaseorder as po ON po.purchase_order_id = r.purchase_order_id INNER JOIN tblproduct as p ON po.product_id = p.product_id WHERE r.status = 'Returned' 

        //SELECT r.refund_id, refund_quantity, (refund_quantity* p.product_price) as total, [created_at] FROM tblrefund as r INNER JOIN tblpurchaseorder as po ON po.purchase_order_id = r.purchase_order_id INNER JOIN tblproduct as p ON po.product_id = p.product_id WHERE r.status = 'Returned' 

        //SELECT r.refund_id, refund_quantity, (refund_quantity* p.product_price) as total, [created_at] FROM tblrefund as r INNER JOIN tblpurchaseorder as po ON po.purchase_order_id = r.purchase_order_id INNER JOIN tblproduct as p ON po.product_id = p.product_id WHERE r.status = 'Returned' AND r.created_at BETWEEN '7/18/2022' AND '7/23/2022'

        //SELECT ifnull(sum(cash), 0) from tblpay where week(sdate)=week(now())



        private void show_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //sales.Items.Clear();
                DateTime? toDate = to.SelectedDate;
                DateTime? fromDate = from.SelectedDate;

                if (toDate.HasValue && fromDate.HasValue)
                {
                    //MessageBox.Show(toDate.ToString());
                    //SqlCommand cmd = new SqlCommand("SELECT * FROM tblsalesorderdetail WHERE date BETWEEN @from AND @to", con);
                    SqlCommand cmd = new SqlCommand("SELECT so.sales_order_id AS INVOICE, p.product_name AS PRODUCT, sod.sales_order_detail_quantity AS QUANTITY, so.sales_order_date AS DATE, sod.sales_order_detail_price AS PRICE, sod.sales_order_detail_total AS TOTAL  FROM tblsalesorderdetail as sod INNER JOIN tblsalesorder AS so ON so.sales_order_id=sod.sales_order_id INNER JOIN tblproduct as p ON p.product_id=sod.product_id WHERE so.sales_order_date BETWEEN @from AND @to", con);
                    cmd.Parameters.AddWithValue("@from", fromDate);
                    cmd.Parameters.AddWithValue("@to", toDate);
                    DataTable dt = new DataTable();
                    con.Open();
                    SqlDataReader sdr = cmd.ExecuteReader();
                    cmd.Parameters.Clear();
                    dt.Load(sdr);
                    con.Close();
                    sales.ItemsSource = dt.DefaultView;

                    cmd.CommandText = "SELECT SUM(sales_order_detail_total) as total FROM tblsalesorderdetail INNER JOIN tblsalesorder AS so ON so.sales_order_id = tblsalesorderdetail.sales_order_id WHERE so.sales_order_date BETWEEN @from AND @to";
                    cmd.Parameters.AddWithValue("@from", fromDate);
                    cmd.Parameters.AddWithValue("@to", toDate);
                    con.Open();
                    //MessageBox.Show("out");
                    var reader = cmd.ExecuteReader();
                    int totalSales = reader.GetOrdinal("total");
                    if (!reader.Read())
                        throw new InvalidOperationException("No records");

                    total.Content = reader.GetDecimal(totalSales);
                    //MessageBox.Show(reader.GetDecimal(totalSales).ToString());
                    con.Close();


                }
                else
                {
                    MessageBox.Show("Err");
                }
            }
            catch
            {
                MessageBox.Show("Error occured");
            }

        }
    }
}
