using APIToken.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.Json;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace APIToken.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class LoginController: ControllerBase
    {

        private IConfiguration _config;

        public LoginController(IConfiguration config)
        {
            _config = config;
        }

        private UserModel AuthenticateUser(UserModel login)
        {
            UserModel user = null;
            if (login.Username.ToLower() == "shahirah" && login.Password.ToLower() == "busuk")
            {
                user = new UserModel { Username = login.Username, Password = login.Password };
            }

            return user;
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login([FromBody]UserModel login)
        {
            IActionResult response = Unauthorized();
            var user = AuthenticateUser(login);
            if (user != null)
            {
                var tokenString = GenerateJSONWebToken(user);
                response = Ok(new { token = tokenString });
            }

            return response;
        }

        private string GenerateJSONWebToken(UserModel userinfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub,userinfo.Username),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(_config["Jwt:Issuer"], 
                _config["Jwt:Issuer"], 
                claims, 
                expires: DateTime.Now.AddSeconds(120), 
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
            
        }

        
    }
}