using Prism.Commands;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Wby.Demo.Shared.Attributes;
using Wby.Demo.Shared.Common.Enums;
using Wby.Demo.Shared.DataInterfaces;
using Wby.Demo.Shared.DataModel;
using Wby.Demo.Shared.Dto;
using Wby.Demo.Shared.Query;
using Wby.PrismDemo.PC.Infrastructure.Common;

namespace Wby.PrismDemo.PC.ViewModels
{
    [Module("权限管理", ModuleType.系统配置)]
    public class GroupViewModel: BaseViewModel<GroupDto>
    {
        private readonly IUserRepository userRepository;
        private readonly IGroupRepository groupRepository;

        public GroupViewModel(IGroupRepository repository) : base(repository)
        {
            userRepository= ContainerLocator.Current.Resolve<IUserRepository>();
            groupRepository = repository;
        }

        #region Override
        public override void Execute(string arg)
        {
            switch (arg)
            {
                case "添加用户": GetUserData(); break;
                case "选中所有功能": break;
                case "返回上一页": SelectCardIndex = 0; break;
                case "添加所有选中项": AddAllUser(); break;
                case "删除所有选中用户": DeleteAllUser(); break;
            }
            base.Execute(arg);
        }

        public override async void AddAsync()
        {
            GroupDataDto = new GroupDataDto();
            await UpdateMenuModules();
            base.AddAsync();
        }

        public override async void UpdateAsync()
        {
            if (GridModel == null) return;
            await UpdateMenuModules();
            var g = await groupRepository.GetGroupAsync(GridModel.Id);
            if (g.StatusCode != 200)
            {
                SendMsgInfo.SendMsgToSnackBar(g.Message);
                return;
            }
            //其实这一步操作就是把当前用户组包含的权限,
            //绑定到所有菜单的列表当中,设定选中
            g.Result?.GroupFuncs?.ForEach(f =>
            {
                for (int i = 0; i < MenuModules.Count; i++)
                {
                    var m = MenuModules[i];
                    if (m.MenuCode == f.MenuCode)
                    {
                        for (int j = 0; j < m.Modules.Count; j++)
                        {
                            if ((f.Auth & m.Modules[j].Value) == m.Modules[j].Value)
                                m.Modules[j].IsChecked = true;
                        }
                    }
                }
            });
            GroupDataDto = g.Result;//绑定编辑项GroupHeader
            this.CreateDefaultCommand();
            SelectedPageIndex = 1;
        }

        public override async Task SaveAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(GroupDataDto.Group.GroupCode) ||
                    string.IsNullOrWhiteSpace(GroupDataDto.Group.GroupName))
                {
                    SendMsgInfo.SendMsgToMsgView("组代码和名称为必填项！", Notify.Error);
                    return;
                };

