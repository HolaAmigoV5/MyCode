using DaJuTestDemo.Core.Presentation;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace DaJuTestDemo.Core
{
    public class DefaultLinkNavigator : ILinkNavigator
    {
        public CommandDictionary Commands { get; set; } = new CommandDictionary();

        private string[] externalSchemes = new string[] { Uri.UriSchemeHttp, Uri.UriSchemeHttps, Uri.UriSchemeMailto };
        public string[] ExternalSchemes
        {
            get => externalSchemes;
            set => externalSchemes = value;
        }

        public DefaultLinkNavigator()
        {
            // register all ApperanceManager commands
            Commands.Add(new Uri("cmd://accentcolor"), AppearanceManager.Current.AccentColorCommand);
            Commands.Add(new Uri("cmd://darktheme"), AppearanceManager.Current.DarkThemeCommand);
            Commands.Add(new Uri("cmd://largefontsize"), AppearanceManager.Current.LargeFontSizeCommand);
            Commands.Add(new Uri("cmd://lighttheme"), AppearanceManager.Current.LightThemeCommand);
            Commands.Add(new Uri("cmd://settheme"), AppearanceManager.Current.SetThemeCommand);
            Commands.Add(new Uri("cmd://smallfontsize"), AppearanceManager.Current.SmallFontSizeCommand);

            // register navigation commands
            Commands.Add(new Uri("cmd://browseback"), NavigationCommands.BrowseBack);
            Commands.Add(new Uri("cmd://refresh"), NavigationCommands.Refresh);

            // register application commands
            Commands.Add(new Uri("cmd://copy"), ApplicationCommands.Copy);
        }

        public void Navigate(Uri uri, FrameworkElement source, string parameter = null)
        {

            if (uri == null)
            {
                throw new ArgumentNullException("uri");
            }

            // first check if uri refers to a command
            if (Commands != null && Commands.TryGetValue(uri, out ICommand command))
            {
                // note: not executed within BBCodeBlock context, Hyperlink instance has Command and CommandParameter set
                if (command.CanExecute(parameter))
                {
                    command.Execute(parameter);
                }
                else
                {
                    // do nothing
                }
            }
            else if (uri.IsAbsoluteUri && externalSchemes != null && externalSchemes.Any(s => uri.Scheme.Equals(s, StringComparison.OrdinalIgnoreCase)))
            {
                // uri is external, load in default browser
                Process.Start(uri.AbsoluteUri);
                return;
            }
            else
            {
                // perform frame navigation
                if (source == null)
                {   // source required
                    throw new ArgumentException(string.Format(CultureInfo.CurrentUICulture, $"Unable to navigate to {uri}, no source specified"));
                }

                // use optional parameter as navigation target to identify target frame (_self, _parent, _top or named target frame)
                var frame = NavigationHelper.FindFrame(parameter, source);
                if (frame == null)
                {
                    throw new ArgumentException(string.Format(CultureInfo.CurrentUICulture, "Unable to navigate to {0}, could not find a ModernFrame target '{1}'", uri, parameter));
                }

                // delegate navigation to the frame
                frame.Source = uri;
            }
        }
    }
}
