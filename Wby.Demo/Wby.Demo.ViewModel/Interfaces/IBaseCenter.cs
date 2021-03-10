using System.Threading.Tasks;

namespace Wby.Demo.ViewModel.Interfaces
{
    /// <summary>
    /// 程序模块的上下文操作接口
    /// </summary>
    public interface IBaseCenter
    {
        /// <summary>
        /// 关联默认数据上下文
        /// </summary>
        void BindDefaultModel();

        /// <summary>
        /// 关联默认数据上下文(包含权限相关)
        /// </summary>
        /// <param name="AuthValue"></param>
        /// <returns></returns>
        Task BindDefaultModel(int AuthValue = 0);


        object GetView();

        
        /// <summary>
        /// 关联表格列
        /// </summary>
        void BindDataGridColumns();
    }
}
