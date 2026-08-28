using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;


public class ProductFlux
{
    public ProductState ProductState {get;set;}
    public int Quantity {get;set;}
    public TransactionState TransactionState {get;set;}
    public string Currency {get;set;} = "R$";
    public double Money {get;set;}

    // Just date of the day [xx/yy/zzzz] (indexed/filtering value)
    public DateOnly RegDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);

    // Just hours
    public TimeOnly RegTime { get; set; } = TimeOnly.FromDateTime(DateTime.Now);
}