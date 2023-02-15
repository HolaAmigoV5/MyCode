using System.Windows;
using System.Windows.Media;

namespace GeneralUpdateDemo.Upgrade
{
    public class ViewData : DependencyObject
    {
        /// <summary>
        /// 已经下载文件大小
        /// </summary>
        public int HandledCount
        {
            get { return (int)GetValue(HandledCountProperty); }
            set { SetValue(HandledCountProperty, value); }
        }

        public static readonly DependencyProperty HandledCountProperty =
            DependencyProperty.Register("HandledCount", typeof(int), typeof(ViewData), new PropertyMetadata(0));

        /// <summary>
        /// 当前更新包需要下载的总大小
        /// </summary>
        public int TotalCount
        {
            get { return (int)GetValue(TotalCountProperty); }
            set { SetValue(TotalCountProperty, value); }
        }

        public static readonly DependencyProperty TotalCountProperty =
            DependencyProperty.Register("TotalCount", typeof(int), typeof(ViewData), new PropertyMetadata(0));

        /// <summary>
        /// 处理过程信息
        /// </summary>
        public string HandleText
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("HandleText", typeof(string), typeof(ViewData), new PropertyMetadata(""));

        /// <summary>
        /// 主提示信息
        /// </summary>
        public string MainText
        {
            get { return (string)GetValue(MainTextProperty); }
            set { SetValue(MainTextProperty, value); }
        }

        public static readonly DependencyProperty MainTextProperty =
            DependencyProperty.Register("MainText", typeof(string), typeof(ViewData), new PropertyMetadata("程序正在升级，请稍候..."));


        /// <summary>
        /// 主提示信息文本颜色
        /// </summary>
        public Brush MainTextColor
        {
            get { return (Brush)GetValue(MainTextColorProperty); }
            set { SetValue(MainTextColorProperty, value); }
        }

        public static readonly DependencyProperty MainTextColorProperty =
            DependencyProperty.Register("MainTextColor", typeof(Brush), typeof(ViewData), new PropertyMetadata(new SolidColorBrush(Colors.White)));
    }
}
