using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace My.Util
{
    /// <summary>
    /// 描述：DataTable的扩展
    /// 作者：wby 2019/9/24 9:21:42
    /// </summary>
    public static partial class Extention
    {
        /// <summary>
        /// DataTable转List<T>
        /// </summary>
        /// <typeparam name="T">转换类型</typeparam>
        /// <param name="dt">数据源</param>
        /// <returns></returns>
        public static List<T> ToList<T>(this DataTable dt)
        {
            List<T> list = new List<T>();

            //确定参数是否有效
            if (dt == null || dt.Rows.Count == 0)
                return list;

            Dictionary<string, string> dicField = new Dictionary<string, string>();
            Dictionary<string, string> dicProperty = new Dictionary<string, string>();
            Type type = typeof(T);

            //创建字段字典，方便查找字段名
            type.GetFields().ForEach(aField => dicField.Add(aField.Name.ToLower(), aField.Name));

            //创建属性字典，方面查找属性名
            type.GetProperties().ForEach(aProperty => dicProperty.Add(aProperty.Name.ToLower(), aProperty.Name));

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                T _t = Activator.CreateInstance<T>();

                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    string memberKey = dt.Columns[i].ColumnName.ToLower();

                    //字段赋值
                    if (dicField.ContainsKey(memberKey))
                    {
                        FieldInfo theField = type.GetField(dicField[memberKey]);
                        var dbValue = dt.Rows[i][j];
                        if (dbValue.GetType() == typeof(DBNull))
                            dbValue = null;
                        if (dbValue != null)
                        {
                            Type memberType = theField.FieldType;
                            dbValue = dbValue.ChangeType_ByConvert(memberType);
                        }
                        theField.SetValue(_t, dbValue);
                    }

                    //属性赋值
                    if(dicProperty.ContainsProperty(memberKey))
                    {
                        PropertyInfo theProperty = type.GetProperty(dicProperty[memberKey]);
                        var dbValue = dt.Rows[i][j];
                        if (dbValue.GetType() == typeof(DBNull))
                            dbValue = null;
                        if(dbValue!=null)
                        {
                            Type memberType = theProperty.PropertyType;
                            dbValue = dbValue.ChangeType_ByConvert(memberType);
                        }

                        theProperty.SetValue(_t, dbValue);
                    }
                }
                list.Add(_t);
            }
            return list;
        }

        /// <summary>
        ///将DataTable转换为标准的CSV字符串
        /// </summary>
        /// <param name="dt">数据表</param>
        /// <returns>返回标准的CSV</returns>
        public static string ToCsvStr(this DataTable dt)
        {
            //以半角逗号（即,）作分隔符，列为空也要表达其存在。
            //列内容如存在半角逗号（即,）则用半角引号（即""）将该字段值包含起来。
            //列内容如存在半角引号（即"）则应替换成半角双引号（""）转义，并用半角引号（即""）将该字段值包含起来。
            StringBuilder sb = new StringBuilder();
            DataColumn column;
            foreach (DataRow row in dt.Rows)
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    column = dt.Columns[i];
                    if (i != 0) sb.Append(",");
                    if (column.DataType == typeof(string) && row[column].ToString().Contains(","))
                        sb.Append($"\"{ row[column].ToString().Replace("\"", "\"\"")}\"");
                    else
                        sb.Append(row[column].ToString());
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}
