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
[Route("api/products")]
public class ProductEndpoint: AuthAwareEndpoint
{
    private readonly ProductDBContext DB;

    public ProductEndpoint(
        ProductDBContext db,
        UserManager<DBUser> users,
        SignInManager<DBUser> signIn,
        RoleManager<IdentityRole> roles
    ): base(users, signIn, roles)
    {
        DB = db;
    }

    [HttpGet("{barcode}")]
    public async Task<ActionResult<Product>> Get(string barcode)
    {
        if(await GetRoleLevel() < 1)
            return Forbid("You are not even a worker, who are you?");
        var product = await DB.Products
            .SingleOrDefaultAsync(x => x.Barcode == barcode);

        if (product == null)
            return NotFound("No such product!");

        return Ok(product);
    }

    [HttpPost]
    public async Task<ActionResult<Product>> Create(Product product)
    {
        if(await GetRoleLevel() < DBUser.LevelCanManageProduct)
            return Forbid("NO PERMISSION");
        DB.Products.Add(product);
        await DB.SaveChangesAsync();

        return Ok("Product registered!");
    }

    [HttpPut("{barcode}")]
    public async Task<IActionResult> Update(string barcode, Product product)
    {
        if(await GetRoleLevel() < DBUser.LevelCanManageProduct)
            return Forbid("NO PERMISSION");
        
        var existing = await DB.Products
            .SingleOrDefaultAsync(x => x.Barcode == barcode);

        if (existing == null)
            return NotFound("No such product.");

        // Copy whatever fields you're allowing to change.
        existing.Name = product.Name;

        await DB.SaveChangesAsync();

        return Ok(existing);
    }

    [HttpDelete("{barcode}")]
    public async Task<IActionResult> Delete(string barcode)
    {
        if(await GetRoleLevel() < DBUser.LevelCanManageProduct)
            return Forbid("NO PERMISSION");

        var product = await DB.Products
            .SingleOrDefaultAsync(x => x.Barcode == barcode);

        if (product == null)
            return NotFound("No such product!");

        DB.Products.Remove(product);
        await DB.SaveChangesAsync();

        return Ok($"Removed item:\n{product}");
    }
}