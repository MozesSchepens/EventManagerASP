using System.Collections.Generic;

public class EditUserViewModel
{
    public string UserId { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public List<RoleSelection> Roles { get; set; }
}

public class RoleSelection
{
    public string RoleName { get; set; }
    public bool IsSelected { get; set; }
}
