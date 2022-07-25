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
        //SqlConnection con = new SqlConnection(@"Data Source=localhost;Initial Catalog=pos;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
        //ObservableCollection<Orders> model = new ObservableCollection<Orders>();
        List<Orders> model = new List<Orders>();


        int order_id;
        
        public void loadProducts()
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM tblproduct INNER JOIN tblcategory ON tblcategory.category_id=tblproduct.category_id", con);
            DataTable dt = new DataTable();
            con.Open();
            SqlDataReader sdr = cmd.ExecuteReader();
            dt.Load(sdr);
            con.Close();
            products.ItemsSource = dt.DefaultView;
        }
        public void loadOrders()
        {
            //orders.ItemsSource = model;
            //CollectionViewSource.GetDefaultView(orders.ItemsSource).Refresh();
            orders.ItemsSource = null;
            orders.ItemsSource = model;

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
                if (totalv <= cashv)
                    balance.Text = (cashv - totalv).ToString();
                else if (totalv > cashv)
                    MessageBox.Show("Invalid payment amount.");



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

                    total.Text = null;
                    cash.Text = null;
                    balance.Text = null;
                    orders.ItemsSource = null;
                }
            }
            catch
            {
                MessageBox.Show("Error submit");
            }
            finally
            {
                con.Close();
                loadProducts();
            }
        }

        private void btnCancelClick(object sender, RoutedEventArgs e)
        {
            model.Clear();
        }

        private void btnPrintClick(object sender, RoutedEventArgs e)
        {
            try
            {
                const int FIRST_COL_PAD = 20;
                const int SECOND_COL_PAD = 7;
                const int THIRD_COL_PAD = 20;

                var sb = new StringBuilder();
                //sb.AppendLine("Start of receipt");
                //sb.AppendLine("================");
                sb.AppendLine("=============================");

                foreach (var item in model)
                {
                    sb.Append(item.product_name.PadRight(FIRST_COL_PAD));

                    //var breakDown = item.product_quantity > 0 ? item.product_quantity + "x" + item.product_price : string.Empty;
                    var breakDown = item.quantity > 0 ? item.quantity + "x" + item.product_price : string.Empty;
                    sb.Append(breakDown.PadRight(SECOND_COL_PAD));

                    sb.AppendLine(string.Format("{0:0.00} A", item.total).PadLeft(THIRD_COL_PAD));

                    //if (item.Discount > 0)
                    //{
                    //    sb.Append(string.Format("DISCOUNT {0:D2}%", item.Discount).PadRight(FIRST_COL_PAD + SECOND_COL_PAD));
                    //    sb.Append(string.Format("{0:0.00} A", -(item.Total / 100 * item.Discount)).PadLeft(THIRD_COL_PAD));
                    //    sb.AppendLine();
                    //}
                }

                //sb.AppendLine("================");
                sb.AppendLine("=============================");
                sb.Append(System.Environment.NewLine);
                sb.AppendLine("TOTAL: " + total.Text);
                //sb.AppendLine(String.Format("TOTAL: {0.00.00}" + Convert.ToDecimal(total)));

                sb.Append(System.Environment.NewLine);
                sb.AppendLine("CASH: " + cash.Text);
                //sb.AppendLine(String.Format("CASH: {0.00.00}" + Convert.ToDecimal(cash)));

                sb.Append(System.Environment.NewLine);
                sb.AppendLine("CHANGE: " + balance.Text);
                //sb.AppendLine(String.Format("CHANGE: {0.00.00}" + Convert.ToDecimal(balance)));

                sb.Append(System.Environment.NewLine);
                sb.AppendLine("THANK YOU!");

                //MessageBox.Show(sb.ToString());

                ReceiptWindow rw = new ReceiptWindow(sb.ToString());
                //rw.DataContext = this;
                rw.ShowDialog();

                //PrintDialog printDlg = new PrintDialog();

                ////FlowDocument doc = new FlowDocument();

                //FlowDocument doc = new FlowDocument(new Paragraph(new Run("Starts...")));
                //doc.Name = "FlowDoc";

                //IDocumentPaginatorSource idpSource = doc;
                //printDlg.PrintDocument(idpSource.DocumentPaginator, "Hello WPF Printing.");
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error document, " + ex.Message);
            }
        }

        //private void btnRemoveClick(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        DataRowView row = orders.SelectedItem as DataRowView;
        //        string s = Convert.ToString(row.Row.ItemArray[1]);
        //        //if (row != null)
        //        //    // due to observable collection?
        //        //    // due to not connected to the clickevent of the datagrid orders
        //        //    MessageBox.Show("nul");

        //        //var order = model.FirstOrDefault(p => p.product_id == order_id);
        //        //if (order != null)
        //        //    model.Remove(order);




        //    }
        //    catch
        //    {
        //        MessageBox.Show("Error");
        //    }
        //}

        private void OrderDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                //var ord = orders.SelectedItems;

                //DataRowView row = orders.SelectedItem as DataRowView;
                //int pid = Convert.ToInt32(row.Row.ItemArray[0]);
                //MessageBox.Show(pid.ToString());

                foreach (var data in orders.SelectedItems)
                {
                    //Orders myData = data as Orders;
                    //MessageBox.Show(myData.product_name);
                    Orders myData = data as Orders;
                    int p_id = myData.product_id;
                    //MessageBox.Show(p_id.ToString());

                    var order = model.FirstOrDefault(p => p.product_id == p_id);
                    if (order != null)
                        model.Remove(order);
                    loadOrders();
                }
            }
            catch
            {

            }
        }

        private void btnSaveChanges(object sender, RoutedEventArgs e)
        {
            try
            {
                //foreach (DataGridViewRow i in orders.Rows)
                //{
                //    if (i.Cells[1].Value != null)
                //    {
                //        name.Add(i.Cells[1].Value.ToString());
                //    }

                //model.Clear();
                List<Orders> updatedList = new List<Orders>();
                foreach (Orders order in orders.ItemsSource)
                {
                    if (order != null)
                    {
                        //MessageBox.Show(order.product_id.ToString() + " " + order.product_name.ToString() + " " + order.product_price.ToString() + " " + order.quantity.ToString() + " "+ order.total.ToString());

                        updatedList.Add(new Orders
                        {
                            product_id = Convert.ToInt32(order.product_id),
                            product_name = order.product_name.ToString(),
                            product_price = Convert.ToDecimal(order.product_price),
                            quantity = Convert.ToInt32(order.quantity),
                            //total = Convert.ToDecimal(order.total),
                            total = Convert.ToDecimal(order.product_price * order.quantity),
                            product_quantity = Convert.ToInt32(order.product_quantity)
                        });
                        //MessageBox.Show("added");
                    }
                    //else
                    //{
                    //    MessageBox.Show("null");
                    //}
                }
                model.Clear();
                //model = updatedList;
                model = null;
                model = updatedList;
                MessageBox.Show("Changes saved.");
                total.Text = null;
                loadOrders();
            }
            catch
            {
                MessageBox.Show("Error1");
            }
        }
        private void exitApp(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
       
    }
}
