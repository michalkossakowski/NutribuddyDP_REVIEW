using Newtonsoft.Json;
using Spectre.Console;

namespace NutribuddyDP.Core.Controllers
{
    internal class DataStorage
    {
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
            catch (Exception ex)
            {
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
            catch (Exception ex)
            {
                AnsiConsole.WriteLine($"Error loading file: {ex.Message}");
                data = [];
            }
            return data;
        }
    }
}
