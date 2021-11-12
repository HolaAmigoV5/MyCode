using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Integration;

namespace MapWithContent
{
    [TemplatePart(Name =PART_MapHost, Type=typeof(WindowsFormsHost))]
    public class I3DMapView : ContentControl,IDisposable
    {
        private const string PART_MapHost = "PART_MapHost";

        MapOperation _operation = null;

        private WindowsFormsHost WindowsFormsHost => Template.FindName(PART_MapHost, this) as WindowsFormsHost;

        public I3DMapView()
        {
            DefaultStyleKey = typeof(I3DMapView);

            _operation = new MapOperation();
        }

        private bool IsDesignMode => (bool)DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof(DependencyObject)).DefaultValue;
        private ForegroundWindow ForegroundWindow { get; set; }
        private bool IsUpdatingContent { get; set; }
        private UIElement ViewContent { get; set; }


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (!IsDesignMode)
            {
                var windowsFormsHost = WindowsFormsHost;
                if (windowsFormsHost != null)
                {
                    ForegroundWindow = new ForegroundWindow(windowsFormsHost)
                    {
                        OverlayContent = ViewContent
                    };

                    _operation.InitializationMapControl(WindowsFormsHost);

                    DataContext = new MainWindowViewModel();
                }
            }
        }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);

            if (IsDesignMode || IsUpdatingContent)
                return;

            IsUpdatingContent = true;
            try
            {
                Content = null;
            }
            finally
            {
                IsUpdatingContent = false;
            }

            ViewContent = newContent as UIElement;
            if (ForegroundWindow != null)
            {
                ForegroundWindow.OverlayContent = ViewContent;
            }

        }


        public void Dispose() => Dispose(true);

        bool disposedValue;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    WindowsFormsHost?.Dispose();
                    ForegroundWindow?.Close();
                }

                ViewContent = null;
                ForegroundWindow = null;
                disposedValue = true;
            }
        }
    }
}
