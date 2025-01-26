using NutribuddyDP.Core.Controllers;
using NutribuddyDP.Core.Interfaces;
using NutribuddyDP.Core.Models;
using Spectre.Console;
//<REVIEW> Często się powtarza poniższy kod
//    AnsiConsole.Clear();
//    AnsiConsole.Write(planningFigletText);
//    można byłoby go wydzielić do osobnej metody

//<REVIEW> Nie które metody wydają się długie i mogłyby być podzielone na mniejsze metody w celu zwiększenia czytelności
namespace NutribuddyDP.UI.Console
{
    internal class MealPlanningView(MealController mealController, DishController dishController, FoodController foodController, ViewManager viewManager) : IView
    {
        private readonly ViewManager _viewManager = viewManager;
        private readonly MealController _mealController = mealController;
        private readonly DishController _dishController = dishController;
        private readonly FoodController _foodController = foodController;
        private readonly static Panel planningFigletText = new Panel(
                    Align.Center(
                        new FigletText("Planning").Color(Color.MediumPurple),
                        VerticalAlignment.Middle))
                .Expand().Padding(new Padding(0, 2));

        public void Show()
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(planningFigletText);
            while (true)
            {
                var options = new List<string>
                {
                    "View planned meals",
                    "Plan a single meal",
                    "Plan meals for a day",
                    "Plan meals for a week",
                    "Remove a planned meal",
                    "Return to main menu"
                };

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[#A2D2FF]Choose an option:[/]")
                        .AddChoices(options)
                        .HighlightStyle(new Style(foreground: Color.MediumPurple))
                );

                switch (choice)
                {
                    case "View planned meals":
                        AnsiConsole.Clear();
                        DisplayPlannedMeals();
                        break;

                    case "Plan a single meal":
                        AnsiConsole.Clear();
                        var m = PlanMeal();
                        if (m != null) _mealController.AddMeal(m);
                        break;

                    case "Plan meals for a day":
                        AnsiConsole.Clear();
                        var d = PlanDay();
                        if (d != null) _mealController.AddMeal(d);
                        break;

                    case "Plan meals for a week":
                        AnsiConsole.Clear();
                        var w = PlanWeek();
                        if (w != null) _mealController.AddMeal(w);
                        break;

                    case "Remove a planned meal":
                        AnsiConsole.Clear();
                        var p = RemovePlan();
                        if (p != null)
                            _mealController.RemovePlan(p);
                        AnsiConsole.Clear();
                        AnsiConsole.Write(planningFigletText);
                        break;
                    case "Return to main menu":
                        // REVIEW - nigdy nie dochodzi do return podobnie jak w wielu innych miejscach
                        _viewManager.ShowView("MainMenu");
                        return;
                }
            }
        }

        private void DisplayPlannedMeals()
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(planningFigletText);
            var plannedMeals = _mealController.GetPlannedMeals();
            if (plannedMeals!.Count == 0)
            {
                AnsiConsole.MarkupLine("[bold red]No meals planned.[/]");
                return;
            }

