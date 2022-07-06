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

using WPF_POS.Pages;

namespace WPF_POS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }



        private void btnHomeClick(object sender, RoutedEventArgs e)
        {

        }

        private void btnProductClick(object sender, RoutedEventArgs e)
        {
            Main.Content = new ProductForm();
        }

        private void btnStockinClick(object sender, RoutedEventArgs e)
        {

        }

        private void btnPurchaseOrderClick(object sender, RoutedEventArgs e)
        {

        }

        private void btnCategoryClick(object sender, RoutedEventArgs e)
        {
            Main.Content = new ProductForm();
        }

        private void btnSupplierClick(object sender, RoutedEventArgs e)
        {

        }

        private void btnUserClick(object sender, RoutedEventArgs e)
        {
            Main.Content = new UserForm();
        }

        private void btnSalesReportClick(object sender, RoutedEventArgs e)
        {

        }

        private void btnInventoryReportClick(object sender, RoutedEventArgs e)
        {

        }
    }
