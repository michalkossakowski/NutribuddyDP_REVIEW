using NutribuddyDP.Core.Interfaces;
using NutribuddyDP.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutribuddyDP.Core.Controllers
{
    internal class ShoppingListContext(MealController mealController)
    {
        private IShoppingListStrategy? _strategy;
        private readonly MealController _mealController = mealController;

        public void SetStrategy(IShoppingListStrategy strategy)
        {
            _strategy = strategy;
        }

        public Dictionary<string, object> GenerateList()
        {
           if (_strategy == null)
                throw new InvalidOperationException("Strategy has not been set.");

            var plannedMeals = _mealController.GetPlannedMeals();
            var shoppingList = CreateShoppingList(plannedMeals);

            return _strategy.GenerateShoppingList(shoppingList);
        }

        private static Dictionary<string, FoodItem> CreateShoppingList(List<IMealComponent> plannedMeals)
        {
            var items = new Dictionary<string, FoodItem>();

            foreach (var mealComponent in plannedMeals)
            {
                ProcessMealComponent(mealComponent, items);
            }

            return items;
        }

        private static void ProcessMealComponent(IMealComponent component, Dictionary<string, FoodItem> items)
        {
            if (component is Meal meal)
            {
                foreach (var dish in meal.Dishes)
                {
                    foreach (var foodItem in dish.Ingredients)
                    {
                        AddOrUpdateItem(items, foodItem);
                    }
                }

                foreach (var foodItem in meal.FoodItems)
                {
                    AddOrUpdateItem(items, foodItem);
                }
            }
            else if (component is Day day)
            {
                foreach (var comp in day.Meals)
                {
                    ProcessMealComponent(comp, items);
                }
            }
            else if (component is Week week)
            {
                foreach (var comp in week.Days)
                {
                    ProcessMealComponent(comp, items);
                }
            }
        }

        private static void AddOrUpdateItem(Dictionary<string, FoodItem> items, FoodItem item)
        {
            if (item.QuantityInGrams > 0)
            {
                if (items.TryGetValue(item.Description, out FoodItem? value))
                {
                    value.QuantityInGrams += item.QuantityInGrams;
                }
                else
                {
                    items[item.Description] = new FoodItem
                    {
                        Description = item.Description,
                        QuantityInGrams = item.QuantityInGrams,
                        Nutrients = new Dictionary<string, double>(item.Nutrients),
                        Category = item.Category,
                    };
                }
            }
        }
    }
}
