using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Media;
using Wby.Demo.PC.Common;
using Wby.Demo.PC.View;
using Wby.Demo.Shared.Attributes;
using Wby.Demo.Shared.Common;
using Wby.Demo.Shared.Common.Enums;
using Wby.Demo.ViewModel.Interfaces;

namespace Wby.Demo.PC.ViewCenter
{
    /// <summary>
    /// 样式控制类
    /// </summary>
    [Module("个性化", ModuleType.系统配置)]
    public class SkinCenter : ComponentCenter<SkinView>, ISkinCenter
    {
        public SkinCenter(ISkinViewModel viewModel) : base(viewModel) { }
    }

    /// <summary>
    /// 系统样式设置
    /// </summary>
    public class SkinViewModel : ObservableObject, ISkinViewModel
    {
        private readonly static PaletteHelper _paletteHelper = new PaletteHelper();

        public string SelectPageTitle { get; } = "个性化设置";

        private ObservableCollection<CommandStruct> toolBarCommandList;
        public ObservableCollection<CommandStruct> ToolBarCommandList
        {
            get { return toolBarCommandList; }
            set { SetProperty(ref toolBarCommandList, value); }
        }


        //可选颜色集合-分组
        public IEnumerable<ISwatch> Swatches { get; } = SwatchHelper.Swatches;

        //改变颜色
        public RelayCommand<object> ChangeHueCommand { get; } = new RelayCommand<object>((t) => ChangeHue(t));

        //改变主题
        public RelayCommand<object> ToggleBaseCommand { get; } = new RelayCommand<object>(o => ApplyBase((bool)o));

        public AsyncRelayCommand<string> ExecuteCommand { get; }

        private static void ApplyBase(bool isDark)
        {
            ModifyTheme(theme => theme.SetBaseTheme(isDark ? Theme.Dark : Theme.Light));
        }

        private static void ModifyTheme(Action<ITheme> modificationAction)
        {
            PaletteHelper paletteHelper = new PaletteHelper();
            ITheme theme = paletteHelper.GetTheme();
            modificationAction?.Invoke(theme);
            paletteHelper.SetTheme(theme);
        }

        private static void ChangeHue(object obj)
        {
            var hue = (Color)obj;
            _paletteHelper.ChangePrimaryColor(hue);
        }
    }
}
