﻿using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using Wby.Demo.Shared.DataInterfaces;
using Wby.Demo.Shared.DataModel;

namespace Wby.Demo.EFCore.Context
{
    public class DataInitializer : IDataInitializer
    {
        private readonly ILogger<DataInitializer> logger;
        private readonly WbyContext context;

        public DataInitializer(ILogger<DataInitializer> logger, WbyContext context)
        {
            this.logger = logger;
            this.context = context;
        }

        public async Task InitSampleDataAsync()
        {
            context.Database.EnsureCreated();
            await this.CreateSampleDataAsync();
        }

        private async Task CreateSampleDataAsync()
        {
            if (!context.Users.Any() && !context.Menus.Any() && !context.AuthItems.Any())
            {
                context.Users.AddRange(
                    new User()
                    {
                        Account = "Diana",
                        UserName = "黛安娜",
                        Address = "Guangzhou",
                        Tel = "1870620584",
                        Password = "123",
                        CreateTime = DateTime.Now,
                        FlagAdmin = 1,
                    },
                    new User()
                    {
                        Account = "Eliza",
                        UserName = "伊萊扎",
                        Address = "Guangzhou",
                        Tel = "1870620584",
                        Password = "123",
                        CreateTime = DateTime.Now,
                        FlagAdmin = 1,
                    },
                    new User()
                    {
                        Account = "Admin",
                        UserName = "弗洛拉",
                        Address = "Guangzhou",
                        Tel = "1870620584",
                        Password = "123",
                        CreateTime = DateTime.Now,
                        FlagAdmin = 1,
                    });

                context.Menus.AddRange(
                    new Menu() { MenuCode = "1001", MenuName = "用户管理", MenuCaption = "AccountBox", MenuNameSpace = "UserCenter", MenuAuth = 7 },
                    new Menu() { MenuCode = "1002", MenuName = "权限管理", MenuCaption = "Group", MenuNameSpace = "GroupCenter", MenuAuth = 7 },
                    new Menu() { MenuCode = "1003", MenuName = "个性化", MenuCaption = "Palette", MenuNameSpace = "SkinCenter", MenuAuth = 8 },
                    new Menu() { MenuCode = "1004", MenuName = "仪表板", MenuCaption = "TelevisionGuide", MenuNameSpace = "DashboardCenter", MenuAuth = 8 },
                    new Menu() { MenuCode = "1005", MenuName = "菜单管理", MenuCaption = "Menu", MenuNameSpace = "MenuCenter", MenuAuth = 7 }
                    );

                context.AuthItems.AddRange(
                    new AuthItem() { AuthColor = "#0080FF", AuthKind = "PlaylistPlus", AuthName = "添加", AuthValue = 1 },
                    new AuthItem() { AuthColor = "#28CBA3", AuthKind = "PlaylistPlay", AuthName = "修改", AuthValue = 2 },
                    new AuthItem() { AuthColor = "#FF5370", AuthKind = "PlaylistRemove", AuthName = "删除", AuthValue = 4 },
                    new AuthItem() { AuthColor = "#FF5370", AuthKind = "FileDocumentBoxSearchOutline", AuthName = "查看", AuthValue = 8 },
                    new AuthItem() { AuthColor = "#FF5370", AuthKind = "LocalPrintShop", AuthName = "打印", AuthValue = 16 },
                    new AuthItem() { AuthColor = "#FF5370", AuthKind = "UploadOutline", AuthName = "导入", AuthValue = 32 },
                    new AuthItem() { AuthColor = "#FF5370", AuthKind = "DownloadOutline", AuthName = "导出", AuthValue = 64 }
                    );

                await context.SaveChangesAsync();
            }
        }
    }
}
