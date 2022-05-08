using System.Collections.Generic;
using System.Windows.Controls;

namespace DropDownMenu
{
    /// <summary>
    /// UserControlDashboard.xaml 的交互逻辑
    /// </summary>
    public partial class Dashboard : UserControl
    {
        public Dashboard()
        {
            InitializeComponent();

            Consumo consumo = new Consumo();
            DataContext = new ConsumoViewModel(consumo);
        }
    }

    internal class ConsumoViewModel
    {
        public List<Consumo> Consumo { get; private set; }

        public ConsumoViewModel(Consumo consumo)
        {
            Consumo = new List<Consumo>
            {
                consumo
            };
        }
    }

    internal class Consumo
    {
        public string Titulo { get; private set; }
        public int Porcentagem { get; private set; }

        public Consumo()
        {
            Titulo = "Consumo Atual";
            Porcentagem = CalcularPorcentagem();
        }

        private int CalcularPorcentagem()
        {
            return 47; 
        }
    }
}
