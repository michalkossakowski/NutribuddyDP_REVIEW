using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// REVIEW - dużo nieużytych usingów

namespace NutribuddyDP.Core.Interfaces
{
    internal interface IMealComponent
    {
        string Name { get; }
        // REVIEW - Add/Remove mogło by ich nie być interfejsie bo są potrzebne tylko w 1 klasie
        // a pozostałe muszą na siłę wyrzucać NotImplementedException
        void Add(IMealComponent component);
        void Remove(IMealComponent component);
    }
}
