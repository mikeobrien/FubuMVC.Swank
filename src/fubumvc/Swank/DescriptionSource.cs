using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FubuMVC.Core.Registration.Nodes;
using MarkdownSharp;

namespace Swank
{
    public class DescriptionSource<T> where T : Description
    {
        private readonly static Func<Assembly, IList<Description>> GetDescriptions =
            Func.Memoize<Assembly, IList<Description>>(a =>
                a.GetTypes().Where(x => typeof(T).IsAssignableFrom(x) && x != typeof(T)).Select(CreateDescription).ToList());

        private readonly static Func<Assembly, string[]> GetResources =
            Func.Memoize<Assembly, string[]>(a => a.GetManifestResourceNames());

        public bool HasDescription(ActionCall action)
        {
            return GetDescription(action) != null;
        }

        public Description GetDescription(ActionCall action)
        {
            return GetDescriptions(action.HandlerType.Assembly)
                .OrderByDescending(x => x.GetType().Namespace)
                .FirstOrDefault(x => action.HandlerType.Namespace.StartsWith(x.GetType().Namespace));
            // Need to add logic in here to match on route instead of namespace if a generic parameter for the handler is specified.
        }

        private static Description CreateDescription(Type type)
        {
            var module = (Description) Activator.CreateInstance(type);
            if (string.IsNullOrEmpty(module.Comments))
            {
                var descriptionResourceName = GetResources(type.Assembly).FirstOrDefault(
                    x => new[] {".txt", ".html", ".md"}.Any(y => type.FullName + y == x));
                if (descriptionResourceName != null)
                {
                    var description = type.Assembly.GetManifestResourceStream(descriptionResourceName).ReadToEnd();
                    if (descriptionResourceName.EndsWith(".txt") || descriptionResourceName.EndsWith(".html"))
                        module.Comments = description;
                    else if (descriptionResourceName.EndsWith(".md"))
                        module.Comments = new Markdown().Transform(description).Trim();
                } 
            }
            return module;
        }
    }
}