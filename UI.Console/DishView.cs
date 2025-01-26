using NutribuddyDP.Core.Controllers;
using NutribuddyDP.Core.Models;
using Spectre.Console;

namespace NutribuddyDP.UI.Console
{
    internal class DishView(EatHistoryController eatHistoryController, FoodController foodController, DishController dishController, ViewManager viewManager) : IView
    {
        private readonly EatHistoryController _eatHistoryController = eatHistoryController;
        private readonly FoodController _foodController = foodController;
        private readonly DishController _dishController = dishController;
        private readonly ViewManager _viewManager = viewManager;
        private readonly static Panel foodFigletText = new Panel(
                Align.Center(
                    new FigletText("Dishes").Color(Color.MediumPurple),
                    VerticalAlignment.Middle))
            .Expand().Padding(new Padding(0, 2));

        public void Show()
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(foodFigletText);
            while (true)
            {
                // REVIEW - mainMenuOptions nigdy się nie zmiania nie musi być w while
                // Top-level menu
                var mainMenuOptions = new List<string>
                {
                    "Add a Dish",
                    "Show Dishes",
                    "Search for Dishes",
                    "Edit a Dish",
                    "Delete a Dish",
                    "Return to main menu"
                };

                var mainChoice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[bold #A2D2FF]What do you want to do?[/]")
                        .AddChoices(mainMenuOptions)
                                    .HighlightStyle(new Style(foreground: Color.MediumPurple))
                );

                switch (mainChoice)
                {
                    case "Add a Dish":
                        AddDishMenu();
                        break;

                    case "Show Dishes":
                        AnsiConsole.Clear();
                        AnsiConsole.Write(foodFigletText);
                        ShowDishes("");
                        break;

                    case "Search for Dishes":
                        var lookingFor = AnsiConsole.Ask<string>(
                            $"What do you want to look for? ");
                        AnsiConsole.Clear();
                        AnsiConsole.Write(foodFigletText);
                        ShowDishes(lookingFor);
                        break;

                    case "Edit a Dish":
                        EditDishMenu();
                        break;

                    case "Delete a Dish":
                        DeleteDishMenu();
                        break;
                                            
                    case "Return to main menu":
                        //_navigateToMainMenu();
                        _viewManager.ShowView("MainMenu");
                        // REVIEW - nigdy nie dochodzi do return podobnie jak w DishView i wielu innych miejscach
                        return;
                }
            }
        }

        private void AddDishMenu()
        {
            AnsiConsole.Markup("[bold #A2D2FF]=== Add a Dish ===[/]\n");

            var dishName = AnsiConsole.Ask<string>("Enter the name of the dish:");
            var newDish = new Dish { Name = dishName };

            while (true)
            {
                // REVIEW - addDishOptions nigdy się nie zmiania nie musi być w while
                // Sub-menu for adding ingredients and finalizing dish
                var addDishOptions = new List<string>
                {
                    "Add an ingredient",
                    "Search for an ingredient",
                    "Finish and save dish",
                    "Exit without saving"
                };

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[#A2D2FF]Is this all?[/]")
                        .AddChoices(addDishOptions)
                                    .HighlightStyle(new Style(foreground: Color.MediumPurple))
                        );

                switch (choice)
                {
                    case "Add an ingredient":
                        AddIngredientToDish(newDish, "");
                        break;

                    case "Search for an ingredient":
                        AddIngredientToDish(newDish, SearchForAnIngredient());
                        break;

                    case "Finish and save dish":
                        FinalizeDish(newDish);
                        return;

                    case "Exit without saving":
                        AnsiConsole.MarkupLine("[bold red]Dish creation canceled. Exiting...[/]");
                        Thread.Sleep(500);
                        AnsiConsole.Clear();
                        AnsiConsole.Write(foodFigletText);
                        return;
                }
            }
        }

        private void AddIngredientToDish(Dish newDish, string searchPhrase)
        {
            var foods = _foodController.GetAllFoods();
            // REVIEW - brak nullchecka dla nullowalnej wartości
            if (foods.Count == 0)
            {
                AnsiConsole.MarkupLine("[bold red]No food items available to add as ingredients.[/]");
                return;
            }

            List<string> foodDescriptions;
            if (searchPhrase == null)
            {
                foodDescriptions = foods.Select(f => f.Description).ToList();
            }
            else
            {
                foodDescriptions = foods
                    .Where(f => f.Description.Contains(searchPhrase, StringComparison.OrdinalIgnoreCase))
                    .Select(f => f.Description)
                    .ToList();
            }

            if (foodDescriptions.Count == 0)
            {
                return;
            }


            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[pink1]Select an ingredient to add:[/]")
                    .AddChoices(foodDescriptions)
                              .HighlightStyle(new Style(foreground: Color.MediumPurple))
            );


