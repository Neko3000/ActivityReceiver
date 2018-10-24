using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ActivityReceiver.Models;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace ActivityReceiver.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Login()
        {
            var username = "Ahri";
            var email = "ahri_lover@gmail.com";

            // set our tokens claims
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString("N")),
                new Claim(JwtRegisteredClaimNames.NameId,"unknownuser"),
                new Claim(JwtRegisteredClaimNames.Email,email),
                new Claim("my key","my value"),
            };

            // crete credentials used to generate the token
            var credentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes("ThisIsMySuperSecretKey")),
                SecurityAlgorithms.HmacSha256
                );

            // generate the Jwt Token
            var token = new JwtSecurityToken(
                issuer: "ActivityReceiver.API",
                audience: "ActivityReceiver.iOS",
                claims:claims,
                expires:DateTime.Now.AddMonths(3),
                signingCredentials:credentials
                );

            return Ok(new {
                token = new JwtSecurityTokenHandler().WriteToken(token)
            });
    
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult PrivateArea()
        {
            return Content("You are in the correct position.");
        }
    }
}
