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
using System.Windows.Shapes;
using System.Data.SqlClient;
using System.Data;

namespace WPF_POS
{
    /// <summary>
    /// Interaction logic for CustomerOrderWindow.xaml
    /// </summary>
    public partial class CustomerOrderWindow : Window
    {
        public class Orders
        {
            public int product_id { get; set; }
            public int category_id { get; set; }
            public string product_name { get; set; }
        }
        public CustomerOrderWindow()
        {
            InitializeComponent();
            loadProducts();
        }

        SqlConnection con = new SqlConnection(@"Data Source=(localdb)\ProjectModels;Initial Catalog=pos;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
        List<Orders> model = new List<Orders>();
        public void loadProducts()
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM tblproduct", con);
            DataTable dt = new DataTable();
            con.Open();
            SqlDataReader sdr = cmd.ExecuteReader();
            dt.Load(sdr);
            con.Close();
            products.ItemsSource = dt.DefaultView;

        }

        public void loadOrders()
        {
            DataTable dt = new DataTable();
            //dt.Load()
        }

        private void ProductDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {

                DataRowView row = products.SelectedItem as DataRowView;
                model.Add(new Orders
                {
                    product_id = Convert.ToInt32(row.Row.ItemArray[0]),
                    category_id = Convert.ToInt32(row.Row.ItemArray[1]),
                    product_name = row.Row.ItemArray[2].ToString(),
                });

                //DataRowView row = products.SelectedItem as DataRowView;
                //string cid = row.Row.ItemArray[1].ToString();
                //string pn = row.Row.ItemArray[2].ToString();
                //MessageBox.Show(cid, pn);

            }
            catch
            {
                MessageBox.Show("Error");
            }
        }

        private void btnCalculateClick(object sender, RoutedEventArgs e)
        {

        }

        private void btnChangeClick(object sender, RoutedEventArgs e)
        {

        }

        private void btnSubmitClick(object sender, RoutedEventArgs e)
        {

        }

        private void btnCancelClick(object sender, RoutedEventArgs e)
        {

        }
    }


}
