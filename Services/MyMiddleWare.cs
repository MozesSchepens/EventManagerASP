using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

public class MyMiddleWare
{
    private readonly RequestDelegate _next;

    public MyMiddleWare(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Cookies.ContainsKey("MyCookie"))
        {
            var cookieValue = context.Request.Cookies["MyCookie"];
            context.Items["MyGlobalVariable"] = cookieValue;
        }

        await _next(context);
    }
}