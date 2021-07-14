using CommonLibrary;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;

namespace FeatureClassQuery
{
    public partial class FieldInfoForm : Form
    {
        List<ColumnInfo> columnInfos;
        public FieldInfoForm(FieldInformation fieldInfo)
        {
            InitializeComponent();
            if (fieldInfo != null)
            {
                columnInfos = new List<ColumnInfo>();
                GetPropertyNameAndValue(fieldInfo);
            }
            this.dataGridView1.DataSource = columnInfos;
        }

        /// <summary>
        /// 反射获取对象和对象值
        /// </summary>
        /// <param name="fieldInfo"></param>
        private void GetPropertyNameAndValue(object fieldInfo)
        {
            Type type = fieldInfo.GetType();
            PropertyInfo[] propertyInfos = type.GetProperties();
            foreach (PropertyInfo item in propertyInfos)
            {
                string name = item.Name;
                var value = item.GetValue(fieldInfo, null);

                if (item.PropertyType.IsValueType || item.PropertyType.Name.StartsWith("String") || value == null)
                    columnInfos.Add(new ColumnInfo { ColumnProperty = name, ColumnValue = value });
                else
                    GetPropertyNameAndValue(value);
            }
        }

        
        delegate object MemberGetDelegate(object obj);
        /// <summary>
        /// 委托动态获取对象和对象值
        /// </summary>
        /// <param name="fieldInfo"></param>
        private void GetPropertyNameAndValue2(object fieldInfo)
        {
            Type type = fieldInfo.GetType();
            PropertyInfo[] propertyInfos = type.GetProperties();
            foreach (PropertyInfo item in propertyInfos)
            {
                string name = item.Name;
                MemberGetDelegate memberGet = (MemberGetDelegate)Delegate.CreateDelegate(typeof(Action<object,object>), item.GetGetMethod());
                var value = memberGet(fieldInfo);
                //var value = item.GetValue(fieldInfo, null);

                if (item.PropertyType.IsValueType || item.PropertyType.Name.StartsWith("String") || value == null)
                    columnInfos.Add(new ColumnInfo { ColumnProperty = name, ColumnValue = value });
                else
                    GetPropertyNameAndValue2(value);
            }
        }
    }

    public class ColumnInfo
    {
        public string ColumnProperty { get; set; }
        public object ColumnValue { get; set; }
    }
}
