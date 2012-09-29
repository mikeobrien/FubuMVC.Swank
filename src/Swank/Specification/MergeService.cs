using System.IO;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using FubuMVC.Swank.Extensions;

namespace FubuMVC.Swank.Specification
{
    public class MergeService
    {
        public Specification Merge(Specification specification, string path)
        {
            if (!Path.IsPathRooted(path))
                path = HttpContext.Current.WhenNotNull(x => x.Server.MapPath(path)).Otherwise(Path.GetFullPath(path));

            var mergeSpecification = new JavaScriptSerializer().Deserialize<Specification>(File.ReadAllText(path));

            if (mergeSpecification.Types != null && mergeSpecification.Types.Any())
            {
                var newTypes = mergeSpecification.Types.Where(x => specification.Types.All(y => y.Id != x.Id)).ToList();
                if (newTypes.Any()) specification.Types.AddRange(newTypes);
                specification.Types.Sort((a, b) => a.Name.CompareTo(b.Name));
            }

            if (mergeSpecification.Modules != null && mergeSpecification.Modules.Any())
            {
                specification.Modules.SelectMany(x => x.Resources.Select(y => new { Module = x, Resource = y }))
                    .Join(mergeSpecification.Modules.SelectMany(x => x.Resources.Select(y => new { Module = x, Resource = y })),
                            x => x.Module.Name + "." + x.Resource.Name, x => x.Module.Name + "." + x.Resource.Name, (source, merge) =>
                                new
                                {
                                    Source = source.Resource,
                                    MergeEndpoints = merge.Resource.Endpoints.Where(y => source.Resource.Endpoints.All(z => y.Url != z.Url)).ToList()
                                })
                    .Where(x => x.MergeEndpoints.Any())
                    .ToList()
                    .ForEach(x =>
                    {
                        x.Source.Endpoints.AddRange(x.MergeEndpoints);
                        x.Source.Endpoints.Sort((a, b) => (a.Name ?? a.Url).CompareTo(b.Name ?? b.Url));
                    });

                specification.Modules
                    .Join(mergeSpecification.Modules, x => x.Name, x => x.Name, (source, merge) =>
                        new { Source = source, MergeResources = merge.Resources.Where(y => source.Resources.All(z => y.Name != z.Name)).ToList() })
                    .Where(x => x.MergeResources.Any())
                    .ToList()
                    .ForEach(x =>
                    {
                        x.Source.Resources.AddRange(x.MergeResources);
                        x.Source.Resources.Sort((a, b) => a.Name.CompareTo(b.Name));
                    });

                var newModules = mergeSpecification.Modules.Where(x => specification.Modules.All(y => y.Name != x.Name)).ToList();
                if (newModules.Any()) specification.Modules.AddRange(newModules);
                specification.Modules.Sort((a, b) => a.Name.CompareTo(b.Name));
            }

            if (mergeSpecification.Resources != null && mergeSpecification.Resources.Any())
            {
                specification.Resources
                    .Join(mergeSpecification.Resources, x => x.Name, x => x.Name, (source, merge) =>
                        new { Source = source, MergeEndpoints = merge.Endpoints.Where(y => source.Endpoints.All(z => y.Url != z.Url)).ToList() })
                    .Where(x => x.MergeEndpoints.Any())
                    .ToList()
                    .ForEach(x => x.Source.Endpoints.AddRange(x.MergeEndpoints));
                var newResources = mergeSpecification.Resources.Where(x => specification.Resources.All(y => y.Name != x.Name)).ToList();
                if (newResources.Any()) specification.Resources.AddRange(newResources);
                specification.Resources.Sort((a, b) => a.Name.CompareTo(b.Name));
            }
            return specification;
        }
    }
}