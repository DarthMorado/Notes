using System.Diagnostics;
using DarthNotes.Enums;
using Microsoft.AspNetCore.Mvc;
using DarthNotes.Web.Models;
using DarthNotes.Web.Services.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;

namespace DarthNotes.Web.Controllers;

public class AuthController : Controller
{
    private readonly IUserService _userService;
    
    public AuthController(IUserService userService)
    {
        _userService = userService;
    }
    
    public IActionResult GoogleLogin()
    {
        var redirectUrl = Url.Action("GoogleResponse");
        var properties = new AuthenticationProperties { RedirectUri = redirectUrl };

        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        //todo[vg]: read about Challenge
    }
    
    public async Task<IActionResult> GoogleResponse()
    {
        var result = await HttpContext.AuthenticateAsync();

        var claims = result.Principal.Identities
            .FirstOrDefault()?.Claims;

        // Example: get email
        var email = claims?.FirstOrDefault(x => x.Type.Contains("email"))?.Value;

        var userId = await _userService.GetUserIdAsync(email, UserTypeEnum.GoogleAuth);
        // TODO: save/find user in DB

        return RedirectToAction("Index", "Home");
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}