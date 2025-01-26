using NutribuddyDP.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutribuddyDP.Core.Controllers
{
    internal class MealController()
    {
        // REVIEW - Dobrą praktyką jest inicjowane listy w kontruktorze, ułatwia to testowanie
        // i zapobiega błędom związanych z niezainicjowanymi listami.
        private readonly List<IMealComponent>? _plannedMeals = DataStorageFacade.GetInstance().ImportPlannedMeals();

        public List<IMealComponent>? GetPlannedMeals()
        {
            return _plannedMeals;
        }

        public void AddMeal(IMealComponent mealComponent)
        {
            _plannedMeals!.Add(mealComponent);
            Save();
        }

        public void RemovePlan(IMealComponent mealComponent)
        {
            _plannedMeals!.Remove(mealComponent);
            Save();
        }

        private void Save()
        {
            DataStorageFacade.Instance.ExportPlannedMeals(_plannedMeals!);
        }

    }
}
