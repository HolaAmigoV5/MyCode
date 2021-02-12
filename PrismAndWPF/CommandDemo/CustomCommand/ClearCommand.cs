using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CommandDemo.CustomCommand
{
    public class ClearCommand : ICommand
    {
        //当命令可执行状态发生改变时，应当被激发
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            throw new NotImplementedException();
        }

        //命令执行，带有业务相关的Clear逻辑
        public void Execute(object parameter)
        {
            IView view = parameter as IView;
            view?.Clear();
        }
    }
}
