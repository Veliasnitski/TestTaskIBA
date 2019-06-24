using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using WebManagerTasks.Data.Models;
using WebManagerTasks.ViewModels;
using WebManagerTasks.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebManagerTasks.Controllers
{
    [Route("api/[controller]")]
    public class IdentityController : Controller
    {
        private readonly IService<User> userService;

        public IdentityController(IService<User> userService)
        {
            this.userService = userService;
        }

        [HttpPost("token")]
        public async Task<IActionResult> Token([FromBody]User model)
        {            
            IReadOnlyCollection<Claim> identity = await GetIdentity(model);
            if (identity == null)
            {
                return Unauthorized();
            }
            var now = DateTime.UtcNow;
            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    notBefore: now,
                    claims: identity,
                    expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return Ok(encodedJwt);
        }

        [HttpPost("claim")]
        public async Task<IActionResult> Claim([FromBody]User model)
        {
            IReadOnlyCollection<Claim> identity = await GetIdentity(model);
            if (identity == null)
            {
                return Unauthorized();
            }
            //add encrypt 
            return Ok(identity);
        }

        private async Task<IReadOnlyCollection<Claim>> GetIdentity(User model)
        {
            List<Claim> claims = null;
            var user = await userService.GetAsync(el => el.Login == model.Login);
            if (user != null)
            {
                SHA256Managed sha256 = new SHA256Managed();
                var passwordHash = Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(model.Password)));
                if (passwordHash == user.Password)
                {
                    claims = new List<Claim>
                    {
                        new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login),
                        new Claim(ClaimsIdentity.DefaultRoleClaimType, user.IdUser.ToString())
                    };
                }
            }
            return claims;
        }
    }
}
