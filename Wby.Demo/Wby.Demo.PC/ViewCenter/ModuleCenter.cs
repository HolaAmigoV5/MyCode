using System.Threading.Tasks;
using System.Windows.Controls;
using Wby.Demo.Shared.Dto;
using Wby.Demo.ViewModel.Interfaces;

namespace Wby.Demo.PC.ViewCenter
{
    /// <summary>
    /// View/ViewModel 控制基类
    /// </summary>
    /// <typeparam name="TView"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public class ModuleCenter<TView, TEntity> where TView : UserControl, new() where TEntity : BaseDto
    {
        public ModuleCenter() { }

        public ModuleCenter(IBaseViewModel<TEntity> viewModel)
        {
            this.viewModel = viewModel;
        }

        public TView view = new TView();
        public IBaseViewModel<TEntity> viewModel;

        public async Task BindDefaultModel(int AuthValue)
        {
            viewModel.InitPermissions(AuthValue);
            await viewModel.GetPageData(0);
            BindDataGridColumns();
            view.DataContext = viewModel;
        }

        public void BindDefaultModel()
        {
            view.DataContext = viewModel;
        }

        public virtual void BindDataGridColumns() { }

        public object GetView()
        {
            return view;
        }
    }

    /// <summary>
    /// View/ViewModel 控制基类(组件)
    /// </summary>
    /// <typeparam name="TView"></typeparam>
    public class ComponentCenter<TView> where TView : UserControl, new()
    {
        public TView view = new TView();
        public IComponentViewModel viewModel;

        public ComponentCenter(IComponentViewModel viewModel)
        {
            this.viewModel = viewModel;
        }


        public void BindDataGridColumns()
        {
        }

        public void BindDefaultModel()
        {
            view.DataContext = viewModel;
        }

        public Task BindDefaultModel(int AuthValue = 0)
        {
            this.BindDefaultModel();
            return Task.FromResult(true);
        }

        public object GetView()
        {
            return view;
        }
    }
}
