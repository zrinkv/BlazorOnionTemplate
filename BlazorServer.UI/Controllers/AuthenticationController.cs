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
            if (result != null)
            {
                authResponse = JsonConvert.DeserializeObject<LoginResponseModel>(result);
                var jwtSecurityToken = new JwtSecurityTokenHandler().ReadJwtToken(authResponse?.Token);

                var claims = new List<Claim>
                {
                    new Claim("jwtToken", authResponse?.Token ?? ""),
                    new Claim(ClaimTypes.Name, jwtSecurityToken.Claims.First(x => x.Type == "sub").Value),
                    new Claim(ClaimTypes.Role, jwtSecurityToken.Claims.First(x => x.Type.Contains("role")).Value),
                    new Claim(ClaimTypes.Email, jwtSecurityToken.Claims.First(x => x.Type.Contains("email")).Value),
                    new Claim(ClaimTypes.Expiration, jwtSecurityToken.Claims.First(x => x.Type == "exp").Value)
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
