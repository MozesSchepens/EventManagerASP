using EventManagerASP.APIModels;
using EventManagerASP.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EventManagerASP.APIControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountsController(SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
        }

        [HttpPost]
        [Route("/api/login")]
        public async Task<ActionResult<Boolean>> PostAccount([FromBody] ApiLoginModel login)
        {
            var result = await _signInManager.PasswordSignInAsync(login.Name, login.Password, true, lockoutOnFailure: false);

            return result.Succeeded;
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

    }

}