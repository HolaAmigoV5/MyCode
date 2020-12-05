using System.Collections.Generic;
using Autofac.Extras.DynamicProxy;
using Microsoft.AspNetCore.Mvc;

namespace Ywdsoft.AutofacTest.Controllers
{
    [Intercept(typeof(LogInterceptor))]
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        public IEnumerable<IDog> dogs { get; set; }

        //public ValuesController(IEnumerable<IDog> _dogs)
        //{
        //    dogs = _dogs;
        //}

        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            List<string> list = new List<string>();
            foreach (var dog in dogs)
            {
                list.Add($"名称：{dog.Name},品种：{dog.Breed}");
            }
            return list.ToArray(); ;
        }
    }
}
