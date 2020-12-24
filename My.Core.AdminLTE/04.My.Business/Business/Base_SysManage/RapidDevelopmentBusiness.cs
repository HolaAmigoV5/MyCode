using Microsoft.AspNetCore.Hosting;
using My.Business.IBusiness.Base_SysManage;
using My.Entity.Base_SysManage;
using My.Util;
using System.Collections.Generic;
using System.Linq;

namespace My.Business.Business.Base_SysManage
{
    /// <summary>
    /// 描述：RapidDevelopmentBusiness
    /// 作者：wby 2019/11/25 16:37:13
    /// </summary>
    public class RapidDevelopmentBusiness : BaseBusiness<Base_DatabaseLink>, IRapidDevelopmentBusiness, IDependency
    {
        #region 构造函数
        public RapidDevelopmentBusiness(IHostingEnvironment hostingEnvironment)
        {
            _contentRootPath = $"{hostingEnvironment.ContentRootPath}\\";
        }
        #endregion

        #region 私有成员
        private string _contentRootPath { get; }
        private DbHelper _dbHelper { get; set; }
        private Dictionary<string, DbTableInfo> _dbTableInfoDic { get; set; } = new Dictionary<string, DbTableInfo>();

        private void BuildEntity(List<TableInfo> tableInfos, string areaName, string tableName)
        {
            string rootPath = _contentRootPath;
            string entityPath = rootPath.Replace("My.Web", "My.Entity") + areaName;
            string filePath = $@"{entityPath}\{tableName}.cs";
            string nameSpace = $@"My.Entity.{areaName}";

            TemplateHelper.SaveEntityToFile(tableInfos, tableName, _dbTableInfoDic[tableName].Description, filePath, nameSpace);
            //_dbHelper.SaveEntityToFile(tableInfos, tableName, _dbTableInfoDic[tableName].Description, filePath, nameSpace);
        }

        private void BuildIBusiness(string areaName,string entityName)
        {
            TemplateHelper.BuildIBusiness(areaName, entityName, _contentRootPath);
        }

        private void BuildBusiness(string areaName,string entityName)
        {
            TemplateHelper.BuildBusiness(areaName, entityName, _contentRootPath);
        }

        private void BuildController(string areaName,string entityName)
        {
            TemplateHelper.BuildController(areaName, entityName, _contentRootPath);
        }

        private void BuildView(List<TableInfo> tableInfoList,string areaName,string entityName)
        {
            TemplateHelper.BuildView(tableInfoList, areaName, entityName, _contentRootPath);
        }

        private DbHelper GetTheDbHelper(string linkId)
        {
            var theLink = GetTheLink(linkId);
            DbHelper dbHelper = DbHelperFactory.GetDbHelper(theLink.DbType, theLink.ConnectionStr);
            return dbHelper;
        }

        private Base_DatabaseLink GetTheLink(string linkId)
        {
            Base_DatabaseLink resObj = new Base_DatabaseLink();
            var theModule = GetIQueryable().Where(x => x.Id == linkId).FirstOrDefault();
            resObj = theModule ?? resObj;

            return resObj;
        }
        #endregion

        #region 接口实现
        /// <summary>
        /// 生成代码
        /// </summary>
        /// <param name="linkId">连接Id</param>
        /// <param name="areaName">区域名</param>
        /// <param name="tables">列表</param>
        /// <param name="buildType">需要生成类型</param>
        public void BuildCode(string linkId, string areaName, string tables, string buildType)
        {
            //内部成员初始化
            _dbHelper = GetTheDbHelper(linkId);
            TemplateHelper.DbHelper = _dbHelper;
        
            GetDbTableList(linkId).ForEach(aTable =>
            {
                _dbTableInfoDic.Add(aTable.TableName, aTable);
            });

            List<string> tableList = tables.ToList<string>();
            List<string> buildTypeList = buildType.ToList<string>();

            tableList.ForEach(aTable => {
                var tableFieldInfo = _dbHelper.GetDbTableInfo(aTable);

                //实体层
                if (buildTypeList.Exists(x => x.ToLower() == "entity"))
                    BuildEntity(tableFieldInfo, areaName, aTable);

                //业务层
                if (buildTypeList.Exists(x => x.ToLower() == "business"))
                {
                    BuildBusiness(areaName, aTable);
                    BuildIBusiness(areaName, aTable);
                }
                //控制器
                if (buildTypeList.Exists(x => x.ToLower() == "controller"))
                    BuildController(areaName, aTable);
                //视图
                if (buildTypeList.Exists(x => x.ToLower() == "view"))
                    BuildView(tableFieldInfo, areaName, aTable);
            });

        }

        /// <summary>
        /// 获取所有数据库连接
        /// </summary>
        /// <returns></returns>
        public List<Base_DatabaseLink> GetAllDbLink()
        {
            return GetList();
        }

        /// <summary>
        /// 获取数据库所有表
        /// </summary>
        /// <param name="linkId">数据库连接Id</param>
        /// <returns></returns>
        public List<DbTableInfo> GetDbTableList(string linkId)
        {
            if (linkId.IsNullOrEmpty())
                return new List<DbTableInfo>();
            else
                return GetTheDbHelper(linkId).GetDbAllTables();
        }
        #endregion
    }
}
