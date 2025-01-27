using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace EventManagerASP.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<string>? Roles { get; set; } = new List<string>();
        public ICollection<Organisator> Organisators { get; set; } = new List<Organisator>();
    }
}
