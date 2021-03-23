using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using Wby.Demo.Shared.Common;
using Wby.Demo.Shared.Common.Aop;
using Wby.Demo.Shared.DataInterfaces;
using Wby.Demo.Shared.Dto;
using Wby.Demo.Shared.Query;
using Wby.PrismDemo.PC.Infrastructure.Common;

namespace Wby.PrismDemo.PC.ViewModels
{
    /// <summary>
    /// 通用ViewModel基类
    /// </summary>
    public class BaseViewModel<TEntity> : BindableBase where TEntity : BaseDto, new()
    {
        public readonly IRepository<TEntity> Repository;
        private IEventAggregator _ea;
        public BaseViewModel(IRepository<TEntity> repository)
        {
            Repository = repository;
            operationBtns = new ObservableCollection<CommandStruct>();
            gridModelList = new ObservableCollection<TEntity>();
            _ea = ContainerLocator.Current.Resolve<IEventAggregator>();
            _ea?.GetEvent<ModuleSentEvent>().Subscribe(ModuleMessageReceived); //订阅事件
           
        }

        #region IOrdinary<TEntity>
        private Module _module;
        private string menuName = "首页";

        /// <summary>
        /// 要打开的菜单名称
        /// </summary>
        public string MenuName
        {
            get { return menuName; }
            set { SetProperty(ref menuName, value); }
        }

        private int selectedPageIndex;

        /// <summary>
        /// 修改界面和现实界面切换
        /// </summary>
        public int SelectedPageIndex
        {
            get { return selectedPageIndex; }
            set { SetProperty(ref selectedPageIndex, value); }
        }

        private string search;
        public string Search
        {
            get { return search; }
            set { SetProperty(ref search, value); }
        }

        private TEntity gridModel;
        /// <summary>
        /// GridView中绑定的数据
        /// </summary>
        public TEntity GridModel
        {
            get { return gridModel; }
            set { SetProperty(ref gridModel, value); }
        }

        private ObservableCollection<TEntity> gridModelList;
        public ObservableCollection<TEntity> GridModelList
        {
            get { return gridModelList; }
            set { SetProperty(ref gridModelList, value); }
        }

        private DelegateCommand queryCommand;
        public DelegateCommand QueryCommand => queryCommand ??= new DelegateCommand(Query);

        private DelegateCommand<string> executeCommand;
        public DelegateCommand<string> ExecuteCommand => executeCommand??=new DelegateCommand<string>(Execute);

        private DelegateCommand<object> loadCommand;
        public DelegateCommand<object> LoadCommand => loadCommand ??= new DelegateCommand<object>(Load);

        private void Load(object obj)
        {
            if (obj is DataGrid dataGrid)
            {
                dataGrid.Columns.Clear();
                VisualHelper.SetDataGridColumns(dataGrid, "Grid", typeof(TEntity));
            }
        }

        public virtual async void Query()
        {
            await GetPageData(PageIndex);
            
        }

        [GlobalLoger]
        public virtual async void Execute(string arg)
        {
            /*
             * 这里使用string来做弱类型处理,防止使用枚举,
             * 其他页面需要重写该方法
             */
            switch (arg)
            {
                case "添加":
                    AddAsync();
                    break;
                case "修改":
                    UpdateAsync();
                    break;
                case "删除":
                    await DeleteAsync();
                    break;
                case "保存":
                    await SaveAsync();
                    break;
                case "查看":
                    ShowDetail();
                    break;
                case "取消":
                    Cancel();
                    break;
            }
        }

        public virtual void AddAsync()
        {
            CreateDefaultCommand();
            if (GridModel == null)
                GridModel = new TEntity();
            SelectedPageIndex = 1;
        }

        public virtual void Cancel()
        {
            InitPermissions(AuthValue);
            SelectedPageIndex = 0;
        }

        public virtual void ShowDetail()
        {
            CreateDefaultCommand();
            SelectedPageIndex = 1;
        }

