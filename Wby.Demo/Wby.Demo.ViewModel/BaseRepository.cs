using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Wby.Demo.Shared.Common;
using Wby.Demo.Shared.Common.Aop;
using Wby.Demo.Shared.DataInterfaces;
using Wby.Demo.Shared.Dto;
using Wby.Demo.Shared.Query;
using Wby.Demo.ViewModel.Common;

namespace Wby.Demo.ViewModel
{
    /// <summary>
    /// 通用基类(实现CRUD/数据分页..)
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class BaseRepository<TEntity> : ObservableObject where TEntity : BaseDto, new()
    {
        public readonly IRepository<TEntity> Repository;
        public BaseRepository(IRepository<TEntity> repository)
        {
            Repository = repository;
        }

        #region 增删改查
        private int selectPageIndex;
        public int SelectPageIndex
        {
            get { return selectPageIndex; }
            set { SetProperty(ref selectPageIndex, value); }
        }

        private string search;
        public string Search
        {
            get { return search; }
            set { SetProperty(ref search, value); }
        }

        private TEntity gridModel;
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


        public AsyncRelayCommand QueryCommand { get { return new AsyncRelayCommand(Query); } }
        public AsyncRelayCommand<string> ExecuteCommand { get { return new AsyncRelayCommand<string>(arg => Execute(arg)); } }

        public virtual async Task Query()
        {
            await GetPageData(PageIndex);
        }

        [GlobalLoger]
        public virtual async Task Execute(string arg)
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
                case "取消":
                    Cancel();
                    break;
            }
        }

        public virtual void AddAsync()
        {
            CreateDefaultCommand();
            GridModel = new TEntity();
            SelectPageIndex = 1;
        }

        public virtual void Cancel()
        {
            InitPermissions(AuthValue);
            SelectPageIndex = 0;
        }

        public virtual async Task DeleteAsync()
        {
            if (GridModel != null)
            {
                if (await Msg.Question("确认删除当前选中行数据?"))
                {
                    var r = await Repository.DeleteAsync(GridModel.Id);
                    if (r.StatusCode == 200)
                        await GetPageData(0);
                    else
                        Msg.SendMsgInfo(r.Message, Notify.Error);
                }
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
            SelectPageIndex = 0;
        }

        [GlobalProgress]
        public virtual async void UpdateAsync()
        {
            if (GridModel == null) return;
            var baseResponse = await Repository.GetAsync(GridModel.Id);
            if (baseResponse.StatusCode == 200)
            {
                GridModel = baseResponse.Result;
                this.CreateDefaultCommand();
                SelectPageIndex = 1;
            }
            else
                Msg.SendMsgInfo("Get data exception!", Notify.Error);
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

        private int pageCount = 0;
        public int PageCount
        {
            get { return pageCount; }
            set { SetProperty(ref pageCount, value); }
        }

        public AsyncRelayCommand GoHomePageCommand { get { return new AsyncRelayCommand(GoHomePage); } }
        public AsyncRelayCommand GoPrePageCommand { get { return new AsyncRelayCommand(GoPrePage); } }
        public AsyncRelayCommand GoNextPageCommand { get { return new AsyncRelayCommand(GoNextPage); } }
        public AsyncRelayCommand GoEndPageCommand { get { return new AsyncRelayCommand(GoEndPage); } }

        /// <summary>
        /// 首页
        /// </summary>
        /// <returns></returns>
        public virtual async Task GoHomePage()
        {
            if (PageIndex == 0) return;
            PageIndex = 0;
            await GetPageData(PageIndex);
        }

        /// <summary>
        /// 上一页
        /// </summary>
        /// <returns></returns>
        public virtual async Task GoPrePage()
        {
            if (PageIndex == 0) return;
            PageIndex--;
            await GetPageData(PageIndex);
        }

        /// <summary>
        /// 下一页
        /// </summary>
        /// <returns></returns>
        public virtual async Task GoNextPage()
        {
            if (PageIndex == 0) return;
            PageIndex++;
            await GetPageData(PageIndex);
        }

        /// <summary>
        /// 尾页
        /// </summary>
        /// <returns></returns>
        public virtual async Task GoEndPage()
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
        private ObservableCollection<CommandStruct> toolBarCommandList;
        public ObservableCollection<CommandStruct> ToolBarCommandList
        {
            get { return toolBarCommandList; }
            set { SetProperty(ref toolBarCommandList, value); }
        }


        /// <summary>
        /// 页面权限值
        /// </summary>
        public int AuthValue { get; private set; }

        public void InitPermissions(int authValue)
        {
            AuthValue = authValue;
            ToolBarCommandList = new ObservableCollection<CommandStruct>();
            Contract.AuthItems.ForEach(arg =>
            {
                if ((AuthValue & arg.AuthValue) == arg.AuthValue)
                {
                    ToolBarCommandList.Add(new CommandStruct()
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
            ToolBarCommandList.Clear();
            ToolBarCommandList.Add(new CommandStruct() { CommandName = "保存", CommandColor = "#0066FF", CommandKind = "ContentSave" });
            ToolBarCommandList.Add(new CommandStruct() { CommandName = "取消", CommandColor = "#FF6633", CommandKind = "Cancel" });
        }
        #endregion
    }
}
