using System;
using System.Windows;

namespace WpfTestWithPrism.Views
{
    /// <summary>
    /// Interaction logic for VideoForm.xaml
    /// </summary>
    public partial class VideoForm
    {
        private string _videoPath;
        public VideoForm()
        {
            InitializeComponent();
            _videoPath = "rtmp://rtmp01open.ys7.com/openlive/4d3dbb462ff541a890bfbcaedbb633c0";
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            MediaPlayer.Source = new Uri(_videoPath);
            MediaPlayer.Play();
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            MediaPlayer.Stop();
        }

        private void btnForward_Click(object sender, RoutedEventArgs e)
        {
            MediaPlayer.Position += TimeSpan.FromSeconds(20);
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            MediaPlayer.Position -= TimeSpan.FromSeconds(20);
        }

        private void MediaPlayer_MediaOpened(object sender, RoutedEventArgs e)
        {
            int duration = MediaPlayer.NaturalDuration.TimeSpan.Seconds;
        }

        private void media_Failed(object sender, ExceptionRoutedEventArgs e)
        {
            var txt = e.ErrorException.Message;
        }
    }
}
