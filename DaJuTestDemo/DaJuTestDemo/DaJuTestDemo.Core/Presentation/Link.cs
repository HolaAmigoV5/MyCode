using System;

namespace DaJuTestDemo.Core.Presentation
{
    public class Link : Displayable
    {
        private Uri source;
        public Uri Source
        {
            get => source;
            set => SetProperty(ref source, value);
        }
    }
}
