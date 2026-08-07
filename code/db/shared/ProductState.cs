using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public enum ProductState: int
{
    IMPORTED = 0,
    EXPORTED = 1,
    LOST = 2,
    PROCESSED = 3,
    FABRICATED = 4
}