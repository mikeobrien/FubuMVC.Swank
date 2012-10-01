using System.IO;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using FubuMVC.Swank.Extensions;

namespace FubuMVC.Swank.Specification
{
    public class MergeService
    {  
        public Specification Merge(Specification specification, string specificationPath)
        {
            if (!Path.IsPathRooted(specificationPath))
                specificationPath = HttpContext.Current.WhenNotNull(x => x.Server.MapPath(specificationPath))
                    .Otherwise(Path.GetFullPath(specificationPath));

            return Merge(specification, File.ReadAllText(specificationPath).DeserializeJson<Specification>());
        }

        public Specification Merge(Specification specification1, Specification specification2)
        {
            return new Specification
                {
                    Name = specification1.Name ?? specification2.Name,
                    Comments = specification1.Comments ?? specification2.Comments,
                    Copyright = specification1.Copyright ?? specification2.Copyright,
                    Types = specification1.Types.OuterJoin(specification2.Types, x => x.Id ?? x.Name,
                        (key, types) => new Type {
                                Id = types.Select(y => y.Id).FirstOrDefault(),
                                Name = types.Select(y => y.Name).FirstOrDefault(),
                                Comments = types.Select(y => y.Comments).FirstOrDefault(),
                                Members = types.Select(y => y.Members).FirstOrDefault(y => y != null && y.Any())
                            })
                        .OrderBy(x => x.Name).ToList(),
                    Modules = specification1.Modules.OuterJoin(specification2.Modules, x => x.Name, 
                        (moduleKey, modules) => new Module {
                                Name = modules.Select(y => y.Name).FirstOrDefault(),
                                Comments = modules.Select(y => y.Comments).FirstOrDefault(),
                                Resources = modules.SelectMany(x => x.Resources).GroupBy(x => x.Name, x => x, 
                                    (resourceKey, resources) => new Resource {
                                        Name = resources.Select(y => y.Name).FirstOrDefault(),
                                        Comments = resources.Select(y => y.Comments).FirstOrDefault(),
                                        Endpoints = resources.SelectMany(x => x.Endpoints).GroupBy(x => x.Name ?? x.Url, x => x, 
                                            (endpointKey, endpoints) => endpoints.First()).OrderBy(x => x.Name ?? x.Url).ToList()
                                    }).OrderBy(x => x.Name).ToList()
                            }).OrderBy(x => x.Name).ToList(),
                    Resources = specification1.Resources.OuterJoin(specification2.Resources, x => x.Name, 
                        (resourceKey, resources) => new Resource {
                                Name = resources.Select(y => y.Name).FirstOrDefault(),
                                Comments = resources.Select(y => y.Comments).FirstOrDefault(),
                                Endpoints = resources.SelectMany(x => x.Endpoints).GroupBy(x => x.Name ?? x.Url, x => x, 
                                    (endpointKey, endpoints) => endpoints.First()).OrderBy(x => x.Name ?? x.Url).ToList()
                            }).OrderBy(x => x.Name).ToList()
                };
        }
    }
}