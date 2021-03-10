using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Wby.Demo.Shared.Collections;
using Wby.Demo.Shared.HttpContact;
using Wby.Demo.Shared.Query;

namespace Wby.Demo.Service
{
    /// <summary>
    /// 具备基础的CRUD功能的请求基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseService<T>
    {
        private readonly string servicesName;

        public BaseService()
        {
            servicesName = typeof(T).Name.Replace("Dto", string.Empty);
        }

        public async Task<BaseResponse> AddAsync(T model)
        {
            var r = await new BaseServiceRequest().GetRequest<BaseResponse>($@"api/{servicesName}/Add", model, Method.POST);
            return r;
        }

        public async Task<BaseResponse> DeleteAsync(int id)
        {
            var r=await new BaseServiceRequest().
                GetRequest<BaseResponse>(@$"api/{servicesName}/Delete?id={id}", string.Empty, Method.DELETE);
            return r;
        }

        public async Task<BaseResponse<PagedList<T>>> GetAllListAsync(QueryParameters pms)
        {
            var r = await new BaseServiceRequest().
               GetRequest<BaseResponse<PagedList<T>>>(@$"api/{servicesName}/GetAll?PageIndex={pms.PageIndex}&PageSize={pms.PageSize}&Search={pms.Search}", string.Empty, Method.GET);
            return r;
        }

        public async Task<BaseResponse<T>> GetAsync(int id)
        {
            var r = await new BaseServiceRequest().
               GetRequest<BaseResponse<T>>(@$"api/{servicesName}/Get?id={id}", string.Empty, Method.GET);
            return r;
        }


        public async Task<BaseResponse> SaveAsync(T model)
        {
            var r = await new BaseServiceRequest().
                GetRequest<BaseResponse>($@"api/{servicesName}/Save", model, Method.POST);
            return r;
        }

        public async Task<BaseResponse> UpdateAsync(T model)
        {
            var r = await new BaseServiceRequest().
                GetRequest<BaseResponse>($@"api/{servicesName}/Update", model, Method.POST);
            return r;
        }
    }
}
