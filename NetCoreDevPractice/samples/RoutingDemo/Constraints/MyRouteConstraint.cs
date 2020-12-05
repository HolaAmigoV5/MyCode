using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace RoutingDemo.Constraints
{
    //自定义约束
    public class MyRouteConstraint : IRouteConstraint
    {
        public bool Match(HttpContext httpContext, IRouter route, 
            string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
        {
            if (RouteDirection.IncomingRequest == routeDirection)
            {
                var v = values[routeKey];
                if (long.TryParse(v.ToString(), out var value)) { return true; }
            }
            return false;
        }
    }
}
