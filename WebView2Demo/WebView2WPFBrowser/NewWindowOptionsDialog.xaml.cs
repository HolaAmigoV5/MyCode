using Microsoft.Web.WebView2.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WebView2WPFBrowser
{
    /// <summary>
    /// NewWindowOptionsDialog.xaml 的交互逻辑
    /// </summary>
    public partial class NewWindowOptionsDialog : Window
    {
        private CoreWebView2CreationProperties? _creationProperties = null;
        public CoreWebView2CreationProperties? CreationProperties
        {
            get
            {
                return _creationProperties;
            }
            set
            {
                _creationProperties = value;
                if (_creationProperties == null)
                {
                    // Reset the controls to defaults.
                    BrowserExecutableFolder.Text = null;
                    UserDataFolder.Text = null;
                    EnvLanguage.Text = null;
                    ProfileName.Text = null;
                    comboBox_IsInPrivateModeEnabled.SelectedIndex = 2;
                }
                else
                {
                    // Copy the values to the controls.
                    BrowserExecutableFolder.Text = _creationProperties.BrowserExecutableFolder;
                    UserDataFolder.Text = _creationProperties.UserDataFolder;
                    EnvLanguage.Text = _creationProperties.Language;
                    ProfileName.Text = _creationProperties.ProfileName;
                    if (_creationProperties.IsInPrivateModeEnabled == null)
                    {
                        comboBox_IsInPrivateModeEnabled.SelectedIndex = 2;
                    }
                    else if (_creationProperties.IsInPrivateModeEnabled == true)
                    {
                        comboBox_IsInPrivateModeEnabled.SelectedIndex = 0;
                    }
                    else if (_creationProperties.IsInPrivateModeEnabled == false)
                    {
                        comboBox_IsInPrivateModeEnabled.SelectedIndex = 1;
                    }
                }
            }
        }

        public NewWindowOptionsDialog()
        {
            InitializeComponent();

            CreationProperties = new CoreWebView2CreationProperties();
            BrowserExecutableFolder.Focus();
            BrowserExecutableFolder.SelectAll();
        }

        void OK_Clicked(object sender, RoutedEventArgs args)
        {
            if (CreationProperties != null)
            {
                CreationProperties.BrowserExecutableFolder = BrowserExecutableFolder.Text == "" ? null : BrowserExecutableFolder.Text;
                CreationProperties.UserDataFolder = UserDataFolder.Text == "" ? null : UserDataFolder.Text;
                CreationProperties.Language = EnvLanguage.Text == "" ? null : EnvLanguage.Text;
                CreationProperties.ProfileName = ProfileName.Text == "" ? null : ProfileName.Text;

                if (comboBox_IsInPrivateModeEnabled.SelectedIndex == 0)
                {
                    CreationProperties.IsInPrivateModeEnabled = true;
                }
                else if (comboBox_IsInPrivateModeEnabled.SelectedIndex == 1)
                {
                    CreationProperties.IsInPrivateModeEnabled = false;
                }
                else if (comboBox_IsInPrivateModeEnabled.SelectedIndex == 2)
                {
                    CreationProperties.IsInPrivateModeEnabled = null;
                }
            }

            DialogResult = true;
        }
    }
}
