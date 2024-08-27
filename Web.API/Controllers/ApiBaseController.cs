using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MediatR;
using Application.Services.Login.Queries.GetUserByUsernameAndPassword;

namespace BlazorServer.API.Controllers
{    
    public class ApiBaseController : ControllerBase
    {
        private ISender _mediator = null!;

        protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();
        private readonly IConfiguration? _config;

        protected ApiBaseController(IConfiguration config)
        {
            _config = config;
        }

        protected ApiBaseController()
        {
        }

        protected string GenerateJSONWebToken(LoggedUserViewModel userInfo, bool RememberMe)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim> {
                new Claim(JwtRegisteredClaimNames.Sub, userInfo.Username),
                new Claim(JwtRegisteredClaimNames.Email, userInfo.Email),          
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),                
                new Claim(ClaimTypes.Name, userInfo.Username),
                new Claim("FullName", "Ime Prezime"),
                new Claim("userId", userInfo.UserId.ToString()),
            };

            for (int i = 0; i < userInfo.Roles?.Count; i++) //adding user roles to claim
                claims.Add(new Claim(ClaimTypes.Role, userInfo.Roles[i]));            

            var token = new JwtSecurityToken(
                _config["Jwt:Issuer"],
                _config["Jwt:Issuer"],
                claims,
                expires: RememberMe ? DateTime.UtcNow.AddHours(24) : DateTime.UtcNow.AddMinutes(int.Parse(_config["Jwt:ExpiresInMinutes"])),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
