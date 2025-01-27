using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EventManagerASP.Data;
using EventManagerASP.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AdminController> _logger;

    public AdminController(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ApplicationDbContext context,
        ILogger<AdminController> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
        _logger = logger;
    }

    public async Task<IActionResult> ManageUsersRoles()
    {
        var users = await _userManager.Users.ToListAsync();
        var userRoles = new List<UserRolesViewModel>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            userRoles.Add(new UserRolesViewModel
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Roles = roles.ToList()
            });
        }

        ViewBag.Roles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
        return View(userRoles);
    }

    [HttpPost]
    public async Task<IActionResult> AssignRole(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            _logger.LogWarning($"Gebruiker met ID {userId} niet gevonden.");
            return NotFound("Gebruiker niet gevonden.");
        }

        if (!await _roleManager.RoleExistsAsync(role))
        {
            _logger.LogInformation($"Rol {role} bestaat niet, aanmaken...");
            await _roleManager.CreateAsync(new IdentityRole(role));
        }

        var result = await _userManager.AddToRoleAsync(user, role);
        if (!result.Succeeded)
        {
            _logger.LogError($"Fout bij toewijzen rol {role} aan gebruiker {user.UserName}.");
            return BadRequest(result.Errors);
        }

        _logger.LogInformation($"Rol {role} succesvol toegewezen aan {user.UserName}.");
        return RedirectToAction("ManageUsersRoles");
    }

    [HttpPost]
    public async Task<IActionResult> RemoveRole(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            _logger.LogWarning($"Gebruiker met ID {userId} niet gevonden.");
            return NotFound("Gebruiker niet gevonden.");
        }

        var result = await _userManager.RemoveFromRoleAsync(user, role);
        if (!result.Succeeded)
        {
            _logger.LogError($"Fout bij verwijderen rol {role} van gebruiker {user.UserName}.");
            return BadRequest(result.Errors);
        }

        _logger.LogInformation($"Rol {role} succesvol verwijderd van {user.UserName}.");
        return RedirectToAction("ManageUsersRoles");
    }
    [HttpPost]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            _logger.LogWarning($"Gebruiker met ID {id} niet gevonden.");
            return NotFound("Gebruiker niet gevonden.");
        }

        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
        {
            _logger.LogError($"Fout bij verwijderen gebruiker {user.UserName}.");
            return BadRequest(result.Errors);
        }

        _logger.LogInformation($"Gebruiker {user.UserName} succesvol verwijderd.");
        return RedirectToAction("ManageUsersRoles");
    }
    [HttpGet]
    public async Task<IActionResult> GetUserRoles(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            _logger.LogWarning($"Gebruiker met ID {userId} niet gevonden.");
            return NotFound("Gebruiker niet gevonden.");
        }

        var roles = await _userManager.GetRolesAsync(user);
        return Json(roles);
    }

}
