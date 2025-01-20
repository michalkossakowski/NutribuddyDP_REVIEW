using NutribuddyDP.Core.Interfaces;
using NutribuddyDP.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutribuddyDP.Core.Controllers
{
    internal class CategoryStrategy: IShoppingListStrategy
    {
        public CategoryStrategy() { }


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
