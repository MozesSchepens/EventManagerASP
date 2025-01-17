using EventManagerADV.Data;
using EventManagerADV.Models;

namespace EventManagerADV.Services
{
    public interface IMyUser
    {
        public Users User { get; }
    }

    public class MyUser : IMyUser
    {
        ApplicationDbContext _context;
        IHttpContextAccessor _httpContext;

        public Users User { get { return GetUser(); } }


        public MyUser(ApplicationDbContext context, IHttpContextAccessor httpContext)
        {
            _context = context;
            _httpContext = httpContext;
        }

        public Users GetUser()
        {
            string name = _httpContext.HttpContext.User.Identity.Name;
            if (name == null || name == "")
                return Globals.DefaultUser;
            else
                return _context.Users.FirstOrDefault(u => u.UserName == name);
        }
    }
}
