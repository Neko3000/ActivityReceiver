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
using ActivityReceiver.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Http;

namespace ActivityReceiver.Controllers
{
    public class UserTokenController : Controller
    {
        private readonly IConfiguration _configuration;

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public UserTokenController(IConfiguration configuration, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _configuration = configuration;

            _userManager = userManager;
            _signInManager = signInManager;
        }

        // login - get token
        [HttpPost]
        public async Task<IActionResult> GetToken([FromBody]UserTokenLoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("the request is forbidden");
            }

            var user = await _userManager.FindByNameAsync(model.Username);
            if(user == null)
            {
                return NotFound("username / password is invalid");
            }

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, model.Password);
            if(!isPasswordValid)
            {
                return NotFound("username / password is invalid");
            }

            // set our tokens claims
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString("N")),
                new Claim(ClaimTypes.NameIdentifier,user.Id),
                new Claim(ClaimTypes.Name,user.UserName),
                new Claim(ClaimTypes.Role,(await _userManager.GetRolesAsync(user)).FirstOrDefault()),
            };

            // crete credentials used to generate the token
            var credentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"])),
                SecurityAlgorithms.HmacSha256
                );

            // generate the Jwt Token
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims:claims,
                expires:DateTime.Now.AddYears(1),
                signingCredentials:credentials
                );

            return Ok(new {
                token = new JwtSecurityTokenHandler().WriteToken(token)
            });
    
        }

        // registers
        [HttpPost]
        public async Task<IActionResult> Register([FromBody]UserTokenRegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("the request is forbidden");
            }

            var user = new ApplicationUser { UserName = model.Username };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors.FirstOrDefault().Description.ToString());
            }

            return Ok();
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public  IActionResult Authorize()
        {
            return Ok();
        }
    }
}
