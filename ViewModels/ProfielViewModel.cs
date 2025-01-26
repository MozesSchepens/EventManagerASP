using System.ComponentModel.DataAnnotations;

public class ProfileViewModel
{
    public string UserName { get; set; }

    [Required, EmailAddress]
    public string Email { get; set; }

    [Required]
    public string FirstName { get; set; }

    [Required]
    public string LastName { get; set; }

    public string PhoneNumber { get; set; }
}
