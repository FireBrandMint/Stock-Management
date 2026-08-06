using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;


public class AuthAwareEndpoint: ControllerBase
{
    protected readonly UserManager<DBUser> Users;
    protected readonly SignInManager<DBUser> SignInManager;
    protected readonly RoleManager<IdentityRole> Roles;

    public AuthAwareEndpoint(
        UserManager<DBUser> users,
        SignInManager<DBUser> signIn,
        RoleManager<IdentityRole> roles)
    {
        Users = users;
        SignInManager = signIn;
        Roles = roles;
    }

    public async Task<DBUser?> GetUser(string username)
    {
        return await Users.FindByNameAsync(username);
    }

    public async Task<IList<string>> GetRoles(DBUser user) => await Users.GetRolesAsync(user);

    public async Task<int> GetRoleLevel(DBUser user)
    {
        int roles_count;
        var roles = await GetRoles(user);
        roles_count = roles.Count;

        int level = -1;

        var all_roles = DBUser.Roles;

        for(int i = 0; i < roles_count; ++i)
            if(
                all_roles.TryGetValue(roles[i], out var curr_level)
                && curr_level > level
            )
                level = curr_level;

        return level;
    }

    public async Task<bool> IsUserAn(DBUser user, string role)
    {
        if(!DBUser.Roles.ContainsKey(role))
            throw new ArgumentException($"Role {role} does not exist currently.");
        
        return await Users.IsInRoleAsync(user, role);
    }
}