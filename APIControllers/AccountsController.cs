using EventManagerADV.APIModels;
using EventManagerADV.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EventManagerADV.APIControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly SignInManager<Users> _signInManager;

        public AccountsController(SignInManager<Users> signInManager) 
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
