﻿using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace BlazorServer.UI.Controllers 
{
    [Route("[controller]/[action]")]
    public class CultureController : Controller
    {        
        public IActionResult Set(string culture, string redirectUri)
        {
            if (culture != null)
            {
                var cookieOptions = new CookieOptions
                {
                    // Set the secure flag, which Chrome's changes will require for SameSite none.
                    // Note this will also require you to be running on HTTPS.
                    Secure = true,

                    // Set the cookie to HTTP only which is good practice unless you really do need to access it client side in scripts.
                    HttpOnly = true,

                    // Add the SameSite attribute, this will emit the attribute with a value of none.
                    SameSite = SameSiteMode.None

                    // The client should follow its default cookie policy.
                    // SameSite = SameSiteMode.Unspecified
                };

                HttpContext.Response.Cookies.Append(
                    CookieRequestCultureProvider.DefaultCookieName,
                    CookieRequestCultureProvider.MakeCookieValue(
                        new RequestCulture(culture, culture)), cookieOptions);
            }

            return LocalRedirect(redirectUri);
        }
    }
}