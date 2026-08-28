using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

public static class DateUtil
{
    public static readonly string DateFormatBR = "dd-MM-yyyy";
    /// <summary>
    /// Parses dd-mm-yyyy, returns DateTime.MinValue as an error.
    /// </summary>
    /// <param name="toParse"></param>
    public static DateTime StringToDateBR(string toParse)
    {
        try
        {
            return DateTime.ParseExact(
                toParse, DateFormatBR, CultureInfo.InvariantCulture
            );
        }
        catch
        {
            return DateTime.MinValue;
        }
    }

    /// <summary>
    /// Parses dd-mm-yyyy, returns DateOnly.MinValue as an error.
    /// </summary>
    /// <param name="toParse"></param>
    /// <returns></returns>
    public static DateOnly StringToDateOnlyBR(string toParse)
    {
        var value = StringToDateBR(toParse);

        if(value == DateTime.MinValue)
            return DateOnly.MinValue;
        
        return DateOnly.FromDateTime(value);
    }
}