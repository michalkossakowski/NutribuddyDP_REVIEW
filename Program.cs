using NutribuddyDP.Core.Controllers;
using NutribuddyDP.UI;
using NutribuddyDP.UI.Console;
using Spectre.Console;

namespace NutribuddyDP
{
    internal class Program
    {
        static void Main()
        {
            Console.Title = "NutribuddyDP";
            var userController = new UserController();
            var foodController = new FoodController();
            var dishController = new DishController();
            var eatHistoryController = new EatHistoryController();
            var mealController = new MealController();
            // REVIEW + SUPER!!!! widok manager bardzo wygodne do użytku
            var viewManager = new ViewManager();
            viewManager.RegisterView("IntroSequence", new IntroSequenceView(viewManager));
            viewManager.RegisterView("MainMenu", new MainMenuView(viewManager));
            viewManager.RegisterView("UserDetails", new UserDetailsView(
                eatHistoryController,
                userController,
                viewManager));
            viewManager.RegisterView("UserConfig", new UserConfigView(userController, viewManager));
            viewManager.RegisterView("Food", new FoodView(eatHistoryController, foodController, viewManager));
            viewManager.RegisterView("Dish", new DishView(eatHistoryController, foodController, dishController, viewManager));
            viewManager.RegisterView("Calendar", new CalendarView(eatHistoryController, viewManager));
            viewManager.RegisterView("ShoppingList", new ShoppingListView(new ShoppingListContext(mealController), viewManager));
            viewManager.RegisterView("MealPlanning", new MealPlanningView(mealController, dishController, foodController, viewManager));

            viewManager.ShowView("IntroSequence");
            AnsiConsole.Clear();
        }
    }
}
