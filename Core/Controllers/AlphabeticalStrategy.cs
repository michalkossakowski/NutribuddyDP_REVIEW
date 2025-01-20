using NutribuddyDP.Core.Interfaces;
using NutribuddyDP.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutribuddyDP.Core.Controllers
{
    internal class AlphabeticalStrategy : IShoppingListStrategy
    {
        public Dictionary<string, object> GenerateShoppingList(Dictionary<string, FoodItem> items)
        {
            // sortowanie po nazwie
            var alphabeticSortedList = items
               .OrderBy(item => item.Value.Description) // Sortowanie po polu Description w FoodItem
               .ToDictionary(item => item.Key, item => (object)item.Value); // Konwersja do Dictionary<string, object>

            return alphabeticSortedList;
        }
    }
}
