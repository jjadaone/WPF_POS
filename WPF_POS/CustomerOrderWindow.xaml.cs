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
using System.Collections.ObjectModel;
//using System.ComponentModel; //Inotifyproperchanged

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
            public string product_name { get; set; }
            public decimal product_price { get; set; }
            public int quantity { get; set; }
            public decimal total { get; set; }
            public int product_quantity { get; set; }


        }
        public CustomerOrderWindow()    
        {
            InitializeComponent();
            loadProducts();
            loadOrders();
        }

        SqlConnection con = new SqlConnection(@"Data Source=(localdb)\ProjectModels;Initial Catalog=pos;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
        ObservableCollection<Orders> model = new ObservableCollection<Orders>();
        int order_id;

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
            orders.ItemsSource = model;
            CollectionViewSource.GetDefaultView(orders.ItemsSource).Refresh();

        }

        private void ProductDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {

                DataRowView row = products.SelectedItem as DataRowView;
                int pid = Convert.ToInt32(row.Row.ItemArray[0]);


                var found = model.FirstOrDefault(x => x.product_id == pid);
                if (found != null)
                {
                    int i = model.IndexOf(found);
                    model[i].quantity += 1;
                    //MessageBox.Show("1", "pid:" + pid + "objp:" + model[i].product_id);
                    model[i].total = model[i].quantity * model[i].product_price;
                }
                else
                {
                    model.Add(new Orders
                    {
                        product_id = Convert.ToInt32(row.Row.ItemArray[0]),
                        product_name = row.Row.ItemArray[2].ToString(),
                        product_price = Convert.ToDecimal(row.Row.ItemArray[5]),
                        quantity = 1,
                        total = Convert.ToDecimal(row.Row.ItemArray[5]),
                        product_quantity = Convert.ToInt32(row.Row.ItemArray[4])
                    });
                }

                loadOrders();

            }
            catch
            {
                MessageBox.Show("Error", model.Count.ToString());
            }
        }
        
        private void btnCalculateClick(object sender, RoutedEventArgs e)
        {
            try
            {
                decimal counter = 0;
                foreach (var order in model)
                {
                    counter += order.total;

                }

                //foreach (DataRowView dr in orders.ItemsSource)
                //{
                //    counter += Convert.ToDecimal(dr[4]);
                //}

                total.Text = counter.ToString();
                
            }
            catch
            {
                MessageBox.Show("Error calculate");
            }
        }

        private void btnChangeClick(object sender, RoutedEventArgs e)
        {
            try
            {
                decimal totalv = Convert.ToDecimal(total.Text);
                decimal cashv = Convert.ToDecimal(cash.Text);
                balance.Text = (cashv - totalv).ToString();
            }
             catch
            {
                MessageBox.Show("Please input an amount.");
            }
        }

        private void btnSubmitClick(object sender, RoutedEventArgs e)
        {
            try
            {
                string sql = "INSERT INTO tblsalesorder(sales_order_date, total, cash, balance) VALUES (@sales_order_date, @total, @cash, @balance) SELECT SCOPE_IDENTITY()";
                string sql_add = "INSERT INTO tblsalesorderdetail(product_id, sales_order_id, sales_order_detail_quantity, sales_order_detail_price, sales_order_detail_total) VALUES (@product_id, @order_id, @detail_quantity, @detail_price, @detail_total)";
                string sql_dec = "UPDATE tblproduct SET product_quantity = product_quantity - @detail_quantity WHERE product_id = @product_id ";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.AddWithValue("@sales_order_date", DateTime.Now);
                cmd.Parameters.AddWithValue("@total", total.Text);
                cmd.Parameters.AddWithValue("@cash", cash.Text);
                cmd.Parameters.AddWithValue("@balance", balance.Text);
                con.Open();
                //MessageBox.Show(DateTime.Now.ToString() +" "+total.Text+" "+cash.Text+" "+balance.Text);
                int order_id = Convert.ToInt32(cmd.ExecuteScalar());
                cmd.Parameters.Clear();
                //MessageBox.Show(order_id.ToString());
                if (order_id != 0)
                {
                    foreach (var order in model)
                    {
                        cmd.CommandText = sql_add;
                        cmd.Parameters.AddWithValue("@product_id", order.product_id);
                        cmd.Parameters.AddWithValue("@order_id", order_id);
                        cmd.Parameters.AddWithValue("@detail_quantity", order.quantity);
                        cmd.Parameters.AddWithValue("@detail_price", order.product_price);
                        cmd.Parameters.AddWithValue("@detail_total", order.total);
                        //MessageBox.Show("in1 "+order.product_id.ToString()+" "+order_id+" "+order.quantity+" ");
                        cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();

                        //query for decreasing product quantities
                        int c_quantity = order.quantity >= order.product_quantity ? order.product_quantity : order.quantity;
                        cmd.CommandText = sql_dec;
                        cmd.Parameters.AddWithValue("@detail_quantity", c_quantity);
                        cmd.Parameters.AddWithValue("@product_id", order.product_id);
                        cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                    }
                    MessageBox.Show("Order successful");
                    model.Clear();
                }
            }
            catch
            {
                MessageBox.Show("Error submit");
            }
            finally
            {
                con.Close();
            }
        }

        private void btnCancelClick(object sender, RoutedEventArgs e)
        {
            model.Clear();
        }

        private void btnPrintClick(object sender, RoutedEventArgs e)
        {
            const int FIRST_COL_PAD = 20;
            const int SECOND_COL_PAD = 7;
            const int THIRD_COL_PAD = 20;

            var sb = new StringBuilder();
            sb.AppendLine("Start of receipt");
            sb.AppendLine("================");

            foreach (var item in model)
            {
                sb.Append(item.product_name.PadRight(FIRST_COL_PAD));

                var breakDown = item.product_quantity > 0 ? item.product_quantity + "x" + item.product_price : string.Empty;
                sb.Append(breakDown.PadRight(SECOND_COL_PAD));

                sb.AppendLine(string.Format("{0:0.00} A", item.total).PadLeft(THIRD_COL_PAD));

                //if (item.Discount > 0)
                //{
                //    sb.Append(string.Format("DISCOUNT {0:D2}%", item.Discount).PadRight(FIRST_COL_PAD + SECOND_COL_PAD));
                //    sb.Append(string.Format("{0:0.00} A", -(item.Total / 100 * item.Discount)).PadLeft(THIRD_COL_PAD));
                //    sb.AppendLine();
                //}
            }

            sb.AppendLine("================");
            sb.AppendLine("Total: " + total.Text);
            sb.AppendLine("THANK YOU!");

            MessageBox.Show(sb.ToString());

        }

        private void btnRemoveClick(object sender, RoutedEventArgs e)
        {
            try
            {
                DataRowView row = products.SelectedItem as DataRowView;
                int order_id = Convert.ToInt32(row.Row.ItemArray[0]);

                model.RemoveAt(order_id);
            }
            catch
            {
                MessageBox.Show("Error");
            }
        }
    }
}
