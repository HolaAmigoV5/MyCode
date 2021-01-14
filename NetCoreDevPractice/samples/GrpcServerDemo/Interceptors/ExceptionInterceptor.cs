using Grpc.Core;
using Grpc.Core.Interceptors;
using System;
using System.Threading.Tasks;

namespace GrpcServerDemo.Interceptors
{
    public class ExceptionInterceptor : Interceptor
    {
        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
        {
            //拦截器，注入自己的异常处理逻辑
            try
            {
                return await base.UnaryServerHandler(request, context, continuation);
            }
            catch (Exception ex)
            {
                var data = new Metadata { { "message", ex.Message } };
                throw new RpcException(new Status(StatusCode.Unknown, "Unknon"), data);

            }
        }
    }
}