            foreach (var item in plannedMeals)
            {
                if (item is Meal meal)
                {
                    var planTitle = new Table().BorderColor(new Color(255, 200, 221));
                    planTitle.HideHeaders().Centered().AddColumn("").AddRow("One-off meal");
                    AnsiConsole.Write(planTitle);
                    DisplayMeal(meal);
                    AnsiConsole.WriteLine(); AnsiConsole.WriteLine();
                }
                else if (item is Day day)
                {
                    var planTitle = new Table().BorderColor(new Color(255, 200, 221));
                    planTitle.HideHeaders().Centered().AddColumn("").AddRow("Full-day plan");
                    AnsiConsole.Write(planTitle);
                    DisplayDay(day);
                    AnsiConsole.WriteLine(); AnsiConsole.WriteLine();
                }
                else if (item is Week week)
                {
                    var planTitle = new Table().BorderColor(new Color(255, 200, 221));
                    planTitle.HideHeaders().Centered().AddColumn("").AddRow("Week-long plan");
                    AnsiConsole.Write(planTitle);
                    DisplayWeek(week);
                    AnsiConsole.WriteLine(); AnsiConsole.WriteLine();
                }
            }
        }

        private Dish? ShowDishes(string searchPhrase)
        {
            var allDishes = _dishController.GetAllDishes()!;

            if (allDishes.Count == 0)
            {
                AnsiConsole.MarkupLine("[bold red]No dishes available.[/]");
                return null;
            }

            List<Dish> dishes;
            if (searchPhrase == null)
            {
                dishes = allDishes;
            }
            else
            {
                dishes = allDishes
                    .Where(f => f.Name.Contains(searchPhrase, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            if (dishes.Count == 0)
            {
                AnsiConsole.MarkupLine("[bold red]No dishes found.[/]\n");
                Thread.Sleep(1000);
                AnsiConsole.Clear();
                AnsiConsole.Write(planningFigletText);
                return null;
            }

            foreach (var dish in dishes)
            {
                PrintDish(dish);
                AnsiConsole.Write("\n\n\n");
            }
            var dishNames = dishes.Select(f => f.Name)
                .ToList();

            var selectedDish = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[#A2D2FF]Select a dish:[/]")
                .AddChoices(dishNames)
                .HighlightStyle(new Style(foreground: Color.MediumPurple)));

            var theDish = dishes.Find(f => f.Name == selectedDish);
            if (theDish != null)
            {
                AnsiConsole.Clear();
                AnsiConsole.Write(planningFigletText);
                PrintDish(theDish);

                var confirmation = AnsiConsole.Prompt(
                  new ConfirmationPrompt("Do you want to add this dish to your meal?"));

                if (confirmation)
                {
                    AnsiConsole.MarkupLine($"[bold #A2D2FF]{theDish.Name} has been added to your meal![/]");
                    Thread.Sleep(1000);
                }

                AnsiConsole.Clear();
                AnsiConsole.Write(planningFigletText);

                return confirmation ? theDish : null;
            }
            return null;
        }

        private FoodItem? DisplayFoodList(string searchPhrase)
        {
            var foodItems = _foodController.GetAllFoods();

            AnsiConsole.Write(planningFigletText);

            if (foodItems!.Count == 0)
            {
                AnsiConsole.MarkupLine("[bold red]No products available.[/]");
                return null;
            }


            List<string> descriptions;
            if (searchPhrase == null)
            {
                descriptions = foodItems.ConvertAll(f => f.Description);
            }
            else
            {
                descriptions = foodItems
                    .Where(f => f.Description.Contains(searchPhrase, StringComparison.OrdinalIgnoreCase))
                    .Select(f => f.Description)
                    .ToList();
            }

            if (descriptions.Count == 0)
            {
                AnsiConsole.MarkupLine("[bold red]No products found.[/]\n");
                Thread.Sleep(1000);
                AnsiConsole.Clear();
                AnsiConsole.Write(planningFigletText);
                return null;
            }

            var selectedFood = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[#A2D2FF]Select a product:[/]")
                    .AddChoices(descriptions)
                    .HighlightStyle(new Style(foreground: Color.MediumPurple))
            );

            var foodItem = foodItems.Find(f => f.Description == selectedFood);
            if (foodItem != null)
            {
                DisplayNutrientTable(foodItem);
                var confirmation = AnsiConsole.Prompt(
                    new ConfirmationPrompt("Do you want to add this food item to your meal?"));

                if (confirmation)
                {
                    double quantity;
                    do
                    {
                        quantity = AnsiConsole.Ask<double>(
                            $"Enter the quantity of [pink1]{foodItem.Description}[/] in grams:");
                    } while (quantity <= 0);

                    var foodWithQuantity = new FoodItem
                    {
                        Description = foodItem.Description,
                        Nutrients = new Dictionary<string, double>(foodItem.Nutrients),
                        QuantityInGrams = quantity,
                        Category = foodItem.Category
                    };

                    AnsiConsole.Progress()
                        .AutoClear(true)
                        .HideCompleted(true)
                    .Start(ctx =>
                    {
                        // Define tasks
                        var task1 = ctx.AddTask("[#9381FF]Adding product...[/]");

                        while (!ctx.IsFinished)
                        {
                            Thread.Sleep(50);
                            task1.Increment(5);
                        }

                        if (ctx.IsFinished)
                        {
                            Thread.Sleep(500);
                        }
                    });



                    AnsiConsole.MarkupLine($"[bold #A2D2FF]{foodWithQuantity.Description} has been added as a meal![/]");
                    Thread.Sleep(1000);

                    AnsiConsole.Clear();
                    AnsiConsole.Write(planningFigletText);

                    return foodWithQuantity;
                }
            }
            AnsiConsole.Clear();
            AnsiConsole.Write(planningFigletText);
            return null;
        }

        private static void DisplayNutrientTable(FoodItem foodItem)
        {
            var table = new Table()
                .AddColumn("[#BDE0FE]Nutrient[/]")
                .AddColumn("[#BDE0FE]Amount[/]")
                .AddColumn("[#BDE0FE]Unit[/]")
                .RoundedBorder();

            foreach (var nutrient in foodItem.Nutrients)
            {
                var unit = nutrient.Key.Contains("Energy") ? "kcal" : nutrient.Key.Contains("(g)") ? "g" : nutrient.Key.Contains("(mg)") ? "mg" : "";
                table.AddRow(nutrient.Key, nutrient.Value.ToString("F2"), unit);
            }

            AnsiConsole.Clear();
            AnsiConsole.Write(planningFigletText);
            AnsiConsole.MarkupLine($"[bold #A2D2FF]Nutritional values for: {foodItem.Description}[/]");
            AnsiConsole.Write(table);
        }

        private static void PrintDish(Dish dish)
        {
            var dishNamePanel = Align.Center(new Panel($"[#FFC8DD]{dish.Name}[/]").BorderColor(new Spectre.Console.Color(255, 200, 221)).Padding(5, 1));
            var tableIngredients = new Table().BorderColor(new Color(162, 210, 255));
            tableIngredients.HideHeaders().Centered();
            tableIngredients.AddColumn("").AddColumn("");
            foreach (var ingredient in dish.Ingredients)
            {
                tableIngredients.AddRow($"[#BDE0FE]{ingredient.Description}[/]", $"[#BDE0FE]{ingredient.QuantityInGrams:F2}g[/]");
            }
            tableIngredients.Caption("Ingredients");

            var tableNutrients = new Table().BorderColor(new Color(162, 210, 255));
            tableNutrients.HideHeaders().Centered();
            tableNutrients.AddColumn("").AddColumn("");
            foreach (var nutrient in dish.TotalNutrients)
            {
                tableNutrients.AddRow($"[#BDE0FE]{nutrient.Key}[/]", $"[#BDE0FE]{nutrient.Value:F2}[/]");
            }
            tableNutrients.Caption("Total nutritional values");
            AnsiConsole.Write(dishNamePanel);
            AnsiConsole.Write(tableIngredients);
            AnsiConsole.Write(tableNutrients);
        }

        private static void DisplayMeal(Meal meal)
        {
            var mealTitle = new Table().BorderColor(new Color(162, 210, 255));
            mealTitle.HideHeaders().Centered().AddColumn("").AddRow(meal.Name);
            AnsiConsole.Write(mealTitle);
            AnsiConsole.WriteLine();

            foreach (var dish in meal.Dishes)
            {
                var dishTitle = new Table().BorderColor(new Color(162, 210, 255));
                dishTitle.HideHeaders().Centered().AddColumn("").AddRow(dish.Name);


                var dishIngredients = new Table().BorderColor(new Color(162, 210, 255));
                dishIngredients.HideHeaders().Centered().AddColumn("").AddColumn("");
                foreach (var ingredient in dish.Ingredients)
                {
                    dishIngredients.AddRow($"[#BDE0FE]{ingredient.Description}[/]", $"[#BDE0FE]{ingredient.QuantityInGrams:F2}g[/]");
                }
                dishIngredients.Caption("Ingredients");

                var dishNutrients = new Table().BorderColor(new Color(162, 210, 255));
                dishNutrients.HideHeaders().Centered().AddColumn("").AddColumn("");
                foreach (var nutrient in dish.TotalNutrients)
                {
                    dishNutrients.AddRow($"[#BDE0FE]{nutrient.Key}[/]", $"[#BDE0FE]{nutrient.Value:F2}[/]");
                }
                dishNutrients.Caption("Total nutrients");

                AnsiConsole.Write(dishTitle);
                AnsiConsole.Write(dishIngredients);
                AnsiConsole.Write(dishNutrients);
            }

            foreach (var product in meal.FoodItems)
            {
                var productTable = new Table()
                    .BorderColor(new Color(162, 210, 255))
                    .HideHeaders()
                    .Centered()
                    .AddColumn("")
                    .AddColumn("")
                    .Caption("Product")
                    .AddRow($"[#BDE0FE]{product.Description}[/]", $"[#BDE0FE]{product.QuantityInGrams:F2}g[/]");

                var nutrientTable = new Table()
                    .AddColumn("[#BDE0FE]Nutrient[/]")
                    .AddColumn("[#BDE0FE]Amount[/]")
                    .AddColumn("[#BDE0FE]Unit[/]")
                    .RoundedBorder()
                    .BorderColor(new Color(162, 210, 255))
                    .Centered()
                    .Caption("Product nutrients");

                foreach (var nutrient in product.Nutrients)
                {
                    var unit = nutrient.Key.Contains("Energy") ? "kcal" : nutrient.Key.Contains("(g)") ? "g" : nutrient.Key.Contains("(mg)") ? "mg" : "";
                    nutrientTable.AddRow($"[#BDE0FE]{nutrient.Key}[/]", $"[#BDE0FE]{nutrient.Value:F2}[/]", $"[#BDE0FE]{unit}[/]");
                }

                AnsiConsole.Write(productTable);
                AnsiConsole.Write(nutrientTable);
            }
        }

        private static void DisplayDay(Day day)
        {
            var dayTitle = new Table().BorderColor(new Color(162, 210, 255));
            dayTitle.HideHeaders().Centered().AddColumn("").AddRow(day.Name);
            AnsiConsole.Write(dayTitle);

            foreach (Meal meal in day.Meals.Cast<Meal>())
            {
                DisplayMeal(meal);
            }
        }

        private static void DisplayWeek(Week week)
        {
            var weekTitle = new Table().BorderColor(new Color(162, 210, 255));
            weekTitle.HideHeaders().Centered().AddColumn("").AddRow(week.Name);
            AnsiConsole.Write(weekTitle);

            foreach (Day day in week.Days.Cast<Day>())
            {
                DisplayDay(day);
            }
        }

        private Meal? PlanMeal()
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(planningFigletText);

            var mealName = AnsiConsole.Ask<string>("Enter the name of the meal:");
            var newMeal = new Meal(mealName);

            while (true)
            {
                var addMealOptions = new List<string>
                {
                    "Add a dish to meal",
                    "Add a product to meal",
                    "Finish and save meal",
                    "Cancel"
                };

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[#A2D2FF]Is this all?[/]")
                        .AddChoices(addMealOptions)
                                    .HighlightStyle(new Style(foreground: Color.MediumPurple))
                        );

                switch (choice)
                {
                    case "Add a dish to meal":
                        var lookingForDish = AnsiConsole.Ask<string>(
                            $"What do you want to look for? ");
                        AnsiConsole.Clear();
                        AnsiConsole.Write(planningFigletText);
                        var selectedDish = ShowDishes(lookingForDish);
                        if (selectedDish != null)
                        {
                            newMeal.AddDish(selectedDish);
                        }
                        break;

                    case "Add a product to meal":
                        var lookingForProduct = AnsiConsole.Ask<string>(
                            $"What do you want to look for? ");
                        AnsiConsole.Clear();
                        var selectedProduct = DisplayFoodList(lookingForProduct);
                        if (selectedProduct != null)
                        {
                            newMeal.AddProduct(selectedProduct);
                        }
                        break;

                    case "Finish and save meal":
                        AnsiConsole.Progress()
                            .AutoClear(true)
                            .HideCompleted(true)
                        .Start(ctx =>
                        {
                            // Define tasks
                            var task1 = ctx.AddTask("[#9381FF]Planning meal...[/]");

                            while (!ctx.IsFinished)
                            {
                                Thread.Sleep(50);
                                task1.Increment(5);
                            }

                            if (ctx.IsFinished)
                            {
                                Thread.Sleep(500);
                            }
                        });
                        return newMeal;

                    case "Cancel":
                        AnsiConsole.MarkupLine("[bold red]Meal planning canceled. Exiting...[/]");
                        Thread.Sleep(500);
                        AnsiConsole.Clear();
                        AnsiConsole.Write(planningFigletText);
                        return null;
                }
            }
        }

        private Day? PlanDay()
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(planningFigletText);

            var dayName = AnsiConsole.Ask<string>("Enter the name of the day:");
            var newDay = new Day(dayName);

            while (true)
            {
                var addMealOptions = new List<string>
                {
                    "Add next meal",
                    "Finish and save day",
                    "Cancel"
                };

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[#A2D2FF]Is this all?[/]")
                        .AddChoices(addMealOptions)
                                    .HighlightStyle(new Style(foreground: Color.MediumPurple))
                        );

                switch (choice)
                {
                    case "Add next meal":
                        var m = PlanMeal();
                        if (m != null) newDay.Add(m);
                        break;

                    case "Finish and save day":
                        AnsiConsole.Progress()
                            .AutoClear(true)
                            .HideCompleted(true)
                        .Start(ctx =>
                        {
                            // Define tasks
                            var task1 = ctx.AddTask("[#9381FF]Planning day...[/]");

                            while (!ctx.IsFinished)
                            {
                                Thread.Sleep(50);
                                task1.Increment(5);
                            }

                            if (ctx.IsFinished)
                            {
                                Thread.Sleep(500);
                            }
                        });
                        return newDay;

                    case "Cancel":
                        AnsiConsole.MarkupLine("[bold red]Day planning canceled. Exiting...[/]");
                        Thread.Sleep(500);
                        AnsiConsole.Clear();
                        AnsiConsole.Write(planningFigletText);
                        return null;
                }
            }
        }

        private Week? PlanWeek()
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(planningFigletText);

            var weekName = AnsiConsole.Ask<string>("Enter the name of the week:");
            var newWeek = new Week(weekName);

            for (int i = 0; i < 7; ++i)
            {
                var addMealOptions = new List<string>
                {
                    "Continue",
                    "Cancel"
                };

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title($"[#A2D2FF]You've filled out {i}/7 days.[/]")
                        .AddChoices(addMealOptions)
                                    .HighlightStyle(new Style(foreground: Color.MediumPurple))
                        );

                switch (choice)
                {
                    case "Continue":
                        var d = PlanDay();
                        if (d is null)
                        {
                            --i;
                        }
                        else
                        {
                            newWeek.Add(d);
                        }
                        break;

                    case "Cancel":
                        AnsiConsole.MarkupLine("[bold red]Week planning canceled. Exiting...[/]");
                        Thread.Sleep(500);
                        AnsiConsole.Clear();
                        AnsiConsole.Write(planningFigletText);
                        return null;
                }
            }
            AnsiConsole.MarkupLine("[#9381FF]Planning week...[/]");
            Thread.Sleep(500);
            AnsiConsole.Clear();
            AnsiConsole.Write(planningFigletText);
            return newWeek;
        }

        private IMealComponent? RemovePlan()
        {
            DisplayPlannedMeals();
            var plannedMeals = _mealController.GetPlannedMeals();
            if (plannedMeals!.Count == 0 )
            {
                return null;
            }

            List<string> removeOptions = [];

            foreach (var plan in plannedMeals)
            {
                removeOptions.Add(plan.Name);
            }

            var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[#A2D2FF]Is this all?[/]")
                        .AddChoices(removeOptions)
                        .HighlightStyle(new Style(foreground: Color.MediumPurple))
                        );

            var confirmation = AnsiConsole.Prompt(
                  new ConfirmationPrompt("Do you want to remove this plan?"));

            return confirmation ? plannedMeals.Find(plan => plan.Name == choice) : null;
        }
    }
}
