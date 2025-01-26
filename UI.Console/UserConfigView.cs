using NutribuddyDP.Core.Controllers;
using Spectre.Console;

namespace NutribuddyDP.UI.Console
{
    internal class UserConfigView(UserController userController, ViewManager viewManager) : IView
    {
        private readonly UserController _userController = userController;
        private readonly ViewManager _viewManager = viewManager;

        public void Show()
        {
            AnsiConsole.MarkupLine("[bold yellow]Edit User Info[/]");

            double weight = AnsiConsole.Ask<double>("Enter your weight (kg):");
            double height = AnsiConsole.Ask<double>("Enter your height (cm):");
            int age = AnsiConsole.Ask<int>("Enter your age (years):");
            string gender = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Select your gender:")
                    .AddChoices("Male", "Female"));

            string activityLevel = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Select your physical activity level:")
                    .AddChoices("Sedentary", "Lightly Active", "Moderately Active", "Very Active", "Extra Active"));

            string goal = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("What is your goal?")
                    .AddChoices("Lose Weight", "Maintain Weight", "Gain Weight"));
            // REVIEW - Walidacja powinna odbywać się również na szczeblu wizualnym.
            // Aktualnie odbywa się ona tylko w kontrolerze i niepoprawne zmiany (np ujemne wartości) nie zostaną zatwierdzone,
            // ale użytkownik nie jet na bieżąco powiadamiany o wprowadzeniu błędu w widoku.
            _userController.UpdateUser(weight, height, age, gender, activityLevel, goal);

            AnsiConsole.MarkupLine("[bold green]User information updated successfully![/]");
            Thread.Sleep(750);

            _viewManager.ShowView("UserDetails");
        }
    }
}
