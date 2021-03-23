using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Windows.Media;
using Wby.Demo.Shared.Attributes;
using Wby.Demo.Shared.Common.Enums;
using Wby.PrismDemo.PC.Infrastructure.Extensions;

namespace Wby.PrismDemo.PC.ViewModels
{
    [Module("个性化", ModuleType.系统配置)]
    public class SkinViewModel : BindableBase
    {
        #region Properties
        public readonly static PaletteHelper _paletteHelper = new PaletteHelper();
        //可选颜色集合-分组
        public IEnumerable<ISwatch> Swatches { get; } = SwatchHelper.Swatches;
        #endregion

        #region Command

        private DelegateCommand<object> _toggleBaseCommand;
        /// <summary>
        /// 改变主题
        /// </summary>
        public DelegateCommand<object> ToggleBaseCommand => _toggleBaseCommand ??= new DelegateCommand<object>(o=>ChangeThemes((bool)o));
        private void ChangeThemes(bool isDark)
        {
            ModifyTheme(theme => theme.SetBaseTheme(isDark ? Theme.Dark : Theme.Light));
        }

        public DelegateCommand<object> _changeHueCommand;
        /// <summary>
        /// 改变颜色
        /// </summary>
        public DelegateCommand<object> ChangeHueCommand => _changeHueCommand ?? (new DelegateCommand<object>(ChangeHue));

        private void ChangeHue(object obj)
        {
            var hue = (Color)obj;
            _paletteHelper.ChangePrimaryColor(hue);
        }
        #endregion

        #region Methods
        private static void ModifyTheme(Action<ITheme> modificationAction)
        {
            var paletteHelper = new PaletteHelper();
            ITheme theme = paletteHelper.GetTheme();
            modificationAction?.Invoke(theme);
            paletteHelper.SetTheme(theme);
        }
        #endregion
    }
}
