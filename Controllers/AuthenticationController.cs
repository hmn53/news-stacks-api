using Microsoft.AspNetCore.Http;
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

        /// <summary>
        /// Login route
        /// </summary>
        /// <param name="model">Login properties</param>
        /// <returns></returns>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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

        /// <summary>
        /// Register a new user
        /// </summary>
        /// <param name="model">Register properties</param>
        /// <returns></returns>
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
