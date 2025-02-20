﻿using NutribuddyDP.Core.Controllers;
using NutribuddyDP.Core.Models;
using Spectre.Console;

namespace NutribuddyDP.UI.Console
{
    internal class FoodView(EatHistoryController eatHistoryController, FoodController foodController, ViewManager viewManager) : IView
    {
        private readonly EatHistoryController _eatHistoryController = eatHistoryController;
        private readonly FoodController _foodController = foodController;
        private readonly ViewManager _viewManager = viewManager;
        private readonly static Panel foodFigletText = new Panel(
                    Align.Center(
                        new FigletText("Food").Color(Color.MediumPurple),
                        VerticalAlignment.Middle))
                .Expand().Padding(new Padding(0, 2));

        public void Show()
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(foodFigletText);
            while (true)
            {

                // <REVIEW> - Można by było przenieść nad pętlą while, zwłaszcza że jest to stała wartość
                var options = new List<string>
                {
                    "View all food items",
                    "Search for a food item",
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
                    case "View all food items":
                        AnsiConsole.Clear();
                        DisplayFoodList("");
                        break;

                    case "Search for a food item":
                        var lookingFor = AnsiConsole.Ask<string>(
                            $"What do you want to look for? ");
                        AnsiConsole.Clear();
                        DisplayFoodList(lookingFor);
                        break;

                    case "Return to main menu":
                        //_navigateToMainMenu();
                        // REVIEW - nigdy nie dochodzi do return podobnie jak w wielu innych miejscach
                        _viewManager.ShowView("MainMenu");
                        return;
                }
            }
        }

        private void DisplayFoodList(string searchPhrase)
        {
            var foodItems = _foodController.GetAllFoods();

            AnsiConsole.Write(foodFigletText);

            if (foodItems.Count == 0)
            {
                AnsiConsole.MarkupLine("[bold red]No food items available.[/]");
                return;
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
                AnsiConsole.MarkupLine("[bold red]No food items found.[/]\n");
                Thread.Sleep(1000);
                AnsiConsole.Clear();
                AnsiConsole.Write(foodFigletText);
                return;
            }

            var selectedFood = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[#A2D2FF]Select a food item:[/]")
                    .AddChoices(descriptions)
                    .HighlightStyle(new Style(foreground: Color.MediumPurple))
            );

            var foodItem = foodItems.Find(f => f.Description == selectedFood);
            if (foodItem != null)
            {
                DisplayNutrientTable(foodItem);
                var confirmation = AnsiConsole.Prompt(
                    new ConfirmationPrompt("Do you want to add this food item as your meal?"));

                if (confirmation)
                {
                    var quantity = AnsiConsole.Ask<double>(
                        $"Enter the quantity of [pink1]{foodItem.Description}[/] in grams:");

                    var foodWithQuantity = new FoodItem
                    {
                        Description = foodItem.Description,
                        Nutrients = new Dictionary<string, double>(foodItem.Nutrients),
                        QuantityInGrams = quantity,
                        Category = foodItem.Category
                    };

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
                        var task1 = ctx.AddTask("[#9381FF]Adding food item...[/]");

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

                    //_eatHistoryController.EatHistory.FoodItemEatHistory.Add((DateTime.Now, foodWithQuantity));
                    DateTime dateNow = DateTime.Now;
                    _eatHistoryController.AddFoodItemToHistory(date, foodWithQuantity);
                    AnsiConsole.MarkupLine($"[bold #A2D2FF]{foodWithQuantity.Description} has been added as a meal![/]");
                    Thread.Sleep(1000);
                }
                AnsiConsole.Clear();
                AnsiConsole.Write(foodFigletText);
            }
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
            AnsiConsole.Write(foodFigletText);
            AnsiConsole.MarkupLine($"[bold #A2D2FF]Nutritional values for: {foodItem.Description}[/]");
            AnsiConsole.Write(table);
        }
    }
}