using Prism.Commands;

namespace PrismAndWPF.Core
{
    public interface IApplicationCommands
    {
        CompositeCommand SaveCommand { get; }
    }
    public class ApplicationCommands : IApplicationCommands
    {
        //复合命令
        //private CompositeCommand _saveCommand = new CompositeCommand();

        //true表示监视复合命令的子命令是否Activity
        private CompositeCommand _saveCommand = new CompositeCommand(true);
        public CompositeCommand SaveCommand { get { return _saveCommand; } }
    }
}
