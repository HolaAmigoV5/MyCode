using Skyversation.UCAS.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace _3DMapWebView2
{
    /// <summary>
    /// 自定义宿主类，用于向网页注册C# 对象，供JS调用
    /// </summary>
    [ClassInterface(ClassInterfaceType.None)]
    [ComVisible(true)]
    public class CustomWebView2HostObject
    {
        public bool BuildingVisiable(bool flag = false)
        {
            bool res;
            try
            {
                CommonClass.MapOperation.BuildingVisibility(flag);
                res = true;
            }
            catch (Exception)
            {
                res = false;
            }

            return res;
        }

        public bool InitlizationPos()
        {
            bool res;
            try
            {
                CommonClass.MapOperation.InitlizedCameraPosition();
                res = true;
            }
            catch (Exception)
            {
                res = false;
            }

            return res;
        }
    }
}