            var selectedFood = foods.First(f => f.Description == choice);

            var quantity = AnsiConsole.Ask<double>(
                $"Enter the quantity of [pink1]{selectedFood.Description}[/] in grams:");

            var foodWithQuantity = new FoodItem
            {
                Description = selectedFood.Description,
                Nutrients = new Dictionary<string, double>(selectedFood.Nutrients),
                QuantityInGrams = quantity,
                Category = selectedFood.Category
            };

            newDish.Ingredients.Add(foodWithQuantity);

            AnsiConsole.Markup($"[bold pink1]Added {quantity}g of {selectedFood.Description}.[/]\n");
        }

        private string SearchForAnIngredient()
        {
            var foods = _foodController.GetAllFoods();
            // REVIEW - brakuje nullchecka
            if (foods.Count == 0)
            {
                AnsiConsole.MarkupLine("[bold red]No food items available to add as ingredients.[/]");
                return "";
            }

            var lookingFor = AnsiConsole.Ask<string>(
                $"What do you want to look for? ");

            return lookingFor;
        }

        private void FinalizeDish(Dish newDish)
        {
            if (newDish.Ingredients.Count == 0)
            {
                AnsiConsole.MarkupLine("[bold red]No ingredients added. Cannot save an empty dish.[/]");
                return;
            }

            newDish.CalculateTotalNutrients();
            _dishController.AddDish(newDish);
            var confirmation = AnsiConsole.Prompt(
                new ConfirmationPrompt("Do you want to add this dish as your meal?"));

            if (confirmation)
            {
                var date = AnsiConsole.Prompt(
                        new TextPrompt<DateTime>("Enter the date and time when you ate this food (e.g., YYYY-MM-DD HH:mm):")
                            .Validate(date =>
                            {
                                return date <= DateTime.Now
                                    ? ValidationResult.Success()
                                    : ValidationResult.Error("Date and time cannot be in the future.");
                            })
                    );

                AnsiConsole.Progress()
                    .AutoClear(true)
                    .HideCompleted(true)
                .Start(ctx =>
                {
                    // Define tasks
                    var task1 = ctx.AddTask("[#9381FF]Adding dish...[/]");

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
                DateTime dateNow = DateTime.Now;
                _eatHistoryController.AddDishToHistory(date, newDish);
                AnsiConsole.MarkupLine($"[bold #A2D2FF]{newDish.Name} has been added as a meal![/]");
                Thread.Sleep(1000);
            }

            AnsiConsole.Clear();
            AnsiConsole.Write(foodFigletText);

            var dishAddedPanel = Align.Center(new Panel($"[#FFC8DD] Dish '{newDish.Name}' has been added![/]").BorderColor(new Spectre.Console.Color(255, 200, 221)).Padding(5, 1));
            var tableNutrients = new Table().BorderColor(new Color(162, 210, 255));
            tableNutrients.HideHeaders().Centered();
            tableNutrients.AddColumn("").AddColumn("");
            foreach (var nutrient in newDish.TotalNutrients)
            {
                tableNutrients.AddRow($"[#BDE0FE]{nutrient.Key}[/]", $"[#BDE0FE]{nutrient.Value:F2}[/]");
            }
            tableNutrients.Caption("Total nutritional values");
            AnsiConsole.Write(dishAddedPanel);
            AnsiConsole.Write(tableNutrients);

            AnsiConsole.Prompt(
                new TextPrompt<string>("Press Enter to continue")
                .AllowEmpty());

            AnsiConsole.Clear();
            AnsiConsole.Write(foodFigletText);
        }

        private void ShowDishes(string searchPhrase)
        {
            var allDishes = _dishController.GetAllDishes();
            // REVIEW - brak nullchecka dla nullowalnej wartości
            if (allDishes.Count == 0)
            {
                AnsiConsole.MarkupLine("[bold red]No dishes available.[/]");
                return;
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
                AnsiConsole.Write(foodFigletText);
                return;
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
                AnsiConsole.Write(foodFigletText);
                PrintDish(theDish);

                var confirmation = AnsiConsole.Prompt(
                  new ConfirmationPrompt("Do you want to add this dish as your meal?"));

                if (confirmation)
                {
                    var date = AnsiConsole.Prompt(
                        new TextPrompt<DateTime>("Enter the date and time when you ate this food (e.g., YYYY-MM-DD HH:mm):")
                            .Validate(date =>
                            {
                                return date <= DateTime.Now
                                    ? ValidationResult.Success()
                                    : ValidationResult.Error("Date and time cannot be in the future.");
                            })
                    );

                    _eatHistoryController.AddDishToHistory(date, theDish);
                    AnsiConsole.MarkupLine($"[bold #A2D2FF]{theDish.Name} has been added as a meal![/]");
                    Thread.Sleep(1000);
                }

                AnsiConsole.Clear();
                AnsiConsole.Write(foodFigletText);
            }
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

        private void EditDishMenu()
        {
            var dishes = _dishController.GetAllDishes()!;

            if (dishes.Count == 0)
            {
                AnsiConsole.MarkupLine("[bold red]No dishes available.[/]");
                return;
            }

            var dishName = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[pink1]Select a dish to edit:[/]")
                .AddChoices(dishes.Select(d => d.Name))
                .HighlightStyle(new Style(foreground: Color.MediumPurple))
            );
            //<REVIEW> - MVC wymaga wyraźnego rozdzielenia odpowiedzialności między warstwami.
            //Widok przekazuje funkcję anonimową do kontrolera, co może naruszać tę zasadę
            _dishController.EditDish(dishName, dish =>
            {
                AnsiConsole.MarkupLine($"Editing dish: [bold gold1]{dish.Name}[/]");
                var editOptions = new List<string> { "Change Name", "Edit Ingredients", "Back" };

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[gold1]What would you like to edit?[/]")
                        .AddChoices(editOptions)
                        .HighlightStyle(new Style(foreground: Color.MediumPurple))
                );

                switch (choice)
                {
                    case "Change Name":
                        var newName = AnsiConsole.Ask<string>("Enter new name:");
                        dish.Name = newName;
                        break;

                    case "Edit Ingredients":
                        EditIngredientsMenu(dish);
                        break;
                }
            });
        }

        private void EditIngredientsMenu(Dish dish)
        {
            while (true)
            {
                var ingredientMenu = new List<string>
                {
                    "Add an Ingredient",
                    "Edit an Ingredient",
                    "Remove an Ingredient",
                    "Back"
                };

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[pink1]What would you like to do with the ingredients?[/]")
                        .AddChoices(ingredientMenu)
                        .HighlightStyle(new Style(foreground: Color.MediumPurple))
                );

                switch (choice)
                {
                    case "Add an Ingredient":
                        AddIngredientToDish(dish, "");
                        break;

                    case "Edit an Ingredient":
                        var ingredientToEdit = AnsiConsole.Prompt(
                            new SelectionPrompt<string>()
                                .Title("[pink1]Select an ingredient to edit:[/]")
                                .AddChoices(dish.Ingredients.Select(i => i.Description))
                                .HighlightStyle(new Style(foreground: Color.MediumPurple))
                        );

                        var newQuantity = AnsiConsole.Ask<double>("Enter new quantity in grams:");
                        var ingredient = dish.Ingredients.First(i => i.Description == ingredientToEdit);
                        ingredient.QuantityInGrams = newQuantity;
                        break;

                    case "Remove an Ingredient":
                        var ingredientToRemove = AnsiConsole.Prompt(
                            new SelectionPrompt<string>()
                                .Title("[pink1]Select an ingredient to remove:[/]")
                                .AddChoices(dish.Ingredients.Select(i => i.Description))
                                .HighlightStyle(new Style(foreground: Color.MediumPurple))
                        );

                        dish.Ingredients.RemoveAll(i => i.Description == ingredientToRemove);
                        break;

                    case "Back":
                        return;
                }
            }
        }

        private void DeleteDishMenu()
        {
            var dishes = _dishController.GetAllDishes();
            // REVIEW - brak nullchecka dla nullowalnej wartości
            if (dishes.Count == 0)
            {
                AnsiConsole.MarkupLine("[bold red]No dishes available.[/]");
                return;
            }

            var dishName = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[pink1]Select a dish to delete:[/]")
                    .AddChoices(dishes.Select(d => d.Name))
                    .HighlightStyle(new Style(foreground: Color.MediumPurple))
            );

            _dishController.DeleteDish(dishName);
            AnsiConsole.MarkupLine($"[bold red]Dish '{dishName}' has been deleted.[/]");
        }
    }
}
