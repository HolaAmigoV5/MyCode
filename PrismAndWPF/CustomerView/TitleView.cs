using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CustomerView
{
    public class TitleView : ViewBase
    {
        private DataTemplate itemTemplate;
        public DataTemplate ItemTemplate
        {
            get => itemTemplate;
            set => itemTemplate = value;
        }

        /// <summary>
        /// 整个ListView控件样式
        /// </summary>
        protected override object DefaultStyleKey
        {
            get { return new ComponentResourceKey(GetType(), "TileView"); }
        }

        /// <summary>
        /// 每个ListViewItem样式
        /// </summary>
        protected override object ItemContainerDefaultStyleKey
        {
            get { return new ComponentResourceKey(GetType(), "TileViewItem"); }
        }

        private Brush selectedBackground = Brushes.Transparent;
        public Brush SelectedBackground
        {
            get => selectedBackground;
            set => selectedBackground = value;
        }

        private Brush selectedBorderBrush = Brushes.Black;
        public Brush SelectedBorderBrush
        {
            get => selectedBorderBrush;
            set => selectedBorderBrush = value;
        }
    }
}
