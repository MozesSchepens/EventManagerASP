using EventManagerASP.Data;
using EventManagerASP.Models;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace EventManagerASP.ViewModels
{
    public class UserViewModel
    {
        [Key]
        [Display (Name="User Id")]
        public string UserId { get; set; }

        [Display (Name="User Name")]
        public string UserName { get; set; }

        [EmailAddress]
        [Display (Name="Email")]
        public string UserEmail { get; set; }

        [Display (Name = "Name")]
        public string Name { get; set; }  // combined first + last name

        [Display (Name = "Roles")]
        public List<string> Roles { get; set; }

        [Display(Name = "Blocked ?")]
        public Boolean IsBlocked { get; set; } = false;


        public UserViewModel()
        {
        }

        public UserViewModel(ApplicationUser user, ApplicationDbContext context)
        {
            UserId = user.Id;
            UserName = user.UserName;
            UserEmail = user.Email;
            Name = user.ToString();
            IsBlocked = user.LockoutEnd > DateTime.Now;
            Roles = (from userRole in context.UserRoles
                    where userRole.UserId == user.Id
                    select userRole.RoleId)
                    .ToList();
        }
    }
}
