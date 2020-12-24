using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace My.Util
{
    /// <summary>
    /// 描述：运行时创建类型
    /// 作者：wby 2019/9/30 16:19:20
    /// </summary>
    public class TypeBuilderHelper
    {
        #region 外部接口
        public static Type BuildType(TypeConfig typeConfig)
        {
            TypeBuilder tb = GetTypeBuilder(typeConfig.FullName, typeConfig.AssemblyName);
            typeConfig.Attributes.ForEach(aAttibute => { tb.SetCustomAttribute(GetCustomAttributeBuilder(aAttibute)); });
            ConstructorBuilder constructor = tb.DefineDefaultConstructor(MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName);
            typeConfig.Properties.ForEach(aProperty =>
            {
                AddProperty(tb, aProperty.PropertyName, aProperty.PropertyType, aProperty.Attributes);
            });

            return tb.CreateTypeInfo();
        }

        public static TypeConfig GetConfig(Type type)
        {
            TypeConfig typeConfig = new TypeConfig
            {
                FullName = type.FullName,
                AssemblyName = type.Assembly.FullName,
                Attributes = GetAttributeConfigs(type),
                Properties = type.GetProperties().Select(x => new PropertyConfig
                {
                    PropertyName = x.Name,
                    Attributes = GetAttributeConfigs(x),
                    PropertyType = x.PropertyType
                }).ToList()
            };

            return typeConfig;

            List<AttributeConfig> GetAttributeConfigs(MemberInfo theType)
            {
                return theType.GetCustomAttributesData().Select(y => new AttributeConfig
                {
                    Attribute = y.AttributeType,
                    ConstructorArgs = y.ConstructorArguments.Select(x => x.Value).ToList(),
                    Properties = y.NamedArguments.Select(x => (x.MemberName, x.TypedValue.Value)).ToList()
                }).ToList();
            }
        }
        #endregion

        #region 私有成员
        private static TypeBuilder GetTypeBuilder(string typeFullName, string assemblyName)
        {
            var an = new AssemblyName(assemblyName);
            AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(an, AssemblyBuilderAccess.Run);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");
            TypeBuilder tb = moduleBuilder.DefineType(typeFullName, TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.AutoClass |
                TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit | TypeAttributes.AutoLayout, null);
            return tb;
        }

        /// <summary>
        /// 获取自定义属性
        /// </summary>
        /// <param name="attributeConfig"></param>
        /// <returns></returns>
        private static CustomAttributeBuilder GetCustomAttributeBuilder(AttributeConfig attributeConfig)
        {
            var attributeType = attributeConfig.Attribute;
            var attributeConstructor = attributeType.GetConstructor(attributeConfig.ConstructorArgs.Select(x => x.GetType()).ToArray());

            List<(PropertyInfo PropertyInfo, object Value)> properties = new List<(PropertyInfo PropertyInfo, object Value)>();
            var allProperties = attributeType.GetProperties().ToList();
            attributeConfig.Properties.ForEach(aProperty =>
            {
                var theProperty = allProperties.Where(x => x.Name == aProperty.PropertyName).FirstOrDefault();
                if (theProperty != null)
                    properties.Add((theProperty, aProperty.Value));
            });

            var attributeBuilder = new CustomAttributeBuilder(
                attributeConstructor, attributeConfig.ConstructorArgs.ToArray(),
                properties.Select(x => x.PropertyInfo).ToArray(),
                properties.Select(x => x.Value).ToArray());

            return attributeBuilder;
        }

        /// <summary>
        /// 添加属性
        /// </summary>
        /// <param name="tb"></param>
        /// <param name="propertyName"></param>
        /// <param name="propertyType"></param>
        /// <param name="attributes"></param>
        private static void AddProperty(TypeBuilder tb, string propertyName, Type propertyType, List<AttributeConfig> attributes)
        {
            //定义字段
            FieldBuilder fieldBuilder = tb.DefineField("_" + propertyName, propertyType, FieldAttributes.Private);

            //定义属性
            PropertyBuilder propertyBuilder = tb.DefineProperty(propertyName, PropertyAttributes.HasDefault, propertyType, null);

            //定义Get属性方法
            MethodBuilder getPropMthdBldr = tb.DefineMethod("get_" + propertyName, MethodAttributes.Public | MethodAttributes.SpecialName
                | MethodAttributes.HideBySig, propertyType, Type.EmptyTypes);
            ILGenerator getIl = getPropMthdBldr.GetILGenerator();
            getIl.Emit(OpCodes.Ldarg_0);  //加载第一个参数
            getIl.Emit(OpCodes.Ldfld, fieldBuilder);  //加载字段的值
            getIl.Emit(OpCodes.Ret);  //方法返回

            //定义Set属性方法
            MethodBuilder setPropMthdBldr = tb.DefineMethod("set_" + propertyName, MethodAttributes.Public | MethodAttributes.SpecialName
                | MethodAttributes.HideBySig, null, new[] { propertyType });
            ILGenerator setIl = setPropMthdBldr.GetILGenerator();
            Label modifyProperty = setIl.DefineLabel();
            Label exitSet = setIl.DefineLabel();

            setIl.MarkLabel(modifyProperty);
            setIl.Emit(OpCodes.Ldarg_0);
            setIl.Emit(OpCodes.Ldarg_1);
            setIl.Emit(OpCodes.Stfld, fieldBuilder);

            setIl.Emit(OpCodes.Nop);
            setIl.MarkLabel(exitSet);
            setIl.Emit(OpCodes.Ret);

            propertyBuilder.SetGetMethod(getPropMthdBldr);
            propertyBuilder.SetSetMethod(setPropMthdBldr);

            //设置特性
            attributes?.ForEach(aAttribute =>
            {
                propertyBuilder.SetCustomAttribute(GetCustomAttributeBuilder(aAttribute));
            });

        }
        #endregion
    }

    #region 相关类型
    /// <summary>
    /// 类型配置
    /// </summary>
    public class TypeConfig
    {
        /// <summary>
        /// 类型名
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// 程序集名
        /// </summary>
        public string AssemblyName { get; set; } = Assembly.GetExecutingAssembly().GetName().FullName;

        /// <summary>
        /// 拥有的属性
        /// </summary>
        public List<PropertyConfig> Properties { get; set; } = new List<PropertyConfig>();

        /// <summary>
        /// 拥有的特性
        /// </summary>
        public List<AttributeConfig> Attributes { get; set; } = new List<AttributeConfig>();
    }

    /// <summary>
    /// 属性配置
    /// </summary>
    public class PropertyConfig
    {
        /// <summary>
        /// 属性名
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// 属性类型
        /// </summary>
        public Type PropertyType { get; set; }

        /// <summary>
        /// 属性包含的自定义特性
        /// </summary>
        public List<AttributeConfig> Attributes { get; set; } = new List<AttributeConfig>();
    }

    /// <summary>
    /// 特性配置
    /// </summary>
    public class AttributeConfig
    {
        /// <summary>
        /// 特性类型
        /// </summary>
        public Type Attribute { get; set; }

        /// <summary>
        /// 构造函数参数
        /// </summary>
        public List<object> ConstructorArgs { get; set; } = new List<object>();

        /// <summary>
        /// 初始化属性
        /// </summary>
        public List<(string PropertyName, object Value)> Properties { get; set; } = new List<(string PropertyName, object Value)>();
    } 
    #endregion
}
