using Newtonsoft.Json;
using NutribuddyDP.Core.Interfaces;
using NutribuddyDP.Core.Models;
using System.Text.Json; // REVIEW - nieużytu using

namespace NutribuddyDP.Core.Controllers
{
    internal class DataStorageFacade()
    {
        // REVIEW - nie powinno się trzymać takich rzeczy jak wszelakie niezmienne
        // ścieżki, klucze, urle powinny one być zadeklarowane np. w appsettings.json
        // lub przy pomocy "Manage User Secrets" a nie jako zmienne w klasie
        private readonly string _filePathDish = "C:\\Users\\Administrator\\Source\\Repos\\NutribuddyDP\\Data\\DishData.json";
        private readonly string _filePathFoodItems = "C:\\Users\\Administrator\\Source\\Repos\\NutribuddyDP\\Data\\FoodData.json";
        private readonly string _filePathDishHistory = "C:\\Users\\Administrator\\Source\\Repos\\NutribuddyDP\\Data\\DishHistory.json";
        private readonly string _filePathFoodHistory = "C:\\Users\\Administrator\\Source\\Repos\\NutribuddyDP\\Data\\FoodHistory.json";
        private readonly string _filePathUserData = "C:\\Users\\Administrator\\Source\\Repos\\NutribuddyDP\\Data\\UserData.json";
        private readonly string _filePathPlannedMeals = "C:\\Users\\Administrator\\Source\\Repos\\NutribuddyDP\\Data\\PlannedMeals.json";
        private static volatile DataStorageFacade? _instance;
        private static readonly object _instanceLock = new();

        public static DataStorageFacade Instance
        {
            get
            {
                return GetInstance();
            }
        }

        public static DataStorageFacade GetInstance()
        {
            if (_instance == null)
            {
                // REVIEW - lock - ciekawy sposób na zabezpieczenie przed problemami wielowątkowością
                // problem jest taki że w całej aplikacji nie ma ani jednego async/await ani Task.Run
                // więc nie ma potrzeby na takie zabezpieczenie bo nic nie dzieje się asynchronicznie
                lock (_instanceLock)
                {
                    _instance ??= new DataStorageFacade();
                }
            }
            return _instance;
        }

        //exports
        public void ExportDishes(List<Dish> dishes)
        {
            DataStorage.ExportData(dishes, _filePathDish);
        }

        public void ExportFoodItems(List<FoodItem> food)
        {
            DataStorage.ExportData(food, _filePathFoodItems);
        }

        public void ExportEatHistory(EatHistory eatHistories)
        {

            DataStorage.ExportData(eatHistories.DishEatHistory, _filePathDishHistory);
            DataStorage.ExportData(eatHistories.FoodItemEatHistory, _filePathFoodHistory);
        }

        public void ExportUser(User user)
        {
            List<User> users = [user];
            DataStorage.ExportData(users, _filePathUserData);

        }

        public void ExportPlannedMeals(List<IMealComponent> plannedMeals)
        {
            // REVIEW - powielenie settings tutaj i w ImportPlannedMeals()
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Formatting = Formatting.Indented
            };
            DataStorage.ExportData(plannedMeals, _filePathPlannedMeals, settings);
        }


        //imports
        public List<Dish>? ImportDishes()
        {
            return DataStorage.ImportData<Dish>(_filePathDish);
        }

        public List<FoodItem>? ImportFoodItems()
        {
            return DataStorage.ImportData<FoodItem>(_filePathFoodItems);
        }

        public EatHistory ImportEatHistory()
        {
            var DataFood = DataStorage.ImportData<(DateTime, FoodItem)>(_filePathFoodHistory);
            var DataDish = DataStorage.ImportData<(DateTime, Dish)>(_filePathDishHistory);
            // REVIEW - przydałby się null check na te dane
            EatHistory eatHistory = new(DataDish!, DataFood!);
            return eatHistory;
        }

        public User ImportUser()
        {
            // REVIEW - Lista typu user która na sztywno używa tylko jednego usera x[0]
            // można by to zastąpić metodą która importuje date bez używania listy
            List<User>? x = DataStorage.ImportData<User>(_filePathUserData);
            return (x.Count == 0) ? new User() : x[0];
        }

        public List<IMealComponent>? ImportPlannedMeals()
        {
            // REVIEW - powielenie settings tutaj i w ExportPlannedMeals()
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Formatting = Formatting.Indented
            };
            return DataStorage.ImportData<IMealComponent>(_filePathPlannedMeals, settings);
        }
    }
}
