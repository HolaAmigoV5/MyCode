using System;
using System.Collections.Generic;
using System.Text;

namespace PrismDemo.Services
{
    public class HomePageService : IHomePageService
    {
        string _baseUrl;
        public HomePageService(string baseUrl)
        {
            _baseUrl = baseUrl;
        }
        public string SayHi()
        {
            return $"你好啊：{_baseUrl}";
        }
    }
}
