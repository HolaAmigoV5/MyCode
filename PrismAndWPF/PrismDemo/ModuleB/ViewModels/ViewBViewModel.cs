using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using PrismAndWPF.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuleB.ViewModels
{
    public class ViewBViewModel : BindableBase
    {
        IEventAggregator _ea;

        private ObservableCollection<string> _messages;
        public ObservableCollection<string> Messages
        {
            get { return _messages; }
            set { SetProperty(ref _messages, value); }
        }
       
        public ViewBViewModel(IEventAggregator ea)
        {
            _ea = ea;
            Messages = new ObservableCollection<string>();

            //这里订阅聚合事件。带有过滤条件的订阅事件
            _ea.GetEvent<MessageSentEvent>().Subscribe(MessageReceived, ThreadOption.PublisherThread, false, 
                filter => filter.Contains("send") || filter.Contains("Clear"));
           
        }

        private void MessageReceived(string message)
        {
            if (message == "Clear")
                Messages.Clear();
            else
                Messages.Add(message);
        }
    }
}
