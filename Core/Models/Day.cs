using Newtonsoft.Json;
using NutribuddyDP.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// REVIEW - dużo nieużytych usingów

namespace NutribuddyDP.Core.Models
{
    // REVIEW - Day implementuje IMealComponent - nazewnictwo jest katastrofalne nie wiadomo o co chodzi
    internal class Day : PlanItem, IMealComponent
    {
        [JsonProperty]
        public List<IMealComponent> Meals { get; set; } = [];

        public Day(string name)
        {
            Name = name;
        }

        public void Add(IMealComponent component)
        {
            Meals.Add(component);
        }

        public void Remove(IMealComponent component)
        {
            Meals.Remove(component);
        }
    }
}
