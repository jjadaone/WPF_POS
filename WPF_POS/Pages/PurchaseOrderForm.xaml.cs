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
using System.Text.RegularExpressions;

namespace WPF_POS.Pages
{
    /// <summary>
    /// Interaction logic for PurchaseOrderForm.xaml
    /// </summary>
    public partial class PurchaseOrderForm : Page
    {
        public PurchaseOrderForm()
        {
            InitializeComponent();
            loadData();
            CBPName_combo();
            CBSName_combo();
        }
        
        //SqlConnection con = new SqlConnection(@"Data Source=(localdb)\ProjectModels;Initial Catalog=pos;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
        SqlConnection con = new SqlConnection(@"Data Source=localhost;Initial Catalog=pos;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");

        public void loadData()
        {
            SqlCommand cmd = new SqlCommand("SELECT purchase_order_id AS 'ORDER ID', product_id AS 'PRODUCT', supplier_id AS 'SUPPLIER', purchase_order_quantity AS 'QUANTITY', status AS 'STATUS', order_date AS 'ORDER DATE' FROM tblpurchaseorder", con);
            DataTable dt = new DataTable();
            con.Open();
            SqlDataReader sdr = cmd.ExecuteReader();
            dt.Load(sdr);
            con.Close();
            DataGrid.ItemsSource = dt.DefaultView;
        }
        public void clearData()
        {

            txtOrderQuantity.Clear();
            txtOrderTotal.Clear();
            txtProductID.Clear();
            txtSuppID.Clear();
          
        }
        public bool isValid()
        {
            if (CBPName.Text == String.Empty)
            {
                MessageBox.Show("Product name is required", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (CBSname.Text == String.Empty)
            {
                MessageBox.Show("Supplier name is required", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (txtOrderQuantity.Text == String.Empty)
            {
                MessageBox.Show("Order Quantity is required", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (txtOrderStatus.Text == String.Empty)
            {
                MessageBox.Show("Order Status is required", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (txtOrderTotal.Text == String.Empty)
            {
                MessageBox.Show("Order Total is required", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
           
            return true;
        }
        private void txtOrderQuantity_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);
        }

        private void AddOrderBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (isValid())
                {
                    SqlCommand cmd = new SqlCommand("INSERT INTO tblpurchaseorder VALUES (@product_id, @supplier_id, @purchase_order_quantity, @purchase_order_total, @status, @order_date, @received_date)", con);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@product_id", txtProductID.Text);
                    cmd.Parameters.AddWithValue("@supplier_id", txtSuppID.Text);
                    cmd.Parameters.AddWithValue("@purchase_order_quantity", txtOrderQuantity.Text);
                    cmd.Parameters.AddWithValue("@purchase_order_total", txtOrderTotal.Text);
                    cmd.Parameters.AddWithValue("@status", txtOrderStatus.Text);
                    cmd.Parameters.AddWithValue("@order_date", txtDateTime.Text = DateTime.Now.ToString());
                    cmd.Parameters.AddWithValue("@received_date", txtReceivedDate.Text = "");

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                    MessageBox.Show("Successfully Entered", "Saved", MessageBoxButton.OK, MessageBoxImage.Information);
                    loadData();
                    clearData();

                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void CBPName_combo()
        {
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("select * FROM tblproduct", con);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    string pname = dr.GetString(2);
                    CBPName.Items.Add(pname);
                }
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        void CBSName_combo()
        {
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("select * FROM tblsupplier", con);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    string sname = dr.GetString(1);
                    CBSname.Items.Add(sname);
                }
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void CBPName_DropDownClosed(object sender, EventArgs e)
        {
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("select * FROM tblproduct where product_name ='" + CBPName.Text + "' ", con);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {

                    string pid = dr.GetInt32(0).ToString();
                    txtProductID.Text = pid;
                    string price = dr.GetDecimal(5).ToString();
                    txtPrice.Text = price;
                }
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void CBSname_DropDownClosed(object sender, EventArgs e)
        {
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("select * FROM tblsupplier where supplier_name ='" + CBSname.Text + "' ", con);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {

                    string sid = dr.GetInt32(0).ToString();
                    txtSuppID.Text = sid;
                }
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void DeleteOrderBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataRowView row = DataGrid.SelectedItem as DataRowView;
                string id = row.Row.ItemArray[0].ToString();

                SqlCommand cmd = new SqlCommand("DELETE FROM tblpurchaseorder WHERE purchase_order_id=" + id, con);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                MessageBox.Show("Deleted successfully");
                loadData();
                clearData();
                con.Close();

            }
            catch (SqlException ex)
            {
                MessageBox.Show("Deletion Error" + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }
 

        private void CBPOID_DropDownClosed(object sender, EventArgs e)
        {
           
        }

        private void CBPOID_DropDownClosed_1(object sender, EventArgs e)
        {
           

        }

        private void UpdateOrderBtn_Click(object sender, RoutedEventArgs e)
        {
        
        }

        private void PriceTotalBtn_Click(object sender, RoutedEventArgs e)
        {

            double n1, answer, n2;


           double.TryParse(txtOrderQuantity.Text, out n1);
           double.TryParse(txtPrice.Text, out n2);
            answer = n1 * n2;
            if (answer > 0)
            {
                txtOrderTotal.Text = answer.ToString();
            }



        }
    }
}
