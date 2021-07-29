using DaJuTestDemo.Core.Presentation;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace DaJuTestDemo.Core.Controls
{
    /// <summary>
    /// Represents a Modern UI styled window.
    /// </summary>
    public class ModernWindow : Window
    {
        #region Properties
        /// <summary>
        /// Identifies the BackgroundContent dependency property.
        /// </summary>
        public static readonly DependencyProperty BackgroundContentProperty = DependencyProperty.Register("BackgroundContent", typeof(object), typeof(ModernWindow));
        /// <summary>
        /// Identifies the MenuLinkGroups dependency property.
        /// </summary>
        public static readonly DependencyProperty MenuLinkGroupsProperty = DependencyProperty.Register("MenuLinkGroups", typeof(LinkGroupCollection), typeof(ModernWindow));
        /// <summary>
        /// Identifies the TitleLinks dependency property.
        /// </summary>
        public static readonly DependencyProperty TitleLinksProperty = DependencyProperty.Register("TitleLinks", typeof(LinkCollection), typeof(ModernWindow));
        /// <summary>
        /// Identifies the IsTitleVisible dependency property.
        /// </summary>
        public static readonly DependencyProperty IsTitleVisibleProperty = DependencyProperty.Register("IsTitleVisible", typeof(bool), typeof(ModernWindow), new PropertyMetadata(false));
        /// <summary>
        /// Identifies the LogoData dependency property.
        /// </summary>
        public static readonly DependencyProperty LogoDataProperty = DependencyProperty.Register("LogoData", typeof(Geometry), typeof(ModernWindow));
        /// <summary>
        /// Defines the ContentSource dependency property.
        /// </summary>
        public static readonly DependencyProperty ContentSourceProperty = DependencyProperty.Register("ContentSource", typeof(Uri), typeof(ModernWindow));
        /// <summary>
        /// Identifies the ContentLoader dependency property.
        /// </summary>
        public static readonly DependencyProperty ContentLoaderProperty = DependencyProperty.Register("ContentLoader", typeof(IContentLoader), typeof(ModernWindow), new PropertyMetadata(new DefaultContentLoader()));
        /// <summary>
        /// Identifies the LinkNavigator dependency property.
        /// </summary>
        public static DependencyProperty LinkNavigatorProperty = DependencyProperty.Register("LinkNavigator", typeof(ILinkNavigator), typeof(ModernWindow), new PropertyMetadata(new DefaultLinkNavigator()));

        /// <summary>
        /// Gets or sets the background content of this window instance.
        /// </summary>
        public object BackgroundContent
        {
            get { return GetValue(BackgroundContentProperty); }
            set { SetValue(BackgroundContentProperty, value); }
        }

        /// <summary>
        /// Gets or sets the collection of link groups shown in the window's menu.
        /// </summary>
        public LinkGroupCollection MenuLinkGroups
        {
            get { return (LinkGroupCollection)GetValue(MenuLinkGroupsProperty); }
            set { SetValue(MenuLinkGroupsProperty, value); }
        }

        /// <summary>
        /// Gets or sets the collection of links that appear in the menu in the title area of the window.
        /// </summary>
        public LinkCollection TitleLinks
        {
            get { return (LinkCollection)GetValue(TitleLinksProperty); }
            set { SetValue(TitleLinksProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the window title is visible in the UI.
        /// </summary>
        public bool IsTitleVisible
        {
            get { return (bool)GetValue(IsTitleVisibleProperty); }
            set { SetValue(IsTitleVisibleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the path data for the logo displayed in the title area of the window.
        /// </summary>
        public Geometry LogoData
        {
            get { return (Geometry)GetValue(LogoDataProperty); }
            set { SetValue(LogoDataProperty, value); }
        }

        /// <summary>
        /// Gets or sets the source uri of the current content.
        /// </summary>
        public Uri ContentSource
        {
            get { return (Uri)GetValue(ContentSourceProperty); }
            set { SetValue(ContentSourceProperty, value); }
        }

        /// <summary>
        /// Gets or sets the content loader.
        /// </summary>
        public IContentLoader ContentLoader
        {
            get { return (IContentLoader)GetValue(ContentLoaderProperty); }
            set { SetValue(ContentLoaderProperty, value); }
        }

        /// <summary>
        /// Gets or sets the link navigator.
        /// </summary>
        /// <value>The link navigator.</value>
        public ILinkNavigator LinkNavigator
        {
            get { return (ILinkNavigator)GetValue(LinkNavigatorProperty); }
            set { SetValue(LinkNavigatorProperty, value); }
        }

        private Storyboard backgroundAnimation; 
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ModernWindow"/> class.
        /// </summary>
        public ModernWindow()
        {
            DefaultStyleKey = typeof(ModernWindow);

            // create empty collections
            SetCurrentValue(MenuLinkGroupsProperty, new LinkGroupCollection());
            SetCurrentValue(TitleLinksProperty, new LinkCollection());

            // associate window commands with this instance
#if NET4
            this.CommandBindings.Add(new CommandBinding(Microsoft.Windows.Shell.SystemCommands.CloseWindowCommand, OnCloseWindow));
            this.CommandBindings.Add(new CommandBinding(Microsoft.Windows.Shell.SystemCommands.MaximizeWindowCommand, OnMaximizeWindow, OnCanResizeWindow));
            this.CommandBindings.Add(new CommandBinding(Microsoft.Windows.Shell.SystemCommands.MinimizeWindowCommand, OnMinimizeWindow, OnCanMinimizeWindow));
            this.CommandBindings.Add(new CommandBinding(Microsoft.Windows.Shell.SystemCommands.RestoreWindowCommand, OnRestoreWindow, OnCanResizeWindow));
#else
            CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, OnCloseWindow));
            CommandBindings.Add(new CommandBinding(SystemCommands.MaximizeWindowCommand, OnMaximizeWindow, OnCanResizeWindow));
            CommandBindings.Add(new CommandBinding(SystemCommands.MinimizeWindowCommand, OnMinimizeWindow, OnCanMinimizeWindow));
            CommandBindings.Add(new CommandBinding(SystemCommands.RestoreWindowCommand, OnRestoreWindow, OnCanResizeWindow));
#endif
            // associate navigate link command with this instance
            CommandBindings.Add(new CommandBinding(LinkCommands.NavigateLink, OnNavigateLink, OnCanNavigateLink));

            // listen for theme changes
            AppearanceManager.Current.PropertyChanged += OnAppearanceManagerPropertyChanged;
        }

        /// <summary>
        /// Raises the System.Windows.Window.Closed event.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // detach event handler
            AppearanceManager.Current.PropertyChanged -= OnAppearanceManagerPropertyChanged;
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call System.Windows.FrameworkElement.ApplyTemplate().
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // retrieve BackgroundAnimation storyboard
            if (GetTemplateChild("WindowBorder") is Border border)
            {
                backgroundAnimation = border.Resources["BackgroundAnimation"] as Storyboard;

                if (backgroundAnimation != null)
                {
                    backgroundAnimation.Begin();
                }
            }
        }

        private void OnAppearanceManagerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // start background animation if theme has changed
            if (e.PropertyName == "ThemeSource" && backgroundAnimation != null)
            {
                backgroundAnimation.Begin();
            }
        }

        #region Window
        private void OnCanNavigateLink(object sender, CanExecuteRoutedEventArgs e)
        {
            // true by default
            e.CanExecute = true;

            if (LinkNavigator != null && LinkNavigator.Commands != null)
            {
                // in case of command uri, check if ICommand.CanExecute is true

                // TODO: CanNavigate is invoked a lot, which means a lot of parsing. need improvements??
                if (NavigationHelper.TryParseUriWithParameters(e.Parameter, out Uri uri, out string parameter, out string targetName))
                {
                    if (LinkNavigator.Commands.TryGetValue(uri, out ICommand command))
                    {
                        e.CanExecute = command.CanExecute(parameter);
                    }
                }
            }
        }

        private void OnNavigateLink(object sender, ExecutedRoutedEventArgs e)
        {
            if (LinkNavigator != null)
            {
                if (NavigationHelper.TryParseUriWithParameters(e.Parameter, out Uri uri, out string parameter, out string targetName))
                {
                    LinkNavigator.Navigate(uri, e.Source as FrameworkElement, parameter);
                }
            }
        }

       
        private void OnCanResizeWindow(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ResizeMode == ResizeMode.CanResize || ResizeMode == ResizeMode.CanResizeWithGrip;
        }

        private void OnCanMinimizeWindow(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ResizeMode != ResizeMode.NoResize;
        }

        private void OnCloseWindow(object target, ExecutedRoutedEventArgs e)
        {
#if NET4
            Microsoft.Windows.Shell.SystemCommands.CloseWindow(this);
#else
            SystemCommands.CloseWindow(this);
#endif
        }

        private void OnMaximizeWindow(object target, ExecutedRoutedEventArgs e)
        {
#if NET4
            Microsoft.Windows.Shell.SystemCommands.MaximizeWindow(this);
#else
            SystemCommands.MaximizeWindow(this);
#endif
        }

        private void OnMinimizeWindow(object target, ExecutedRoutedEventArgs e)
        {
#if NET4
            Microsoft.Windows.Shell.SystemCommands.MinimizeWindow(this);
#else
            SystemCommands.MinimizeWindow(this);
#endif
        }

        private void OnRestoreWindow(object target, ExecutedRoutedEventArgs e)
        {
#if NET4
            Microsoft.Windows.Shell.SystemCommands.RestoreWindow(this);
#else
            SystemCommands.RestoreWindow(this);
#endif
        } 
        #endregion
    }
}
