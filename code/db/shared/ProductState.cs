using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public enum ProductState: int
{
    #region DECREMENT_PRODUCT
    PROCESSED = 0,
    EXPORTED = 1,
    LOST = 2,
    #endregion
    #region INCREMENT_PRODUCT
    IMPORTED = 3,
    FABRICATED = 4
    #endregion
}