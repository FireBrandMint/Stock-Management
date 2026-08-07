using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;


public class ProductFlux
{
    public Guid Id {get;set;}
    public ProductState ProductState {get;set;}
    public int Quantity {get;set;}
    public TransactionState TransactionState {get;set;}
    public string Currency {get;set;} = "R$";
    public double Money {get;set;}
    public DateTime RegDate {get;set;} = DateTime.Now;
}