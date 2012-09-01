using System;
using System.Collections.Generic;
using System.Reflection;
using FubuMVC.Core.Registration.Nodes;

namespace Swank
{
    public class Configuration
    {
        public Configuration()
        {
            AppliesToAssemblies = new List<Assembly>();
        }

        public string Url { get; private set; }
        public string SpecificationUrl { get; private set; }
        public List<Assembly> AppliesToAssemblies { get; private set; }
        public Func<ActionCall, bool> Filter { get; private set; } 

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
            SpecificationUrl = url + "/specification/";
            return this;
        }

        public Configuration Where(Func<ActionCall, bool> filter)
        {
            Filter = filter;
            return this;
        }
    }
}
