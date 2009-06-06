using System;
using System.Collections.Generic;
using System.Reflection;

namespace ToYuml
{
    public class AssemblyFilter
    {
        public IList<Type> Types { get; set; }

        public AssemblyFilter(Assembly assembly)
        {
            Types = new List<Type>(assembly.GetTypes());
        }
    }
}
