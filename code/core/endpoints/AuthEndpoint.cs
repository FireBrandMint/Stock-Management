using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/auth")]
public class AuthEndpoint: AuthAwareEndpoint
{

    public AuthEndpoint(
        UserManager<DBUser> users,
        SignInManager<DBUser> signIn,
        RoleManager<IdentityRole> roles): base(users, signIn, roles
    )
    {}

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var user = await GetUser(request.UserField);

        if (user == null)
            return Unauthorized();

        var result = await SignInManager.PasswordSignInAsync(
            request.UserField,
            request.PWField,
            isPersistent: true,
            lockoutOnFailure: true
        );

        if (!result.Succeeded)
            return Unauthorized();
        
        await SignInManager.SignInAsync(user, isPersistent: true);

        // Success
        return Ok();
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await SignInManager.SignOutAsync();
        return Ok();
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<string>> Me()
    {
        var user = await Users.GetUserAsync(User);

        return user!.UserName!;
    }

    public class LoginRequest
    {
        public string UserField {get;set;} = "";
        public string PWField {get;set;} = "";
    }
}