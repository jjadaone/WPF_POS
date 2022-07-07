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
    /// Interaction logic for ProductForm.xaml
    /// </summary>
    public partial class ProductForm : Page
    {
        public ProductForm()
        {
            InitializeComponent();
            loadData();
            fill_combo();
        }
        SqlConnection con = new SqlConnection(@"Data Source=localhost;Initial Catalog=pos;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");

       
        private void CBCat_DropDownClosed(object sender, EventArgs e)
        {
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("select * FROM tblcategory where category_name ='" + CBCat.Text+"' ", con);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                { 
                    string id = dr.GetInt32(0).ToString();
                    txtCategoryID.Text = id;

                }
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        void fill_combo()
        {
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("select * FROM tblcategory", con);
                SqlDataReader dr = cmd.ExecuteReader();
                while(dr.Read())
                {
                    string name = dr.GetString(1);
                    CBCat.Items.Add(name);
                    
                }
                con.Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        public void loadData()
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM tblproduct", con);
            DataTable dt = new DataTable();
            con.Open();
            SqlDataReader sdr = cmd.ExecuteReader();
            dt.Load(sdr);
            con.Close();
            DataGrid.ItemsSource = dt.DefaultView;
        }
        public void clearData()
        {
            txtProduct.Clear();
            txtDesc.Clear();
            txtPrice.Clear();   
            txtQty.Clear();
            CBCat.Items.Clear();
        }
        public bool isValid()
        {
            if (txtProduct.Text == String.Empty)
            {
                MessageBox.Show("Product is required", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (txtDesc.Text == String.Empty)
            {
                MessageBox.Show("Product Description is required", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (txtPrice.Text == String.Empty)
            {
                MessageBox.Show("Product Price is required", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (txtQty.Text == String.Empty)
            {
                MessageBox.Show("Product Quantity is required", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (CBCat.Text == String.Empty)
            {
                MessageBox.Show("Category is required", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            
            return true;
        }

        private void txtPrice_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);
        }
        private void txtQty_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);
        }

        private void AddProdBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (isValid())
                {
                    SqlCommand cmd = new SqlCommand("INSERT INTO tblproduct VALUES (@category_id, @product_name, @product_description, @product_quantity, @sales_price)", con);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@category_id", txtCategoryID.Text);
                    cmd.Parameters.AddWithValue("@product_name", txtProduct.Text);
                    cmd.Parameters.AddWithValue("@product_description", txtDesc.Text);
                    cmd.Parameters.AddWithValue("@product_quantity", txtQty.Text);
                    cmd.Parameters.AddWithValue("@sales_price", txtPrice.Text);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                    loadData();
                    MessageBox.Show("Successfully Entered", "Saved", MessageBoxButton.OK, MessageBoxImage.Information);
                    clearData();
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void DeleteProdBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataRowView row = DataGrid.SelectedItem as DataRowView;
                string id = row.Row.ItemArray[0].ToString();

                SqlCommand cmd = new SqlCommand("DELETE FROM tblproduct WHERE product_id=" + id, con);
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
    }
}
