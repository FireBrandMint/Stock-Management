using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

public class ProductDBContext : DbContext
{
    public DbSet<Product> Products => Set<Product>();

    public ProductDBContext(DbContextOptions<ProductDBContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>()
            .HasKey(p => p.Id);

        modelBuilder.Entity<Product>()
            .HasIndex(p => p.Barcode)
            .IsUnique();
    }
}