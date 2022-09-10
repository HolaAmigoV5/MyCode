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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VLCDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainWindowModel model;
        public MainWindow()
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;

            model = new MainWindowModel();
            DataContext = model;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                player1.SetVideoPlayer();
                player2.SetVideoPlayer();

                //Stop();
                model.GetData();
                Play();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private void Play()
        {
            player1.Play();
            player2.Play();
        }

        private void Stop()
        {
            player1.Stop();
            player2.Stop();
        }

        private void btn_click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Stop();
                model.SwitchVideo();
                Play();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private void btn_click1(object sender, RoutedEventArgs e)
        {
            try
            {
                //Stop();
                model.SwitchVideo2();
                Play();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }

    public class MainWindowModel : BindableBase
    {
        public string DriviceId;

        private string webCamURL01;
        public string WebCamURL01
        {
            get => webCamURL01;
            set => SetProperty(ref webCamURL01, value);
        }
        private string webCamName01;
        public string WebCamName01
        {
            get => webCamName01;
            set => SetProperty(ref webCamName01, value);
        }
        private string webCamURL02;
        public string WebCamURL02
        {
            get => webCamURL02;
            set => SetProperty(ref webCamURL02, value);
        }
        private string webCamName02;
        public string WebCamName02
        {
            get => webCamName02;
            set => SetProperty(ref webCamName02, value);
        }

        public void GetData()
        {
            WebCamName01 = "垃圾厢房左";
            WebCamURL01 = "rtsp://123.60.13.203:10554/rtp/44010200491110000016_44010200491320000005";
            WebCamName02 = "垃圾厢房右";
            WebCamURL02 = "rtsp://123.60.13.203:10554/rtp/44010200491110000017_44010200491320000006";
        }

        private const string urlHead = "rtsp://123.60.13.203:10554/rtp/";
        int i = 1;
        public void SwitchVideo()
        {
            if (i >= 20)
                i = 1;

            WebCamName01 = $"车棚摄像头{i}";
            WebCamURL01 = $"{urlHead}4401020049111000010{i:D2}_340200000013200000{i:D2}";
            i++;
            WebCamName02 = $"车棚摄像头{i}";
            WebCamURL02 = $"{urlHead}4401020049111000010{i:D2}_340200000013200000{i:D2}";
            i++;
        }

        int m = 11, n = 0;
        public void SwitchVideo2()
        {
            if (m >= 17 && n >= 6)
            {
                m = 11;
                n = 0;
            }

            WebCamName01 = $"车棚摄像头{m}";
            WebCamURL01 = $"{urlHead}440102004911100000{m}_4401020049132000000{n}";
            m++; n++;
            WebCamName02 = $"车棚摄像头{m}";
            WebCamURL02 = $"{urlHead}440102004911100000{m}_4401020049132000000{n}";
            m++; n++;
        }
    }
}
