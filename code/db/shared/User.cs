using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;


public class DBUser: IdentityUser
{
    public static FrozenDictionary<string, int> Roles = new Dictionary<string, int>()
    {
        {"Guest", 0},
        {"Worker", 1},
        {"Manager", 2},
        {"Owner", 3} 
    }.ToFrozenDictionary();
}