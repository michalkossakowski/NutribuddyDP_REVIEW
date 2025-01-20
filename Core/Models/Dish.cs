using Newtonsoft.Json;

namespace NutribuddyDP.Core.Models
{
    internal class Dish
    {
        [JsonProperty]
        public string Name { get; set; } = String.Empty;
        [JsonProperty]
        public List<FoodItem> Ingredients { get; set; }
        public Dictionary<string, double> TotalNutrients { get; set; }
        public DateTime Date { get; set; }

        public Dish()
        {
            Ingredients = [];
            TotalNutrients = [];
            Date = DateTime.Now;
        }

        public void CalculateTotalNutrients()
        {
            TotalNutrients.Clear(); //wyczyszczenie buforu

            foreach (var food in Ingredients)
            {
                foreach (var nutrient in food.Nutrients)
                {
                    double quantity = (nutrient.Value * food.QuantityInGrams) / 100; //przeliczenie ilości składnika potrzebnego do dania

                    if (TotalNutrients.ContainsKey(nutrient.Key))
                        TotalNutrients[nutrient.Key] += quantity;
                    else
                        TotalNutrients[nutrient.Key] = quantity;
                }
            }
        }
    }
}
