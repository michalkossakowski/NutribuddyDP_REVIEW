﻿using NutribuddyDP.Core.Controllers;
using Spectre.Console;

namespace NutribuddyDP.UI.Console
{
    internal class CalendarView(EatHistoryController eatHistoryController, ViewManager viewManager) : IView
    {
        private readonly EatHistoryController _eatHistoryController = eatHistoryController;
        private readonly ViewManager _viewManager = viewManager;
        private readonly static Panel calendarFigletText = new Panel(
                Align.Center(
                    new FigletText("Calendar").Color(Color.MediumPurple),
                    VerticalAlignment.Middle))
            .Expand().Padding(new Padding(0, 2));

        public void Show()
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(calendarFigletText);

            var calendar = _eatHistoryController.Calendar;
            calendar.Centered();
            _eatHistoryController.BuildCalendar(calendar, DateTime.Now.Year, DateTime.Now.Month);
            while (true)
            {
                AnsiConsole.Write(calendar);

                // REVIEW - powinno być poza whilem bo nigdy się nie zmienia
                var menuOptions = new List<string>
                {
                    "Show calories from day this month",
                    "Change calendar page",
                    "Return to main menu"
                };

                var mainChoice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[#A2D2FF]What do you want to do?[/]")
                        .AddChoices(menuOptions)
                                    .HighlightStyle(new Style(foreground: Color.MediumPurple))
                );

                switch (mainChoice)
                {
                    case "Show calories from day this month":
                        var selectedDay = AnsiConsole.Ask<int>(
                            $"Enter the [pink1]day[/]: ");

                        // REVIEW - nie każdy miesiąc ma 31 dni
                        if (selectedDay < 1 || selectedDay > 31)
                        {
                            AnsiConsole.Write(new Markup($"[red]Invalid day[/]"));
                            // REVIEW - czemu znika po 1 sekundzie co jak użytkownik mrugnie ?
                            Thread.Sleep(1000);
                            AnsiConsole.Clear();
                            AnsiConsole.Write(calendarFigletText);
                            continue;
                        }

                        var eventList = _eatHistoryController.Calendar.CalendarEvents;
                        var eatHistory = _eatHistoryController.GetTotalNutrientsFromDay(
                            new DateTime(calendar.Year, calendar.Month, selectedDay));
                        var found = false;
                        foreach (var e in eventList)
                        {
                            if (e.Day == selectedDay && e.Month == calendar.Month && e.Year == calendar.Year)
                            {
                                found = true;
                                AnsiConsole.Write(new Markup($"[pink1]{calendar.Year} - {calendar.Month} - {selectedDay}:[/]\n"));
                                var tableNutrients = new Table().BorderColor(new Color(162, 210, 255));
                                tableNutrients.HideHeaders().Centered();
                                tableNutrients.AddColumn("").AddColumn("");
                                foreach (var nutrient in eatHistory)
                                {
                                    tableNutrients.AddRow($"[#BDE0FE]{nutrient.Key}[/]", $"[#BDE0FE]{nutrient.Value:F2}[/]");
                                }
                                tableNutrients.Caption("Total nutritional values");
                                AnsiConsole.Write(tableNutrients);
                                //AnsiConsole.Write(new Markup($"\tYou've eaten [pink1]{e.Description} kcal[/]!\n"));
                            }
                        }
                        if (!found)
                        {
                            AnsiConsole.Write(new Markup($"[pink1]{calendar.Year} - {calendar.Month} - {selectedDay}:[/]"));
                            AnsiConsole.Write(new Markup($"\t[pink1]No records found for the selected day[/]\n"));
                        }

                        AnsiConsole.Prompt(
                            new TextPrompt<string>("Press Enter to continue")
                            .AllowEmpty());

                        AnsiConsole.Clear();
                        AnsiConsole.Write(calendarFigletText);
                        break;

                    case "Change calendar page":
                        var selectedYear = AnsiConsole.Ask<int>(
                            $"Enter the [pink1]year[/]: ");
                        // REVIEW - $ nie jest potrzebny jak nie ma zmiennych łamanie zasady KISS - keep it simple stupid
                        var selectedMonth = AnsiConsole.Ask<int>(
                            $"Enter the [pink1]month[/]: ");

                        if (selectedYear < 1 || selectedMonth < 1 || selectedMonth > 12)
                        {
                            AnsiConsole.Write(new Markup("[red]Wrong date.[/]"));
                            Thread.Sleep(1000);
                        }
                        else
                        {
                            _eatHistoryController.BuildCalendar(calendar, selectedYear, selectedMonth);
                        }


                        AnsiConsole.Clear();
                        AnsiConsole.Write(calendarFigletText);
                        break;

                    case "Return to main menu":
                        //_navigateToMainMenu();
                        // REVIEW - odpala nowy widok z main menu które odpala kolejny widok i tak dalej ...
                        _viewManager.ShowView("MainMenu");
                        // REVIEW - do returna nigdy nie dojdzie, problem jest w każdym widoku
                        // powinno zakończyć działanie tego widoku i wtedy uruchamaiać następny
                        return;

                    default:
                        AnsiConsole.Clear();
                        AnsiConsole.Write(calendarFigletText);
                        break;
                }
            }
        }
    }
}
