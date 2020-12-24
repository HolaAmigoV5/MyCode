using StackExchange.Redis;

namespace My.Util.DataAccess
{
    /// <summary>
    /// 描述：Redis帮助类
    /// 作者：wby 2019/10/22 14:51:32
    /// </summary>
    public class RedisHelper
    {
        /// <summary>
        /// 获取Redis连接
        /// </summary>
        /// <param name="serverIp">Redis服务器Ip</param>
        /// <param name="port">端口</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        public static ConnectionMultiplexer GetConnection(string serverIp="localhost", int port=6379,string password=null)
        {
            string config = string.Empty;
            config = $"{serverIp}:{port}";
            if (!password.IsNullOrEmpty())
                config += $", password={password}";

            return GetConnection(config);
        }

        /// <summary>
        /// 获取Redis连接
        /// 注：此对象无需一直创建，建议使用单列模式
        /// </summary>
        /// <param name="config">配置字符串</param>
        /// <returns></returns>
        public static ConnectionMultiplexer GetConnection(string config)
        {
            return ConnectionMultiplexer.Connect(config);
        }
    }
}
