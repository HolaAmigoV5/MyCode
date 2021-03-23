using System.Windows.Controls;
using Wby.Demo.Shared.Dto;
using Wby.PrismDemo.PC.Infrastructure.Common;

namespace Wby.PrismDemo.PC.Views
{
    /// <summary>
    /// UserView.xaml 的交互逻辑
    /// </summary>
    public partial class UserView1 : UserControl
    {
        public UserView1()
        {
            InitializeComponent();

            VisualHelper.SetDataGridColumns<UserControl>(this, "Grid", typeof(UserDto));
        }
    }
}
