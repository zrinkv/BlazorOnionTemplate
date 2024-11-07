using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using BlazorServer.UI.SharedServices;
using Models.Login;

namespace BlazorServer.UI.Controllers
{
    public class AuthController : Controller
    {
        private readonly IBaseHttpClient _baseHttpClient;

        public AuthController(IBaseHttpClient baseHttpClient)
        {
            _baseHttpClient = baseHttpClient;
        }

        [HttpGet("/auth/login")]
        [AllowAnonymous]
        public async Task<IActionResult> LogInUser(string username, string password, bool rememberMe)
        {
            LoginRequestModel model = new LoginRequestModel();
            model.Username = username;
            model.Password = password;           
            model.RememberMe = rememberMe;

            LoginResponseModel? authResponse = new LoginResponseModel();

            var result = await _baseHttpClient.PostRequestAsync("Login/Login", model);
            if (!result.Contains("wrong_username_password"))
            {
                authResponse = JsonConvert.DeserializeObject<LoginResponseModel>(result);
                var jwtSecurityToken = new JwtSecurityTokenHandler().ReadJwtToken(authResponse?.Token);
                
                var claims = new List<Claim>
                {
                    new Claim("jwtToken", authResponse?.Token ?? string.Empty),
                    new Claim(ClaimTypes.Name, jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == "sub")?.Value ?? string.Empty),
                    new Claim(ClaimTypes.Role, jwtSecurityToken.Claims.FirstOrDefault(x => x.Type.Contains("role"))?.Value ?? string.Empty),
                    new Claim(ClaimTypes.Email, jwtSecurityToken.Claims.FirstOrDefault(x => x.Type.Contains("email"))?.Value ?? string.Empty),
                    new Claim(ClaimTypes.Expiration, jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == "exp")?.Value ?? string.Empty)
                };
               
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(principal);

                return Redirect("/");
            }

            return Redirect("/login/true");
        }

        [HttpGet("/auth/logout/{displayLogoutMsg}")]        
        public async Task<IActionResult> LogOutUser(bool displayLogoutMsg)
        {            
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("/login/" + displayLogoutMsg);
        }
    }
}
