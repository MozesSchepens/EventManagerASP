using EventManagerASP.Data;
using EventManagerASP.Models;
using Microsoft.AspNetCore.Http;
using System.Linq;

public interface IMyUser
{
    public ApplicationUser User { get; }
}

public class MyUser : IMyUser
{
    private readonly ApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContext;

    public ApplicationUser User => GetUser();

    public MyUser(ApplicationDbContext context, IHttpContextAccessor httpContext)
    {
        _context = context;
        _httpContext = httpContext;
    }

    public ApplicationUser GetUser()
    {
        var httpContext = _httpContext.HttpContext;
        if (httpContext == null || httpContext.User == null || httpContext.User.Identity == null || string.IsNullOrEmpty(httpContext.User.Identity.Name))
        {
            return null;
        }

        string name = httpContext.User.Identity.Name;
        return _context.Users.FirstOrDefault(u => u.UserName == name);
    }
}
