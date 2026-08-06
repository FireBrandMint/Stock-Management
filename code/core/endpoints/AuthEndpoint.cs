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

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        // Username already exists?
        if (await Users.FindByNameAsync(request.UserName) != null)
            return BadRequest("Username already exists.");

        var user = new DBUser
        {
            UserName = request.UserName,
            Email = request.Email
        };

        var result = await Users.CreateAsync(user, request.Password);

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        // True if first user because
        // it was already added,
        // so count becomes 1 on the 1st time.
        bool firstUser = Users.Users.Count() == 1;

        await Users.AddToRoleAsync(
            user,
            firstUser ? "DEV" : "Guest");

        await SignInManager.SignInAsync(user, isPersistent: true);

        return Ok();
    }

    [Authorize]
    [HttpPost("promote")]
    public async Task<IActionResult> Promote(PromoteRequest request)
    {
        var caller = await Users.GetUserAsync(User);

        if (caller == null)
            return Unauthorized();
        
        if(!DBUser.Roles.TryGetValue(request.Role, out var desired_level))
            return BadRequest("Unknown role.");

        int user_level = await this.GetRoleLevel(caller);

        if (user_level < DBUser.LevelCanPromote || user_level <= desired_level)
            return Forbid();

        var target = await Users.FindByNameAsync(request.UserName);

        if (target == null)
            return NotFound();
        
        var target_level = await GetRoleLevel(target);

        if(user_level <= target_level)
            return Forbid("Lower staff cannot de-rank higher staff");

        // Remove existing hierarchy roles
        var currentRoles = await Users.GetRolesAsync(target);

        foreach (var role in currentRoles)
        {
            if (DBUser.Roles.ContainsKey(role))
                await Users.RemoveFromRoleAsync(target, role);
        }

        await Users.AddToRoleAsync(target, request.Role);

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

    public class RegisterRequest
    {
        public string UserName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
    }

    public class PromoteRequest
    {
        public string UserName { get; set; } = "";
        public string Role { get; set; } = "";
    }
}