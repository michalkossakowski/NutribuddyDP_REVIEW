using NutribuddyDP.Core.Models;
using Spectre.Console;

namespace NutribuddyDP.Core.Controllers
{
    internal class DishController
    {
        private readonly List<Dish>? _dishes;

        public DishController()
        {
            _dishes = DataStorageFacade.GetInstance().ImportDishes();
        }

        public List<Dish>? GetAllDishes()
        {
            return _dishes;
        }

        public Dictionary<string, double> GetForeverNutrients() // NIE KORZYSTAĆ - DEPRECATED
        {
            // ta funkcja na razie zlicza wszystkie dania.
            // INNA IMPLEMENTACJA W EatHistoryController, BIORĄCA NUTRIENTS TYLKO Z JEDNEGO DNIA
            Dictionary<string, double> totalNutrients = [];
            foreach (var dish in _dishes!)
            {
                foreach (var nutrient in dish.TotalNutrients)
                {
                    var exists = !totalNutrients.TryAdd(nutrient.Key, nutrient.Value);

                    if (exists)
                    {
                        totalNutrients[nutrient.Key] += nutrient.Value;
                    }
                }
            }

            return totalNutrients;
        }

        // <Review> - Controller nie powinnień odpowiadać za wypisywanie do konsoli,
        // lepiej zwracać string lub bool jeżeli chcemy poinformować o błędzie wykonywania lub pustej tablicy
        public static void SetIngredientQuantity(Dish dish, string foodDescription, double quantityInGrams)
        {
            var ingredient = dish.Ingredients.FirstOrDefault(f => f.Description == foodDescription);
            if (ingredient != null)
            {
                ingredient.QuantityInGrams = quantityInGrams;
                dish.CalculateTotalNutrients();
            }
            else
            {
                AnsiConsole.WriteLine($"Ingredient '{foodDescription}' not found in the dish.");
            }
        }

        public void AddDish(Dish dish)
        {
            dish.CalculateTotalNutrients();
            _dishes!.Add(dish);
            SaveDishes();
        }



        // <Review> - Controller nie powinnień odpowiadać za wypisywanie do konsoli,
        // lepiej zwracać string lub bool jeżeli chcemy poinformować o błędzie wykonywania lub pustej tablicy
        public void EditDish(string dishName, Action<Dish> editAction)
        {
            var dish = _dishes!.FirstOrDefault(d => d.Name.Equals(dishName, StringComparison.OrdinalIgnoreCase));
            if (dish != null)
            {
                editAction(dish);
                dish.CalculateTotalNutrients();
                SaveDishes();
            }
            else
            {
                AnsiConsole.WriteLine($"Dish '{dishName}' not found.");
            }
        }


        // <Review> - Controller nie powinnień odpowiadać za wypisywanie do konsoli,
        // lepiej zwracać string lub bool jeżeli chcemy poinformować o błędzie wykonywania lub pustej tablicy
        public void DeleteDish(string dishName)
        {
            var dish = _dishes!.FirstOrDefault(d => d.Name.Equals(dishName, StringComparison.OrdinalIgnoreCase));
            if (dish != null)
            {
                _dishes!.Remove(dish);
                SaveDishes();
            }
            else
            {
                AnsiConsole.WriteLine($"Dish '{dishName}' not found.");
            }
        }

        private void SaveDishes() //zapis do pliku działa tylko przy podaniu sciezki absolutnej :(
        {
            DataStorageFacade.GetInstance().ExportDishes(_dishes!);
        }
    }
}
