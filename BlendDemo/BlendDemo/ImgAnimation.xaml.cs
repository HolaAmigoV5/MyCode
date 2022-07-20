using System.Collections.Generic;
using System.Windows.Controls;
using System.Threading.Tasks;
using System;
using System.IO;
using Newtonsoft.Json;

namespace BlendDemo
{
    /// <summary>
    /// ImgAnimation.xaml 的交互逻辑
    /// </summary>
    public partial class ImgAnimation : UserControl
    {
        string baseUrl = "http://47.103.223.121:8090/";
        public ImgAnimation()
        {
            InitializeComponent();

            Images imgs = ReadJsonData();
            ImageList = TransformImgUrl(imgs);

            //ImageList = new List<string> {
            //"https://img.zcool.cn/community/01f6dc5a74149ba80120a12366a277.jpg@1280w_1l_2o_100sh.jpg",
            //"https://www.euweb.cn/wp-content/uploads/2016/12/302636-106.jpg",
            //"https://youimg1.c-ctrip.com/target/0104a120008ah3n3q93E0_D_10000_1200.jpg?proc=autoorient",
            //"https://www.keaidian.com/uploads/allimg/190424/24110307_8.jpg",
            //"https://img.zcool.cn/community/01c8f15aeac135a801207fa16836ae.jpg@1280w_1l_2o_100sh.jpg"
            //};

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


        private Images ReadJsonData()
        {
            string jsonPath = Path.Combine(Environment.CurrentDirectory, "Assets", "imgs.json");
            string data = ReadFile(jsonPath);
            Images images = DeserializeJsonToObject<Images>(data);
            return images;
        }

        private List<string> TransformImgUrl(Images images)
        {
            var list = new List<string>(images.Data.Count);
            for (int i = 0; i < images.Data.Count; i++)
            {
                list.Add($"{baseUrl}{images.Data[i].Value}");
            }
            return list;
        }

        public static string ReadFile(string Path)
        {
            string s;
            if (!File.Exists(Path))
            {
                s = "不存在相应的目录";
            }
            else
            {
                StreamReader f2 = new StreamReader(Path, System.Text.Encoding.UTF8);
                s = f2.ReadToEnd();
                f2.Close();
                f2.Dispose();
            }

            return s;
        }

        public T DeserializeJsonToObject<T>(string json) where T : class
        {
            JsonSerializer serializer = new JsonSerializer();
            StringReader sr = new StringReader(json);
            object o = serializer.Deserialize(new JsonTextReader(sr), typeof(T));
            T t = o as T;
            return t;
        }
    }

    public class Images
    {
        public string Msg { get; set; }
        public int Code { get; set; }
        public List<StrValue> Data { get; set; }
    }

    public class StrValue
    {
        public string Str { get; set; }
        public string Value { get; set; }
    }
}
