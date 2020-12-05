using System;
using System.Linq;

namespace Wby.Infrastructure.Core.Extensions
{
    public static class GenericTypeExtensions
    {
        /// <summary>
        /// 获取泛型类型名
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static string GetGenericTypeName(this Type type)
        {
            var typeName = string.Empty;
            if (type.IsGenericType)
            {
                var genericTypes = string.Join(",", type.GetGenericArguments().Select(t => t.Name).ToArray());
                typeName = $"{type.Name.Remove(type.Name.IndexOf('`'))}<{genericTypes}>";
            }
            else
                typeName = type.Name;

            return typeName;
        }

        public static string GetGenericTypeName(this object @object)
        {
            return @object.GetType().GetGenericTypeName();
        }
    }
}
