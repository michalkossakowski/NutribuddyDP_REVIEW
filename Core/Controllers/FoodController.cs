﻿using NutribuddyDP.Core.Models;

namespace NutribuddyDP.Core.Controllers
{
    // REVIEW - gitem jest B)
    internal class FoodController
    {
        private readonly List<FoodItem>? _foodItems;

        public FoodController()
        {
            _foodItems = DataStorageFacade.GetInstance().ImportFoodItems();
        }

        public List<FoodItem>? GetAllFoods()
        {
            return _foodItems;
        }
    }
}