                //把选择的功能对应的权限保存到提交的参数当中
                GroupDataDto.GroupFuncs = new List<GroupFunc>();
                for (int i = 0; i < MenuModules.Count; i++)
                {
                    var m = MenuModules[i];
                    int value = m.Modules.Where(t => t.IsChecked).Sum(t => t.Value);
                    if (value > 0)
                    {
                        GroupDataDto.GroupFuncs.Add(new GroupFunc()
                        {
                            MenuCode = m.MenuCode,
                            Auth = value
                        });
                    }
                }
                var r = await groupRepository.SaveGroupAsync(GroupDataDto);
                if (r.StatusCode != 200)
                {
                    SendMsgInfo.SendMsgToSnackBar(r.Message);
                    return;
                }
                await GetPageData(0);
                InitPermissions(this.AuthValue);
                SelectedPageIndex = 0;
            }
            catch (Exception ex)
            {
                SendMsgInfo.SendMsgToSnackBar(ex.Message);
            }
        }
        #endregion

        #region Property

        private int selectCardIndex = 0;

        /// <summary>
        /// 切换检索用户列表的页索引
        /// </summary>
        public int SelectCardIndex
        {
            get { return selectCardIndex; }
            set { SetProperty(ref selectCardIndex, value); }
        }


        private string userSearch = string.Empty;

        /// <summary>
        /// 检索用户条件
        /// </summary>
        public string UserSearch
        {
            get { return userSearch; }
            set { SetProperty(ref userSearch, value); }
        }


        private GroupDataDto groupDataDto;

        /// <summary>
        /// 操作实体
        /// </summary>
        public GroupDataDto GroupDataDto
        {
            get { return groupDataDto; }
            set { SetProperty(ref groupDataDto, value); }
        }

        private ObservableCollection<UserDto> gridUserModelList;

        /// <summary>
        /// 所有的用户列表
        /// </summary>
        public ObservableCollection<UserDto> GridUserModelList
        {
            get { return gridUserModelList; }
            set { SetProperty(ref gridUserModelList, value); }
        }

        private ObservableCollection<MenuModuleGroupDto> menuModules;
        public ObservableCollection<MenuModuleGroupDto> MenuModules
        {
            get { return menuModules; }
            set { SetProperty(ref menuModules, value); }
        }
        #endregion

        #region Command

        private DelegateCommand<UserDto> addUserCommand; 
        public DelegateCommand<UserDto> AddUserCommand =>addUserCommand ??= new DelegateCommand<UserDto>(ExecuteCommandName);

        private DelegateCommand<GroupUserDto> delUserCommand;
        public DelegateCommand<GroupUserDto> DelUserCommand =>delUserCommand ??= new DelegateCommand<GroupUserDto>(ExecuteDelUserCommand);

        void ExecuteDelUserCommand(GroupUserDto arg)
        {
            if (arg == null) return;
            var u = GroupDataDto.GroupUsers?.FirstOrDefault(t => t.Account == arg.Account);
            if (u != null) GroupDataDto.GroupUsers?.Remove(u);
        }
        void ExecuteCommandName(UserDto arg)
        {
            if (arg == null) return;
            var u = GroupDataDto.GroupUsers?.FirstOrDefault(t => t.Account == arg.Account);
            if (u == null)
                GroupDataDto.GroupUsers?.Add(new GroupUserDto() { Account = arg.Account });
                
        }
        #endregion

        #region Method
        /// <summary>
        /// 获取用户列表
        /// </summary>
        private async void GetUserData()
        {
            var r = await userRepository.GetAllListAsync(new QueryParameters()
            {
                PageIndex = 0,
                PageSize = 30,
                Search = UserSearch
            });
            GridUserModelList = new ObservableCollection<UserDto>();
            if (r.StatusCode == 200)
                GridUserModelList = new ObservableCollection<UserDto>(r.Result.Items?.ToList());
            SelectCardIndex = 1;
        }

        /// <summary>
        /// 添加所有选中用户
        /// </summary>
        private void AddAllUser()
        {
            for (int i = 0; i < GridUserModelList.Count; i++)
            {
                var arg = GridUserModelList[i];
                if (arg.IsChecked)
                {
                    var u = GroupDataDto.GroupUsers?.FirstOrDefault(t => t.Account == arg.Account);
                    if (u == null) 
                        GroupDataDto.GroupUsers?.Add(new GroupUserDto() { Account = arg.Account });
                }
            }
        }

        /// <summary>
        /// 删除所有用户
        /// </summary>
        private void DeleteAllUser()
        {
            for (int i = GroupDataDto.GroupUsers.Count - 1; i >= 0; i--)
            {
                var arg = GroupDataDto.GroupUsers[i];
                if (arg.IsChecked)
                    GroupDataDto.GroupUsers.Remove(arg);
            }
        }

        /// <summary>
        /// 刷新菜单列表
        /// </summary>
        /// <returns></returns>
        private async Task UpdateMenuModules()
        {
            if (MenuModules != null && MenuModules.Count > 0)
            {
                for (int i = 0; i < MenuModules.Count; i++)
                {
                    var m = MenuModules[i].Modules;
                    for (int j = 0; j < m.Count; j++)
                        m[j].IsChecked = false;
                }
                return;
            }
            var tm = await groupRepository.GetMenuModuleListAsync();
            if (tm.StatusCode == 200)
                MenuModules = new ObservableCollection<MenuModuleGroupDto>(tm.Result);
        }
        #endregion
    }
}
