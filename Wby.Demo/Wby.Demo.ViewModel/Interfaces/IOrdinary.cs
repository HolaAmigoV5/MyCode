using Microsoft.Toolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Wby.Demo.ViewModel.Interfaces
{
    /// <summary>
    /// 基础增删改查接口
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IOrdinary<TEntity> where TEntity : class
    {
        /// <summary>
        /// 选中表单
        /// </summary>
        TEntity GridModel { get; set; }

        /// <summary>
        /// 页索引
        /// </summary>
        int SelectPageIndex { get; set; }

        /// <summary>
        /// 搜索参数
        /// </summary>
        string Search { get; set; }

        /// <summary>
        /// 表单
        /// </summary>
        ObservableCollection<TEntity> GridModelList { get; set; }

        /// <summary>
        /// 搜索命令
        /// </summary>
        AsyncRelayCommand QueryCommand { get; }

        /// <summary>
        /// 其它命令
        /// </summary>
        AsyncRelayCommand<string> ExecuteCommand { get; }

        /// <summary>
        /// 添加
        /// </summary>
        void AddAsync();

        /// <summary>
        /// 编辑
        /// </summary>
        void UpdateAsync();
        
        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        Task DeleteAsync();
        
        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        Task SaveAsync();
        
        /// <summary>
        /// 取消
        /// </summary>
        void Cancel();
    }
}
