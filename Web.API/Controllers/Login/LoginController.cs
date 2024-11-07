using Application.Services.Login.Queries.GetUserByUsernameAndPassword;
using Common.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlazorServer.API.Controllers.Login
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LoginController(IConfiguration config, ILogger<LoginController> logger) : ApiBaseController(config)
    {
        private readonly ILogger<LoginController> _logger = logger;

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<LoggedUserViewModel>> Login(GetUserByUsernameAndPasswordQuery request)
        {            
            LoggedUserViewModel user = await Mediator.Send(request);

            if (user != null)
                return Ok(new { token = GenerateJSONWebToken(user, request.RememberMe) });            

            return BadRequest("wrong_username_password");
        }

        [HttpGet]
        [Authorize(Roles = Role.Administrator)]
        public ActionResult<IEnumerable<string>> GetData()
        {
            var currentUser = HttpContext.User;
            int spendingTimeWithCompany = 0;

            if (currentUser.HasClaim(c => c.Type == "DateOfJoining"))
            {
                DateTime date = DateTime.Parse(currentUser.Claims.First(c => c.Type == "DateOfJoining").Value);
                spendingTimeWithCompany = DateTime.Today.Year - date.Year;
            }

            if (spendingTimeWithCompany > 5)
            {
                return new string[] { "High Time1", "High Time2", "High Time3", "High Time4", "High Time5" };
            }
            else
            {
                return new string[] { "value1", "value2", "value3", "value4", "value5" };
            }
        }
    }
}
