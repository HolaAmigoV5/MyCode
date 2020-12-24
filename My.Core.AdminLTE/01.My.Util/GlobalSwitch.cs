using Microsoft.AspNetCore.Hosting;
using System;

namespace My.Util
{
    /// <summary>
    /// 描述：全局控制
    /// 作者：wby 2019/9/23 17:14:51
    /// </summary>
    public class GlobalSwitch
    {
        #region 构造函数
        public GlobalSwitch()
        {
#if !DEBUG
            RunModel=RunModel.Publish;
#endif
        }
        #endregion

        #region 参数
        /// <summary>
        /// 项目名称
        /// </summary>
        public static string ProjectName { get; } = "My.Core.AdminLTE";

        /// <summary>
        /// 网站根地址
        /// </summary>
        public static string WebRootUrl { get; set; } = "http://localhost:50000";
        #endregion

        #region 运行
        /// <summary>
        /// 运行模式
        /// </summary>
        public static RunModel RunModel { get; } = RunModel.LocalTest;

        /// <summary>
        /// 网站文件根路径
        /// </summary>
        public static string WebRootPath { get => AutofacHelper.GetScopeService<IHostingEnvironment>().WebRootPath; } 
        #endregion

        #region 数据库
        /// <summary>
        /// 默认数据库类型
        /// </summary>
        public static DatabaseType DatabaseType { get; } = DatabaseType.SqlServer;

        /// <summary>
        /// 默认数据库连接名
        /// </summary>
        public static string DefaultDbConName { get; } = "DbBase";
        #endregion

        #region 缓存
        /// <summary>
        /// 默认缓存
        /// </summary>
        public static CacheType CacheType { get; } = CacheType.SystemCache;

        /// <summary>
        /// Redis配置字符串
        /// </summary>
        public static string RedisConfig { get; } = null;
        #endregion

        #region 日志相关
        /// <summary>
        /// 日志记录方式
        /// 注:可用位运算,LoggerType.RDBMS | LoggerType.File表示同时记录到数据库和文件
        /// </summary>
        public static LoggerType LoggerType { get; set; } = LoggerType.RDBMS;

        /// <summary>
        /// ElasticSearch服务器配置
        /// </summary>
        public static Uri[] ElasticSearchNodes { get; set; } = new Uri[] { new Uri("http://localhost:9200/") }; 
        #endregion
    }

    #region 枚举
    /// <summary>
    /// 运行模式
    /// </summary>
    public enum RunModel
    {
        /// <summary>
        /// 本地测试模式，默认Admin账户，不需要登录
        /// </summary>
        LocalTest,

        /// <summary>
        /// 发布模式
        /// </summary>
        Publish
    }

    /// <summary>
    /// 默认缓存类型
    /// </summary>
    public enum CacheType
    {
        /// <summary>
        /// 系统缓存
        /// </summary>
        SystemCache,

        /// <summary>
        /// Redis缓存
        /// </summary>
        RedisCache
    } 
    #endregion
}
