using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestSharpDemo
{
    public interface ITwitterClient
    {
        Task<TwitterUser> GetUser(string user);
    }
    public record TwitterUser(string Id, string Name, string Username);
}
