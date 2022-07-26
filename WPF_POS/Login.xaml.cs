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
using MaterialDesignThemes.Wpf;


namespace WPF_POS
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {

        SqlCommand com = new SqlCommand();
        SqlDataReader dr;
        public Login()
        {
            InitializeComponent();
        }

        SqlConnection con = new SqlConnection(@"Data Source=localhost;Initial Catalog=pos;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
        CustomerOrderWindow sales = new CustomerOrderWindow();
        MainWindow admin = new MainWindow();
        private void btnLoginClick(object sender, RoutedEventArgs e)
        {
            if (con.State == System.Data.ConnectionState.Open)
            {
                con.Close();
            }
            if (VerifyUser(username.Text, password.Password))
            {
                MessageBox.Show("Login Successfully", "Congrats", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Username or password is incorrect", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            /*try
            {
                string sql = @"SELECT * FROM tbluser WHERE username=@username AND password=@password";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@username", username.Text);
                cmd.Parameters.AddWithValue("@password", password.Password);
                con.Open();
                //MessageBox.Show(username.Text, password.Text);

                //cmd.ExecuteNonQuery();
                int user_id = Convert.ToInt32(cmd.ExecuteScalar());


                if (user_id != 0)
                {
                    this.Close();
                    sales.Show();
                }
                else
                {
                    MessageBox.Show("Invalid username and password");
                }


            }
            catch
            {
                MessageBox.Show("Error login");
            }
            finally
            {
                con.Close();
            } */
        }
        private bool VerifyUser(string username, string password)
        {
            con.Open();
            com.Connection = con;
            com.CommandText = "select role, user_id, fullname from tbluser where username='" + username + "' and password='" + password + "'";
            dr = com.ExecuteReader();
            if (dr.Read())
            {
                if ((dr["role"]).ToString() == "Administrator" )
                {
                    admin.lblname.Content = (dr["fullname"]).ToString();
                    admin.lblUserID.Content = (dr["user_id"]).ToString();
                    this.Close();
                    admin.Show();
                    return true;
                }
                else if ((dr["role"]).ToString() == "Cashier")
                {

                    sales.lblname.Content = (dr["fullname"]).ToString();
                    sales.lblUserID.Content = (dr["user_id"]).ToString();
                    this.Close();
                    sales.Show();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public bool IsDarkTheme { get; set; }
        private readonly PaletteHelper paletteHelper = new PaletteHelper();
        private void toggleTheme(object sender, RoutedEventArgs e)
        {
            ITheme theme = paletteHelper.GetTheme();
            if (IsDarkTheme = theme.GetBaseTheme()== BaseTheme.Dark)
            {
                IsDarkTheme = false;
                theme.SetBaseTheme(Theme.Light);
            }
            else
            {
                IsDarkTheme = true;
                theme.SetBaseTheme(Theme.Dark);
            }
            paletteHelper.SetTheme(theme);
        }

        private void exitApp(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();

        }
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            DragMove();
        }
    }
}
