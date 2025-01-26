using NutribuddyDP.Core.Interfaces;
using NutribuddyDP.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//<REVIEW> + Prawidłowo zastosowano wzorzec strategy.
// + Dzięki zastosowaniu interfejsu łatwo dodawać nowe strategie.
// + Strategie mogą być zmieniane dynamicznie patrz(ShoppingListView).
// + Kod jest reużywalny.
// - Było by fajnie jakby podzielić odpowiednio na foldery i nazwać je zgodnie z konwencją a nie wszystko wrzucać do Controllers.
// - Zwiększono liczbe klas zaś sam kod nie wydaję się być zbyt skomplikowany, aby usprawiedliwiać takie coś.
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
