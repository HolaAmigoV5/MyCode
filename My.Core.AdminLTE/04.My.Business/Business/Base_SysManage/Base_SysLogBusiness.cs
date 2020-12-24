using My.Business.IBusiness.Base_SysManage;
using My.Entity.Base_SysManage;
using My.Util;
using System;
using System.Collections.Generic;

namespace My.Business.Business.Base_SysManage
{
    /// <summary>
    /// 描述：日志查询
    /// 作者：wby 2019/11/25 15:53:41
    /// </summary>
    public class Base_SysLogBusiness : BaseBusiness<Base_SysLog>, IBase_SysLogBusiness, IDependency
    {
        /// <summary>
        /// 获取日志列表
        /// </summary>
        /// <param name="logContent">日志内容</param>
        /// <param name="logType">日志类型</param>
        /// <param name="level">日志级别</param>
        /// <param name="opUserName">操作人用户名</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="pagination">分页参数</param>
        /// <returns></returns>
        public List<Base_SysLog> GetLogList(Pagination pagination, string logContent, string logType,
            string level, string opUserName, DateTime? startTime, DateTime? endTime)
        {
            ILogSearcher logSearcher = null;
            if (GlobalSwitch.LoggerType.HasFlag(LoggerType.RDBMS))
                logSearcher = new RDBMSTarget();
            else if (GlobalSwitch.LoggerType.HasFlag(LoggerType.ElasticSearch))
                logSearcher = new ElasticSearchTarget();
            else
                throw new Exception("请指定日志类型为RDBMS或ElasticSearch!");

            return logSearcher.GetLogList(pagination, logContent, logType, level, opUserName, startTime, endTime);
        }
    }
}
