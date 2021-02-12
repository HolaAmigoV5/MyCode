using CommandDemo.CustomCommand;
using System.Windows;

namespace CommandDemo
{
    /// <summary>
    /// MiniView.xaml 的交互逻辑
    /// </summary>
    public partial class MiniView : Window
    {
        public MiniView()
        {
            InitializeComponent();

            //声明命令并使用命令源和目标与之关联
            ClearCommand clearCommand = new ClearCommand();
            this.ctrlClear.Command = clearCommand;
            this.ctrlClear.CommandTarget = this.miniView;
        }
    }
}
