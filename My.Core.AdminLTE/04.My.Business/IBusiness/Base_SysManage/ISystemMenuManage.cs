using Coldairarrow.Util;
using My.Util;
using System.Collections.Generic;

namespace My.Business.IBusiness.Base_SysManage
{
    /// <summary>
    /// 描述：系统菜单管理接口
    /// 作者：wby 2019/11/29 16:15:26
    /// </summary>
    public interface ISystemMenuManage
    {
        /// <summary>
        /// 获取系统所有菜单
        /// </summary>
        /// <returns></returns>
        List<Menu> GetAllSysMenu();

        /// <summary>
        /// 获取用户菜单
        /// </summary>
        /// <returns></returns>
        List<Menu> GetOperatorMenu();
    }

    public class Menu
    {
        public string Id { get => Url ?? IdHelper.GetId(); }
        public string Text { get; set; }
        public string Icon { get; set; }
        public string Url { get => PathHelper.GetUrl(VirtualUrl); set => VirtualUrl = value; }
        public string VirtualUrl { get; set; }
        public string Permission { get; set; }
        public bool IsShow { get; set; } = true;
        public string targetType { get; } = "iframe-tab";
        public bool IsHeader { get; } = false;
        public List<Menu> Children { get; set; }
    }
}
