﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutribuddyDP.Core.Interfaces
{
    internal interface IMealComponent
    {
        string Name { get; }
        void Add(IMealComponent component);
        void Remove(IMealComponent component);
    }
}
