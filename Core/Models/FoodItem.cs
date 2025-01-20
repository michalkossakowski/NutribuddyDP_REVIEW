using Newtonsoft.Json;

namespace NutribuddyDP.Core.Models
{
    internal class FoodItem
    {
        [JsonProperty]
        public string Description { get; set; } = String.Empty;
        [JsonProperty]
        public Dictionary<string, double> Nutrients { get; set; } = [];
        [JsonProperty]
        public double QuantityInGrams { get; set; } // ilość danego skłdanika w gramach
        public string Category { get; set; } = String.Empty;
    }
}
