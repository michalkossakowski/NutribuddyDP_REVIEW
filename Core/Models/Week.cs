using Newtonsoft.Json;
using NutribuddyDP.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutribuddyDP.Core.Models
{
    internal class Week : PlanItem, IMealComponent
    {
        [JsonProperty]
        public List<IMealComponent> Days { get; set; } = [];

        public Week(string name)
        {
            Name = name;
        }

        public void Add(IMealComponent component)
        {
            if (Days.Count < 7)
            {
                Days.Add(component);
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(component), "Week can only consist of 7 days.");
            }
        }

        public void Remove(IMealComponent component)
        {
            Days.Remove(component);
        }
    }
}
