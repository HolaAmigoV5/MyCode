using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MaterialDesignThemes.Wpf.Transitions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Media;

namespace CommandDemo.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            kinds = new List<TransitionEffectKind>
            {
                TransitionEffectKind.ExpandIn,
                TransitionEffectKind.FadeIn,
                TransitionEffectKind.SlideInFromLeft,
                TransitionEffectKind.SlideInFromTop,
                TransitionEffectKind.SlideInFromRight,
                TransitionEffectKind.SlideInFromBottom
            };

            #region ÑÕÉ«
            colors = new List<string>
            {
                "#FF8C69",
                "#FF8247",
                "#FF7256",
                "#FF6347",
                "#FFA07A",
                "#FF82AB",
                "#FF7F00",
                "#FF69B4",
                "#FFA500",
                "#FF83FA",
                "#FF7F24",
                "#FF6A6A",
                "#FFA54F",
                "#FF8C00",
                "#FF7F50",
                "#FF6EB4",
                "#00CED1",
                "#00C5CD",
                "#008B8B",
                "#00688B",
                "#00E5EE",
                "#00CD00",
                "#009ACD",
                "#00868B",
                "#00EE00",
                "#00CD66",
                "#00B2EE",
                "#008B00",
                "#00EE76",
                "#00CDCD",
                "#00BFFF",
                "#008B45",
                "#87CEFF",
                "#858585",
                "#838B83",
                "#7FFF00",
                "#7D7D7D",
                "#87CEFA",
                "#848484",
                "#836FFF",
                "#7F7F7F",
                "#7D26CD",
                "#87CEEB",
                "#8470FF",
                "#828282",
                "#7EC0EE",
                "#7CFC00",
                "#878787",
                "#838B8B",
                "#7FFFD4",
                "#7D9EC0",
                "#7CCD7C"
            };
            #endregion

            metroInfos = new ObservableCollection<MetroInfo>();
            RefCommand = new RelayCommand(async () =>
              {
                  metroInfos.Clear();
                  for (int i = 0; i < 60; i++)
                  {
                      metroInfos.Add(new MetroInfo()
                      {
                          Name = "APP" + i.ToString(),
                          Color = new SolidColorBrush((Color)ColorConverter.ConvertFromString(colors[new Random().Next(0, 51)])),
                          Width = new Random().Next(0, 8) == 3 ? 206 : 100,
                          Height = 100,
                          Effact = new TransitionEffect()
                          {
                              Kind = kinds[new Random().Next(2, 6)],
                              Duration = new TimeSpan(0, 0, 0, 0, 900)
                          }
                      });
                      await Task.Delay(10);
                  }

              });
        }

        public RelayCommand RefCommand { get; set; }
        public List<TransitionEffectKind> kinds;
        public List<string> colors;

        private ObservableCollection<MetroInfo> metroInfos;
        public ObservableCollection<MetroInfo> MetroInfos
        {
            get { return metroInfos; }
            set { metroInfos = value; RaisePropertyChanged(); }
        }
    }

    public class MetroInfo
    {
        public string Name { get; set; }
        public Brush Color { get; set; }
        public TransitionEffect Effact { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}