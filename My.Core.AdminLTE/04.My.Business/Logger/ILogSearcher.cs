using My.Entity.Base_SysManage;
using My.Util;
using System;
using System.Collections.Generic;

namespace My.Business
{
    public interface ILogSearcher
    {
        List<Base_SysLog> GetLogList(
            Pagination pagination,
            string logContent,
            string logType,
            string level,
            string opUserName,
            DateTime? startTime,
            DateTime? endTime);
    }
}
