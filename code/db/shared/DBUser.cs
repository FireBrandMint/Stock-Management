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
        //Stock Registration Especialist
        {"SRE", 2},
        //Product Registration Especialist
        {"PRE", 3},
        {"Manager", 4},
        {"Owner", 5},
        {"DEV", 6}
    }.ToFrozenDictionary();
    public const int LevelCanPromote = 4;
    public const int LevelCanManageStock = 2;
    public const int LevelCanManageProduct = 3;
}