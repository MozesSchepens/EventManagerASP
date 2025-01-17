using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventManagerADV.Models
{
    public class Users:IdentityUser
    {
        [Required]
        public string FirstName { get; set; }

        [Required] 
        public string LastName { get; set; }


        public override string ToString()
        {
            return FirstName + " " + LastName;
        }

    }
}
