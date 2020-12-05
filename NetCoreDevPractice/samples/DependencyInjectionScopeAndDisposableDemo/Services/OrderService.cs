using System;

namespace DependencyInjectionScopeAndDisposableDemo.Services
{
    public interface IOrderService { }
    public class OrderService : IOrderService, IDisposable
    {
        public void Dispose()
        {
            Console.WriteLine($"DisposableOrderService Disposed:{this.GetHashCode()}");
        }
    }
}
