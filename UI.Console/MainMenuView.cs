using Spectre.Console;

namespace NutribuddyDP.UI.Console
{
    internal class MainMenuView(ViewManager viewManager) : IView
    {
        private readonly ViewManager _viewManager = viewManager;
        private readonly Padding menuPad = new(5, 1);

        public void Show()
        {
            string[] menuOptions = [
                "View my profile",
                "Browse food",
                "Browse dishes",
                "Plan meals",
                "Generate Shopping List",
                "View calendar",
                "Exit"
            ];
            AnsiConsole.Clear();

            AnsiConsole.Write(new Panel(
                    Align.Center(
                        new FigletText("NutribuddyDP").Color(Color.MediumPurple),
                        VerticalAlignment.Middle))
                .Expand().Padding(new Padding(0, 2)));

            AnsiConsole.Write(new Grid()
                    .AddColumn()
                    .AddColumn()
                    // REVIEW - można to skrócić używając fora, gdzie umiesczone by było AddRow a w miejsce wartości
                    // menuOptions możnaby było podstawić zmienne z fora np. i oraz i+=1
                    .AddRow(
                    [
                        Align.Center(new Panel(menuOptions[0]).Padding(menuPad)),
                        Align.Center(new Panel(menuOptions[1]).Padding(menuPad))
                    ])
                    .AddRow(
                    [
                        Align.Center(new Panel(menuOptions[2]).Padding(menuPad)),
                        Align.Center(new Panel(menuOptions[3]).Padding(menuPad))
                    ])
                    .AddRow(
                    [
                        Align.Center(new Panel(menuOptions[4]).Padding(menuPad)),
                        Align.Center(new Panel(menuOptions[5]).Padding(menuPad))
                    ]).Expand());

            AnsiConsole.Write(new Rule());

            var selected = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                .Title("[#A2D2FF]What do you want to do?[/]")
                .AddChoices(menuOptions)
                .HighlightStyle(new Style(foreground: Color.MediumPurple)));

            //REVIEW - Lepiej jeśli switch case będzie na zmiennych statycznych a nie tekstowych
            switch (selected)
            {
                case "View my profile":
                    _viewManager.ShowView("UserDetails");
                    break;

                case "Browse food":
                    _viewManager.ShowView("Food");
                    break;

                case "Browse dishes":
                    _viewManager.ShowView("Dish");
                    break;

                case "Plan meals":
                    _viewManager.ShowView("MealPlanning");
                    break;

                case "Generate Shopping List":
                    _viewManager.ShowView("ShoppingList");
                    break;

                case "View calendar":
                    _viewManager.ShowView("Calendar");
                    break;

                case "Exit":
                    return;
            }
        }
    }
}
