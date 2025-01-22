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
        public async Task<ActionResult<Boolean>> PostAccount([FromBody] LoginModel @login)
        {
            var result = await _signInManager.PasswordSignInAsync(login.Name, login.Password, true, lockoutOnFailure:false);

            return result.Succeeded;
        }

    }

}
