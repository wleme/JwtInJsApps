using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace JwtInJsApps.Web.Controllers
{
    [ApiController]
    [Route("[CONTROLLER]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration config;

        public AuthController(IConfiguration config)
        {
            this.config = config;
        }


        [HttpPost]
        [Route("login")]
        public IActionResult Login(AuthLoginDto dto)
        {
            if (dto.UserName == "username" && dto.Password == "password")
            {
                var claims = new List<Claim>()
                {
                    new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.UniqueName,dto.UserName),
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Auth:Key"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token= new JwtSecurityToken(
                    config["Auth:Issuer"],
                    config["Auth:Audience"],
                    claims,
                    expires: DateTime.UtcNow.AddHours(1),
                    signingCredentials: creds
                    );


                var results = new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expirationUtc = token.ValidTo
                };

                if (Request.Headers.TryGetValue("X-Return-Cookie", out var returnCookie))
                {
                    if (returnCookie.ToString().ToUpper() == true.ToString().ToUpper())
                    {
                        Response.Cookies.Append(Support.Constants.Auth.JwtCookieName, results.token, new Microsoft.AspNetCore.Http.CookieOptions()
                        {
                            HttpOnly = true,
                            Expires = dto.RememberMe ? results.expirationUtc : (DateTime?)null,
                            Secure = true
                        });
                    }
                }

                return Created("", results);
            }

            return BadRequest("Cannot login");
        }
    }
}
