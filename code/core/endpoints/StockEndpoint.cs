using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize]
[ApiController]
[Route("api/stock")]
public class StockEndpoint: AuthAwareEndpoint
{
    public StockDBContext DB;
    public StockEndpoint(
        StockDBContext db,
        UserManager<DBUser> users,
        SignInManager<DBUser> signIn,
        RoleManager<IdentityRole> roles
    ): base(users, signIn, roles)
    {
        DB = db;
    }

    [HttpGet("{dateStr}")]
    public async Task<ActionResult<ProductFlux[]?>> Get(string dateStr)
    {

        var date = DateUtil.StringToDateOnlyBR(dateStr);
        
        //Date returned bad parse;
        //can't do anything with the data.
        if(date == DateOnly.MinValue)
            return BadRequest();

        var result = await DB.Flux.Where(x => x.RegDate == date).ToArrayAsync();
        
        return result;
    }

    [HttpPost]
    public async Task<ActionResult<ProductFlux>> Create(ProductFlux flux)
    {
        DB.Flux.Add(flux);
        await DB.SaveChangesAsync();

        return Ok();
    }

    //Remove not included because frankly why would you need it?
}