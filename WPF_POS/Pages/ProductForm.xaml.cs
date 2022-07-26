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
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.IO;
using System.Drawing;

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
            PID_combo();
        }

        //SqlConnection con = new SqlConnection(@"Data Source=(localdb)\ProjectModels;Initial Catalog=pos;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");

        SqlConnection con = new SqlConnection(@"Data Source=localhost;Initial Catalog=pos;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
        string imgLoc = "";
       
        private void CBCat_DropDownClosed(object sender, EventArgs e)
        {
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("select * FROM tblcategory where category_name ='" + CBCat.Text+"' ", con);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    string cid = dr.GetInt32(0).ToString();
                    txtCategoryID.Text = cid;

                }
                con.Close();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
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
                    
                    string cname = dr.GetString(1);
                    CBCat.Items.Add(cname);
                    
                }
                con.Close();
            }
            catch(Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }

        }
        private void CBPID_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           
        }

        private void CBPID_DropDownClosed(object sender, EventArgs e)
        {
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("select * FROM tblproduct where product_id ='" + CBPID.Text + "' ", con);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    string cid = dr.GetInt32(1).ToString();
                    CBCat.Text = cid;

                    string prname = dr.GetString(2);
                    txtProduct.Text = prname;
                    string desc = dr.GetString(3);
                    txtDesc.Text = desc;
                    string pquantity = dr.GetInt32(4).ToString();
                    txtQty.Text = pquantity;

                    string pprice = dr.GetDecimal(5).ToString();
                    txtPrice.Text = pprice;


                    byte[] img = (byte[])(dr[6]);
                    if (img == null)
                    {
                        ImgProduct.Source = null;
                    }
                    else
                    {
                        MemoryStream ms = new MemoryStream(img);
                        ImgProduct.Source = BitmapFrame.Create(ms, BitmapCreateOptions.None,BitmapCacheOption.OnLoad);
                    }

                }
                con.Close();
            }
            catch (Exception ex)
            {
                con.Close();
                System.Windows.MessageBox.Show(ex.Message);
            }


        }
        void PID_combo()
        {
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("select * FROM tblproduct", con);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    string pid = dr.GetInt32(0).ToString();
                    CBPID.Items.Add(pid);
                    


                }
                con.Close();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }

        }
        public void clearData()
        {
            txtProduct.Clear();
            txtDesc.Clear();
            txtPrice.Clear();
            txtQty.Clear();
            CBCat.Items.Clear();
            txtPrice.Clear();
            ImgProduct.Source = null;

        }
        public void loadData()
        {
            //SqlCommand cmd = new SqlCommand("SELECT * FROM tbluser", con);
            //DataTable dt = new DataTable();
            //dt.
            //con.Open();
            //SqlDataReader sdr = cmd.ExecuteReader();
            //dt.Load(sdr);
            //con.Close();
            //dataGridBasket.ItemsSource = dt.DefaultView;

            //SqlCommand cmd = new SqlCommand("SELECT * FROM tblproduct", con);
            ////DataTable dt = new DataTable();

            //DataColumn textColumn = new DataColumn();
            //DataTable dt = new DataTable();
            ////textColumn. = "First Name";
            ////textColumn.Binding = new Binding("FirstName");
            //dt.Columns.Add("product id");
            //dt.Columns.Add("product name");
            //dt.Columns.Add("Image");
            //con.Open();
            //SqlDataReader sdr = cmd.ExecuteReader();
            //while (sdr.Read())
            //{
            //    string pid = sdr.GetInt32(0).ToString();
            //    dt.Rows.Add(pid);
            //    string pname = sdr.GetString(2);
            //    dt.Rows.Add(pname);

            //    //byte[] img = (byte[])(sdr[6]);
            //    //MemoryStream ms = new MemoryStream(img);

            //    //dt.Rows.Add(BitmapFrame.Create(ms, BitmapCreateOptions.None, BitmapCacheOption.OnLoad));




            //}
            //sdr.Close();
            //DataGrid.ItemsSource = dt.DefaultView;
            //con.Close();
           SqlCommand cmd = new SqlCommand("SELECT product_id AS 'ID', product_name AS 'NAME', product_price AS 'PRICE' FROM tblproduct", con);
            DataTable dt = new DataTable();
            con.Open();
            SqlDataReader sdr = cmd.ExecuteReader();
            dt.Load(sdr);
            con.Close();
            DataGrid.ItemsSource = dt.DefaultView;

        }


       /* public static byte[] imgToByteConverter(System.Drawing.Image inImg)
        {
            ImageConverter imgCon = new ImageConverter();
            return (byte[])imgCon.ConvertTo(inImg, typeof(byte[]));
        }
        public System.Drawing.Image byteArrayToImage(byte[] byteArrayIn)
        {
            using (MemoryStream mStream = new MemoryStream(byteArrayIn))
            {
                return System.Drawing.Image.FromStream(mStream);
            }
        }*/

        public bool isValid()
        {
            if (txtProduct.Text == String.Empty)
            {
                System.Windows.MessageBox.Show("Product is required", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (txtDesc.Text == String.Empty)
            {
                System.Windows.MessageBox.Show("Product Description is required", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (txtPrice.Text == String.Empty)
            {
                System.Windows.MessageBox.Show("Product Price is required", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (txtQty.Text == String.Empty)
            {
                System.Windows.MessageBox.Show("Product Quantity is required", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (CBCat.Text == String.Empty)
            {
                System.Windows.MessageBox.Show("Category is required", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
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

                    byte[] img = null;

                    FileStream fs = new FileStream(imgLoc, FileMode.Open, FileAccess.Read);
                    BinaryReader br = new BinaryReader(fs);
                    img = br.ReadBytes((int)fs.Length);


                    SqlCommand cmd = new SqlCommand("INSERT INTO tblproduct VALUES (@category_id, @product_name, @product_description, @product_quantity, @product_price, @product_image)", con);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@category_id", txtCategoryID.Text);
                    cmd.Parameters.AddWithValue("@product_name", txtProduct.Text);
                    cmd.Parameters.AddWithValue("@product_description", txtDesc.Text);
                    cmd.Parameters.AddWithValue("@product_quantity", txtQty.Text);
                    cmd.Parameters.AddWithValue("@product_price", txtPrice.Text);
                    cmd.Parameters.AddWithValue("@product_image", img);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                    System.Windows.MessageBox.Show("Successfully Entered", "Saved", MessageBoxButton.OK, MessageBoxImage.Information);
                    loadData();
                    clearData();
                }
            }
            catch (SqlException ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
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
                System.Windows.MessageBox.Show("Deleted successfully");
                loadData();
                //clearData();
                con.Close();

            }
            catch (SqlException ex)
            {
                System.Windows.MessageBox.Show("Deletion Error" + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        private void UpdateProdBtn_Click(object sender, RoutedEventArgs e)
        {
            byte[] img = null;

            FileStream fs = new FileStream(imgLoc, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            img = br.ReadBytes((int)fs.Length);


            string sql = @"UPDATE tblproduct SET product_name=@product_name, product_description=@product_description, product_quantity=@product_quantity, product_price=@product_price, product_image=@product_image WHERE product_id=@product_id";
            SqlCommand cmd = new SqlCommand(sql, con);

            cmd.Parameters.AddWithValue("@category_id", txtCategoryID.Text);
            cmd.Parameters.AddWithValue("@product_name", txtProduct.Text);
            cmd.Parameters.AddWithValue("@product_description", txtDesc.Text);
            cmd.Parameters.AddWithValue("@product_quantity", txtQty.Text);
            cmd.Parameters.AddWithValue("@product_price", txtPrice.Text);
            cmd.Parameters.AddWithValue("@product_image", img);
            cmd.Parameters.AddWithValue("@product_id",CBPID.Text);
            con.Open();
           
            //SqlCommand cmd = new SqlCommand("update tblproduct set product_name = '" + txtProduct.Text + "', product_description = '" + txtDesc.Text + "', product_quantity = '" + txtQty.Text + "', product_price = '" + txtPrice.Text + "' WHERE product_id = '" + CBPID.Text + "' ", con);

            try
            {
                cmd.ExecuteNonQuery();
                System.Windows.MessageBox.Show("Product updated successfully", "Updated", MessageBoxButton.OK, MessageBoxImage.Information);
                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
            }
            catch (SqlException ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
            finally
            {
                con.Close();
                //clearData();
                loadData();


            }

        }

        private void ClrBtn_Click(object sender, RoutedEventArgs e)
        {
            clearData();

        }

        private void BrowseBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.Filter = "JPG Files (*.jpg)|*.jpg|PNG Files (*.png)|*.png| All Files (*.*)|*.*";
                dlg.Title = "Product Photo";
                    
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    imgLoc = dlg.FileName.ToString();
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(imgLoc);
                    bitmap.EndInit();
                    ImgProduct.Source = bitmap;
                    
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }
    }
}
