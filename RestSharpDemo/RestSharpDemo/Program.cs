using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.Design;

namespace RestSharpDemo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IServiceCollection services = new ServiceCollection();
            services.AddSingleton<ITwitterClient>(new TwitterClient("abc", "cdf"));

            Console.WriteLine("Hello, World!");
        }
    }
}