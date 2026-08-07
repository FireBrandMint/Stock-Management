using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
[ApiController]
[Route("api/test_products")]
public class ProductTestEndpoint: ControllerBase
{
    //TODO
    /*
    [HttpGet("{id}")]
    public ActionResult<TestProduct.GetResult> Get(int id)
    {
        var product = FindProduct(id);

        if (product == null)
            return NotFound();

        return Ok(product);
    }
    */
}