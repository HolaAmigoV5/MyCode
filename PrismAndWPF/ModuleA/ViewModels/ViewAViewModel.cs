using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using PrismAndWPF.Core;
using System;

namespace ModuleA.ViewModels
{
    public class ViewAViewModel : BindableBase
    {
        IEventAggregator _ea;

        //这里展示DelegateCommand的使用
        private string _message;
        public string Message
        {
            get { return _message; }
            set { SetProperty(ref _message, value); }
        }

        private string _updateText;

        public string UpdateText
        {
            get { return _updateText; }
            set { SetProperty(ref _updateText, value); }
        }

        private bool _isEnabled;

        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                SetProperty(ref _isEnabled, value);
                ExecuteDelegateCommand.RaiseCanExecuteChanged();
            }
        }


        public DelegateCommand ExecuteDelegateCommand { get; private set; }
        public DelegateCommand<string> ExecuteGenericDelegateCommand { get; private set; }
        public DelegateCommand DelegateCommandObservesProperty { get; private set; }
        public DelegateCommand DelegateCommandObservesCanExecute { get; private set; }

        public DelegateCommand SendMessageCommand { get; private set; }
        public DelegateCommand ClearCommand { get; private set; }


        public ViewAViewModel(IEventAggregator ea)
        {
            Message = "send View A";

            ExecuteDelegateCommand = new DelegateCommand(Execute, CanExecute);
            DelegateCommandObservesProperty = new DelegateCommand(Execute, CanExecute).ObservesProperty(() => IsEnabled);
            DelegateCommandObservesCanExecute = new DelegateCommand(Execute).ObservesCanExecute(() => IsEnabled);
            ExecuteGenericDelegateCommand = new DelegateCommand<string>(ExecuteGeneric).ObservesCanExecute(() => IsEnabled);

            
            _ea = ea;
            SendMessageCommand = new DelegateCommand(SendMessage);
            ClearCommand = new DelegateCommand(ClearViewBListBox);
        }

        private void Execute()
        {
            UpdateText = $"Updated:{DateTime.Now}";
        }

        private void ExecuteGeneric(string parameter)
        {
            UpdateText = parameter;
        }

        private bool CanExecute()
        {
            return IsEnabled;
        }

        private void SendMessage()
        {
            //这里发布聚合事件
            _ea.GetEvent<MessageSentEvent>().Publish(Message);
        }

        private void ClearViewBListBox()
        {
            _ea.GetEvent<MessageSentEvent>().Publish("Clear");
        }
    }
}
