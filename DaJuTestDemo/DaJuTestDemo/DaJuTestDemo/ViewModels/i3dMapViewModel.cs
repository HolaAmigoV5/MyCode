using DaJuTestDemo.Common;
using Prism.Mvvm;
using System;
using System.Collections.Generic;

namespace DaJuTestDemo.ViewModels
{
    public class I3dMapViewModel : BindableBase
    {
        public I3dMapViewModel()
        {
            //LoadShpFile();
        }

        private void LoadShpFile()
        {
            string sShpFileName = Environment.CurrentDirectory + @"\data\ShapeFile\XJDL.shp";
            ShpRead m_Shp = new ShpRead();
            // 初始化GDAL和OGR
            m_Shp.InitinalGdal();
            // 
            m_Shp.GetShpLayer(sShpFileName);
            // 获取所有属性字段名称,存放在m_FeildList中
            m_Shp.GetFeilds();

            m_Shp.GetFeildContent(0, out List<string> FeildStringList);

            // 获取某条FID的数据
            m_Shp.GetGeometry(0);
        }
    }
}
