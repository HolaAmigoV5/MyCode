using System;
using System.Collections.Generic;
using System.Text;
using Wby.Demo.PC.Common;
using Wby.Demo.PC.View;
using Wby.Demo.Shared.Attributes;
using Wby.Demo.Shared.Common.Enums;
using Wby.Demo.Shared.Dto;
using Wby.Demo.ViewModel.Interfaces;

namespace Wby.Demo.PC.ViewCenter
{
    /// <summary>
    /// 用户菜单控制类
    /// </summary>
    [Module("菜单管理", ModuleType.系统配置)]
    public class MenuCenter : ModuleCenter<MenuView, MenuDto>, IMenuCenter
    {
        public MenuCenter(IMenuViewModel viewModel):base(viewModel)
        {

        }

        public override void BindDataGridColumns()
        {
            VisualHelper.SetDataGridColumns(view, "Grid", typeof(MenuDto));
        }
    }
}
