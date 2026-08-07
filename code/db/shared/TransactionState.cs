using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public enum TransactionState: int
{
    NONE = 0,
    /// <summary>
    /// Generic transfer in case bought and sold are obviously implied.
    /// </summary>
    TRANSFER = 1,
    BOUGHT = 2,
    SOLD = 3
}