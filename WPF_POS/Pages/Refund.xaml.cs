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
    /// Interaction logic for Refund.xaml
    /// </summary>
    public partial class Refund : Page
    {
        public Refund()
        {
            InitializeComponent();
            fillPurchase();
            loadRefund();
            loadExchange();
        }

        SqlConnection con = new SqlConnection(@"Data Source=(localdb)\ProjectModels;Initial Catalog=pos;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");

        public void loadRefund()
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM tblrefund WHERE status='Returned'", con);
            DataTable dt = new DataTable();
            con.Open();
            SqlDataReader sdr = cmd.ExecuteReader();
            dt.Load(sdr);
            con.Close();
            dgrefund.ItemsSource = dt.DefaultView;
        }

        public void loadExchange()
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM tblrefund WHERE status='Exchanged'", con);
            DataTable dt = new DataTable();
            con.Open();
            SqlDataReader sdr = cmd.ExecuteReader();
            dt.Load(sdr);
            con.Close();
            dgexchange.ItemsSource = dt.DefaultView;
        }

        public void fillPurchase()
        {
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT purchase_order_id, product_name FROM tblpurchaseorder INNER JOIN tblproduct ON tblproduct.product_id=tblpurchaseorder.product_id WHERE status='Received' AND tblpurchaseorder.purchase_order_quantity>0", con);
                //SqlCommand cmd = new SqlCommand("SELECT po.purchase_order_id, product_name FROM tblpurchaseorder as po INNER JOIN tblproduct as pd ON pd.product_id=po.product_id INNER JOIN tblrefund as r ON r.purchase_order_id=po.purchase_order_id WHERE po.status='Received'", con);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    int id = dr.GetInt32(0);
                    //string text = dr.GetString(1);
                    purchase.Items.Add(id);
                }
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void cbPurchase_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                string purchase_id = purchase.SelectedItem.ToString();
                string sql = "SELECT pd.product_id, supplier_name, product_quantity, product_price, purchase_order_quantity, product_name FROM tblpurchaseorder AS po INNER JOIN tblproduct AS pd ON pd.product_id=po.product_id INNER JOIN tblsupplier as s ON s.supplier_id=po.supplier_id WHERE po.purchase_order_id=@purchase_order_id ";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("purchase_order_id", purchase_id);
                con.Open();


                var reader = cmd.ExecuteReader();
                cmd.Parameters.Clear();

                int p_id = reader.GetOrdinal("product_id");
                int supplier_name = reader.GetOrdinal("supplier_name");
                int p_price = reader.GetOrdinal("product_price");
                int po_quantity = reader.GetOrdinal("purchase_order_quantity");
                int p_name = reader.GetOrdinal("product_name");

                if (!reader.Read())
                    throw new InvalidOperationException("No records");

                supplier.Text = reader.GetString(supplier_name);
                if (exchange.IsChecked == true)
                {
                    int pd_id = reader.GetInt32(p_id);
                    decimal product_price = reader.GetDecimal(p_price);
                    refund_quantity.Text = reader.GetInt32(po_quantity).ToString();
                    reader.Close();
                    cmd.CommandText = "select product_id, product_name from tblproduct where product_price=@p_price and product_id!=@p_id";
                    cmd.Parameters.AddWithValue("@p_price", product_price);
                    cmd.Parameters.AddWithValue("@p_id", pd_id);
                    SqlDataReader sdr = cmd.ExecuteReader();
                    product.Items.Clear();
                    while (sdr.Read())
                    {
                        int id = sdr.GetInt32(0);
                        string name = sdr.GetString(1);
                        product.Items.Add(String.Format("{0} - {1}", id, name));
                    }
                }
                else if (refund.IsChecked == true)
                {
                    int id = reader.GetInt32(p_id);
                    string name = reader.GetString(p_name);
                    product.Items.Clear();
                    product.Items.Add(String.Format("{0} - {1}", id, name));
                }

            }
            catch
            {
                //MessageBox.Show("Error1");
            }
            finally
            {
                con.Close();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // for refund
                string purchase_id = purchase.SelectedItem.ToString();
                con.Open();

                if (refund.IsChecked == true)
                {
                    string sql_insert = "INSERT INTO tblrefund(purchase_order_id, refund_quantity, status, created_at) VALUES (@purchase_id, @refund_quantity, @status, @created_at)";
                    SqlCommand cmd = new SqlCommand(sql_insert, con);
                    cmd.Parameters.AddWithValue("@purchase_id", purchase_id);
                    cmd.Parameters.AddWithValue("@refund_quantity", refund_quantity.Text);
                    cmd.Parameters.AddWithValue("@status", "Returned");
                    cmd.Parameters.AddWithValue("@created_at", DateTime.Now);
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();

                    string po_update = "UPDATE tblpurchaseorder SET purchase_order_quantity=purchase_order_quantity-@refund_quantity WHERE purchase_order_id=@purchase_id";
                    cmd.CommandText = po_update;
                    cmd.Parameters.AddWithValue("@refund_quantity", refund_quantity.Text);
                    cmd.Parameters.AddWithValue("@purchase_id", purchase_id);
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();

                    string sql_update = "UPDATE tblproduct SET product_quantity=product_quantity-@refund_quantity WHERE product_id=@product_id";
                    cmd.CommandText = sql_update;
                    cmd.Parameters.AddWithValue("@refund_quantity", refund_quantity.Text);
                    //cmd.Parameters.AddWithValue("@product_id", product.Text);
                    cmd.Parameters.AddWithValue("@product_id", product.SelectedItem.ToString().Split(" ")[0]);
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                    MessageBox.Show("Refunded");
                }
                else if (exchange.IsChecked == true)
                {
                    string sql_insert = "INSERT INTO tblrefund(purchase_order_id, refund_quantity, status, created_at) VALUES (@purchase_id, @refund_quantity, @status, @created_at)";
                    SqlCommand cmd = new SqlCommand(sql_insert, con);
                    cmd.Parameters.AddWithValue("@purchase_id", purchase_id);
                    cmd.Parameters.AddWithValue("@refund_quantity", refund_quantity.Text);
                    cmd.Parameters.AddWithValue("@status", "Exchanged");
                    cmd.Parameters.AddWithValue("@created_at", DateTime.Now);
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();

                    string sql_updatepd = "UPDATE tblproduct SET product_quantity=product_quantity-@refund_quantity WHERE product_id=@product_id";
                    cmd.CommandText = sql_updatepd;
                    cmd.Parameters.AddWithValue("@refund_quantity", refund_quantity.Text);
                    //cmd.Parameters.AddWithValue("@product_id", product.Text);
                    cmd.Parameters.AddWithValue("@product_id", product.SelectedItem.ToString().Split(" ")[0]);
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();

                    string sql_updatepurchase = "UPDATE tblpurchaseorder SET product_id=@new_product_id, status=@new_status WHERE purchase_order_id=@purchase_id";
                    cmd.CommandText = sql_updatepurchase;
                    //cmd.Parameters.AddWithValue("@new_product_id", product.Text);
                    cmd.Parameters.AddWithValue("@new_product_id", product.SelectedItem.ToString().Split(" ")[0]);
                    cmd.Parameters.AddWithValue("@new_status", "Ordered");
                    cmd.Parameters.AddWithValue("@purchase_id", purchase_id);
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Exchanged");
                }

                purchase.SelectedIndex = -1;
                product.SelectedIndex = -1;
                supplier.Text = null;
                refund_quantity.Text = null; 

            }
            catch
            {
                MessageBox.Show("Errror submit");
            }
            finally
            {
                con.Close();
            }
        }

        private void returncheck(object sender, RoutedEventArgs e)
        {
            refund_quantity.Text = null;
            product.Items.Clear();

            supplier.Text = null;
            purchase.SelectedIndex = -1;
            product.SelectedIndex = -1;
            purchase.IsReadOnly = false;
            refund_quantity.IsReadOnly = false;
            product.IsEnabled = true;
            purchase.IsEnabled = true;
        }

        private void exchangecheck(object sender, RoutedEventArgs e)
        {
            product.Items.Clear();
            refund_quantity.Text = null;

            supplier.Text = null;
            purchase.SelectedIndex = -1;
            product.SelectedIndex = -1;
            purchase.IsReadOnly = false;
            refund_quantity.IsReadOnly = true;
            product.IsEnabled = true;
            purchase.IsEnabled = true;
        }
    }
}
