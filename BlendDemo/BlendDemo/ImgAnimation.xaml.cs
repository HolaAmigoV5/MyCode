using System.Collections.Generic;
using System.Windows.Controls;
using System.Threading.Tasks;

namespace BlendDemo
{
    /// <summary>
    /// ImgAnimation.xaml 的交互逻辑
    /// </summary>
    public partial class ImgAnimation : UserControl
    {
        public ImgAnimation()
        {
            InitializeComponent();

            ImageList = new List<string> {
            "https://img.zcool.cn/community/01f6dc5a74149ba80120a12366a277.jpg@1280w_1l_2o_100sh.jpg",
            "https://www.euweb.cn/wp-content/uploads/2016/12/302636-106.jpg",
            "https://youimg1.c-ctrip.com/target/0104a120008ah3n3q93E0_D_10000_1200.jpg?proc=autoorient",
            "https://www.keaidian.com/uploads/allimg/190424/24110307_8.jpg",
            "https://img.zcool.cn/community/01c8f15aeac135a801207fa16836ae.jpg@1280w_1l_2o_100sh.jpg"
            };

            DataContext = this;
        }

        public List<string> ImageList { get; set; }

        private async void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            int i = -1;
            while (i++ < ImageList.Count)
            {
                await Task.Delay(3000);
                if (i == ImageList.Count)
                    i = 0;
                imgList.SelectedIndex = i;
            }
        }
    }
}
