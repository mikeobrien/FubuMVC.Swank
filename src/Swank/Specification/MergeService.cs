using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using FubuCore;
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
            Func<string, IEnumerable<Resource>, Resource> mergeResources = 
                (resourceKey, resources) => new Resource
                {
                    Name = resources.Select(y => y.Name).FirstOrDefault(),
                    Comments = resources.Select(y => y.Comments).FirstOrDefault(),
                    Endpoints = resources.SelectMany(x => x.Endpoints).GroupBy(
                        x => x.Name ?? ("{0}:{1}".ToFormat(x.Method, x.Url)), x => x,
                        (endpointKey, endpoints) => endpoints.First()).OrderBy(x => x.Url.Split('?').First())
                                .ThenBy(x => SpecificationService.HttpVerbRank(x.Method)).ToList()
                };
            return new Specification
                {
                    Name = specification1.Name ?? specification2.Name,
                    Comments = specification1.Comments ?? specification2.Comments,
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
                                Resources = modules.SelectMany(x => x.Resources)
                                    .GroupBy(x => x.Name, x => x, mergeResources)
                                    .OrderBy(x => x.Name).ToList()
                            }).OrderBy(x => x.Name).ToList()
                };
        }
    }
}