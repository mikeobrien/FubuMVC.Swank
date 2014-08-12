using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace FubuMVC.Swank.Description
{
    public static class Assert
    {
        public static void AllEmbeddedCommentsMatchTypes(Func<string, bool> resourceFilter = null)
        {
            AllEmbeddedCommentsMatchTypes(Assembly.GetCallingAssembly(), resourceFilter);
        }

         public static void AllEmbeddedCommentsMatchTypes<TAssembly>(Func<string, bool> resourceFilter = null)
         {
             AllEmbeddedCommentsMatchTypes(typeof(TAssembly), resourceFilter);
         }

         public static void AllEmbeddedCommentsMatchTypes(Type type, Func<string, bool> resourceFilter = null)
         {
             AllEmbeddedCommentsMatchTypes(type.Assembly, resourceFilter);
         }

         public static void AllEmbeddedCommentsMatchTypes(Assembly assembly, Func<string, bool> resourceFilter = null)
         {
             var types = assembly.GetTypes().Where(x => x.IsPublic).ToList();
             var validNames = types.Select(x => x.FullName)
                  .Concat(types.Select(x => x.Namespace).Distinct())
                  .Concat(types.SelectMany(x => x.GetMethods().Where(y => y.IsPublic).Select(y => x.FullName + "." + y.Name)))
                  .Join(new[] { "", ".Resource", ".Request", ".Response" }, x => true, x => true, (x, y) => x + y)
                  .Join(new[] { ".md", ".html", ".txt" }, x => true, x => true, (x, y) => x + y)
                  .ToList();
             var orphans = assembly.GetManifestResourceNames()
                 .Where(x => Regex.IsMatch(x, "(\\.md$|\\.html$|\\.txt$)"))
                 .Where(resourceFilter ?? (x => true))
                 .Where(x => !validNames.Any(y => y.Equals(x, StringComparison.OrdinalIgnoreCase)))
                 .ToList();
             if (orphans.Any()) 
                 throw new Exception("The following embedded comments do not refer to a type:\r\n" + string.Join(",\r\n", orphans.ToArray()));
         }
    }
}