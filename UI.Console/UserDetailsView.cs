﻿using NutribuddyDP.Core.Controllers;
using Spectre.Console;

namespace NutribuddyDP.UI.Console
{
    internal class UserDetailsView(EatHistoryController eatHistoryController, UserController userController, ViewManager viewManager) : IView
    {
        private readonly EatHistoryController _eatHistoryController = eatHistoryController;
        private readonly UserController _userController = userController;
        private readonly ViewManager _viewManager = viewManager;
        private readonly static Panel userFigletText = new Panel(
                    Align.Center(
                        new FigletText("User Profile").Color(Color.MediumPurple),
                        VerticalAlignment.Middle))
                .Expand().Padding(new Padding(0, 2));

        public void Show()
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(userFigletText);
            var user = _userController.GetUser();
            var table = new Table();
            table.Caption("User Data", style: null);
            table.AddColumn("").Centered();
            table.AddColumn("").Centered();
            table.HideHeaders();

            AnsiConsole.Live(table)
            .Start(ctx =>
            {
                table.AddRow("Gender", $"{user.Gender}");
                ctx.Refresh();
                Thread.Sleep(100);

                table.AddRow("Age", $"{user.Age}");
                ctx.Refresh();
                Thread.Sleep(100);

                table.AddRow("Height (cm)", $"{user.Height}");
                ctx.Refresh();
                Thread.Sleep(100);

                table.AddRow("Weight (kg)", $"{user.Weight}");
                ctx.Refresh();
                Thread.Sleep(100);

                table.AddRow("BMI", $"{Math.Truncate(user.BMI * 100) / 100}");
                ctx.Refresh();
                Thread.Sleep(100);

                table.AddRow("Your caloric needs", $"{Math.Truncate(user.CaloricNeeds * 100) / 100}");

                ctx.Refresh();
                Thread.Sleep(100);

                table.AddRow("Activity Level", $"{user.PhysicalActivityLevel}");
                ctx.Refresh();
                Thread.Sleep(100);

                table.AddRow("Goal", $"{user.Goal}");
                ctx.Refresh();
                Thread.Sleep(100);
            });



            //AnsiConsole.Write(table);

            // Personalized kcal counter
            DisplayMyKcal();

            DisplayCharts();

            var options = new List<string>
                {
                    "Edit User Info",
                    "Return to main menu"
                };

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[#A2D2FF]What do you want to do?[/]")
                    .AddChoices(options)
                    .HighlightStyle(new Style(foreground: Color.MediumPurple))
            );

            switch (choice)
            {
                case "Edit User Info":
                    //_navigateToUserConfig();
                    _viewManager.ShowView("UserConfig");
                    break;

                case "Return to main menu":
                    //_navigateToMainMenu();
                    _viewManager.ShowView("MainMenu");
                    // REVIEW - nigdy nie dochodzi do break podobnie jak w wielu innych miejscach
                    break;
            }
        }

        public void DisplayMyKcal()
        {
            // Personalized kcal counter

            var user = _userController.GetUser();
            var todayNutrients = _eatHistoryController.GetTotalNutrientsFromDay(DateTime.Now);
            var godDid = todayNutrients.TryGetValue("Energy (kcal)", out var calories);
            Align kcalGuardPanel;
            //<REVIEW>Można takie wartości jak 1.2 i 0.8 wyciągnąć do stałych
            if (godDid)
            {
                Markup centeredText;
                if (calories > user.CaloricNeeds * 1.2)
                {
                    centeredText = new Markup($"[bold #FFAFCC]You've consumed {calories:F2} kcal!\nTry not to eat too much...[/]")
                    .Centered();
                    kcalGuardPanel = Align.Center(new Panel(centeredText).BorderColor(new Color(255, 175, 204)).Padding(5, 1));
                }
                else if (calories < user.CaloricNeeds * 0.8)
                {
                    centeredText = new Markup($"[bold #FFAFCC]You've consumed {calories:F2} kcal!\nTry to eat more![/]")
                    .Centered();
                    kcalGuardPanel = Align.Center(new Panel(centeredText).BorderColor(new Color(255, 175, 204)).Padding(5, 1));
                }
                else if (calories > user.CaloricNeeds * 1.1)
                {
                    centeredText = new Markup($"[bold #FFD8BE]You've consumed {calories:F2} kcal!\nYou've had a bit too much..[/]")
                    .Centered();
                    kcalGuardPanel = Align.Center(new Panel(centeredText).BorderColor(new Color(255, 216, 190)).Padding(5, 1));
                }
                else if (calories < user.CaloricNeeds * 0.9)
                {
                    centeredText = new Markup($"[bold #FFD8BE]You've consumed {calories:F2} kcal!\nJust a little bit more![/]")
                    .Centered();
                    kcalGuardPanel = Align.Center(new Panel(centeredText).BorderColor(new Color(255, 216, 190)).Padding(5, 1));
                }
                else
                {
                    centeredText = new Markup($"[bold #FFD8BE]You've consumed {calories:F2} kcal!\nPerfect![/]")
                    .Centered();
                    kcalGuardPanel = Align.Center(new Panel(centeredText).BorderColor(new Color(162, 210, 255)).Padding(5, 1));
                }

            }
            else
            {
                kcalGuardPanel = Align.Center(
                    new Panel(
                        $"[bold #FFAFCC]You haven't eaten anything today.\nYou know how that makes us feel...[/]")
                    .BorderColor(new Color(255, 175, 204)).Padding(5, 1));
            }
            AnsiConsole.Write(kcalGuardPanel);
        }

