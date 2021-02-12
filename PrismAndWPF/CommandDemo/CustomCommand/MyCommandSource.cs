using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CommandDemo.CustomCommand
{
    public class MyCommandSource : UserControl, ICommandSource
    {
        //继承自ICommandSource的三个属性
        public ICommand Command { get; set; }

        public object CommandParameter { get; set; }

        public IInputElement CommandTarget { get; set; }

        //在组件被单击时连带执行命令
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            if (this.CommandTarget != null)
                this.Command.Execute(this.CommandTarget);
        }
    }
}
