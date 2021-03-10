using System;
using System.Collections.Generic;
using System.Text;

namespace Wby.Demo.Shared.Attributes
{
    /// <summary>
    /// 禁止序列化特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class PreventAttribute : Attribute
    {
    }
}