        public void DisplayCharts()
        {
            var user = _userController.GetUser();
            var chartEnergy = new BarChart().Width(100);
            chartEnergy.MaxValue = user.CaloricNeeds;

            var chartCarbs = new BarChart().Width(100);
            chartCarbs.MaxValue = user.CaloricNeeds * 0.5;

            var chartFat = new BarChart().Width(100);
            chartFat.MaxValue = user.CaloricNeeds * 0.25;

            var chartProtein = new BarChart().Width(100);
            chartProtein.MaxValue = user.Weight;

            var chartSodium = new BarChart().Width(100);
            chartSodium.MaxValue = 1750;

            var chartFiber = new BarChart().Width(100);
            if (user.Gender == "Male")
            {
                chartFiber.MaxValue = 38;
            }
            else
            {
                chartFiber.MaxValue = 25;
            }

            var todayNutrients = _eatHistoryController.GetTotalNutrientsFromDay(DateTime.Now);

            if (todayNutrients.Count == 0)
            {
                return;
            }
            // <REVIEW> Nie które (lepiej wszystkie ) z stych wartości można by wyciągnąć do stałych, np. "Energy (kcal)"
            string[] nutrients = [
                "Energy (kcal)",
                "Carbohydrate, by difference (g)",
                "Total lipid (fat) (g)",
                "Protein (g)",
                "Sodium, Na (mg)",
                "Fiber, total dietary (g)"
            ];

            if (todayNutrients.TryGetValue(nutrients[0], out double energyLevel))
            {
                chartEnergy.AddItem(nutrients[0], Math.Truncate(energyLevel * 100) / 100, color: Color.Yellow);
            }
            if (todayNutrients.TryGetValue(nutrients[1], out double carbsLevel))
            {
                chartCarbs.AddItem(nutrients[1], Math.Truncate(carbsLevel * 100) / 100, Color.SandyBrown);
            }
            if (todayNutrients.TryGetValue(nutrients[2], out double fatLevel))
            {
                chartFat.AddItem(nutrients[2], Math.Truncate(fatLevel * 100) / 100, Color.NavajoWhite1);
            }
            if (todayNutrients.TryGetValue(nutrients[3], out double proteinLevel))
            {
                chartProtein.AddItem(nutrients[3], Math.Truncate(proteinLevel * 100) / 100, Color.White);
            }
            if (todayNutrients.TryGetValue(nutrients[4], out double sodiumLevel))
            {
                chartSodium.AddItem(nutrients[4], Math.Truncate(sodiumLevel * 100) / 100, Color.Silver);
            }
            if (todayNutrients.TryGetValue(nutrients[5], out double fiberLevel))
            {
                chartFiber.AddItem(nutrients[5], Math.Truncate(fiberLevel * 100) / 100, Color.DarkOliveGreen1);
            }

            AnsiConsole.Write(Align.Center(new Panel("[#A2D2FF]Nutrients for today[/]").BorderColor(new Color(162, 210, 255))));
            AnsiConsole.Write(Align.Center(new Padder(chartEnergy)));
            AnsiConsole.Write(Align.Center(new Padder(chartCarbs)));
            AnsiConsole.Write(Align.Center(new Padder(chartFat)));
            AnsiConsole.Write(Align.Center(new Padder(chartProtein)));
            AnsiConsole.Write(Align.Center(new Padder(chartSodium)));
            AnsiConsole.Write(Align.Center(new Padder(chartFiber)));
        }
    }
}
