using Newtonsoft.Json;
using NutribuddyDP.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutribuddyDP.Core.Models
{
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
