using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Wby.Infrastructure.Core.Extensions;

namespace Wby.Infrastructure.Core.Behaviors
{
    public class TransactionBehavior<TDbContext, TRequest, TResponse>: IPipelineBehavior<TRequest, TResponse>
        where TDbContext : EFContext
    {
        ILogger _logger;
        TDbContext _dbContext;

        public TransactionBehavior(TDbContext dbContext, ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var response = default(TResponse);
            var typeName = request.GetGenericTypeName();

            try
            {
                if (_dbContext.HasActiveTransaction)
                {
                    //Todo...
                    //next()指具体的CommandHandler或者EventHandler。可以在此之前或者之后加入相关逻辑
                    return await next();
                }
                var strategy = _dbContext.Database.CreateExecutionStrategy();
                await strategy.ExecuteAsync(async () => {
                    Guid transactionId;
                    using (var transaction=await _dbContext.BeginTransactionAsync())
                    using (_logger.BeginScope("TransactionContext:{TransactionId}", transaction.TransactionId))
                    {
                        _logger.LogInformation("----开始事务 {TransactionId}({@Command})", 
                            transaction.TransactionId, typeName, request);

                        response = await next();

                        _logger.LogInformation("----提交事务 {TransactionId} {CommandName}", 
                            transaction.TransactionId, typeName);

                        await _dbContext.CommitTransactionAsync(transaction);
                        transactionId = transaction.TransactionId;
                    }
                });
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "处理事务出错 {CommandName} ({@Command})", typeName, request);
                throw;
            }
        }
    }
}
