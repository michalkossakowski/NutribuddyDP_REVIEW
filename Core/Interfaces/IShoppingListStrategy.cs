﻿using NutribuddyDP.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// REVIEW - dużo nieużytych usingów

namespace NutribuddyDP.Core.Interfaces
{
    internal interface IShoppingListStrategy
    {
        Dictionary<string, object> GenerateShoppingList(Dictionary<string, FoodItem> shoppingList);

    }
}
