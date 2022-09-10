using LibVLCSharp.Shared;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace VLCDemo
{
    /// <summary>
    /// VideoPlayer.xaml 的交互逻辑
    /// </summary>
    public partial class VideoPlayer : UserControl, IDisposable
    {
        private LibVLC libvlc;
        private MediaPlayer mPlayer;
        private MediaPlayer recorder;

        public VideoPlayer()
        {
            InitializeComponent();
        }

        public void SetVideoPlayer()
        {
            try
            {
                Core.Initialize();
                libvlc = new LibVLC("--reset-plugins-cache");
                libvlc.Log += _libvlc_Log;

                mPlayer = new MediaPlayer(libvlc)
                {
                    EnableHardwareDecoding = true
                };

                player.MediaPlayer = mPlayer;

                player.MediaPlayer.Opening += MediaPlayer_Opening;
                player.MediaPlayer.Playing += MediaPlayer_Playing;
                player.MediaPlayer.Stopped += MediaPlayer_Stopped;
                player.MediaPlayer.EncounteredError += MediaPlayer_EncounteredError;
            }
            catch (Exception ex)
            {
                throw new Exception($"执行{nameof(SetVideoPlayer)}错误！", ex);
            }
        }


        #region 依赖属性
        /// <summary>
        /// 视频链接
        /// </summary>
        public string VideoUrl
        {
            get { return (string)GetValue(VideoUrlProperty); }
            set { SetValue(VideoUrlProperty, value); }
        }
        public static readonly DependencyProperty VideoUrlProperty =
            DependencyProperty.Register("VideoUrl", typeof(string), typeof(VideoPlayer), new PropertyMetadata(null,
                new PropertyChangedCallback(VideoUrlPropertyChanged)));

        private static void VideoUrlPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var currentObj = (VideoPlayer)obj;

            if (args.NewValue == null || string.IsNullOrEmpty(args.NewValue?.ToString()))
            {
                currentObj.tipPan.Visibility = Visibility.Collapsed;
                currentObj.player.Visibility = Visibility.Collapsed;
                currentObj.errorText.Text = "视频链接地址为空，请重新播放";
                currentObj.errorText.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// 视频高
        /// </summary>
        public double VideoHeight
        {
            get { return (double)GetValue(VideoHeightProperty); }
            set { SetValue(VideoHeightProperty, value); }
        }

        public static readonly DependencyProperty VideoHeightProperty =
            DependencyProperty.Register("VideoHeight", typeof(double), typeof(VideoPlayer), new PropertyMetadata(200d));

        /// <summary>
        /// 视频宽
        /// </summary>
        public double VideoWidth
        {
            get { return (double)GetValue(VideoWidthProperty); }
            set { SetValue(VideoWidthProperty, value); }
        }

        public static readonly DependencyProperty VideoWidthProperty =
            DependencyProperty.Register("VideoWidth", typeof(double), typeof(VideoPlayer), new PropertyMetadata(200d));

        /// <summary>
        /// 视频标题
        /// </summary>
        public string TitleStr
        {
            get { return (string)GetValue(TitleStrProperty); }
            set { SetValue(TitleStrProperty, value); }
        }

        public static readonly DependencyProperty TitleStrProperty =
            DependencyProperty.Register("TitleStr", typeof(string), typeof(VideoPlayer), new PropertyMetadata(string.Empty));

        /// <summary>
        /// 数据来源本地0 网络1
        /// </summary>
        public int FromTypeSource
        {
            get { return (int)GetValue(FromTypeSourceProperty); }
            set { SetValue(FromTypeSourceProperty, value); }
        }

        public static readonly DependencyProperty FromTypeSourceProperty =
            DependencyProperty.Register("FromTypeSource", typeof(int), typeof(VideoPlayer), new PropertyMetadata(1));

        /// <summary>
        /// 视频类型
        /// </summary>
        public int VideoType
        {
            get { return (int)GetValue(VideoTypeProperty); }
            set { SetValue(VideoTypeProperty, value); }
        }

        public static readonly DependencyProperty VideoTypeProperty =
            DependencyProperty.Register("VideoType", typeof(int), typeof(VideoPlayer), new PropertyMetadata(1, (d, e) =>
            {
                var vp = (VideoPlayer)d;
                var type = int.Parse(e.NewValue.ToString());
                vp.errorText.Visibility = Visibility.Collapsed;
                if (type == 2)
                {
                    vp.reloadBtn.Visibility = Visibility.Collapsed;
                    vp.Pan.Visibility = Visibility.Visible;
                }
                else
                {
                    vp.reloadBtn.Visibility = Visibility.Visible;
                    vp.Pan.Visibility = Visibility.Collapsed;
                }
            }));

        #endregion

        #region Event
        private void Player_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Core.Initialize();
                libvlc = new LibVLC("--reset-plugins-cache");
                libvlc.Log += _libvlc_Log;

                mPlayer = new MediaPlayer(libvlc)
                {
                    EnableHardwareDecoding = true
                };

                player.MediaPlayer = mPlayer;

                player.MediaPlayer.Opening += MediaPlayer_Opening;
                player.MediaPlayer.Playing += MediaPlayer_Playing;
                player.MediaPlayer.EncounteredError += MediaPlayer_EncounteredError;
                player.MediaPlayer.Stopped += MediaPlayer_Stopped;
            }
            catch (Exception ex)
            {
                throw new Exception($"执行{nameof(Player_Loaded)}错误！", ex);
            }
        }

        private void MediaPlayer_Stopped(object sender, EventArgs e)
        {
            Dispatcher.InvokeAsync(() =>
            {
                errorText.Text = "视频已停止播放，请重新加载！";
                errorText.Visibility = Visibility.Visible;
                tipPan.Visibility = Visibility.Collapsed;
            });
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var hostWindow = Window.GetWindow(this);
                if (hostWindow == null)
                {
                    return;
                }
                hostWindow.Closed -= HostWindow_Closed;
                hostWindow.Closed += HostWindow_Closed;
            }
            catch (Exception ex)
            {
                throw new Exception($"执行{nameof(UserControl_Loaded)}错误！", ex);
            }
        }

        private void _libvlc_Log(object sender, LogEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(e.FormattedLog);
        }

        private void MediaPlayer_EncounteredError(object sender, EventArgs e)
        {
            Dispatcher.InvokeAsync(() =>
            {
                errorText.Text = "视频播放错误，请关闭后重新打开！";
                errorText.Visibility = Visibility.Visible;
                tipPan.Visibility = Visibility.Collapsed;
            });
        }

        private void MediaPlayer_Playing(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                errorText.Visibility = Visibility.Collapsed;
                tipPan.Visibility = Visibility.Collapsed;
                player.Visibility = Visibility.Visible;
                tip.Text = "等待中";
            });
        }

        private void MediaPlayer_Opening(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                errorText.Visibility = Visibility.Collapsed;
                tipPan.Visibility = Visibility.Visible;
                player.Visibility = Visibility.Collapsed;
                tip.Text = "加载中";
            });
        }

        private void HostWindow_Closed(object sender, EventArgs e)
        {
            Dispose();
        }

        private void ReloadBtn_Click(object sender, RoutedEventArgs e)
        {
            if (VideoUrl != null)
                Play();
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            _ = StopAsync();
        }

        /// <summary>
        /// 注册路由事件
        /// </summary>
        public static readonly RoutedEvent EnlargeBtnEvent = EventManager.RegisterRoutedEvent("EnlargeBtnCommond",
          RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(VideoPlayer));

        public event RoutedEventHandler EnlargeBtnCommond
        {
            add { AddHandler(EnlargeBtnEvent, value); }
            remove { RemoveHandler(EnlargeBtnEvent, value); }
        }

        private void EnlargeBtn_Click(object sender, RoutedEventArgs e)
        {
            RoutedEventArgs args = new RoutedEventArgs(EnlargeBtnEvent);
            RaiseEvent(args);
        }
        #endregion

        #region 视频操作（播放，停止，抓拍，录像）
        public void Play()
        {
            try
            {
                if (!string.IsNullOrEmpty(VideoUrl))
                {
                    using var mediaPlay = new Media(libvlc, VideoUrl, FromType.FromLocation);
                    SetCommonMediaOptions(mediaPlay);
                    mPlayer.Play(mediaPlay);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"执行{nameof(Play)}错误！", ex);
            }

        }
        public async Task StopAsync()
        {
            // 该方法会阻塞线程。为防止主线程假死，需要在另一个线程调用
            await Task.Run(() => { mPlayer?.Stop(); });
        }

        public void Stop()
        {
            mPlayer?.Stop();
        }

        /// <summary>
        /// 抓拍
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public bool TakeSnapshot(string filePath)
        {
            if (!player.MediaPlayer.IsPlaying)
            {
                return false;
            }
            return player.MediaPlayer.TakeSnapshot(0, filePath, 0, 0);
        }

        /// <summary>
        /// 录像
        /// </summary>
        /// <param name="savedFilePath"></param>
        /// <returns></returns>
        public async Task<(bool isSuccess, string errMsg)> RecordAsync(string savedFilePath)
        {
            var videoUrl = VideoUrl;

            if (string.IsNullOrEmpty(videoUrl))
            {
                return (false, "未获取到视频地址");
            }

            var ret = await Task.Run(() =>
            {
                recorder = new MediaPlayer(libvlc);
                using Media mediaRecord = new Media(libvlc, videoUrl, FromType.FromLocation);
                SetCommonMediaOptions(mediaRecord);
                mediaRecord.AddOption($":sout=#file{{dst={savedFilePath}}}");
                var ok = recorder.Play(mediaRecord);
                return ok;
            });

            return (ret, ret ? "" : "录制失败");
        }

        /// <summary>
        /// 停止录像
        /// </summary>
        /// <returns></returns>
        public async Task StopRecordAsync()
        {
            await Task.Run(() =>
            {
                recorder?.Stop();
            });
        }

        private void SetCommonMediaOptions(Media media)
        {
            media.AddOption(":rtsp-tcp");
            media.AddOption("clock-synchro=0");
            media.AddOption(":live-caching=0");
            media.AddOption(":network-caching=333");
            media.AddOption(":file-caching=0");
            media.AddOption(":grayscale");
            //media.AddOption("sout-keep");
        }
        #endregion

        #region Dispose
        bool disposed = false;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    mPlayer?.Stop();
                    mPlayer?.Dispose();
                    mPlayer = null;

                    recorder?.Stop();
                    recorder?.Dispose();
                    recorder = null;

                    libvlc?.Dispose();
                    libvlc = null;
                }
                disposed = true;
            }
        }

        ~VideoPlayer()
        {
            Dispose(false);
        }
        #endregion
    }
}
