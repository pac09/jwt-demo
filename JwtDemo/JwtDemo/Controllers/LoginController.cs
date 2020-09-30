using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using JwtDemo.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace JwtDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private IConfiguration _config;

        public LoginController(IConfiguration config) 
        {
            _config = config;
        }

        [HttpGet]
        public IActionResult Login(string username, string password, string emailAddress)
        {
            UserModel login = new UserModel()
            { 
                UserName = username,
                Password = password,
                EmailAddress = emailAddress
            };

            var user = AuthenticateUser(login);
            if (user != null)
            {
                var strToken = GenerateJsonWebToken(user);
                return Ok(new { token = strToken });
            }
            else
                return Unauthorized();
        }

        [Authorize]
        [HttpPost("Post")]
        public string Post() 
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            IList<Claim> claim = identity.Claims.ToList();

            var userName = claim[0].Value;
            return "Welcome To: " + userName;
        }

        [Authorize]
        [HttpGet("GetValue")]
        public ActionResult<IEnumerable<string>> Get() 
        {
            return new string[] { "One", "Two", "Three" };
        }
        

        #region Private_Methods
        private UserModel AuthenticateUser(UserModel login)
        {
            if (login.UserName.Equals("Pastor") && login.Password.Equals("Secret123"))
                return new UserModel()
                {
                    UserName = login.UserName,
                    Password = login.Password,
                    EmailAddress = login.EmailAddress
                };
            else
                return null;
        }

        private string GenerateJsonWebToken(UserModel userData)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF32.GetBytes(_config["JwtConfig:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[] 
            {
                new Claim(JwtRegisteredClaimNames.Sub, userData.UserName),
                new Claim(JwtRegisteredClaimNames.Email, userData.EmailAddress),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(issuer: _config["JwtConfig:Issuer"], audience: _config["JwtConfig:Issuer"], claims, expires: DateTime.Now.AddMinutes(5), signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        #endregion


    }
}
