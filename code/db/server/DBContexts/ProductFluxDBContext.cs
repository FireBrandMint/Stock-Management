using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


public class ProductFluxDBContext: DbContext
{
    public DbSet<ProductFlux> Products => Set<ProductFlux>();
}