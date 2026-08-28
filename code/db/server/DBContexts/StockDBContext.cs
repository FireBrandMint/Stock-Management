using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

public class StockDBContext: DbContext
{
    public DbSet<ProductFlux> Flux => Set<ProductFlux>();

    public StockDBContext(DbContextOptions<StockDBContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<ProductFlux>()
            .HasIndex(p => p.RegDate);
    }
}