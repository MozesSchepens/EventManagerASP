using EventManagerADV.Data;
using EventManagerADV.Models;

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
        string name = _httpContext.HttpContext.User.Identity.Name;
        if (string.IsNullOrEmpty(name))
            return null;
        else
            return _context.Users.FirstOrDefault(u => u.UserName == name);
    }
}
