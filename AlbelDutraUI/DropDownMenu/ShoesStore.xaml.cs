using System.Collections.Generic;
using System.Windows.Controls;

namespace DropDownMenu
{
    /// <summary>
    /// ResponsiveHorizontalListView.xaml 的交互逻辑
    /// </summary>
    public partial class ShoesStore : UserControl
    {
        public ShoesStore()
        {
            InitializeComponent();

            var products = GetProducts();
            if (products.Count > 0)
                ListViewProducts.ItemsSource = products;
        }

        private List<Product> GetProducts()
        {
            return new List<Product>() {
                new Product("李宁",200.4, "/Assets/x1.jpg"),
                new Product("安踏",340.5, "/Assets/x2.jpg"),
                new Product("耐克",899.0, "/Assets/x3.jpg"),
                new Product("阿迪达斯",1299.0, "/Assets/x4.jpg"),
                new Product("鸿星尔克",399.0, "/Assets/x5.jpg"),
                new Product("飞跃",99.9, "/Assets/x6.jpg"),
                new Product("匡威",200.4, "/Assets/x7.jpg"),
                new Product("添柏岚",2299, "/Assets/x8.jpg"),
            };
        }
    }
}
