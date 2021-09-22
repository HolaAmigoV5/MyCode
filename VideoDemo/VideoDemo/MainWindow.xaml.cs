using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;

namespace VideoDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string currentDirectory = Environment.CurrentDirectory;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btn_click(object sender, RoutedEventArgs e)
        {
            var vlcLibDirectory = new DirectoryInfo(Path.Combine(currentDirectory, "libvlc", IntPtr.Size == 4 ? "win-x86" : "win-x64"));
            var options = new string[] {
                 "--file-logging", "-vvv", "--logfile=Logs.log","--rtsp-timeout=300","--rtsp-tcp"
            };
            myControl.SourceProvider.CreatePlayer(vlcLibDirectory, options);
            myControl.SourceProvider.MediaPlayer.Play("rtsp://47.110.152.146:10657/JT808://47.110.152.146:25010:1:1:20815821905?pushuuid=c9131e41954efaa28&recvuuid=669a824f9c6d7ac3b&mode=0");
        }
    }
}
