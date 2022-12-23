using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using QuasarCoreTimesheetsApp.Auth;
using QuasarCoreTimesheetsApp.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace QuasarCoreTimesheetsApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : BaseController
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IConfiguration _configuration;         // need to access the appsettings.json values

        public UserController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            RoleManager<Role> roleManager,
            IConfiguration configuration
        ) 
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]RegisterRequest request) 
        {
            var user = new User
            {
                UserName = request.Email,
                Email = request.Email,
                DisplayName = request.DisplayName
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                return BadRequest(new { Errors = result.Errors.Select(e => e.Description) });
            }

            return await LoginSuccessResponse(user);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
            {
                return BadRequest();
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (!result.Succeeded)
            {
                return BadRequest();
            }

            return await LoginSuccessResponse(user);
        }

        public async Task<IActionResult> RefreshToken()
        {
            var userId = GetUserId();
            var user = await _userManager.FindByIdAsync(userId.ToString());
            return await LoginSuccessResponse(user);
        }

        private async Task<IActionResult> LoginSuccessResponse(User user)
        {
            /*
             * https://learn.microsoft.com/en-us/aspnet/core/security/authorization/claims?view=aspnetcore-5.0
             A claim is a name value pair that represents what the subject is, not what the subject can do. For example, you may have a driver's license, issued by a local driving license authority. Your driver's license has your date of birth on it. In this case the claim name would be DateOfBirth, the claim value would be your date of birth, for example 8th June 1970 and the issuer would be the driving license authority. 
             */

            // This is a list, so I assume you can add more claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            // Fetching roles might take time
            // We need to add these roles into Claims list
            var roles = await _userManager.GetRolesAsync(user);
            foreach(var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // Configuring access token for this user
            var key = _configuration.GetValue<string>("Auth:Jwt");
            var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(30),
                    signingCredentials: new SigningCredentials(
                            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                            SecurityAlgorithms.HmacSha256
                        )
                );
            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

            // Wrap the response in a model
            return Ok(new LoginSuccessResponseModel 
            { 
                AccessToken =  accessToken
            });
        }
    }
}
