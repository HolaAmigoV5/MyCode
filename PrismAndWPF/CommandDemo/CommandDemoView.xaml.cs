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

namespace CommandDemo
{
    /// <summary>
    /// CommandDemoView.xaml 的交互逻辑
    /// </summary>
    public partial class CommandDemoView : Window
    {
        public CommandDemoView()
        {
            InitializeComponent();
            InitializeCommand();

        }

        //声明并定义命令
        private RoutedCommand clearCmd = new RoutedCommand("Clear", typeof(CommandDemoView));

        private void InitializeCommand()
        {
            //把命令赋值给命令源（发送者）并指定快捷键
            this.btn1.Command = clearCmd;
            this.clearCmd.InputGestures.Add(new KeyGesture(Key.C, ModifierKeys.Alt));

            //指定命令目标
            this.btn1.CommandTarget = this.tBoxA;

            //创建命令关联
            CommandBinding cb = new CommandBinding();
            cb.Command = this.clearCmd;
            cb.CanExecute += Cb_CanExecute;
            cb.Executed += Cb_Executed;

            //把命令关联安置在外围控件上
            this.sp1.CommandBindings.Add(cb);
        }

        //当命令送达目标后，此方便被调用
        private void Cb_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.tBoxA.Clear();
            e.Handled = true;
        }

        //当探测命令是否可执行时，此方法被调用
        private void Cb_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !string.IsNullOrEmpty(this.tBoxA.Text);

            //避免继续向上传而降低程序性能。
            e.Handled = true;
        }
    }
}
