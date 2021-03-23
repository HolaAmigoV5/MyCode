namespace Wby.PrismDemo.PC.Infrastructure.Common
{
    public class Module
    {
        /// <summary>
        /// 模块图标代码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 模块名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 模块对应的View名称
        /// </summary>
        public string ViewName { get; set; }

        /// <summary>
        /// 权限值
        /// </summary>
        public int Auth { get; set; }
    }
}
