﻿using Microsoft.Web.WebView2.Core;
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
    /// ClientCertificateSelectionDialog.xaml 的交互逻辑
    /// </summary>
    public partial class ClientCertificateSelectionDialog : Window
    {
        public ClientCertificateSelectionDialog(string? title = null, string? host = null, int port = 0,
            IReadOnlyList<CoreWebView2ClientCertificate>? client_cert_list = null)
        {
            InitializeComponent();
            if (title != null)
            {
                Title = title;
            }
            if (host != null && port > 0)
            {
                Description.Text = string.Format("Site {0}:{1} needs your credentials:", host, port);
            }
            if (client_cert_list != null)
            {
                CertificateDataBinding.SelectedItem = client_cert_list[0];
                CertificateDataBinding.ItemsSource = client_cert_list;
            }
        }

        void ok_Clicked(object sender, RoutedEventArgs args)
        {
            DialogResult = true;
        }
    }
}