        public virtual async Task DeleteAsync()
        {
            try
            {
                if (GridModel != null)
                {
                    if (SendMsgInfo.SendMsgToMsgView("确认删除当前选中行数据？", Notify.Question))
                    {
                        var r = await Repository.DeleteAsync(GridModel.Id);
                        if (r.StatusCode == 200)
                            await GetPageData(0);
                        else
                            SendMsgInfo.SendMsgToSnackBar(r.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                SendMsgInfo.SendMsgToSnackBar(ex.Message);
            }
            
        }

        [GlobalProgress]
        public virtual async Task SaveAsync()
        {
            //Before you save, you need to verify the validity of the data.
            if (GridModel == null) return;
            await Repository.SaveAsync(GridModel);
            InitPermissions(this.AuthValue);
            await GetPageData(0);
            SelectedPageIndex = 0;
        }

        [GlobalProgress]
        public virtual async void UpdateAsync()
        {
            try
            {
                if (GridModel == null) return;
                var baseResponse = await Repository.GetAsync(GridModel.Id);
                if (baseResponse?.StatusCode == 200)
                {
                    GridModel = baseResponse.Result;
                    CreateDefaultCommand();
                    SelectedPageIndex = 1;
                }
                else
                    SendMsgInfo.SendMsgToSnackBar("Get data exception!");
            }
            catch (Exception ex)
            {
                SendMsgInfo.SendMsgToSnackBar(ex.Message);
            }

        }
        #endregion

        #region IDataPager（数据分页）
        private int totalCount = 0;
        public int TotalCount
        {
            get { return totalCount; }
            set { SetProperty(ref totalCount, value); }
        }

        private int pageSize = 30;
        public int PageSize
        {
            get { return pageSize; }
            set { SetProperty(ref pageSize, value); }
        }

        private int pageIndex = 0;
        public int PageIndex
        {
            get { return pageIndex; }
            set { SetProperty(ref pageIndex, value); }
        }

        private int pageCount = 1;
        public int PageCount
        {
            get { return pageCount; }
            set { SetProperty(ref pageCount, value); }
        }

        private DelegateCommand goHomePageCommand;
        public DelegateCommand GoHomePageCommand => goHomePageCommand ??= new DelegateCommand(GoHomePage);

        private DelegateCommand goPrePageCommand;
        public DelegateCommand GoPrePageCommand => goPrePageCommand ??= new DelegateCommand(GoPrePage);

        private DelegateCommand goNextPageCommand;
        public DelegateCommand GoNextPageCommand => goNextPageCommand ??= new DelegateCommand(GoNextPage);

        private DelegateCommand goEndPageCommand;
        public DelegateCommand GoEndPageCommand => goEndPageCommand ??= new DelegateCommand(GoEndPage);

        /// <summary>
        /// 首页
        /// </summary>
        /// <returns></returns>
        public virtual async void GoHomePage()
        {
            if (PageIndex == 0) return;
            PageIndex = 0;
            await GetPageData(PageIndex);
        }

        /// <summary>
        /// 上一页
        /// </summary>
        /// <returns></returns>
        public virtual async void GoPrePage()
        {
            if (PageIndex == 0) return;
            PageIndex--;
            await GetPageData(PageIndex);
        }

        /// <summary>
        /// 下一页
        /// </summary>
        /// <returns></returns>
        public virtual async void GoNextPage()
        {
            if (PageIndex == 0) return;
            PageIndex++;
            await GetPageData(PageIndex);
        }

        /// <summary>
        /// 尾页
        /// </summary>
        /// <returns></returns>
        public virtual async void GoEndPage()
        {
            PageIndex = PageCount;
            await GetPageData(PageCount);
        }


        public virtual async Task GetPageData(int pageIndex)
        {
            var r = await Repository.GetAllListAsync(new QueryParameters()
            {

                PageIndex = this.PageIndex,
                PageSize = this.PageSize,
                Search = this.Search
            });
            if (r.StatusCode == 200)
            {
                GridModelList = new ObservableCollection<TEntity>(r.Result.Items.ToList());
                TotalCount = r.Result.TotalCount;
                PageCount = r.Result.TotalPages;
            }
        }

        public virtual void SetPageCount()
        {
            PageCount = Convert.ToInt32(Math.Ceiling((double)TotalCount / (double)pageSize));
        }

        #endregion

        #region IAuthority （权限内容）
        private ObservableCollection<CommandStruct> operationBtns;

        /// <summary>
        /// 权限的操作按钮(新增，修改，删除等)
        /// </summary>
        public ObservableCollection<CommandStruct> OperationBtns
        {
            get { return operationBtns; }
            set { SetProperty(ref operationBtns, value); }
        }


        /// <summary>
        /// 页面权限值
        /// </summary>
        public int AuthValue { get; private set; }

        public void InitPermissions(int authValue)
        {
            AuthValue = authValue;
            OperationBtns.Clear();
            Contract.AuthItems.ForEach(arg =>
            {
                //Debug.WriteLine($"Value1:{AuthValue & arg.AuthValue}, Value2:{arg.AuthValue}");
                //这里使用位运算实现权限管理。比如authValue=7,则有添加，删除，修改权限，authValue=15,则有小于16之前的所有权限。实质是所有权限值之和
                if ((AuthValue & arg.AuthValue) == arg.AuthValue)
                {
                    OperationBtns.Add(new CommandStruct()
                    {
                        CommandName = arg.AuthName,
                        CommandKind = arg.AuthKind,
                        CommandColor = arg.AuthColor
                    });
                }
            });
        }

        public void CreateDefaultCommand()
        {
            OperationBtns.Clear();
            OperationBtns.Add(new CommandStruct() { CommandName = "保存", CommandColor = "#0066FF", CommandKind = "ContentSave" });
            OperationBtns.Add(new CommandStruct() { CommandName = "取消", CommandColor = "#FF6633", CommandKind = "Cancel" });
        }
        #endregion

        #region Methods
        private void ModuleMessageReceived(Module module)
        {
            MenuName = module.Name;
            SelectedPageIndex = 0;
            _module = module;
            InitPermissions(module.Auth);
            InitData();
        }

        private async void InitData()
        {
            var name = Repository.GetType().Name;
            if (!(name.Replace("Service", "") == _module.ViewName.Replace("View", "")))
                return;

            var r = await Repository.GetAllListAsync(new QueryParameters()
            {
                PageIndex = this.PageIndex,
                PageSize = this.PageSize,
                Search = this.Search
            });
            if (r.StatusCode == 200)
            {
                GridModelList.Clear();
                GridModelList.AddRange(r.Result.Items.ToList());
                TotalCount = r.Result.TotalCount;
                SetPageCount();
            }
        }

        #endregion
    }
}
