using My.Business.IBusiness.Base_SysManage;
using My.Util;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace My.Business.Business.Base_SysManage
{
    /// <summary>
    /// 描述：系统菜单管理
    /// 作者：wby 2019/11/29 16:14:37
    /// </summary>
    public class SystemMenuManage : ISystemMenuManage, IDependency
    {
        IOperator _operator;
        IPermissionManage _permissionManage;

        private static List<Menu> _allMenu { get; set; }
        private static string _configFile { get => PathHelper.GetAbsolutePath("~/Config/SystemMenu.config"); }
        public static string GetUrl(string virtualUrl) => PathHelper.GetUrl(virtualUrl);

        public SystemMenuManage(IOperator @operator,IPermissionManage permissionManage)
        {
            _operator = @operator;
            _permissionManage = permissionManage;
        }

        static SystemMenuManage()
        {

        }

        private static void InitAllMenu()
        {
            //Action<Menu,XElement> SetMenuProperty=(menu,element)=>
        }


        #region 接口实现
        public List<Menu> GetAllSysMenu()
        {
            throw new NotImplementedException();
        }

        public List<Menu> GetOperatorMenu()
        {
            throw new NotImplementedException();
        } 
        #endregion
    }
}
