using Wby.Demo.Shared.DataModel;

namespace Wby.Demo.EFCore
{
    public class CustomUserRepository : Repository<User>
    {
        public CustomUserRepository(WbyContext dbContext) : base(dbContext) { }
    }

    public class CustomUserLogRepository : Repository<UserLog>
    {
        public CustomUserLogRepository(WbyContext dbContext) : base(dbContext) { }
    }

    public class CustomMenuRepository : Repository<Menu>
    {
        public CustomMenuRepository(WbyContext dbContext) : base(dbContext) { }
    }

    public class CustomGroupRepository : Repository<Group>
    {
        public CustomGroupRepository(WbyContext dbContext) : base(dbContext) { }
    }

    public class CustomBasicRepository : Repository<Basic>
    {
        public CustomBasicRepository(WbyContext dbContext) : base(dbContext) { }
    }

    public class CustomAuthItemRepository : Repository<AuthItem>
    {
        public CustomAuthItemRepository(WbyContext dbContext) : base(dbContext) { }
    }
}
