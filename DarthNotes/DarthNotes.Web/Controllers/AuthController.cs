using System.Diagnostics;
using DarthNotes.Enums;
using Microsoft.AspNetCore.Mvc;
using DarthNotes.Web.Models;
using DarthNotes.Web.Services.Auth;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;

namespace DarthNotes.Web.Controllers;

public class AuthController : Controller
{
    private readonly IUserService _userService;
    private readonly IUserContext _userContext;
    private readonly IMapper _mapper;
    
    public AuthController(IUserService userService,
        IMapper mapper,
        IUserContext userContext)
    {
        _mapper = mapper;
        _userService = userService;
        _userContext = userContext;
    }
    
    public IActionResult GoogleLogin(bool isForApp = false)
    {
        string redirectUrl;
        if (isForApp)
        {
            redirectUrl = Url.Action("GoogleAppResponse");
        }
        else
        {
            redirectUrl = Url.Action("GoogleResponse");    
        }
        
        var properties = new AuthenticationProperties { RedirectUri = redirectUrl };

        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        //todo[vg]: read about Challenge
    }

    public async Task<IActionResult> GoogleAppResponse()
    {
        int userId = await GetUserIdFromClaims();

        var token = await _userService.GenerateAuthToken(userId);

        return Redirect("darthnotes://auth-success");
    }
    
    public async Task<IActionResult> GoogleResponse()
    {
        int userId = await GetUserIdFromClaims();
        
        return RedirectToAction("Index", "Home");
    }

    private async Task<int> GetUserIdFromClaims()
    {
        var result = await HttpContext.AuthenticateAsync();

        var claims = result.Principal.Identities
            .FirstOrDefault()?.Claims;

        // Example: get email
        var email = claims?.FirstOrDefault(x => x.Type.Contains("email"))?.Value;

        var userId = await _userService.GetUserIdAsync(email, UserTypeEnum.GoogleAuth);

        return userId.Value;
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Profile()
    {
        var userId = _userContext.GetUserId().Value;
        var user = await _userService.GetUserAsync(userId);
        var model = _mapper.Map<UserModel>(user);
        return View("Profile", model);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateProfileAsync(UserModel model)
    {
        await _userService.UpdateUserProfileAsync(model.Id, model.IsPasswordAuthEnabled, model.Password);
        return RedirectToAction("Profile");
    }

    [HttpGet]
    public async Task<IActionResult> MultiLoginAsync()
    {
        var model = new LoginModel();
        return View("Login", model);
    }

    // [HttpPost]
    // public async Task<IActionResult> SignIn(LoginModel model)
    // {
    //     
    // }
}