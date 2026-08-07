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
    public string RegDate {get;set;} = GetCurrentDate();

    public static string GetCurrentDate()
    {
        var now = DateTime.Now;
        var culture = new CultureInfo("pt-BR");
        //dd/MM/yyyy HH:mm (e.g., 06/08/2026 23:13)
        return now.ToString("g", culture);
    }
}