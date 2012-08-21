using System;
using System.Collections.Generic;
using System.Reflection;

namespace Swank
{
    public class Configuration
    {
        public Configuration()
        {
            AppliesToAssemblies = new List<Assembly>();
        }

        public string Url { get; set; }
        public List<Assembly> AppliesToAssemblies { get; private set; } 

        public Configuration AppliesTo<T>()
        {
            AppliesTo(typeof (T));
            return this;
        }

        public Configuration AppliesTo(Type type)
        {
            AppliesToAssemblies.Add(type.Assembly);
            return this;
        }

        public Configuration At(string url)
        {
            Url = url;
            return this;
        }
    }
}
