using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace EventManagerADV.Services
{
    public class MyMiddleWare
    {
        private readonly RequestDelegate _next;

        public MyMiddleWare(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext httpContext)
        {

            return _next(httpContext);
        }
    }

    public static class MyMiddleWareExtensions
    {
        public static IApplicationBuilder UseMyMiddleWare(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<MyMiddleWare>();
        }
    }
}
