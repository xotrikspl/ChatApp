using Chat_App.MVVM.Code;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Chat_App.MVVM.AdditionalWindow
{
    public partial class ProductsWindow : Window
    {
        private List<Product> products = new List<Product>();
        private List<Product> filteredProducts = new List<Product>();

        public ProductsWindow()
        {
            InitializeComponent();

            // Połącz z bazą danych i wczytaj dane
            Database database = new Database();
            string query = "SELECT * FROM product_list";
            var dataReader = database.ExecuteQuery(query);

            if (dataReader != null)
            {
                while (dataReader.Read())
                {
                    Product product = new Product();
                    product.id = (int)dataReader["id"];
                    product.business = (string)dataReader["business"];
                    product.name = (string)dataReader["product_name"];
                    product.flavor = (string)dataReader["product_flavor"];
                    product.description = (string)dataReader["product_description"];
                    product.type = (string)dataReader["product_type"];
                    product.price = (string)dataReader["price"];
                    product.ml = (string)dataReader["ml"];
                    products.Add(product);
                }
            }
            database.CloseConnection();

            // Ustawienie źródła danych dla tabeli
            DataContext = this;
            filteredProducts = products;
        }

        public List<Product> Products
        {
            get { return filteredProducts; }
            set { filteredProducts = value; }
        }

        public Product SelectedProduct { get; set; }

        private void SearchProducts(object sender, TextChangedEventArgs e)
        {
            filteredProducts = products.Where(product => product.name.ToLower().Contains(txtSearch.Text.ToLower())).ToList();
            DataContext = null;
            DataContext = this;
        }

        private void FilterProducts(object sender, RoutedEventArgs e)
        {
            List<string> selectedTypes = new List<string>();
            if (chkFilter1.IsChecked == true) selectedTypes.Add("Longfill");
            if (chkFilter2.IsChecked == true) selectedTypes.Add("Shortfill");
            if (chkFilter3.IsChecked == true) selectedTypes.Add("Aromaty");
            if (chkFilter4.IsChecked == true) selectedTypes.Add("Liquid");

            if (selectedTypes.Count > 0)
            {
                filteredProducts = products.Where(product => selectedTypes.Contains(product.type)).ToList();
            }
            else
            {
                filteredProducts = products;
            }

            // Dodanie uwzględnienia wyników wyszukiwania w filtrach
            if (!string.IsNullOrEmpty(txtSearch.Text))
            {
                filteredProducts = filteredProducts.Where(product => product.name.ToLower().Contains(txtSearch.Text.ToLower())).ToList();
            }

            DataContext = null;
            DataContext = this;
        }

        private void FilterProducts_Unchecked(object sender, RoutedEventArgs e)
        {
            FilterProducts(sender, e);
        }
    }
}
