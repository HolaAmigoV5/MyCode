// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder.Internal;
using Microsoft.AspNetCore.Identity.Test;
using Microsoft.AspNetCore.Testing.xunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Microsoft.AspNetCore.Identity.EntityFrameworkCore.Test
{
    public class UserOnlyTest : IClassFixture<ScratchDatabaseFixture>
    {
        private readonly ApplicationBuilder _builder;
        private const string DatabaseName = nameof(UserOnlyTest);

        public class TestUserDbContext : IdentityUserContext<IdentityUser>
        {
            public TestUserDbContext(DbContextOptions options) : base(options) { }
        }

        public UserOnlyTest(ScratchDatabaseFixture fixture)
        {
            var services = new ServiceCollection();

            services
                .AddSingleton<IConfiguration>(new ConfigurationBuilder().Build())
                .AddDbContext<TestUserDbContext>(
                    o => o.UseSqlServer(fixture.ConnectionString)
                        .ConfigureWarnings(b => b.Log(CoreEventId.ManyServiceProvidersCreatedWarning)))
                .AddIdentityCore<IdentityUser>(o => { })
                .AddEntityFrameworkStores<TestUserDbContext>();

            services.AddLogging();

            var provider = services.BuildServiceProvider();
            _builder = new ApplicationBuilder(provider);

            using (var scoped = provider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            using (var db = scoped.ServiceProvider.GetRequiredService<TestUserDbContext>())
            {
                db.Database.EnsureCreated();
            }
        }

        [ConditionalFact]
        [FrameworkSkipCondition(RuntimeFrameworks.Mono)]
        [OSSkipCondition(OperatingSystems.Linux)]
        [OSSkipCondition(OperatingSystems.MacOSX)]
        public async Task EnsureStartupUsageWorks()
        {
            var userStore = _builder.ApplicationServices.GetRequiredService<IUserStore<IdentityUser>>();
            var userManager = _builder.ApplicationServices.GetRequiredService<UserManager<IdentityUser>>();

            Assert.NotNull(userStore);
            Assert.NotNull(userManager);

            const string userName = "admin";
            const string password = "1qaz@WSX";
            var user = new IdentityUser { UserName = userName };
            IdentityResultAssert.IsSuccess(await userManager.CreateAsync(user, password));
            IdentityResultAssert.IsSuccess(await userManager.DeleteAsync(user));
        }

        [ConditionalFact]
        [FrameworkSkipCondition(RuntimeFrameworks.Mono)]
        [OSSkipCondition(OperatingSystems.Linux)]
        [OSSkipCondition(OperatingSystems.MacOSX)]
        public async Task FindByEmailThrowsWithTwoUsersWithSameEmail()
        {
            var userStore = _builder.ApplicationServices.GetRequiredService<IUserStore<IdentityUser>>();
            var manager = _builder.ApplicationServices.GetRequiredService<UserManager<IdentityUser>>();

            Assert.NotNull(userStore);
            Assert.NotNull(manager);

            var userA = new IdentityUser(Guid.NewGuid().ToString());
            userA.Email = "dupe@dupe.com";
            const string password = "1qaz@WSX";
            IdentityResultAssert.IsSuccess(await manager.CreateAsync(userA, password));
            var userB = new IdentityUser(Guid.NewGuid().ToString());
            userB.Email = "dupe@dupe.com";
            IdentityResultAssert.IsSuccess(await manager.CreateAsync(userB, password));
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await manager.FindByEmailAsync("dupe@dupe.com"));
        }
    }
}
