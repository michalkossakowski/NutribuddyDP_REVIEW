using Newtonsoft.Json;
using NutribuddyDP.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutribuddyDP.Core.Models
{
    internal class Meal : PlanItem, IMealComponent
    {
        [JsonProperty]
        public List<FoodItem> FoodItems { get; set; } = [];
        [JsonProperty]
        public List<Dish> Dishes { get; set; } = [];

        public Meal(string name)
        {
            Name = name;
        }

        public void AddProduct(FoodItem product)
        {
            FoodItems.Add(product);
        }
            
        public void AddDish(Dish dish)
        {
            Dishes.Add(dish);
        }

        public void Add(IMealComponent component)
        {
            throw new NotImplementedException("Meals can only contain Products or Dishes.");
        }

        public void Remove(IMealComponent component)
        {
            throw new NotImplementedException("Meals can only contain Products or Dishes.");
        }
    }
}
