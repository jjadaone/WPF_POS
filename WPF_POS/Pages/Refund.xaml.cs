﻿using System;
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
                SqlCommand cmd = new SqlCommand("SELECT purchase_order_id, product_name FROM tblpurchaseorder INNER JOIN tblproduct ON tblproduct.product_id=tblpurchaseorder.product_id WHERE status='Received'", con);
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

        public void fillProduct()
        {
            try
            {
                con.Open();
                if ()
                {
                    SqlCommand cmd = new SqlCommand("SELECT product_id, product_price FROM tblproduct WHERE product_price=@product_price", con);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        int id = dr.GetInt32(0);
                        //string text = dr.GetString(1);
                        purchase.Items.Add(id);
                    }
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
                string sql = "SELECT pd.product_id, supplier_name, product_quantity FROM tblpurchaseorder AS po INNER JOIN tblproduct AS pd ON pd.product_id=po.product_id INNER JOIN tblsupplier as s ON s.supplier_id=po.supplier_id WHERE po.purchase_order_id=@purchase_order_id ";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("purchase_order_id", purchase_id);
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    int p_id = dr.GetInt32(0);
                    //MessageBox.Show(p_id.ToString());
                    product.Text = p_id.ToString();
                    supplier.Text = dr.GetString(1);
                }
                con.Close();
            }
            catch
            {
                MessageBox.Show("Error");
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
                    string sql_update = "UPDATE tblproduct SET product_quantity=product_quantity-@refund_quantity WHERE product_id=@product_id";
                    SqlCommand cmd = new SqlCommand(sql_insert, con);
                    cmd.Parameters.AddWithValue("@purchase_id", purchase_id);
                    cmd.Parameters.AddWithValue("@refund_quantity", refund_quantity.Text);
                    cmd.Parameters.AddWithValue("@status", "Returned");
                    cmd.Parameters.AddWithValue("@created_at", DateTime.Now);
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();

                    cmd.CommandText = sql_update;
                    cmd.Parameters.AddWithValue("@refund_quantity", refund_quantity.Text);
                    cmd.Parameters.AddWithValue("@product_id", product.Text);
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                    MessageBox.Show("Refunded");
                } else if(exchange.IsChecked == true)
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
                    cmd.Parameters.AddWithValue("@product_id", product.Text);
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();

                    string sql_updatepurchase = "UPDATE tblpurchaseorder SET product_id=@new_product_id, status=@new_status WHERE purchase_order_id=@purchase_id";
                    cmd.CommandText = sql_updatepurchase;
                    cmd.Parameters.AddWithValue("@new_product_id", product.Text);
                    cmd.Parameters.AddWithValue("@new_status", "Ordered");
                    cmd.Parameters.AddWithValue("@purchase_id", purchase_id);
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Exchanged");
                }

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

    }
}
