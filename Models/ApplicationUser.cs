using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace EventManagerASP.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public ICollection<Organisator> Organisators { get; set; } = new List<Organisator>();
    }
}
