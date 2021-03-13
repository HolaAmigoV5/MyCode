using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using Wby.Demo.EFCore;
using Wby.Demo.Shared.DataModel;
using Wby.Demo.Shared.HttpContact.Response;

namespace Wby.Demo.Api.ApiManager
{
    public class AuthManager : IAuthItemManager
    {
        private readonly ILogger<AuthManager> logger;
        private readonly IUnitOfWork work;

        public AuthManager(ILogger<AuthManager> logger, IUnitOfWork work)
        {
            this.logger = logger;
            this.work = work;
        }

        public async Task<ApiResponse> GetAll()
        {
            try
            {
                var models = await work.GetRepository<AuthItem>().GetAllAsync();
                return new ApiResponse(200, models.OrderBy(t => t.AuthValue).ToList());
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "");
                return new ApiResponse(201, "");
            }
        }
    }
}
