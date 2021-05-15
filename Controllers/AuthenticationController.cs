using Microsoft.AspNetCore.Mvc;
using NewsStacksAPI.Models.Dto;
using NewsStacksAPI.Repository.IRepository;
using System;
using System.Linq;

namespace NewsStacksAPI.Controllers
{
    [Route("api/authenticate")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAccountRepository _accountRepo;

        public AuthenticationController(IAccountRepository accountRepo)
        {
            _accountRepo = accountRepo;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto model)
        {
            if (!User.Identity.IsAuthenticated)
            {
                var token = _accountRepo.Login(model);
                if (token == null)
                {
                    return BadRequest(new { message = "Username or Password is incorrect" });
                }

                return Ok(token);
            }
            return Ok(new { message = "Already logged in" });
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterDto model)
        {
            string[] RoleTypes = { "Reader", "Writer", "Publisher" };
            if (!RoleTypes.Contains(model.Role))
            {
                return BadRequest(new { message = "Role must be Reader, Writer or Publisher" });
            }

            var uniqueUser = _accountRepo.isUniqueUser(model.Username);
            if (!uniqueUser)
            {
                return BadRequest(new { message = "Username already exists" });
            }

            var userObj = _accountRepo.Register(model);
            if (userObj == null)
            {
                return BadRequest(new { message = "Error while registering" });
            }

            var token = _accountRepo.GenerateToken(userObj.Username, userObj.Role);

            return Ok(token);
        }

    }
}
