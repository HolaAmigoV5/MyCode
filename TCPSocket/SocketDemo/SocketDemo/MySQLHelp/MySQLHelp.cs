using MySql.Data.MySqlClient;
using NLog;
using SocketDemo.Modes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketDemo.MySQLHelp
{
    public class MySQLHelp
    {
        /// <summary>
        /// 日志
        /// </summary>
        Logger log = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// 判断数据库是否连接
        /// </summary>
        public bool isConn = false;
#if !DEBUG
        /// <summary>
        /// 服务器数据库用户
        /// </summary>
        private static readonly string databaseUser = "root";
        /// <summary>
        /// 服务器数据库密码
        /// </summary>
        private static readonly string pwd = "mypassword0902";
        /// <summary>
        /// 服务器数据库
        /// </summary>
        private static readonly string databaseName = "jinze";
#else
        /// 服务器数据库用户
        /// </summary>
        private static readonly string databaseUser = "root";
        /// <summary>
        /// 本地数据库密码
        /// </summary>
        private static readonly string pwd = "Wby@1qaz";
        /// <summary>
        /// 本地数据库
        /// </summary>
        private static readonly string databaseName = "wby_test";
#endif
        private string ConnStr = string.Format("server=120.53.236.107;user={0};password={1};database={2}", databaseUser, pwd, databaseName);

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="para"></param>
        /// <param name="type">类型（1：气象站，2：燃气站）</param>
        public void InsertData(MySqlParameter[] para, int type)
        {
            //创建数据库连接
            MySqlConnection mySqlConn = new MySqlConnection(ConnStr);
            try
            {
                //打开数据库
                mySqlConn.Open();
                Console.WriteLine(string.Format("【{0}】已连接数据库。", DateTime.Now.ToString()));
                //定义插入语句
                string str = "";
                if (type == 1)
                {
                    str = "insert into weatherstations (deviceID,temperature,humidity,pm25,noise,co,so2,no2,windDirection,signalIntensity,errorCode,versionNumber,updateTime) values (@deviceID,@temperature,@humidity,@pm25,@noise,@co,@so2,@no2,@windDirection,@signalIntensity,@errorCode,@versionNumber,@updateTime)";
                }
                else if (type == 2)
                {
                    str = "insert into gasstations (deviceID,combustibleGas,signalIntensity,errorCode,versionNumber,updateTime) values (@deviceID,@combustibleGas,@signalIntensity,@errorCode,@versionNumber,@updateTime)";
                }
                else
                {
                    Console.WriteLine(string.Format("【{0}】非正确存储类型。", DateTime.Now.ToString()));
                    return;
                }

                //执行插入命令
                using (MySqlCommand mySqlComm = new MySqlCommand(str, mySqlConn))
                {
                    //定义SQL语句类型
                    mySqlComm.CommandType = CommandType.Text;
                    //添加参数
                    foreach (MySqlParameter p in para)
                    {
                        mySqlComm.Parameters.Add(p);
                    }
                    //执行操作
                    int row = mySqlComm.ExecuteNonQuery();
                    //执行结果
                    if (row > 0)
                    {
                        Console.WriteLine(string.Format("【{0}】存储数据成功，数据库返回结果：{1}", DateTime.Now.ToString(), row.ToString()));
                    }
                    else
                    {
                        log.Error(string.Format("【{0}】存储数据失败，数据库返回结果：{1}", DateTime.Now.ToString(), row.ToString()));
                        Console.WriteLine(string.Format("【{0}】存储数据失败，数据库返回结果：{1}", DateTime.Now.ToString(), row.ToString()));
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                Console.WriteLine(string.Format("【{0}】数据库操作异常，原因：{1}", DateTime.Now.ToString(), ex.Message));
            }
            finally
            {
                mySqlConn.Close();
                Console.WriteLine(string.Format("【{0}】关闭数据库。", DateTime.Now.ToString()));
            }
        }

        /// <summary>
        /// 查询设备ID类型
        /// </summary>
        /// <param name="typeID"></param>
        /// <returns></returns>
        public ModeType QueryType(string typeID) 
        {
            //创建数据库连接
            MySqlConnection mySqlConn = new MySqlConnection(ConnStr);
            try
            {
                //打开数据库
                mySqlConn.Open();
                Console.WriteLine(string.Format("【{0}】已连接数据库。", DateTime.Now.ToString()));
                //定义查询语句
                string str = "select a.typeID from devicetable a left join typetable b on a.typeID=b.id where a.id='" + typeID+"'";
                //Console.WriteLine(string.Format("【{0}】执行查询语句：{1}", DateTime.Now.ToString(), str));
                //执行查询命令
                using (MySqlCommand mySqlComm = new MySqlCommand(str, mySqlConn))
                {
                    //定义SQL语句类型
                    mySqlComm.CommandType = CommandType.Text;
                    //执行操作
                    MySqlDataReader reader = mySqlComm.ExecuteReader();
                    //读取返回结果
                    reader.Read();
                    if (reader != null && (!reader[0].Equals("")))
                    {
                        if ((int)reader[0] == 1)
                        {
                            Console.WriteLine(string.Format("【{0}】查询数据成功，数据库返回结果：{1}", DateTime.Now.ToString(), reader[0].ToString()));
                            return ModeType.气象;
                        }
                        else if ((int)reader[0] == 2)
                        {
                            Console.WriteLine(string.Format("【{0}】查询数据成功，数据库返回结果：{1}", DateTime.Now.ToString(), reader[0].ToString()));
                            return ModeType.燃气;
                        }
                        else
                        {
                            Console.WriteLine(string.Format("【{0}】查询数据成功，数据库返回结果：{1}", DateTime.Now.ToString(), reader[0].ToString()));
                            return ModeType.其它;
                        }
                    }
                    else 
                    {
                        Console.WriteLine(string.Format("【{0}】未查询到此ID：{1}", DateTime.Now.ToString(), typeID));
                        return ModeType.其它;
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                Console.WriteLine(string.Format("【{0}】数据库查询异常，原因：{1}", DateTime.Now.ToString(), ex.Message));
                return ModeType.其它;
            }
            finally
            {
                mySqlConn.Close();
                Console.WriteLine(string.Format("【{0}】关闭数据库。", DateTime.Now.ToString()));
            }
        }

    }
}
