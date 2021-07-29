using Prism.Mvvm;

namespace DaJuTestDemo.Core.Presentation
{
    public class Displayable : BindableBase
    {
        private string displayName;
        public string DisplayName
        {
            get => displayName;
            set => SetProperty(ref displayName, value);
        }
    }
}
