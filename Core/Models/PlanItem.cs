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
    [JsonObject(MemberSerialization.OptIn)]
    internal abstract class PlanItem
    {
        [JsonProperty]
        public string Name { get; set; } = String.Empty;
    }
}
