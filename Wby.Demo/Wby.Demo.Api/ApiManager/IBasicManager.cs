﻿using System.Threading.Tasks;
using Wby.Demo.Shared.Dto;
using Wby.Demo.Shared.HttpContact.Response;
using Wby.Demo.Shared.Query;

namespace Wby.Demo.Api.ApiManager
{
    public interface IBasicManager
    {
        Task<ApiResponse> GetAll(QueryParameters param);

        Task<ApiResponse> Add(BasicDto param);

        Task<ApiResponse> Delete(int id);

        Task<ApiResponse> Save(BasicDto param);
    }
}
