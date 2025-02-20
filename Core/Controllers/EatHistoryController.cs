﻿using NutribuddyDP.Core.Models;
using Spectre.Console;

namespace NutribuddyDP.Core.Controllers
{
    // <REVIEW> kontroller jest poprawnie zaimplementowany i rozdziela odpowiednią widok od controllera
    internal class EatHistoryController
    {
        public EatHistory EatHistory { get; private set; }
        public Calendar Calendar { get; private set; }

        public EatHistoryController()
        {
            EatHistory = DataStorageFacade.GetInstance().ImportEatHistory();
            Calendar = new Calendar(DateTime.Now);
            BuildCalendar(Calendar, DateTime.Now.Year, DateTime.Now.Month);
        }

        public Dictionary<string, double> GetTotalNutrientsFromDay(DateTime date)
        {
            var dishesForDay = EatHistory.DishEatHistory
                .FindAll(record => record.Item1.Date == date.Date);

            var totalNutrients = new Dictionary<string, double>();

            foreach (var record in dishesForDay)
            {
                foreach (var nutrient in record.Item2.TotalNutrients)
                {
                    if (totalNutrients.ContainsKey(nutrient.Key))
                    {
                        totalNutrients[nutrient.Key] += nutrient.Value;
                    }
                    else
                    {
                        totalNutrients[nutrient.Key] = nutrient.Value;
                    }
                }
            }

            var foodItemsForDay = EatHistory.FoodItemEatHistory
                .FindAll(record => record.Item1.Date == date.Date);

            foreach (var food in foodItemsForDay)
            {
                foreach (var nutrient in food.Item2.Nutrients)
                {
                    double quantity = (nutrient.Value * food.Item2.QuantityInGrams) / 100;

                    if (totalNutrients.ContainsKey(nutrient.Key))
                        totalNutrients[nutrient.Key] += quantity;
                    else
                        totalNutrients[nutrient.Key] = quantity;
                }
            }
            return totalNutrients;
        }

        // <REVIEW> Podana metoda jest zależna od biblioteki Spectre.Console co jest złym pomysłem w kontekście wzorca MVC.
        // Jeżeli chcemy zachować zgodność z wzorcem MVC to metoda powinna zwracać dane a nie generować widok.
        public void BuildCalendar(Calendar calendar, int year, int month)
        {
            var tempMonth = month;
            DateTime date = new(year, month, 1);
            var nutrientsDay = new Dictionary<int, Dictionary<string, double>>();
            calendar.HighlightStyle(Style.Parse("bold #A2D2FF"));

            calendar.Year = year;
            calendar.Month = month;
            calendar.Day = 1;
            calendar.CalendarEvents.Clear();
            while (tempMonth == month)
            {
                if (date > DateTime.Now)
                {
                    return;
                }

                nutrientsDay[date.Day] = GetTotalNutrientsFromDay(date);
                if (!nutrientsDay[date.Day].ContainsKey("Energy (kcal)"))
                {
                    date = date.AddDays(1);
                    continue;
                }

                Calendar = calendar.AddCalendarEvent(nutrientsDay[date.Day]["Energy (kcal)"].ToString(), date);
                date = date.AddDays(1);
            }
        }

        public void AddDishToHistory(DateTime dateTime, Dish dish)
        {
            EatHistory.DishEatHistory.Add((dateTime, dish));
            SaveEatHistory();
        }

        public void AddFoodItemToHistory(DateTime dateTime, FoodItem foodItem)
        {
            EatHistory.FoodItemEatHistory.Add((dateTime, foodItem));
            SaveEatHistory();
        }

        private void SaveEatHistory()
        {
            DataStorageFacade.GetInstance().ExportEatHistory(EatHistory);
        }
    }
}
