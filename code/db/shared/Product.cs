using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


public class Product
{
    public Guid Id { get; set; }

    public string Name { get; set; } = "";

    //TODO: Possibly implement GS1 barcode system to generate this.
    public string Barcode { get; set; } = "";

    public ProductState State { get; set; }
}