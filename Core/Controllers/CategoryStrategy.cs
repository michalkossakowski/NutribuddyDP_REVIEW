using NutribuddyDP.Core.Interfaces;
using NutribuddyDP.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutribuddyDP.Core.Controllers
{
    //<REVIEW> + Prawidłowo zastosowano wzorzec strategy.
    // + Dzięki zastosowaniu interfejsu łatwo dodawać nowe strategie.
    // + Strategie mogą być zmieniane dynamicznie patrz(ShoppingListView).
    // + Kod jest reużywalny.
    // - Było by fajnie jakby podzielić odpowiednio na foldery i nazwać je zgodnie z konwencją a nie wszystko wrzucać do Controllers.
    // - Zwiększono liczbe klas zaś sam kod nie wydaję się być zbyt skomplikowany, aby usprawiedliwiać takie coś.
    internal class CategoryStrategy: IShoppingListStrategy
    {
        public CategoryStrategy() { }

        // REVIEW - Wartość w słowniku dicst jest typu object ze względu na ogólność,
        // ale w praktyce zawsze będzie rzutowana na List<FoodItem> zatem nie ma sensu.
        // Jak już chcemy, aby potencjalnie dodawać dodatkowe grupy to lepiej stworzyć klasę/interface,
        // którego obiekty będziemy trzymać zamiast object.
        public Dictionary<string, object> GenerateShoppingList(Dictionary<string, FoodItem> items)
        {
            var dicst = new Dictionary<string, object>();

            foreach (var item in items)
            {
                // Sprawdź, czy kategoria już istnieje w słowniku
                if (!dicst.TryGetValue(item.Value.Category, out object? value))
                {
                    value = new List<FoodItem>();
                    // Jeśli nie istnieje, dodaj nową kategorię z pustą listą
                    dicst[item.Value.Category] = value;
                }

                // Dodaj produkt do odpowiedniej listy
                ((List<FoodItem>)value).Add(item.Value);
            }

            return dicst;
        }


    }
}
