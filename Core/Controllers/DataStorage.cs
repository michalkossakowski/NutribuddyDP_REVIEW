using Newtonsoft.Json;
using Spectre.Console;

namespace NutribuddyDP.Core.Controllers
{
    internal class DataStorage
    {
        // REVIEW - konstruktor nigdy nie użyty więc niepotrzebny
        // klasa ma same metody statyczne więc nie tworzy się jej instancja
        public DataStorage() { }

        public static void ExportData<T>(List<T> data, string filePath, JsonSerializerSettings? jsonSerializerSettings = null)
        {
            jsonSerializerSettings ??= new JsonSerializerSettings();
            //todo sprawdzenie czy data ma dane
            try
            {
                var jsonData = JsonConvert.SerializeObject(data, Formatting.Indented, jsonSerializerSettings);
                File.WriteAllText(filePath, jsonData);
            }
            catch (Exception ex) // REVIEW - powinien być łapany konkretny wyjątek a nie Exception
            {
                // REVIEW - przy wzorcu MVC nie MOŻNA użależniać kontrolera od widoku
                // wypisywanie danych w konsoli powinno odbywać się jedynie w View
                AnsiConsole.WriteLine($"Error saving file: {ex.Message}");
            }

        }
        public static List<T> ImportData<T>(string filePath, JsonSerializerSettings? jsonSerializerSettings = null)
        {
            List<T> data;
            jsonSerializerSettings ??= new JsonSerializerSettings();
            try
            {
                var jsonData = File.Exists(filePath) ? File.ReadAllText(filePath) : "[]";
                data = JsonConvert.DeserializeObject<List<T>>(jsonData, jsonSerializerSettings)!;
            }
            catch (Exception ex) // REVIEW - powinno się łapać konkretny exception
            {
                // REVIEW - ponownie wypisanie w konsoli z poziomu kontrolera jest surowo ZABRONIONE 
                AnsiConsole.WriteLine($"Error loading file: {ex.Message}");
                data = [];
            }
            return data;
        }
    }
}
