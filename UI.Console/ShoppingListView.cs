using NutribuddyDP.Core.Models;
using NutribuddyDP.Core.Controllers;
using System;
using System.Collections.Generic;
using NutribuddyDP.UI;
using Spectre.Console;

namespace NutribuddyDP.UI.Console
{
    internal class ShoppingListView(ShoppingListContext shoppingListContext, ViewManager viewManager) : IView
    {
        private readonly ShoppingListContext _shoppingListContext = shoppingListContext;
        private readonly ViewManager _viewManager = viewManager;
        private readonly static Panel shoppingFigletText = new Panel(
                    Align.Center(
                        new FigletText("Shopping List").Color(Color.MediumPurple),
                        VerticalAlignment.Middle))
                .Expand().Padding(new Padding(0, 2));

        public void Show()
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(shoppingFigletText);
            
            while (true)
            {
                var options = new List<string>
                {
                   /* "Return to dish menu",
                    "Return to main menu",*/

                    "Generate list alphabetically",
                    "Generate list by categories",
                    "Back to main menu"
                };

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[#A2D2FF]Choose an option:[/]")
                        .AddChoices(options)
                        .HighlightStyle(new Style(foreground: Color.MediumPurple))
                );
                
                //REVIEW - jeśli mamy pustą listę to program wychodzi z wihle'a bez określenia czy zostajemy w ekranie czy idziemy do MainMenu przez returna. Można by było dać breaka, aby program został w while'u i pokazywał komunikat wywołany z DisplayAlphabetical bądź DisplayCategorical i został w widoku
                switch (choice)
                {
                    case "Generate list alphabetically":
                        DisplayAlphabetical();
                        return;

                    case "Generate list by categories":
                        DisplayCategorical();
                        break;

                    case "Back to main menu":
                        _viewManager.ShowView("MainMenu");
                        // REVIEW - nigdy nie dochodzi do return podobnie jak w wielu innych miejscach
                        return;

                }
            }
        }
        public void DisplayAlphabetical()
        {
            _shoppingListContext.SetStrategy(new AlphabeticalStrategy());
            // Generowanie listy zakupowej przy użyciu kontekstu
            var shoppingList = _shoppingListContext.GenerateList();

            if (shoppingList == null || shoppingList.Count == 0)
            {
                AnsiConsole.MarkupLine("[bold red]The shopping list is empty.[/]");
                return;
            }

            Panel panel;
            List<Text> list = [];

            // Wyświetlanie elementów listy zakupowej
            foreach (var item in shoppingList)
            {
                var food = item.Value as FoodItem;
                list.Add(new Text($"- {food!.Description}: {food.QuantityInGrams:F2} grams"));
            }
            panel = new Panel(new Rows(list))
            {
                Header = new PanelHeader("Alphabetical shopping list"),
                Padding = new Padding(2)
            };

            AnsiConsole.Write(panel);


            while (true)
            {
                var options = new List<string>
                {
                    "Back",
                };

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[#A2D2FF]Choose an option:[/]")
                        .AddChoices(options)
                        .HighlightStyle(new Style(foreground: Color.MediumPurple))
                );

                switch (choice)
                {
                    case "Back":
                        _viewManager.ShowView("MainMenu");
                        // REVIEW - nigdy nie dochodzi do return podobnie jak w wielu innych miejscach
                        return;
                }
            }
        }


        public void DisplayCategorical()
        {
            _shoppingListContext.SetStrategy(new CategoryStrategy());

            // Generowanie listy zakupowej przy użyciu kontekstu
            var shoppingList = _shoppingListContext.GenerateList();

            if (shoppingList == null || shoppingList.Count == 0)
            {
                AnsiConsole.MarkupLine("[bold red]The shopping list is empty.[/]");
                return;
            }

            // Wyświetlanie produktów w każdej kategorii
            foreach (var category in shoppingList)
            {
                List<Text> list = [];
                Panel panel;
                // Rzutowanie wartości kategorii na List<FoodItem>
                if (category.Value is List<FoodItem> foodItems && foodItems.Count > 0)
                {
                    foreach (var food in foodItems)
                    {
                        list.Add(new Text($"- {food.Description}: {food.QuantityInGrams:F2} grams"));
                    }
                    panel = new Panel(new Rows(list));
                }
                else
                {
                    panel = new Panel("[bold red]\"No items found in this category.[/]");
                }
                panel.Header = new PanelHeader(category.Key);
                panel.Padding = new Padding(2);
                AnsiConsole.Write(panel);
                AnsiConsole.WriteLine(); // Dla czytelności oddzielamy kategorie
            }
            //<REVIEW> Kod się powtarza, można by go wydzielić do osobnej metody
            while (true)
            {
                var options = new List<string>
                {
                    "Back",
                };

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[#A2D2FF]Choose an option:[/]")
                        .AddChoices(options)
                        .HighlightStyle(new Style(foreground: Color.MediumPurple))
                );

                switch (choice)
                {
                    case "Back":
                        _viewManager.ShowView("MainMenu");
                        // REVIEW - nigdy nie dochodzi do return podobnie jak w wielu innych miejscach
                        return;
                }
            }
        }



    }
}
